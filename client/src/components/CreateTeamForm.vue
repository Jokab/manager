<script setup lang="ts">
import { createTeam as apiCreateTeam } from '@/api'
import { ref } from 'vue'

const teamName = ref<string>()

const response = ref<string>()

async function createTeam() {
  const res = await apiCreateTeam({
    name: teamName.value!,
  })
  response.value = res.id
}
</script>

<template>
  Skapa lag!
  <div
    style="display: grid; grid: auto-flow / 1fr 1fr;
      justify-items: right; grid-gap: 10px"
  >
    <label for="teamName">Namn</label>
    <input
      id="teamName"
      v-model="teamName"
      type="text"
    >
  </div>
  <button @click="createTeam">
    Skicka
  </button>
  <div>{{ response }}</div>
</template>
