<script setup lang="ts">
import { getDraft as apiGetDraft } from '@/api'
import { useManagerStore } from '@/store'

const store = useManagerStore()
const { params } = useRoute('/drafts/[id]')

const id = ref<string>()

onMounted(async () => {
  const response = await apiGetDraft(params.id, store.token!)
  id.value = response.id
  console.log(response)
})
</script>

<template>
  hej draft! id är: {{ id }}
</template>
