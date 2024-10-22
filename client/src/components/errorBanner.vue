<script setup lang="ts">
const { errorState, clearError } = useErrorHandler()
</script>

<template>
  <div v-if="errorState.hasError" class="error-banner">
    <p>{{ errorState.errorMessage }}</p>
    <button @click="clearError">
      Dismiss
    </button>
  </div>
</template>

<style scoped>
.error-banner {
  background-color: #f8d7da;
  color: #721c24;
  padding: 1rem;
  border-radius: 5px;
  margin: 1rem 0;
}
</style>
