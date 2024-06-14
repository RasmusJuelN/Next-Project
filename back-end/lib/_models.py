from pydantic import BaseModel
from typing import List


class Option(BaseModel):
    value: int
    label: str


class Question(BaseModel):
    id: int
    text: str
    options: List[Option]

    class Config:
        json_schema_extra = {
            "example": {
                "id": 1,
                "text": "What is your favorite programming language?",
                "options": [
                    {"value": 1, "label": "Python"},
                    {"value": 2, "label": "JavaScript"},
                    {"value": 3, "label": "Java"},
                ],
            }
        }


class AllQuestions(BaseModel):
    questions: List[Question]

    class Config:
        json_schema_extra = {
            "example": {
                "questions": [
                    {
                        "id": 1,
                        "text": "What is your favorite programming language?",
                        "options": [
                            {"value": 1, "label": "Python"},
                            {"value": 2, "label": "JavaScript"},
                            {"value": 3, "label": "Java"},
                        ],
                    },
                    {
                        "id": 2,
                        "text": "What is your favorite IDE?",
                        "options": [
                            {"value": 1, "label": "PyCharm"},
                            {"value": 2, "label": "VS Code"},
                            {"value": 3, "label": "IntelliJ IDEA"},
                        ],
                    },
                ]
            }
        }
