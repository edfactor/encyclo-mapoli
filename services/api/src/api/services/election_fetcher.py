"""Service for fetching election data from MA election stats website."""

import httpx

from api.config import settings


async def fetch_election_csv(election_id: str, include_precincts: bool = True) -> str:
    """Fetch CSV data for an election from electionstats.state.ma.us."""
    url = f"{settings.ma_election_stats_base_url}/elections/download/{election_id}/"
    if include_precincts:
        url += "precincts_include:1/"

    async with httpx.AsyncClient() as client:
        response = await client.get(url)
        response.raise_for_status()
        return response.text


async def search_elections(query: str | None = None) -> list[dict]:
    """Search for elections on the MA election stats website."""
    # TODO: Implement election search
    # The website uses a search interface at /elections/search
    # This will need to be implemented based on the actual site structure
    raise NotImplementedError("Election search not yet implemented")
