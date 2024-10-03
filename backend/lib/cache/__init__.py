"""
Provides a simple caching mechanism per module.

It uses the cachetools library to create a time-to-live (TTL) cache with a
maximum size of 100 entries and a TTL of 60 seconds. The cache is stored in
a dictionary keyed by the name of the module that imported this module.
This allows each module to have its own cache instance without being aware of other modules'.

Functions:
    `_get_cache_instance`: Retrieves or creates a TTLCache instance for the calling module.
    `retrieve_cache`: Retrieves the TTLCache instance for the calling module.
    `read`: Reads a value from the cache using the specified key.
    `write`: Writes a value to the cache using the specified key.
    `pop`: Removes a value from the cache using the specified key.
    `clear`: Clears all entries from the cache.

Returns:
    None
"""

from cachetools import TTLCache
from inspect import stack
from typing import Any, cast, Optional, Dict
from logging import Logger, DEBUG, INFO

from backend.lib._logger import LogHelper

logger: Logger = LogHelper.create_logger(
    logger_name="backend API (cache)",
    log_file="backend/logs/backend.log",
    file_log_level=DEBUG,
    stream_log_level=INFO,
)

_cache_instances: Dict[str, TTLCache] = {}


def _get_cache_instance() -> TTLCache:
    """
    Retrieves or creates a TTLCache instance for the current module.

    This function checks if a TTLCache instance already exists for the module
    that called this function. If it does not exist, it creates a new TTLCache
    instance with a maximum size of 100 entries and a time-to-live (TTL) of 60
    seconds. The cache instance is then stored in a dictionary keyed by the
    calling module's name. Effectively, each module which imports this module
    will have its own TTLCache instance, without being aware.

    Returns:
        TTLCache: The TTLCache instance associated with the calling module.
    """
    calling_module: str = stack()[3].frame.f_globals["__name__"]

    if calling_module not in _cache_instances:
        _cache_instances[calling_module] = TTLCache(
            maxsize=100, ttl=60
        )  # LRU cache with 100 max entries and 60 second TTL
        logger.debug(
            msg=f"No cache found for module: {calling_module}. Created new cache."
        )

    return cast(TTLCache, _cache_instances[calling_module])


def retrieve_cache() -> TTLCache:
    """
    Retrieves or creates a TTLCache instance for the calling module.

    Returns:
        TTLCache: The TTLCache instance associated with the calling module.
    """
    return _get_cache_instance()


def read(key: str) -> Optional[Any]:
    """
    Retrieve a value from the cache using the given key.

    Args:
        key (str): The key to look up in the cache.

    Returns:
        Optional[Any]: The value associated with the key if it exists in the cache, otherwise None.
    """
    cache: TTLCache = retrieve_cache()
    try:
        value: Any = cache[key]
        logger.debug(msg=f"Cache hit: {key}")
        return value
    except KeyError:
        logger.debug(msg=f"Cache miss: {key}")
        return None


def write(key: str, value: Any) -> None:
    """
    Write a key-value pair to the cache.

    Args:
        key (str): The key to store in the cache.
        value (Any): The value associated with the key.

    Returns:
        None
    """
    cache: TTLCache = retrieve_cache()
    cache[key] = value
    logger.debug(
        msg=f"Cache write: {key}. TTL: {cache.ttl}. {cache.currsize}/{cache.maxsize} entries."
    )


def pop(key: str) -> Optional[Any]:
    """
    Remove and return the value associated with the given key from the cache.

    Args:
        key (str): The key to remove from the cache.

    Returns:
        Optional[Any]: The value associated with the key if it exists, otherwise None.
    """
    cache: TTLCache = retrieve_cache()
    try:
        value: Any = cache.pop(key=key)
        logger.debug(msg=f"Cache pop: {key}.")
        return value
    except KeyError:
        logger.debug(msg=f"Cache pop miss: {key}.")
        return None


def clear() -> None:
    """
    Clears the TTLCache by retrieving the current cache instance and invoking its clear method.

    Returns:
        None
    """
    cache: TTLCache = retrieve_cache()
    cache.clear()
    logger.debug(msg="Cache cleared.")
