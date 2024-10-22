<script setup lang="ts">
import { useManagerStore } from './store'

const store = useManagerStore()
const router = useRouter()

function logout() {
  store.logout()
  router.push('/login')
}

watch(() => store.token, (newValue) => {
  if (!newValue)
    router.push('/login')
})
</script>

<template>
  <div v-if="store.token" style="position: absolute; top: 10px; right: 10px; display: flex; flex-direction: column;">
    <div>inloggad som {{ store.manager?.email }}</div>
    <button @click="logout">
      logga ut
    </button>
  </div>
  <Suspense>
    <router-view v-slot="{ Component }">
      <component :is="Component" />
    </router-view>
    <template #fallback>
      <div>
        laddar...!
      </div>
    </template>
  </Suspense>
</template>
