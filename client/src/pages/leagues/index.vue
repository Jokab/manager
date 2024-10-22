<script setup lang="ts">
import { createLeague as apiCreateLeague } from '@/api'
import { useManagerStore } from '@/store'

const store = useManagerStore()
const router = useRouter()

async function createLeague() {
  const response = await apiCreateLeague({}, store.token!)
  router.push({ name: '/leagues/[id]', params: { id: response.id } })
}
</script>

<template>
  <div>
    ligor
    <button @click="createLeague">
      skapa liga
    </button>
  </div>
</template>
