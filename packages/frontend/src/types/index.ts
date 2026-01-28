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

export interface SearchParams {
  query?: string
  year?: number
  type?: string
  office?: string
}
