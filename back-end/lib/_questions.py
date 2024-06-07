"""
Contains the questions for the questionnaire for temporary storage and testing.
"""

from typing import Dict, List, Union
import json


questionnaire: Dict[
    str, List[Dict[str, Union[str, List[Dict[str, Union[int, str]]]]]]
] = {
    "questions": [
        {
            "id": "1",
            "question": "Indlæringsevne",
            "answers": [
                {
                    "id": 1,
                    "text": "Viser lidt eller ingen forståelse for arbejdsopgaverne",
                },
                {
                    "id": 2,
                    "text": "Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden",
                },
                {
                    "id": 3,
                    "text": "Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.",
                },
                {
                    "id": 4,
                    "text": "Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.",
                },
                {
                    "id": 5,
                    "text": "Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.",
                },
            ],
        },
    ]
}


json_questionnaire: str = json.dumps(questionnaire)
