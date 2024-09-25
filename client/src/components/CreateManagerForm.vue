<script setup lang="ts">
import { createManager as apiCreateManager } from '@/api'
import { ref } from 'vue'

const managerName = ref<string>()
const managerEmail = ref<string>()

const managerId = ref<string>()

async function createManager() {
  const res = await apiCreateManager({
    name: managerName.value!,
    email: managerEmail.value!,
  })
  managerId.value = res.id
}
</script>

<template>
  Skapa manager!
  <div
    style="display: grid; grid: auto-flow / 1fr 1fr;
      justify-items: right; grid-gap: 10px"
  >
    <label for="managerName">Namn</label>
    <input
      id="managerName"
      v-model="managerName"
      type="text"
    >
    <label for="managerEmail">Mejl</label>
    <input
      id="managerEmail"
      v-model="managerEmail"
      type="email"
    >
  </div>
  <button @click="createManager">
    Skicka
  </button>
  <div v-if="managerId">
    Skapat manager-ID! {{ managerId }}
  </div>
</template>
