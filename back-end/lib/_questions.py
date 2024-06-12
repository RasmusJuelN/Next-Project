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
            "text": "Indlæringsevne",
            "options": [
                {
                    "value": 1,
                    "label": "Viser lidt eller ingen forståelse for arbejdsopgaverne",
                },
                {
                    "value": 2,
                    "label": "Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden",
                },
                {
                    "value": 3,
                    "label": "Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.",
                },
                {
                    "value": 4,
                    "label": "Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.",
                },
                {
                    "value": 5,
                    "label": "Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.",
                },
            ],
        },
        {
            "id": "2",
            "text": "Kreativitet og selvstændighed",
            "options": [
                {
                    "value": 1,
                    "label": "Viser intet initiativ. Er passiv, uinteresseret og uselvstændig",
                },
                {
                    "value": 2,
                    "label": "Viser ringe initiativ. Kommer ikke med løsningsforslag. Viser ingen interesse i at tilægge eget arbejde.",
                },
                {
                    "value": 3,
                    "label": "Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.",
                },
                {
                    "value": 4,
                    "label": "Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde.",
                },
                {
                    "value": 5,
                    "label": "Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre.",
                },
            ],
        },
        {
            "id": "3",
            "text": "Arbejdsindsats",
            "options": [
                {"value": 1, "label": "Uacceptabel"},
                {"value": 2, "label": "Under middel"},
                {"value": 3, "label": "Middel"},
                {"value": 4, "label": "Over middel"},
                {"value": 5, "label": "Særdeles god"},
            ],
        },
        {
            "id": "4",
            "text": "Orden og omhyggelighed",
            "options": [
                {
                    "value": 1,
                    "label": "Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig.",
                },
                {
                    "value": 2,
                    "label": "Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed.",
                },
                {
                    "value": 3,
                    "label": "Bruger maskiner, materialer og værktøj med påpasselighed og omhyggelighed middel. Rimelig god orden.",
                },
                {
                    "value": 4,
                    "label": "Meget påpasselig både i praktik og teori. God orden.",
                },
                {
                    "value": 5,
                    "label": "I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden.",
                },
            ],
        },
    ]
}


json_questionnaire: str = json.dumps(questionnaire)
