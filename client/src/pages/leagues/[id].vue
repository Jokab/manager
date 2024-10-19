<script setup lang="ts">
import { createDraft as apiCreateDraft } from '@/api'
import { useManagerStore } from '@/store'

const store = useManagerStore()
const { params } = useRoute('/leagues/[id]')

async function createDraft() {
  const response = await apiCreateDraft({
    leagueId: params.id,
  }, store.token!)
  console.log(response)
}
</script>

<template>
  hej draft
  <button @click="createDraft">
    Skapa draft
  </button>
</template>
