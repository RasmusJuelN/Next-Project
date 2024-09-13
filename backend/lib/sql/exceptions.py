class TemplateNotFoundException(Exception):
    def __init__(self, template_id: str) -> None:
        self.template_id: str = template_id
        self.message: str = f"Template with ID {template_id} not found."
        super().__init__(self.message)
