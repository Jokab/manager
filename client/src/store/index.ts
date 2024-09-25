import type { ManagerDto } from '@/api'
import { defineStore } from 'pinia'

export const useManagerStore = defineStore('manager', () => {
  const token = ref<string>()
  const manager = ref<ManagerDto>()

  return { token, manager }
}, {
  persist: true,
})
