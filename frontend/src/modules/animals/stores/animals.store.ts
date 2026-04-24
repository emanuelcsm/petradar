import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { animalsService } from '../services/animals.service'
import type { AnimalCardDto, AnimalPostedEventDto } from '@/types/api.types'

export const useAnimalsStore = defineStore('animals', () => {
  const items = ref<AnimalCardDto[]>([])
  const nextPageToken = ref<string | null>(null)
  const hasNextPage = ref(false)
  const isLoading = ref(false)
  const error = ref<string | null>(null)
  const isCreating = ref(false)
  const createError = ref<string | null>(null)

  const selectedAnimal = ref<AnimalCardDto | null>(null)
  const isLoadingDetail = ref(false)
  const detailError = ref<string | null>(null)

  const isSendingTip = ref(false)
  const isDeletingAnimal = ref(false)

  const hasItems = computed(() => items.value.length > 0)
  const hasError = computed(() => error.value !== null)

  async function fetchNearby(
    lat: number,
    lng: number,
    radius: number,
    pageToken?: string,
  ): Promise<void> {
    isLoading.value = true
    error.value = null

    try {
      const result = await animalsService.getNearby(lat, lng, radius, pageToken)

      if (pageToken) {
        items.value.push(...result.data)
      } else {
        items.value = result.data
      }

      nextPageToken.value = result.pagination.nextPageToken
      hasNextPage.value = result.pagination.hasNextPage
    } catch {
      error.value = 'Não foi possível carregar os animais.'
    } finally {
      isLoading.value = false
    }
  }

  async function uploadMedia(file: File): Promise<string | null> {
    try {
      const result = await animalsService.uploadMedia(file)
      return result.data.mediaId
    } catch {
      return null
    }
  }

  async function createAnimal(
    description: string,
    latitude: number,
    longitude: number,
    mediaIds: string[],
  ): Promise<void> {
    isCreating.value = true
    createError.value = null
    try {
      await animalsService.create(description, latitude, longitude, mediaIds)
    } catch {
      createError.value = 'Não foi possível publicar o animal. Tente novamente.'
    } finally {
      isCreating.value = false
    }
  }

  function addFromSignalR(animal: AnimalCardDto): void {
    const exists = items.value.some((a) => a.id === animal.id)
    if (!exists) {
      items.value.unshift(animal)
    }
  }

  async function handleAnimalPostedEvent(payload: AnimalPostedEventDto): Promise<void> {
    try {
      const result = await animalsService.getById(payload.animalPostId)
      addFromSignalR(result.data)
    } catch {
      // falha silenciosa
    }
  }

  async function fetchById(id: string): Promise<void> {
    isLoadingDetail.value = true
    detailError.value = null
    try {
      const result = await animalsService.getById(id)
      selectedAnimal.value = result.data
    } catch {
      detailError.value = 'Não foi possível carregar o animal.'
    } finally {
      isLoadingDetail.value = false
    }
  }

  async function markFoundById(id: string): Promise<void> {
    try {
      await animalsService.markFound(id)
      if (selectedAnimal.value?.id === id) {
        selectedAnimal.value = { ...selectedAnimal.value, status: 'Found' }
      }
      const itemInFeed = items.value.find((a) => a.id === id)
      if (itemInFeed) itemInFeed.status = 'Found'
    } catch {
      // falha silenciosa — UI permanece no estado anterior
    }
  }

  async function sendTip(animalId: string, message: string): Promise<void> {
    isSendingTip.value = true
    try {
      await animalsService.sendTip(animalId, { message })
    } finally {
      isSendingTip.value = false
    }
  }

  async function removeAnimal(id: string): Promise<void> {
    isDeletingAnimal.value = true
    try {
      await animalsService.deletePost(id)
      items.value = items.value.filter((a) => a.id !== id)
      if (selectedAnimal.value?.id === id) {
        selectedAnimal.value = null
      }
    } finally {
      isDeletingAnimal.value = false
    }
  }

  return {
    items,
    nextPageToken,
    hasNextPage,
    isLoading,
    error,
    isCreating,
    createError,
    selectedAnimal,
    isLoadingDetail,
    detailError,
    hasItems,
    hasError,
    fetchNearby,
    uploadMedia,
    createAnimal,
    addFromSignalR,
    handleAnimalPostedEvent,
    fetchById,
    markFoundById,
    isSendingTip,
    isDeletingAnimal,
    sendTip,
    removeAnimal,
  }
})
