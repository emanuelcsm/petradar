import api from '@/services/api'
import type { AnimalCardDto, PagedApiResponse, UploadMediaResponseDto, ApiResponse } from '@/types/api.types'

export const animalsService = {
  async getNearby(
    _lat: number,
    _lng: number,
    _pageToken?: string,
  ): Promise<PagedApiResponse<AnimalCardDto>> {
    const params: Record<string, unknown> = { lat: _lat, lng: _lng }
    if (_pageToken) params.nextPageToken = _pageToken
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

  async markFound(_id: string): Promise<void> {
    await api.patch(`/api/animals/${_id}/found`)
  },

  async uploadMedia(_file: File): Promise<ApiResponse<UploadMediaResponseDto>> {
    const form = new FormData()
    form.append('file', _file)
    const { data } = await api.post<ApiResponse<UploadMediaResponseDto>>('/api/media', form, {
      headers: { 'Content-Type': 'multipart/form-data' },
    })
    return data
  },
}
