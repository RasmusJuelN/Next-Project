from logging import DEBUG, INFO, Logger
from fastapi import FastAPI, HTTPException, Depends, status, Request
from fastapi.responses import JSONResponse, RedirectResponse
from fastapi.middleware.cors import CORSMiddleware
from jose import JWTError, ExpiredSignatureError  # type: ignore
from ldap3.core.exceptions import (  # type: ignore
    LDAPException,
    LDAPSocketOpenError,
)
from typing import Dict, Union, List

from .lib._logger import LogHelper
from .lib.auth.routes import router as auth_router
from .lib.auth.models import TokenData
from .lib.auth.dependencies import get_token_data, is_student, is_teacher, is_admin
from .lib.questions import questionnaire
from .lib.models import Question, AllQuestions
from .lib.i18n_middleware import I18nMiddleware, Translator

# noqa: W291 (trailing whitespace) prevents Flake8 from complaining about trailing whitespace. Used for docstrings.
# fmt: off/on (black formatting) disables/enables Black formatting for the code block. Used for docstrings.

logger: Logger = LogHelper.create_logger(
    logger_name="backend API (main)",
    log_file="logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)

app = FastAPI(root_path="/api/v1")

app.add_middleware(
    middleware_class=CORSMiddleware,
    allow_origins=["http://localhost", "https://localhost"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(router=auth_router)
app.add_middleware(middleware_class=I18nMiddleware)


@app.exception_handler(exc_class_or_status_code=HTTPException)
async def http_exception_handler(request: Request, exc: HTTPException) -> JSONResponse:
    return JSONResponse(
        status_code=exc.status_code,
        content={"detail": exc.detail},
    )


@app.exception_handler(exc_class_or_status_code=LDAPException)
async def ldap_exception_handler(request: Request, exc: LDAPException) -> JSONResponse:
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
async def jwt_exception_handler(request: Request, exc: JWTError) -> JSONResponse:
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
async def read_root() -> RedirectResponse:
    return RedirectResponse(url="/docs")


@app.get(
    path="/protected",
    response_model=TokenData,
    tags=["users"],
    response_description="The token data",
)
async def read_protected(
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


@app.get(
    path="/protected/student",
    tags=["users"],
    response_model=TokenData,
    response_description="The token data",
    dependencies=[Depends(dependency=is_student)],
)
async def read_protected_student(
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


@app.get(
    path="/protected/teacher",
    tags=["users"],
    response_model=TokenData,
    response_description="The token data",
    dependencies=[Depends(dependency=is_teacher)],
)
async def read_protected_teacher(
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


@app.get(
    path="/protected/admin",
    tags=["users"],
    response_model=TokenData,
    response_description="The token data",
    dependencies=[Depends(dependency=is_admin)],
)
async def read_protected_admin(
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


@app.get(
    path="/questions/",
    response_model=Question,
)
async def read_question(
    id: int,
    request: Request,
) -> Dict[str, Union[str, List[Dict[str, Union[int, str]]]]]:
    """
    Call this endpoint with the ID of the question you want to retrieve.

    - **arg**:  `/api/v1/questions/?id={id}` (int): The ID of the question to retrieve.

    - **return**:  `{"id": int, "text": str, "options": [{"value": int, "label": str}]}`: The question with the specified ID.

    - **raises**:  `HTTPException`: A 404 error if the question is not found.
    """
    translator = Translator(lang=request.state.language)
    for question in questionnaire["questions"]:
        if question["id"] == str(object=id):
            return question
    raise HTTPException(
        status_code=status.HTTP_404_NOT_FOUND,
        detail=f"{translator.t(key='errors.question_not_found')}: {id}",
    )


@app.get(
    path="/questions/all",
    response_model=AllQuestions,
)
async def read_questions(
    request: Request,
) -> Dict[str, List[Dict[str, Union[str, List[Dict[str, Union[int, str]]]]]]]:
    """
    Call this endpoint to retrieve all questions.

    - **arg**:  `/api/v1/questions/all`

    - **return**:  `{"questions": [{"id": int, "text": str, "options": [{"value": int, "label": str}]}]}`: All questions.

    - **raises**:  `HTTPException`: A 404 error if no questions are found.
    """
    translator = Translator(lang=request.state.language)
    if len(questionnaire["questions"]) > 0:
        return questionnaire
    raise HTTPException(
        status_code=status.HTTP_404_NOT_FOUND,
        detail=translator.t(key="errors.no_questions_found"),
    )
