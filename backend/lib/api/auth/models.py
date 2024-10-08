from pydantic import BaseModel, ConfigDict, AliasGenerator, Field, field_validator
from pydantic.alias_generators import to_camel
from typing import Sequence
from fastapi import HTTPException, status
from string import ascii_letters, digits

from backend import app_settings


class CamelBaseModel(BaseModel):
    model_config = ConfigDict(
        alias_generator=AliasGenerator(
            validation_alias=to_camel,
            serialization_alias=to_camel,
        ),
        populate_by_name=True,
    )


class TokenData(CamelBaseModel):
    username: str
    full_name: str
    scope: str
    uuid: str


class User(CamelBaseModel):
    """
    Represents a user in the system.

    Attributes:
        id (str): A hashed version of the user's UUID in the LDAP server.
        user_name (str): The username of the user.
        full_name (str): The full name of the user.
        role (str): The role of the user within the system.
    """

    id: str
    user_name: str
    full_name: str
    role: str


class UserSearchRequest(CamelBaseModel):
    """
    UserSearchRequest model for handling user search requests.

    Validates that the role is a valid role defined in app settings and that the search query only contains letters, digits, and spaces. Converts the search query to lowercase.

    Attributes:
        role (str): The role of the user making the search request. Must be a valid role defined in app settings.
        search_query (str): The search query string. Must only contain letters, digits, and spaces.
        page (int): The page number for pagination. Defaults to 1. Must be greater than or equal to 1.
        limit (int): The number of results per page. Defaults to 10. Must be between 1 and 10 inclusive.
    """

    role: str
    search_query: str
    page: int = Field(default=1, ge=1)
    limit: int = Field(default=10, le=10, ge=1)

    @field_validator("role")
    @classmethod
    def validate_role(cls, value: str) -> str:
        if value not in app_settings.settings.auth.scopes:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=f"Invalid role: {value}",
            )
        return value

    @field_validator("search_query")
    @classmethod
    def validate_search_query(cls, value: str) -> str:
        if not all(char in ascii_letters + digits + " " for char in value):
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="Search query must only contain letters, digits, and spaces",
            )
        return value.lower()


class UserSearchResponse(CamelBaseModel):
    """
    UserSearchResponse model.

    Attributes:
        users (Sequence[User]): A sequence of User objects representing the search results.
    """

    users: Sequence[User]
