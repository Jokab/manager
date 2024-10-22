<script setup lang="ts">
import { createTeam as apiCreateTeam } from '@/api'
import { useManagerStore } from '@/store'
import { ref } from 'vue'

const store = useManagerStore()
const teamName = ref<string>()

const response = ref<string>()

async function createTeam() {
  const res = await apiCreateTeam({
    name: teamName.value!,
    managerId: store.manager!.id,
  }, store.token!)
  response.value = res.id
}
</script>

<template>
  <div>
    <div
      style="display: grid; grid: auto-flow / 1fr 1fr;
      justify-items: right; grid-gap: 10px"
    >
      <div>Skapa lag!</div>
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
  </div>
</template>
