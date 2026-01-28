"""Simple in-memory cache for election data."""

from datetime import datetime, timedelta
from typing import Any

from api.config import settings


class Cache:
    """Simple in-memory cache with TTL."""

    def __init__(self, ttl_seconds: int = settings.cache_ttl_seconds):
        self._cache: dict[str, tuple[Any, datetime]] = {}
        self._ttl = timedelta(seconds=ttl_seconds)

    def get(self, key: str) -> Any | None:
        """Get a value from cache if not expired."""
        if key not in self._cache:
            return None

        value, timestamp = self._cache[key]
        if datetime.now() - timestamp > self._ttl:
            del self._cache[key]
            return None

        return value

    def set(self, key: str, value: Any) -> None:
        """Set a value in the cache."""
        self._cache[key] = (value, datetime.now())

    def clear(self) -> None:
        """Clear all cached values."""
        self._cache.clear()


# Global cache instance
cache = Cache()
