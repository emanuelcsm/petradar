import api from '@/services/api'
import type { NotificationDto, PagedApiResponse } from '@/types/api.types'

export const notificationsService = {
  async getAll(_pageToken?: string): Promise<PagedApiResponse<NotificationDto>> {
    const params: Record<string, unknown> = {}
    if (_pageToken) params.nextPageToken = _pageToken
    const { data } = await api.get<PagedApiResponse<NotificationDto>>('/api/notifications', { params })
    return data
  },

  async markRead(_id: string): Promise<void> {
    await api.patch(`/api/notifications/${_id}/read`)
  },
}
