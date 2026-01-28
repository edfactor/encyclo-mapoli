"""Basic API tests."""

import pytest
from fastapi.testclient import TestClient

from api.main import app


@pytest.fixture
def client():
    return TestClient(app)


def test_health_check(client):
    response = client.get("/api/health")
    assert response.status_code == 200
    assert response.json() == {"status": "healthy"}


def test_list_elections(client):
    response = client.get("/api/elections")
    assert response.status_code == 200
    assert isinstance(response.json(), list)


def test_get_election(client):
    response = client.get("/api/elections/140749")
    assert response.status_code == 200
    data = response.json()
    assert data["id"] == "140749"


def test_get_results(client):
    response = client.get("/api/results/140749")
    assert response.status_code == 200
    assert isinstance(response.json(), list)
