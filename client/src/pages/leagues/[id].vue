<script setup lang="ts">
import type { DraftDto, TeamDto } from '@/api'
import { admitTeam as apiAdmitTeam, createDraft as apiCreateDraft, getLeague, getTeam } from '@/api'
import { useManagerStore } from '@/store'

const store = useManagerStore()
const { params } = useRoute('/leagues/[id]')

const teams = ref<TeamDto[]>()
const drafts = ref<DraftDto[]>()
const teamIdToAdd = ref<string>()

onMounted(async () => {
  const response = await getLeague(params.id, store.token!)
  teams.value = response.teams
  drafts.value = response.drafts
})

async function createDraft() {
  const response = await apiCreateDraft({
    leagueId: params.id,
  }, store.token!)
  console.log(response)
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
        {{ draft.id }}
      </li>
    </ul>
    <button @click="createDraft">
      Skapa draft
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
