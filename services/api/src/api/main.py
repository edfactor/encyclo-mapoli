from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware

from api.config import settings
from api.routers import elections, results, reports

app = FastAPI(
    title=settings.app_name,
    description="API for Massachusetts election results and statistics",
    version="0.1.0",
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=["http://localhost:5173"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(elections.router, prefix="/api/elections", tags=["elections"])
app.include_router(results.router, prefix="/api/results", tags=["results"])
app.include_router(reports.router, prefix="/api/reports", tags=["reports"])


@app.get("/api/health")
async def health_check():
    """Health check endpoint."""
    return {"status": "healthy"}
