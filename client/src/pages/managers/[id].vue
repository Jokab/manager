<script setup lang="ts">
import { getManager as apiGetManager } from '@/api'
import { useManagerStore } from '@/store'
import { storeToRefs } from 'pinia'

const { params } = useRoute('/managers/[id]')
const store = useManagerStore()

const { manager } = storeToRefs(store)

const response = await apiGetManager(params.id, store.token!)
manager.value = response
</script>

<template>
  <div style="display: flex; flex-direction: column; justify-content: space-around;">
    {{ manager?.id }}
    <RouterLink
      to="/leagues"
    >
      skapa liga
    </RouterLink>
    <RouterLink
      :to="{ name: '/create-team' }"
    >
      skapa lag
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
