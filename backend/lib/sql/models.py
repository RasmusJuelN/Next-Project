from sqlalchemy import Boolean, ForeignKey, Integer, String, DateTime
from sqlalchemy.orm import DeclarativeBase, relationship, mapped_column, Mapped
from datetime import datetime
from typing import List


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

    selected_option: Mapped[int] = mapped_column(
        type_=Integer, index=False, nullable=True
    )
    custom_answer: Mapped[str] = mapped_column(type_=String, index=False, nullable=True)
    template: Mapped["QuestionTemplate"] = relationship(back_populates="questions")
    options: Mapped[List["Option"]] = relationship(
        back_populates="question", cascade="all, delete-orphan"
    )


class QuestionTemplate(Base):
    __tablename__: str = "question_templates"

    id: Mapped[int] = mapped_column(type_=Integer, primary_key=True, index=True)
    template_id: Mapped[str] = mapped_column(type_=String, index=True, unique=True)
    title: Mapped[str] = mapped_column(type_=String, index=True)
    description: Mapped[str] = mapped_column(type_=String, index=False)
    created_at: Mapped[datetime] = mapped_column(type_=DateTime, index=False)
    questions: Mapped[List["Question"]] = relationship(
        back_populates="template", cascade="all, delete-orphan"
    )
