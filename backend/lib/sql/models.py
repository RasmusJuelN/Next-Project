from sqlalchemy import Boolean, CheckConstraint, ForeignKey, Integer, String, DateTime
from sqlalchemy.orm import DeclarativeBase, relationship, mapped_column, Mapped
from datetime import datetime
from typing import List


class Base(DeclarativeBase):
    pass


class Option(Base):
    __tablename__: str = "options"

    id: Mapped[int] = mapped_column(type_=Integer, primary_key=True, index=True)
    questionId: Mapped[int] = mapped_column(
        type_=Integer, __type_pos=ForeignKey(column="questions.id")
    )
    value: Mapped[int] = mapped_column(type_=Integer, index=True)
    label: Mapped[str] = mapped_column(type_=String, index=True)
    isCustom: Mapped[bool] = mapped_column(type_=Boolean, index=False)
    question: Mapped["Question"] = relationship(back_populates="options")


class Question(Base):
    __tablename__: str = "questions"

    id: Mapped[int] = mapped_column(type_=Integer, primary_key=True, index=True)
    templateReferenceId: Mapped[str] = mapped_column(
        type_=String, __type_pos=ForeignKey(column="question_templates.templateId")
    )
    title: Mapped[str] = mapped_column(type_=String, index=True)

    selectedOption: Mapped[int] = mapped_column(
        type_=Integer, index=False, nullable=True
    )
    customAnswer: Mapped[str] = mapped_column(type_=String, index=False, nullable=True)
    template: Mapped["QuestionTemplate"] = relationship(back_populates="questions")
    options: Mapped[List["Option"]] = relationship(back_populates="question")


class QuestionTemplate(Base):
    __tablename__: str = "question_templates"

    id: Mapped[int] = mapped_column(type_=Integer, primary_key=True, index=True)
    templateId: Mapped[str] = mapped_column(type_=String, index=True)
    title: Mapped[str] = mapped_column(type_=String, index=True)
    description: Mapped[str] = mapped_column(type_=String, index=False)
    createdAt: Mapped[datetime] = mapped_column(type_=DateTime, index=False)
    questions: Mapped[List["Question"]] = relationship(back_populates="template")
