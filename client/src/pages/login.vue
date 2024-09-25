<script setup lang="ts">
import { getManager as apiGetManager, login as apiLogin } from '@/api'

const email = ref<string>()
const managerId = ref<string>()

async function login() {
  const response = await apiLogin({
    managerEmail: 'jakob@jakob.com',
    password: 'jakob',
  })
  const response2 = await apiGetManager(response.manager.id, response.token)
  managerId.value = response2.id
}
</script>

<template>
  Logga in!
  <div>
    <label for="email">Email</label>
    <input
      id="email"
      v-model="email"
      type="text"
    >
    <button @click="login">
      Logga in
    </button>
  </div>
  <div v-if="managerId">
    Inloggad som: {{ managerId }}
  </div>
</template>
