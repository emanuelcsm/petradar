import { ref, computed } from 'vue'

export function useGeolocation() {
  const latitude = ref<number | null>(null)
  const longitude = ref<number | null>(null)
  const error = ref<string | null>(null)
  const isLoading = ref(false)
  const permissionState = ref<PermissionState | 'unknown'>('unknown')

  const isSupported = computed(() => 'geolocation' in navigator)
  const isPermanentlyDenied = computed(() => permissionState.value === 'denied')

  if (isSupported.value && navigator.permissions) {
    navigator.permissions
      .query({ name: 'geolocation' })
      .then((status) => {
        permissionState.value = status.state
      })
      .catch(() => {
        // Permissions API unavailable — permission state discovered on first request
      })
  }

  async function requestLocation(): Promise<void> {
    if (!isSupported.value) {
      error.value = 'Geolocalização não suportada neste navegador.'
      return
    }

    if (permissionState.value === 'denied') {
      error.value =
        'Permissão de localização bloqueada. Habilite nas configurações do navegador e recarregue a página.'
      return
    }

    isLoading.value = true
    error.value = null

    return new Promise((resolve) => {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          latitude.value = position.coords.latitude
          longitude.value = position.coords.longitude
          permissionState.value = 'granted'
          isLoading.value = false
          resolve()
        },
        (err) => {
          if (err.code === GeolocationPositionError.PERMISSION_DENIED) {
            permissionState.value = 'denied'
            error.value =
              'Permissão de localização bloqueada. Habilite nas configurações do navegador e recarregue a página.'
          } else {
            error.value = 'Não foi possível obter sua localização.'
          }
          isLoading.value = false
          resolve()
        },
        { timeout: 10_000 },
      )
    })
  }

  return {
    latitude,
    longitude,
    error,
    isLoading,
    isSupported,
    permissionState,
    isPermanentlyDenied,
    requestLocation,
  }
}
