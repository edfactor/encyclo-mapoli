from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    """Application settings."""

    model_config = SettingsConfigDict(env_prefix="MA_ELECTION_")

    app_name: str = "MA Election API"
    debug: bool = False
    ma_election_stats_base_url: str = "https://electionstats.state.ma.us"
    cache_ttl_seconds: int = 3600  # 1 hour


settings = Settings()
