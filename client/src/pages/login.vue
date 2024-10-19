<script setup lang="ts">
import { getManager as apiGetManager, login as apiLogin } from '@/api'
import { useManagerStore } from '@/store'

const router = useRouter()

const store = useManagerStore()
const email = ref<string>()
const managerId = ref<string>(store.manager?.id || '')

async function login() {
  const response = await apiLogin({
    managerEmail: email.value!,
    password: '',
  })
  store.token = response.token
  // const response2 = await apiGetManager(response.manager.id, response.token)
  // managerId.value = response2.id
  // store.manager = response2

  router.push({ name: '/managers/[id]', params: { id: response.manager.id } })
}
</script>

<template>
  Logga in!
  <div>
    <form>
      <label>
        Email
        <input
          id="email"
          v-model="email"
          type="email"
          required
        >
      </label>
      <button @click.prevent="login">
        Logga in
      </button>
    </form>
  </div>
  <div v-if="managerId">
    Inloggad som: {{ managerId }}
  </div>
</template>
