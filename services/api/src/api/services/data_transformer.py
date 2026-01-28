"""Service for parsing and transforming election CSV data."""

import csv
from io import StringIO

from api.models import ElectionResult


def parse_election_csv(csv_content: str, election_id: str) -> list[ElectionResult]:
    """Parse CSV content from MA election stats into structured results."""
    results: list[ElectionResult] = []
    reader = csv.DictReader(StringIO(csv_content))

    for row in reader:
        # Skip totals row if present
        if row.get("City/Town", "").lower() == "totals":
            continue

        # Extract candidate columns (everything except known columns)
        known_columns = {"City/Town", "Ward", "Pct", "All Others", "Blanks", "Total Votes Cast"}
        candidates = {}

        for key, value in row.items():
            if key not in known_columns and value:
                try:
                    candidates[key] = int(value.replace(",", ""))
                except ValueError:
                    continue

        # Handle "All Others" as a candidate entry if present
        if row.get("All Others"):
            try:
                candidates["All Others"] = int(row["All Others"].replace(",", ""))
            except ValueError:
                pass

        result = ElectionResult(
            election_id=election_id,
            city_town=row.get("City/Town", ""),
            ward=row.get("Ward", ""),
            precinct=row.get("Pct", ""),
            candidates=candidates,
            blanks=int(row.get("Blanks", "0").replace(",", "") or 0),
            total_votes=int(row.get("Total Votes Cast", "0").replace(",", "") or 0),
        )
        results.append(result)

    return results
