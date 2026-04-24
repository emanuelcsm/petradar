import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { notificationsService } from '../services/notifications.service'
import type { NotificationDto, AnimalFoundEventDto, NewNotificationEventDto } from '@/types/api.types'

export const useNotificationsStore = defineStore('notifications', () => {
  const items = ref<NotificationDto[]>([])
  const nextPageToken = ref<string | null>(null)
  const hasNextPage = ref(false)
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const _seenEventKeys = new Set<string>()

  const unreadCount = computed(() => items.value.filter((n) => !n.read).length)
  const hasItems = computed(() => items.value.length > 0)
  const hasError = computed(() => error.value !== null)

  async function fetchAll(pageToken?: string): Promise<void> {
    isLoading.value = true
    error.value = null
    try {
      const result = await notificationsService.getAll(pageToken)
      if (pageToken) {
        items.value.push(...result.data)
      } else {
        items.value = result.data
      }
      nextPageToken.value = result.pagination.nextPageToken
      hasNextPage.value = result.pagination.hasNextPage
    } catch {
      error.value = 'Não foi possível carregar as notificações.'
    } finally {
      isLoading.value = false
    }
  }

  function addFromSignalREvent(payload: AnimalFoundEventDto): void {
    const key = `animal-found:${payload.animalPostId}`
    if (_seenEventKeys.has(key)) return
    _seenEventKeys.add(key)

    items.value.unshift({
      id: `pending:${payload.animalPostId}`,
      eventName: 'animal-found',
      message: 'Seu animal foi marcado como encontrado.',
      createdAt: payload.foundAt,
      read: false,
    })
  }

  function addTipNotificationFromSignalR(payload: NewNotificationEventDto): void {
    const key = `new-notification:${payload.animalPostId}:${payload.senderName}`
    if (_seenEventKeys.has(key)) return
    _seenEventKeys.add(key)

    items.value.unshift({
      id: `pending-tip:${payload.animalPostId}`,
      eventName: 'new-notification',
      message: `${payload.senderName} enviou uma dica sobre o seu animal.`,
      createdAt: new Date().toISOString(),
      read: false,
      tipPayload: {
        senderName: payload.senderName,
        tipMessage: payload.message,
      },
    })
  }

  async function markRead(id: string): Promise<void> {
    const item = items.value.find((n) => n.id === id)
    if (!item || item.read) return

    if (id.startsWith('pending')) {
      item.read = true
      return
    }

    try {
      await notificationsService.markRead(id)
      item.read = true
    } catch {
      // falha silenciosa: estado local não é alterado se a API falhar
    }
  }

  function reset(): void {
    items.value = []
    nextPageToken.value = null
    hasNextPage.value = false
    isLoading.value = false
    error.value = null
    _seenEventKeys.clear()
  }

  return {
    items,
    nextPageToken,
    hasNextPage,
    isLoading,
    error,
    unreadCount,
    hasItems,
    hasError,
    fetchAll,
    addFromSignalREvent,
    addTipNotificationFromSignalR,
    markRead,
    reset,
  }
})
