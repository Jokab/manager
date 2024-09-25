<script setup lang="ts">
import { getManager as apiGetManager, login as apiLogin } from '@/api'
import { useManagerStore } from '@/store'

const store = useManagerStore()
const email = ref<string>()
const managerId = ref<string>(store.manager?.id || '')

async function login() {
  const response = await apiLogin({
    managerEmail: 'jakob@jakob.com',
    password: 'jakob',
  })
  store.token = response.token
  const response2 = await apiGetManager(response.manager.id, response.token)
  managerId.value = response2.id
  store.manager = response2
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
