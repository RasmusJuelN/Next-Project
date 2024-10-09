from logging import DEBUG, INFO, Logger
from fastapi import FastAPI, HTTPException, Depends, status, Request
from fastapi.responses import JSONResponse, RedirectResponse
from fastapi.middleware.cors import CORSMiddleware
from jose import JWTError, ExpiredSignatureError
from ldap3.core.exceptions import (
    LDAPException,
    LDAPSocketOpenError,
)
from typing_extensions import deprecated

from backend.lib._logger import LogHelper
from backend.lib.api.auth.routes import router as auth_router
from backend.lib.api.questionnaire.routes import router as questionnaire_router
from backend.lib.api.settings.routes import router as settings_router
from backend.lib.api.logs.routes import router as logs_router
from backend.lib.api.auth.models import TokenData
from backend.lib.api.auth.dependencies import (
    get_token_data,
    is_student,
    is_teacher,
    is_admin,
)
from backend.lib.i18n_middleware import I18nMiddleware
from backend.lib.sql.database import engine
from backend.lib.sql.models import Base

# noqa: W291 (trailing whitespace) prevents Flake8 from complaining about trailing whitespace. Used for docstrings.
# fmt: off/on (black formatting) disables/enables Black formatting for the code block. Used for docstrings.

logger: Logger = LogHelper.create_logger(
    logger_name="backend API (main)",
    log_file="backend/logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)

# Drop and recreate the database tables.
# TODO: This is only for development purposes. Remove this in production.
# Base.metadata.drop_all(bind=engine, checkfirst=True)
Base.metadata.create_all(bind=engine, checkfirst=True)


app = FastAPI(root_path="/api/v1")

app.add_middleware(
    middleware_class=CORSMiddleware,
    allow_origins=["http://localhost", "https://localhost"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(router=auth_router)
app.include_router(router=questionnaire_router)
app.include_router(router=settings_router)
app.include_router(router=logs_router)
app.add_middleware(middleware_class=I18nMiddleware)


@app.exception_handler(exc_class_or_status_code=HTTPException)
def http_exception_handler(request: Request, exc: HTTPException) -> JSONResponse:
    logger.error(
        msg=f"HTTPException occurred: {exc.status_code} - {exc.detail}",
    )
    return JSONResponse(
        status_code=exc.status_code,
        content={"detail": exc.detail},
    )


@app.exception_handler(exc_class_or_status_code=LDAPException)
def ldap_exception_handler(request: Request, exc: LDAPException) -> JSONResponse:
    if isinstance(exc, LDAPSocketOpenError):
        logger.exception(
            msg="Timed out trying to connect to server.",
            exc_info=True,
        )
        return JSONResponse(
            status_code=status.HTTP_503_SERVICE_UNAVAILABLE,
            content={"detail": "Timed out trying to connect to server"},
        )
    else:
        logger.exception(msg=f"Unexpected error occurred: {exc}")
        return JSONResponse(
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            content={
                "detail": "An unexpected error occurred while trying to authenticate. Please try again later."
            },
        )


@app.exception_handler(exc_class_or_status_code=JWTError)
def jwt_exception_handler(request: Request, exc: JWTError) -> JSONResponse:
    if isinstance(exc, ExpiredSignatureError):
        return JSONResponse(
            status_code=status.HTTP_401_UNAUTHORIZED,
            content={"detail": "Token has expired"},
        )
    else:
        return JSONResponse(
            status_code=status.HTTP_401_UNAUTHORIZED,
            content={"detail": "Invalid token"},
        )


@app.get(path="/", include_in_schema=False)
def read_root() -> RedirectResponse:
    return RedirectResponse(url="/docs")


@deprecated("This endpoint is deprecated. It was used for testing purposes only.")
@app.get(
    path="/protected",
    response_model=TokenData,
    tags=["users"],
    response_description="The token data",
)
def read_protected(
    token_data: TokenData = Depends(dependency=get_token_data),
) -> dict[str, str]:
    # fmt: off
    """
    Test endpoint to check if the token is valid and to return the token data.

    username: NH, MF, AMT, userteacher or sysadmin  
    password: Pa$$w0rd

    - **arg**:  `/api/v1/protected`

    - **return**:  `{"username": str, "full_name": str, "scope": str, "uuid": str}`: The token data.

    - **raises**:  `HTTPException`: A 401 error if the token is invalid
    """  # noqa: W291
    # fmt: on
    return {**token_data.model_dump()}


@deprecated("This endpoint is deprecated. It was used for testing purposes only.")
@app.get(
    path="/protected/student",
    tags=["users"],
    response_model=TokenData,
    response_description="The token data",
    dependencies=[Depends(dependency=is_student)],
)
def read_protected_student(
    token_data: TokenData = Depends(dependency=get_token_data),
) -> dict[str, str]:
    # fmt: off
    """
    Test endpoint to check if the token is valid, if the user role matches the required role, and to return the token data.

    username: NH, MF or AMT  
    password: Pa$$w0rd

    - **arg**:  `/api/v1/protected/student`

    - **return**:  `{"username": str, "full_name": str, "scope": str, "uuid": str}`: The token data.

    - **raises**:  `HTTPException`: A 401 error if the token is invalid or a 403 error if the user does not have the required permissions
    """  # noqa: W291
    # fmt: on

    return {**token_data.model_dump()}


@deprecated("This endpoint is deprecated. It was used for testing purposes only.")
@app.get(
    path="/protected/teacher",
    tags=["users"],
    response_model=TokenData,
    response_description="The token data",
    dependencies=[Depends(dependency=is_teacher)],
)
def read_protected_teacher(
    token_data: TokenData = Depends(dependency=get_token_data),
) -> dict[str, str]:
    # fmt: off
    """
    Test endpoint to check if the token is valid, if the user role matches the required role, and to return the token data.

    username: userteacher  
    password: Pa$$w0rd

    - **arg**:  `/api/v1/protected/teacher`

    - **return**:  `{"username": str, "full_name": str, "scope": str, "uuid": str}`: The token data.

    - **raises**:  `HTTPException`: A 401 error if the token is invalid or a 403 error if the user does not have the required permissions
    """  # noqa: W291
    # fmt: on
    return {**token_data.model_dump()}


@deprecated("This endpoint is deprecated. It was used for testing purposes only.")
@app.get(
    path="/protected/admin",
    tags=["users"],
    response_model=TokenData,
    response_description="The token data",
    dependencies=[Depends(dependency=is_admin)],
)
def read_protected_admin(
    token_data: TokenData = Depends(dependency=get_token_data),
) -> dict[str, str]:
    # fmt: off
    """
    Test endpoint to check if the token is valid, if the user role matches the required role, and to return the token data.

    username: sysadmin  
    password: Pa$$w0rd

    - **arg**:  `/api/v1/protected/admin`

    - **return**:  `{"username": str, "full_name": str, "scope": str, "uuid": str}`: The token data.

    - **raises**:  `HTTPException`: A 401 error if the token is invalid or a 403 error if the user does not have the required permissions
    """  # noqa: W291
    # fmt: on
    return {**token_data.model_dump()}
