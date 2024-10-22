import type { ManagerDto } from '@/api'
import { defineStore } from 'pinia'

export const useManagerStore = defineStore('manager', () => {
  const token = ref<string>()
  const manager = ref<ManagerDto>()

  function $reset() {
    token.value = undefined
    manager.value = undefined
  }

  function logout() {
    token.value = undefined
    manager.value = undefined
  }
  return { token, manager, $reset, logout }
}, {
  persist: true,
})
