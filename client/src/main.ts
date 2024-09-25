import { createApp } from 'vue'
import { createRouter, createWebHistory } from 'vue-router'
import { routes } from 'vue-router/auto-routes'
import app from './app.vue'

import '@unocss/reset/tailwind.css'
import './styles/main.css'
import 'uno.css'

const a = createApp(app)
const router = createRouter({
  routes,
  history: createWebHistory(import.meta.env.BASE_URL),
})
a.use(router)
a.mount('#app')
