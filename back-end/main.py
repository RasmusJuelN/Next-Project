from logging import DEBUG, INFO, Logger
from fastapi import FastAPI, HTTPException, Depends, status, Request
from fastapi.responses import JSONResponse, RedirectResponse
from jose import JWTError, ExpiredSignatureError  # type: ignore
from ldap3.core.exceptions import (  # type: ignore
    LDAPException,
    LDAPSocketOpenError,
)
from typing import Dict, Union, List

from external.log_helper.log_helper import LogHelper
from lib._auth import (
    router as auth_router,
    get_token_data,
    is_admin,
    is_student,
    is_teacher,
    TokenData,
)
from lib._questions import questionnaire
from lib._models import Question, AllQuestions


logger: Logger = LogHelper.create_logger(
    logger_name="backend API (main)",
    log_file="logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)

app = FastAPI(root_path="/api/v1")

app.include_router(router=auth_router)


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
    """
    Test endpoint to check if the token is valid and to return the token data.
    """
    return {**token_data.model_dump()}


@app.get(
    path="/protected/elev",
    tags=["users"],
    response_model=TokenData,
    response_description="The token data",
)
async def read_protected_elev(
    token_data: TokenData = Depends(dependency=get_token_data),
    is_elev=Depends(dependency=is_student),
) -> dict[str, str]:
    """
    Test endpoint to check if the token is valid, if the user role matches the required role, and to return the token data.
    """
    return {**token_data.model_dump()}


@app.get(
    path="/protected/laerer",
    tags=["users"],
    response_model=TokenData,
    response_description="The token data",
)
async def read_protected_laerer(
    token_data: TokenData = Depends(dependency=get_token_data),
    is_laerer=Depends(dependency=is_teacher),
) -> dict[str, str]:
    """
    Test endpoint to check if the token is valid, if the user role matches the required role, and to return the token data.
    """
    return {**token_data.model_dump()}


@app.get(
    path="/protected/admin",
    tags=["users"],
    response_model=TokenData,
    response_description="The token data",
)
async def read_protected_admin(
    token_data: TokenData = Depends(dependency=get_token_data),
    is_admin=Depends(dependency=is_admin),
) -> dict[str, str]:
    """
    Test endpoint to check if the token is valid, if the user role matches the required role, and to return the token data.
    """
    return {**token_data.model_dump()}


@app.get(
    path="/questions/",
    response_model=Question,
)
async def read_question(
    id: int,
) -> Dict[str, Union[str, List[Dict[str, Union[int, str]]]]]:
    for question in questionnaire["questions"]:
        if question["id"] == str(object=id):
            return question
    raise HTTPException(
        status_code=status.HTTP_404_NOT_FOUND,
        detail="Question not found",
    )


@app.get(
    path="/questions/all",
    response_model=AllQuestions,
)
async def read_questions() -> (
    Dict[str, List[Dict[str, Union[str, List[Dict[str, Union[int, str]]]]]]]
):
    return questionnaire
