export interface ApiResponse<T> {
  data: T
  success: boolean
}

export interface PagedApiResponse<T> {
  data: T[]
  pagination: {
    nextPageToken: string | null
    hasNextPage: boolean
    pageSize: number
  }
  success: boolean
}

export interface ApiError {
  error: {
    code: string
    message: string
  }
  success: false
}

export interface AnimalMediaDto {
  mediaId: string
  url: string
}

export interface AnimalCardDto {
  id: string
  userId: string
  description: string
  status: 'Lost' | 'Found'
  latitude: number
  longitude: number
  media: AnimalMediaDto[]
  createdAt: string
}

export interface UserMeDto {
  id: string
  name: string
  email: string
}

export interface LoginResponseDto {
  token: string
  userId: string
  email: string
  name: string
}

export interface NotificationDto {
  id: string
  eventName: string
  message: string
  createdAt: string
  read: boolean
}

export interface UploadMediaResponseDto {
  mediaId: string
}
