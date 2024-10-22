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
  <ErrorBanner />
  <div v-if="store.token && store.manager" style="position: absolute; top: 10px; right: 10px; display: flex; flex-direction: column;">
    <div>
      inloggad som <RouterLink :to="{ name: '/managers/[id]', params: { id: store.manager.id } }">
        {{ store.manager?.email }}
      </RouterLink>
    </div>
    <button @click="logout">
      logga ut
    </button>
  </div>
  <RouterView v-slot="{ Component }">
    <template v-if="Component">
      <Transition mode="out-in">
        <KeepAlive>
          <Suspense>
            <component :is="Component" />
            <template #fallback>
              Laddar...
            </template>
          </Suspense>
        </KeepAlive>
      </Transition>
    </template>
  </RouterView>
</template>
