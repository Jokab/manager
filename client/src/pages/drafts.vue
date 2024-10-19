<script setup lang="ts">
import { createDraft as apiCreateDraft } from '@/api'
import { useManagerStore } from '@/store'

const store = useManagerStore()

async function createDraft() {
  const response = await apiCreateDraft({
    leagueId: 'hejhopp',

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
