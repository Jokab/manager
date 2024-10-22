import { ref } from 'vue'

const errorState = ref({
  hasError: false,
  errorMessage: '',
})

function setError(message: string) {
  errorState.value.hasError = true
  errorState.value.errorMessage = message
}

function clearError() {
  errorState.value.hasError = false
  errorState.value.errorMessage = ''
}

export function useErrorHandler() {
  return {
    errorState,
    setError,
    clearError,
  }
}
