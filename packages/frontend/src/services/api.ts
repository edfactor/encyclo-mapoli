const API_BASE = '/api'

export interface Election {
  id: string
  name: string
  date: string
  type: string
}

export interface ElectionResult {
  electionId: string
  cityTown: string
  ward: string
  precinct: string
  candidates: Record<string, number>
  blanks: number
  totalVotes: number
}

export async function getElections(): Promise<Election[]> {
  const response = await fetch(`${API_BASE}/elections`)
  if (!response.ok) {
    throw new Error('Failed to fetch elections')
  }
  return response.json()
}

export async function getElection(id: string): Promise<Election> {
  const response = await fetch(`${API_BASE}/elections/${id}`)
  if (!response.ok) {
    throw new Error('Failed to fetch election')
  }
  return response.json()
}

export async function getResults(electionId: string): Promise<ElectionResult[]> {
  const response = await fetch(`${API_BASE}/results/${electionId}`)
  if (!response.ok) {
    throw new Error('Failed to fetch results')
  }
  return response.json()
}
