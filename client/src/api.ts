// 1. Login (POST /api/login)
export interface LoginRequest {
  managerEmail: string
  password: string
}

export interface LoginDto {
  manager: ManagerDto
  token: string
}

export async function login(requestBody: LoginRequest): Promise<LoginDto> {
  return await fetchWithAuth('/login', 'POST', null, requestBody)
}

// 2. CreateManager (POST /api/managers)
export interface CreateManagerRequest {
  name: string
  email: string
}

export interface ManagerDto {
  id: string // Guid in C# maps to string in TypeScript
  createdDate: string // DateTime in C# can be represented as string (ISO date format) in TypeScript
  updatedDate: string
  deletedDate?: string | null // Nullable DateTime can be either string or null
  name: string // Assuming ManagerName is a string-like object
  email: string // Assuming Email is a string-like object
  // teams: Team[] // Assuming Team is another export interface defined elsewhere
}

export interface DraftDto {
  id: string
  createdDate: string
  updatedDate: string
  deletedDate?: string | null
  leagueId: string
  state: string
}

export interface StartDraftDto {
  id: string
  state: string
}

export interface LeagueDto {
  id: string
  createdDate: string
  updatedDate: string
  deletedDate?: string | null
  teams: TeamDto[]
  drafts: DraftDto[]
}

export interface TeamDto {
  id: string
  name: string
  managerId: string
}

export async function createManager(requestBody: CreateManagerRequest): Promise<ManagerDto> {
  return await fetchWithAuth('/managers', 'POST', null, requestBody)
}

// 3. GetManager (GET /api/managers/{id}) - Requires JWT token
export async function getManager(id: string, token: string): Promise<ManagerDto> {
  return await fetchWithAuth(`/managers/${id}`, 'GET', token)
}

// 4. CreateTeam (POST /api/teams) - Requires JWT token
export interface CreateTeamRequest {
  name: string
  managerId: string
}

export async function createTeam(requestBody: CreateTeamRequest, token: string) {
  return await fetchWithAuth('/teams', 'POST', token, requestBody)
}

// 5. GetTeam (GET /api/teams/{id}) - Requires JWT token
export async function getTeam(id: string, token: string): Promise<TeamDto> {
  return await fetchWithAuth(`/teams/${id}`, 'GET', token)
}

// 6. SignPlayer (POST /api/teams/sign) - Requires JWT token
export interface SignPlayerRequest {
  teamId: string
  playerId: string
}

export async function signPlayer(requestBody: SignPlayerRequest, token: string): Promise<void> {
  return await fetchWithAuth('/teams/sign', 'POST', token, requestBody)
}

export async function getDraft(id: string, token: string): Promise<DraftDto> {
  return await fetchWithAuth(`/drafts/${id}`, 'GET', token)
}

// 7. CreateDraft (POST /api/drafts) - Requires JWT token
export interface CreateDraftRequest {
  leagueId: string
}

export async function createDraft(requestBody: CreateDraftRequest, token: string): Promise<DraftDto> {
  return await fetchWithAuth('/drafts', 'POST', token, requestBody)
}

// 8. StartDraft (POST /api/drafts/start) - Requires JWT token
export interface StartDraftRequest {
  draftId: string
}

export async function startDraft(requestBody: StartDraftRequest, token: string): Promise<StartDraftDto> {
  return await fetchWithAuth('/drafts/start', 'POST', token, requestBody)
}

// 9. CreateLeague (POST /api/leagues) - Requires JWT token
export interface CreateLeagueRequest {}

export async function createLeague(requestBody: CreateLeagueRequest, token: string) {
  return await fetchWithAuth('/leagues', 'POST', token, requestBody)
}

export async function getLeague(id: string, token: string): Promise<LeagueDto> {
  return await fetchWithAuth(`/leagues/${id}`, 'GET', token)
}

// 10. AdmitTeam (POST /api/leagues/admitTeam) - Requires JWT token
export interface AdmitTeamRequest {
  leagueId: string
  teamId: string
}

export async function admitTeam(requestBody: AdmitTeamRequest, token: string) {
  return await fetchWithAuth('/leagues/admitTeam', 'POST', token, requestBody)
}

export interface PlayerDto {
  id: string
  createdDate: string
  updatedDate: string
  deletedDate: string | null
  country: string
  name: string
  teamId: string
  position: string
  isSigned: boolean
}

export async function getPlayers(): Promise<PlayerDto[]> {
  return await fetchWithAuth('/players', 'GET')
}

// Helper function to handle fetch requests
async function fetchWithAuth(url: string, method: string, token: string | null = null, body: any = null) {
  const headers: HeadersInit = {
    'Content-Type': 'application/json',
  }

  if (token) {
    headers.Authorization = `Bearer ${token}`
  }

  const options: RequestInit = {
    method,
    headers,
    body: body ? JSON.stringify(body) : null,
  }

  const response = await fetch(`${import.meta.env.VITE_API_URL}/api${url}`, options)

  if (!response.ok) {
    throw new Error(`Failed to ${method} ${url}`)
  }

  return await response.json()
}
