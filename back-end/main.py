from logging import DEBUG, INFO, Logger
from fastapi import FastAPI, HTTPException, Depends, status, Request
from fastapi.responses import JSONResponse
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
    is_elev,
    is_laerer,
    TokenData,
)
from lib._questions import questionnaire
from lib._models import Question


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


@app.get(path="/", response_model=dict)
async def read_root() -> dict[str, str]:
    return {"message": "Hello World"}


@app.get(path="/protected", response_model=TokenData)
async def read_protected(
    token_data: TokenData = Depends(dependency=get_token_data),
) -> dict[str, str]:
    return {**token_data.model_dump()}


@app.get(path="/protected/elev", response_model=dict)
async def read_protected_elev(
    token_data: TokenData = Depends(dependency=get_token_data),
    is_elev=Depends(dependency=is_elev),
) -> dict[str, str]:
    return {
        "message": f"Hello, {token_data.full_name}. Your scope is {token_data.scope} and your UUID is {token_data.uuid}"
    }


@app.get(path="/protected/laerer", response_model=dict)
async def read_protected_laerer(
    token_data: TokenData = Depends(dependency=get_token_data),
    is_laerer=Depends(dependency=is_laerer),
) -> dict[str, str]:
    return {
        "message": f"Hello, {token_data.full_name}. Your scope is {token_data.scope} and your UUID is {token_data.uuid}"
    }


@app.get(path="/protected/admin", response_model=dict)
async def read_protected_admin(
    token_data: TokenData = Depends(dependency=get_token_data),
    is_admin=Depends(dependency=is_admin),
) -> dict[str, str]:
    return {
        "message": f"Hello, {token_data.full_name}. Your scope is {token_data.scope} and your UUID is {token_data.uuid}"
    }


@app.get(
    path="/questions/{question_id}",
    response_model=Question,
)
async def read_question(
    question_id: int,
) -> Dict[str, Union[str, List[Dict[str, Union[int, str]]]]]:
    for question in questionnaire["questions"]:
        if question["id"] == str(object=question_id):
            return question
    raise HTTPException(
        status_code=status.HTTP_404_NOT_FOUND,
        detail="Question not found",
    )
