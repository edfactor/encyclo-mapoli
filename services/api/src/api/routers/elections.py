from fastapi import APIRouter

from api.models import Election

router = APIRouter()


@router.get("")
async def list_elections(
    query: str | None = None,
    year: int | None = None,
    election_type: str | None = None,
) -> list[Election]:
    """List elections with optional filtering."""
    # TODO: Implement election listing from MA election stats
    return [
        Election(
            id="140749",
            name="Governor's Council - Third District",
            date="2024-11-05",
            type="General",
        )
    ]


@router.get("/{election_id}")
async def get_election(election_id: str) -> Election:
    """Get a single election by ID."""
    # TODO: Implement fetching election details
    return Election(
        id=election_id,
        name="Governor's Council - Third District",
        date="2024-11-05",
        type="General",
    )
