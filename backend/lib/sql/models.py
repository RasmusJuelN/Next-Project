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
)
from sqlalchemy.orm import DeclarativeBase, relationship, mapped_column, Mapped
from datetime import datetime
from typing import List, Optional, LiteralString
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
    pass


class Option(Base):
    __tablename__: str = "options"

    id: Mapped[int] = mapped_column(type_=Integer, primary_key=True, index=True)
    question_id: Mapped[int] = mapped_column(
        type_=Integer, __type_pos=ForeignKey(column="questions.id")
    )
    value: Mapped[int] = mapped_column(type_=Integer, index=True)
    label: Mapped[str] = mapped_column(type_=String, index=True)
    is_custom: Mapped[bool] = mapped_column(type_=Boolean, index=False, default=False)
    question: Mapped["Question"] = relationship(back_populates="options")


class Question(Base):
    __tablename__: str = "questions"

    id: Mapped[int] = mapped_column(type_=Integer, primary_key=True, index=True)
    template_reference_id: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="question_templates.id")
    )
    title: Mapped[str] = mapped_column(type_=String, index=True)

    selected_option: Mapped[Optional[int]] = mapped_column(
        type_=Integer, index=False, nullable=True
    )
    custom_answer: Mapped[Optional[str]] = mapped_column(
        type_=String, index=False, nullable=True
    )

    template: Mapped["QuestionTemplate"] = relationship(
        argument="QuestionTemplate", back_populates="questions"
    )
    options: Mapped[List["Option"]] = relationship(
        argument="Option", back_populates="question", cascade="all, delete-orphan"
    )


class QuestionTemplate(Base):
    __tablename__: str = "question_templates"

    id: Mapped[str] = mapped_column(
        type_=String, primary_key=True, index=True, default=sql_func_generate_id
    )
    title: Mapped[str] = mapped_column(type_=String, index=True)
    description: Mapped[str] = mapped_column(type_=String, index=False)
    created_at: Mapped[datetime] = mapped_column(type_=DateTime, index=False)

    questions: Mapped[List["Question"]] = relationship(
        argument="Question", back_populates="template", cascade="all, delete-orphan"
    )
    template_questionnaires: Mapped[List["ActiveQuestionnaire"]] = relationship(
        argument="ActiveQuestionnaire", back_populates="template"
    )


class User(Base):
    __tablename__: str = "users"

    id: Mapped[str] = mapped_column(type_=String, primary_key=True, index=True)
    user_name: Mapped[str] = mapped_column(type_=String, index=True)
    full_name: Mapped[str] = mapped_column(type_=String, index=False)
    role: Mapped[str] = mapped_column(type_=String, index=False)

    student_questionnaires: Mapped[list["ActiveQuestionnaire"]] = relationship(
        argument="ActiveQuestionnaire",
        foreign_keys="[ActiveQuestionnaire.student_id]",
        back_populates="student",
    )
    teacher_questionnaires: Mapped[list["ActiveQuestionnaire"]] = relationship(
        argument="ActiveQuestionnaire",
        foreign_keys="[ActiveQuestionnaire.teacher_id]",
        back_populates="teacher",
    )


class ActiveQuestionnaire(Base):
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
    is_student_finished: Mapped[bool] = mapped_column(type_=Boolean, index=False)
    is_teacher_finished: Mapped[bool] = mapped_column(type_=Boolean, index=False)
    template_reference_id: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="question_templates.id")
    )
    created_at: Mapped[datetime] = mapped_column(
        type_=DateTime(timezone=True), index=False, server_default=func.now()
    )

    student: Mapped["User"] = relationship(
        argument="User",
        foreign_keys=[student_id],
        back_populates="student_questionnaires",
    )
    teacher: Mapped["User"] = relationship(
        argument="User",
        foreign_keys=[teacher_id],
        back_populates="teacher_questionnaires",
    )
    template: Mapped["QuestionTemplate"] = relationship(
        argument="QuestionTemplate",
        foreign_keys=[template_reference_id],
        back_populates="template_questionnaires",
    )
