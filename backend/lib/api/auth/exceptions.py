class MissingSecretKeyError(Exception):
    def __init__(self) -> None:
        super().__init__(
            "A secret key in the settings file under `auth.secret_key` must be set. It is used to sign and verify JWT tokens. It should be a long, random string. One can be generated with `openssl rand -hex 32`."
        )
