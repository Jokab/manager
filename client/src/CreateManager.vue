<script setup lang="ts">
import { useFetch } from '@vueuse/core';
import { ref } from 'vue';
const managerName = ref<string>();
const managerEmail = ref<string>();

const response = ref<string>();

console.log(import.meta.env.VITE_API_URL);
async function createManager() {
  const { data } = await useFetch(import.meta.env.VITE_API_URL + "/api/managers").post({
    name: {
      name: managerName.value
    },
    email: {
      emailAddress: managerEmail.value
    }
  }).json();
  response.value = data.value ;
}
</script>

<template>
  <div
    style="display: grid; grid: auto-flow / 1fr 1fr;
      justify-items: right; grid-gap: 10px"
  >
    <label for="managerName">Namn </label>
    <input
      id="managerName"
      v-model="managerName"
      type="text"
    >
    <label for="managerEmail">Mejl </label>
    <input
      id="managerEmail"
      v-model="managerEmail"
      type="email"
    >
  </div>
  <button @click="createManager">
    Skicka
  </button>
  <div>{{ response }}</div>
</template>
