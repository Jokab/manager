<template>
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
