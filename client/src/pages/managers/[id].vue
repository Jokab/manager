<script setup lang="ts">
import type { ManagerDto } from '@/api'
import { getManager as apiGetManager, login as apiLogin, getTeam } from '@/api'
import { useManagerStore } from '@/store'
import { storeToRefs } from 'pinia'

const { params } = useRoute('/managers/[id]')
const store = useManagerStore()

const { manager } = storeToRefs(store)

const response2 = await apiGetManager(params.id, store.token!)
manager.value = response2

// watch(() => store.manager, async (val: ManagerDto | undefined) => {
//   if (val) {
//     await getTeams()
//   }
// })
</script>

<template>
  <div style="display: flex; flex-direction: column; justify-content: space-around;">
    {{ manager?.id }}
    <RouterLink
      to="/drafts"
    >
      draft
    </RouterLink>
    <RouterLink
      to="/leagues"
    >
      skapa liga
    </RouterLink>
    <div>
      <div>lag</div>
      <ul v-if="manager">
        <RouterLink v-for="team in manager.teams" :key="team.id" :to="{ name: '/teams/[id]', params: { id: team.id } }">
          - {{ team.name }}
        </RouterLink>
      </ul>
    </div>
  </div>
</template>
