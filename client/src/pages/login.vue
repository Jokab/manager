<script setup lang="ts">
import { login as apiLogin } from '@/api'
import { useManagerStore } from '@/store'
import { storeToRefs } from 'pinia'

const router = useRouter()

const store = useManagerStore()

const { manager } = storeToRefs(store)
const email = ref<string>()

async function login() {
  const response = await apiLogin({
    managerEmail: email.value!,
    password: '',
  })
  store.token = response.token

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
  <div v-if="manager?.id">
    Inloggad som: {{ manager?.id }}
  </div>
</template>
