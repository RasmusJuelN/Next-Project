class TemplateNotFoundException(Exception):
    def __init__(self, template_id: str) -> None:
        self.template_id: str = template_id
        self.message: str = f"Template with ID {template_id} not found."
        super().__init__(self.message)


class TemplateAlreadyExistsException(Exception):
    def __init__(self, template_id: str) -> None:
        self.template_id: str = template_id
        self.message: str = f"Template with ID {template_id} already exists."
        super().__init__(self.message)


class TemplateIdMismatchException(Exception):
    def __init__(self, existing_template_id: str, updated_template_id: str) -> None:
        self.existing_template_id: str = existing_template_id
        self.updated_template_id: str = updated_template_id
        self.message: str = (
            f"Template ID {existing_template_id} does not match the ID {updated_template_id} in the request body."
        )
        super().__init__(self.message)


class TemplateCreationError(Exception):
    def __init__(self, template_id: str) -> None:
        self.template_id: str = template_id
        self.message: str = f"Error creating template with ID {template_id}."
        super().__init__(self.message)


class QuestionnaireNotFound(Exception):
    def __init__(self, questionnaire_id: str) -> None:
        self.questionnaire_id: str = questionnaire_id
        self.message: str = f"Questionnaire with ID {questionnaire_id} not found."
        super().__init__(self.message)
