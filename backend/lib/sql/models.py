from sqlalchemy import Boolean, ForeignKey, Integer, String, DateTime, func
from sqlalchemy.orm import DeclarativeBase, relationship, mapped_column, Mapped
from datetime import datetime
from typing import List, Optional


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
        type_=String, __type_pos=ForeignKey(column="question_templates.template_id")
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

    id: Mapped[int] = mapped_column(type_=Integer, primary_key=True, index=True)
    template_id: Mapped[str] = mapped_column(type_=String, index=True, unique=True)
    title: Mapped[str] = mapped_column(type_=String, index=True)
    description: Mapped[str] = mapped_column(type_=String, index=False)
    created_at: Mapped[datetime] = mapped_column(type_=DateTime, index=False)

    questions: Mapped[List["Question"]] = relationship(
        argument="Question", back_populates="template", cascade="all, delete-orphan"
    )


class User(Base):
    __tablename__: str = "users"

    id: Mapped[str] = mapped_column(type_=String, primary_key=True, index=True)
    username: Mapped[str] = mapped_column(type_=String, index=True)
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

    id: Mapped[str] = mapped_column(type_=String, primary_key=True, index=True)
    student_id: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="users.id")
    )
    teacher_id: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="users.id")
    )
    is_student_finished: Mapped[bool] = mapped_column(type_=Boolean, index=False)
    is_teacher_finished: Mapped[bool] = mapped_column(type_=Boolean, index=False)
    template_reference_id: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="question_templates.template_id")
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
