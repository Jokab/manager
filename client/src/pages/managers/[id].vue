<script setup lang="ts">
import { getManager as apiGetManager, login as apiLogin } from '@/api'
import { useManagerStore } from '@/store'

const { params } = useRoute('/managers/[id]')
const store = useManagerStore()

const managerId = ref<string>()

onMounted(async () => {
  const response2 = await apiGetManager(params.id, store.token!)
  managerId.value = response2.id
  store.manager = response2
})
</script>

<template>
  {{ managerId }}
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
</template>
