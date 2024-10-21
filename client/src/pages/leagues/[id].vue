<script setup lang="ts">
import type { DraftDto, TeamDto } from '@/api'
import { admitTeam as apiAdmitTeam, createDraft as apiCreateDraft, getLeague, getTeam } from '@/api'
import { useManagerStore } from '@/store'

const router = useRouter()
const store = useManagerStore()
const { params } = useRoute('/leagues/[id]')

const teams = ref<TeamDto[]>()
const drafts = ref<DraftDto[]>()
const teamIdToAdd = ref<string>()

const response = await getLeague(params.id, store.token!)
teams.value = response.teams
drafts.value = response.drafts

async function createDraft() {
  const response = await apiCreateDraft({
    leagueId: params.id,
  }, store.token!)
  router.push({ name: '/drafts/[id]', params: { id: response.id } })
}

async function admitTeam() {
  const team = await getTeam(teamIdToAdd.value!, store.token!)
  const _ = await apiAdmitTeam({
    leagueId: params.id,
    teamId: team.id,
  }, store.token!)
}
</script>

<template>
  <div style="display: flex; flex-direction: column;">
    teams
    <ul>
      <li v-for="team in teams" :key="team.id">
        {{ team.name }}
      </li>
    </ul>
    drafts
    <ul>
      <li v-for="draft in drafts" :key="draft.id">
        <RouterLink :to="{ name: '/drafts/[id]', params: { id: draft.id } }">
          Draft {{ draft.id.split("-")[0] }}
        </RouterLink>
      </li>
    </ul>
    <button @click="createDraft">
      Ny draft
    </button>
    <form>
      <label>
        Lägg till lag
        <input v-model="teamIdToAdd" required type="text">
      </label>
      <button @click.prevent="admitTeam">
        Lägg till
      </button>
    </form>
  </div>
</template>
