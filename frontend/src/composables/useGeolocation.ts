import { ref, computed } from 'vue'

export function useGeolocation() {
  const latitude = ref<number | null>(null)
  const longitude = ref<number | null>(null)
  const error = ref<string | null>(null)
  const isLoading = ref(false)

  const isSupported = computed(() => 'geolocation' in navigator)

  async function requestLocation(): Promise<void> {
    if (!isSupported.value) {
      error.value = 'Geolocalização não suportada neste navegador.'
      return
    }

    isLoading.value = true
    error.value = null

    return new Promise((resolve) => {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          latitude.value = position.coords.latitude
          longitude.value = position.coords.longitude
          isLoading.value = false
          resolve()
        },
        (err) => {
          error.value =
            err.code === GeolocationPositionError.PERMISSION_DENIED
              ? 'Permissão de localização negada.'
              : 'Não foi possível obter sua localização.'
          isLoading.value = false
          resolve()
        },
        { timeout: 10_000 },
      )
    })
  }

  return { latitude, longitude, error, isLoading, isSupported, requestLocation }
}
