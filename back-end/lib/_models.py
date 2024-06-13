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
                "question": "What is your favorite programming language?",
                "options": [
                    {"option": 1, "text": "Python"},
                    {"option": 2, "text": "JavaScript"},
                    {"option": 3, "text": "Java"},
                ],
            }
        }
