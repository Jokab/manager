<script setup lang="ts">
import type { DraftDto, TeamDto } from '@/api'
import { admitTeam as apiAdmitTeam, createDraft as apiCreateDraft, getLeague, getTeam } from '@/api'
import { useManagerStore } from '@/store'

const store = useManagerStore()
const { params } = useRoute('/teams/[id]')

const team = ref<TeamDto>()

team.value = await getTeam(params.id, store.token!)
</script>

<template>
  <div style="display: flex; flex-direction: column;">
    liga
    <RouterLink v-if="team && team.league" :to="{ name: '/leagues/[id]', params: { id: team.league.id } }">
      Liga {{ team.league.id.split("-")[0] }}
    </RouterLink>
  </div>
</template>
