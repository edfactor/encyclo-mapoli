from pydantic import BaseModel


class Election(BaseModel):
    """Election metadata."""

    id: str
    name: str
    date: str
    type: str


class ElectionResult(BaseModel):
    """Election result for a precinct."""

    election_id: str
    city_town: str
    ward: str
    precinct: str
    candidates: dict[str, int]
    blanks: int
    total_votes: int


class SearchParams(BaseModel):
    """Search parameters for elections."""

    query: str | None = None
    year: int | None = None
    type: str | None = None
    office: str | None = None
