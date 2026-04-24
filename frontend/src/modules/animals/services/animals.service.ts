import api from '@/services/api'
import type { AnimalCardDto, PagedApiResponse, UploadMediaResponseDto, ApiResponse, SendTipDto } from '@/types/api.types'

export const animalsService = {
  async getNearby(
    lat: number,
    lng: number,
    radius: number,
    pageToken?: string,
    pageSize?: number,
  ): Promise<PagedApiResponse<AnimalCardDto>> {
    const params: Record<string, unknown> = { lat, lng, radius }
    if (pageToken) params.nextPageToken = pageToken
    if (pageSize) params.pageSize = pageSize
    const { data } = await api.get<PagedApiResponse<AnimalCardDto>>('/api/animals/nearby', { params })
    return data
  },

  async create(
    _description: string,
    _latitude: number,
    _longitude: number,
    _mediaIds?: string[],
  ): Promise<void> {
    await api.post('/api/animals', {
      description: _description,
      latitude: _latitude,
      longitude: _longitude,
      mediaIds: _mediaIds,
    })
  },

  async getById(id: string): Promise<ApiResponse<AnimalCardDto>> {
    const { data } = await api.get<ApiResponse<AnimalCardDto>>(`/api/animals/${id}`)
    return data
  },

  async markFound(_id: string): Promise<void> {
    await api.patch(`/api/animals/${_id}/found`)
  },

  async uploadMedia(_file: File): Promise<ApiResponse<UploadMediaResponseDto>> {
    const form = new FormData()
    form.append('file', _file)
    const { data } = await api.post<ApiResponse<UploadMediaResponseDto>>('/api/media', form)
    return data
  },

  async sendTip(animalId: string, payload: SendTipDto): Promise<void> {
    await api.post(`/api/animals/${animalId}/tips`, payload)
  },

  async deletePost(animalId: string): Promise<void> {
    await api.delete(`/api/animals/${animalId}`)
  },
}
