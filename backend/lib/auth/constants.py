SECRET_KEY = "fcd67c9b07b2d022a3cff8570a1f48b0e73d78abefe3156aa6fde53afacf0210"  # TODO: CHANGE BEFORE DEPLOYMENT AND MOVE TO ENVIRONMENT VARIABLES
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 30

DOMAIN = "dc.next.dev"
LDAP_SERVER = f"ldap://{DOMAIN}"
LDAP_BASE_DN = "DC=NEXT,DC=dev"

SCOPES: dict[str, str] = {"student": "student", "teacher": "teacher", "admin": "admin"}
