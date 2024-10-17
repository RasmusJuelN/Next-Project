from sqlalchemy import (
    Boolean,
    ExecutionContext,
    Exists,
    ForeignKey,
    Integer,
    String,
    DateTime,
    func,
    exists,
    select,
    update,
    delete,
    event,
    Connection,
    MetaData,
)
from sqlalchemy.orm import (
    DeclarativeBase,
    relationship,
    mapped_column,
    Mapped,
    Session,
    Mapper,
)
from datetime import datetime
from typing import List, Optional, LiteralString, Type, Union
from random import choice
from string import ascii_letters, digits

from backend.lib.sql import db_logger

URL_FRIENDLY_BASE64: LiteralString = ascii_letters + digits + "-_"


def sql_func_generate_id(context: ExecutionContext) -> str:
    """
    Generates a unique random string ID for a SQL database entry.

    This function attempts to generate a unique 10-character random string
    using URL-friendly base64 characters. It checks the generated string
    against the `QuestionTemplate` table to ensure uniqueness. If a unique
    string is not generated within 5 attempts, it logs an error and raises
    a `ValueError`.

    Args:
        context (ExecutionContext): The SQLAlchemy execution context containing
                                    the database engine.

    Returns:
        str: A unique 10-character random string.

    Raises:
        ValueError: If a unique random string cannot be generated after 5 attempts.
    """
    attempts: int = 0
    with context.engine.begin() as connection:
        while True:
            random_string: str = "".join(
                choice(seq=URL_FRIENDLY_BASE64) for _ in range(10)
            )
            exists_criteria: Exists = exists().where(
                QuestionTemplate.id == random_string
            )
            result: Optional[bool] = connection.execute(
                statement=select(exists_criteria)
            ).scalar()
            if result is False:
                return random_string
            else:
                if attempts >= 5:
                    msg = "Failed to generate a unique random string after 5 attempts"
                    db_logger.error(msg=msg)
                    raise ValueError(msg)
                attempts += 1
                continue


class Base(DeclarativeBase):
    metadata = MetaData(
        naming_convention={
            "ix": "ix_%(column_0_label)s",
            "uq": "uq_%(table_name)s_%(column_0_name)s",
            "ck": "ck_%(table_name)s_`%(constraint_name)s`",
            "fk": "fk_%(table_name)s_%(column_0_name)s_%(referred_table_name)s",
            "pk": "pk_%(table_name)s",
        }
    )


class Option(Base):
    """
    Represents an option for a question in the database.

    Attributes:
        id (int): The primary key of the option. Auto-incremented.
        question_id (int): The foreign key linking to the associated
            question.
        value (int): The value of the option. Can be used as a local
            index scoped to the question. I.e., if the answer has
            `selected_option_value` set to 2, it corresponds to the
            option with `value` set to 2.
        label (str): The label, title, or text of the option. This is
            what the user sees when selecting an option.
        is_custom (bool): Indicates if the option is custom or predefined.
            Defaults to False. Has no effect on the internal logic, but
            can be used to differentiate between custom and predefined
            options in the UI.

    Relationships:
        question (Question): The relationship to the associated question.

    Note:
        Relationships are automatically handled by SQLAlchemy. Once the
        ForeignKey `question_id` is set, the relationship is automatically
        established between the `Option` and `Question` models.
    """

    __tablename__: str = "options"

    id: Mapped[int] = mapped_column(type_=Integer, primary_key=True, index=True)
    question_id: Mapped[int] = mapped_column(
        type_=Integer, __type_pos=ForeignKey(column="questions.id")
    )
    value: Mapped[int] = mapped_column(type_=Integer, index=True, nullable=False)
    label: Mapped[str] = mapped_column(type_=String, index=True, nullable=False)
    is_custom: Mapped[bool] = mapped_column(type_=Boolean, index=False, default=False)

    # A many-to-one bidirectional relationship with the Question model,
    # indicating that an option belongs to a question
    question: Mapped["Question"] = relationship(
        back_populates="options",  # The back reference to the options field in the Question model
        lazy="select",  # Lazy load the question
    )


class Question(Base):
    """
    Represents a question in the database.

    Attributes:
        id (Mapped[int]): The primary key of the question. Auto-incremented.
        template_id: (Mapped[str]): The foreign key referencing the question
            template with which the question is associated.
        title (Mapped[str]): The title of the question. This is the question
            which the user sees.

    Relationships:
        template (Mapped["QuestionTemplate"]): The relationship to the
            QuestionTemplate model.
        options (Mapped[List["Option"]]): The relationship to the
            Option model, with cascading delete-orphan.


    Note:
        Relationships are automatically handled by SQLAlchemy. Once the
        ForeignKey `template_id` is set, the relationship is automatically
        established between the `Question` and `QuestionTemplate` models.
    """

    __tablename__: str = "questions"

    id: Mapped[int] = mapped_column(type_=Integer, primary_key=True, index=True)
    template_id: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="question_templates.id")
    )
    title: Mapped[str] = mapped_column(type_=String, index=True)

    # A many-to-one bidirectional relationship with the QuestionTemplate model,
    # indicating that a question belongs to a question template
    template: Mapped["QuestionTemplate"] = relationship(
        argument="QuestionTemplate",  # The model to relate to
        back_populates="questions",
        lazy="select",
    )

    # A one-to-many bidirectional relationship with the Option model,
    # indicating that a question can have multiple options
    options: Mapped[List["Option"]] = relationship(
        argument="Option",  # The model to relate to
        back_populates="question",
        cascade="all, delete-orphan",  # Delete options when the question is deleted
        lazy="select",
    )


class QuestionTemplate(Base):
    """
    Represents a template for questions in the database.

    Attributes:
        id (Mapped[str]): The unique identifier for the question
            template, generated by `sql_func_generate_id`.
        title (Mapped[str]): The title of the question template.
        description (Mapped[Optional[str]]): The description of the
            question template. Can be used to provide additional context
            or instructions for the questions. Is not required.
        created_at (Mapped[datetime]): The timestamp when the question
            template was created. Automatically set to the current time.
        last_updated (Mapped[datetime]): The timestamp when the question
            template was last updated. Automatically runs on row update.

    Relationships:
        questions (Mapped[List["Question"]]): The list of questions
            associated with this template.
        template_questionnaires (Mapped[List["ActiveQuestionnaire"]]):
            The list of active questionnaires associated with this template.

    Note:
        Relationships are automatically handled by SQLAlchemy. Once the
        ForeignKey `template_id` is set, the relationship is automatically
        established between the `Question` and `QuestionTemplate` models.
    """

    __tablename__: str = "question_templates"

    id: Mapped[str] = mapped_column(
        type_=String, primary_key=True, index=True, default=sql_func_generate_id
    )
    title: Mapped[str] = mapped_column(type_=String, index=True, nullable=False)
    description: Mapped[Optional[str]] = mapped_column(
        type_=String, index=False, nullable=True
    )
    created_at: Mapped[Optional[datetime]] = mapped_column(
        type_=DateTime(timezone=True),
        index=False,
        server_default=func.now(),
        nullable=False,
    )
    last_updated: Mapped[Optional[datetime]] = mapped_column(
        type_=DateTime(timezone=True),
        index=False,
        server_default=func.now(),
        onupdate=func.now(),
        nullable=False,
    )

    questions: Mapped[List["Question"]] = relationship(
        argument="Question",
        back_populates="template",
        cascade="all, delete-orphan",
        lazy="select",
    )
    template_questionnaires: Mapped[List["ActiveQuestionnaire"]] = relationship(
        argument="ActiveQuestionnaire",
        back_populates="template",
        lazy="select",
    )


class User(Base):
    """
    Represents a user in the system.

    Attributes:
        id (Mapped[str]): The primary key of the user, indexed. A hashed
            version of their Active Directory UUID.
        user_name (Mapped[str]): The username of the user, indexed.
        full_name (Mapped[str]): The full name of the user.
        role (Mapped[str]): The role of the user.

    Relationships:
        student_questionnaires (Mapped[list["ActiveQuestionnaire"]]):
            The list of questionnaires associated with the user as a student.
        teacher_questionnaires (Mapped[list["ActiveQuestionnaire"]]):
            The list of questionnaires associated with the user as a teacher.

    Note:
        Relationships are automatically handled by SQLAlchemy. Once the
        ForeignKey `student_id` or `teacher_id` is set, the relationship is
        automatically established between the `User` and `ActiveQuestionnaire`
    """

    __tablename__: str = "users"

    id: Mapped[str] = mapped_column(type_=String, primary_key=True, index=True)
    user_name: Mapped[str] = mapped_column(type_=String, index=True, nullable=False)
    full_name: Mapped[str] = mapped_column(type_=String, index=False, nullable=False)
    role: Mapped[str] = mapped_column(type_=String, index=False, nullable=False)

    # A one-to-many bidirectional relationship with the ActiveQuestionnaire model,
    # indicating that both students and teachers can have multiple questionnaires
    student_questionnaires: Mapped[list["ActiveQuestionnaire"]] = relationship(
        argument="ActiveQuestionnaire",
        foreign_keys="[ActiveQuestionnaire.student_id]",  # The foreign key to the student_id field in the ActiveQuestionnaire model
        back_populates="student",
        lazy="select",
    )
    teacher_questionnaires: Mapped[list["ActiveQuestionnaire"]] = relationship(
        argument="ActiveQuestionnaire",
        foreign_keys="[ActiveQuestionnaire.teacher_id]",  # The foreign key to the teacher_id field in the ActiveQuestionnaire model
        back_populates="teacher",
        lazy="select",
    )


class ActiveQuestionnaire(Base):
    """
    ActiveQuestionnaire model represents the active questionnaires assigned to students and teachers.

    Attributes:
        id (Mapped[str]): The primary key of the active questionnaire,
            generated by `sql_func_generate_id`.
        student_id (Mapped[str]): Foreign key referencing the `id`
            column in the `users` table.
        teacher_id (Mapped[str]): Foreign key referencing the `id`
            column in the `users` table.
        template_reference_id (Mapped[str]): Foreign key referencing
            the `id` column in the `question_templates` table.
        created_at (Mapped[datetime]): The timestamp when the active
            questionnaire was created, with timezone support.
        student_finished_at (Mapped[Optional[datetime]]): The timestamp
            when the student finished the questionnaire, with timezone
            support. Can be `None` if the student has not finished.
        teacher_finished_at (Mapped[Optional[datetime]]): The timestamp
            when the teacher finished the questionnaire, with timezone
            support. Can be `None` if the teacher has not finished.

    Relationships:
        student (Mapped["User"]): Relationship to the `User` model for
            the student.
        teacher (Mapped["User"]): Relationship to the `User` model for
            the teacher.
        template (Mapped["QuestionTemplate"]): Relationship to the
            `QuestionTemplate` model.
    """

    __tablename__: str = "active_questionnaires"

    id: Mapped[str] = mapped_column(
        type_=String, primary_key=True, index=True, default=sql_func_generate_id
    )
    student_id: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="users.id")
    )
    teacher_id: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="users.id")
    )
    template_reference_id: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="question_templates.id")
    )
    created_at: Mapped[datetime] = mapped_column(
        type_=DateTime(timezone=True), index=False, server_default=func.now()
    )
    student_finished_at: Mapped[Optional[datetime]] = mapped_column(
        type_=DateTime(timezone=True), index=False, nullable=True
    )
    teacher_finished_at: Mapped[Optional[datetime]] = mapped_column(
        type_=DateTime(timezone=True), index=False, nullable=True
    )

    student: Mapped["User"] = relationship(
        argument="User",
        foreign_keys=[student_id],
        back_populates="student_questionnaires",
        lazy="select",
    )
    teacher: Mapped["User"] = relationship(
        argument="User",
        foreign_keys=[teacher_id],
        back_populates="teacher_questionnaires",
        lazy="select",
    )
    template: Mapped["QuestionTemplate"] = relationship(
        argument="QuestionTemplate",
        foreign_keys=[template_reference_id],
        back_populates="template_questionnaires",
        lazy="select",
    )


@event.listens_for(target=ActiveQuestionnaire, identifier="after_delete")
def delete_user_if_no_questionnaires(
    mapper: Mapper, connection: Connection, target: Type[ActiveQuestionnaire]
) -> None:
    """
    Event listener to delete a user if they have no remaining questionnaires.

    Args:
        mapper: The mapper.
        connection: The database connection.
        target: The instance of ActiveQuestionnaire being deleted.
    """
    with Session(bind=connection) as session:
        with session.begin():
            student_questionnaire_count: int = session.execute(
                statement=select(func.count(ActiveQuestionnaire.id)).where(
                    ActiveQuestionnaire.student_id == target.student_id
                )
            ).scalar_one()
            if student_questionnaire_count == 0:
                session.execute(
                    statement=delete(table=User).where(User.id == target.student_id)
                )

            teacher_questionnaire_count: int = session.execute(
                statement=select(func.count(ActiveQuestionnaire.id)).where(
                    ActiveQuestionnaire.teacher_id == target.teacher_id
                )
            ).scalar_one()
            if teacher_questionnaire_count == 0:
                session.execute(
                    statement=delete(table=User).where(User.id == target.teacher_id)
                )


def update_template_on_question_or_option_change(
    mapper: Mapper, connection: Connection, target: Type[Union[Question, Option]]
) -> None:
    """
    Updates the 'last_updated' field of the associated QuestionTemplate when a Question or Option changes.

    This function is triggered by an event listener and determines whether the target is a Question or an Option.
    It then updates the 'last_updated' timestamp of the associated QuestionTemplate. If, somehow, the target is
    neither a Question nor an Option, the function does nothing. Uses context managers to handle the database
    connection and session.

    Args:
        mapper (Mapper): The SQLAlchemy mapper.
        connection (Connection): The database connection.
        target (Type[Union[Question, Option]]): The target object that triggered the event, either a Question or an Option.

    Returns:
        None
    """
    if isinstance(target, Question) and target.template is not None:
        template: QuestionTemplate = target.template
    elif isinstance(target, Option) and target.question is not None:
        template = target.question.template
    else:
        return

    with Session(bind=connection) as session:
        with session.begin():
            session.execute(
                statement=update(table=QuestionTemplate)
                .where(QuestionTemplate.id == template.id)
                .values(last_updated=datetime.now())
            )


# Listen for changes to the Question and Option models
for event_type in ("after_insert", "after_update", "after_delete"):
    event.listen(
        target=Question,
        identifier=event_type,
        fn=update_template_on_question_or_option_change,
    )
    event.listen(
        target=Option,
        identifier=event_type,
        fn=update_template_on_question_or_option_change,
    )
