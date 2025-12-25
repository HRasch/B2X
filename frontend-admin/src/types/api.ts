export interface ApiResponse<T> {
  success: boolean
  data?: T
  error?: ApiError
  message?: string
  timestamp: string
}

export interface ApiError {
  code: string
  message: string
  details?: ErrorDetail[]
}

export interface ErrorDetail {
  field: string
  message: string
}

export interface PaginatedResponse<T> {
  items: T[]
  total: number
  page: number
  pageSize: number
  totalPages: number
}

export interface PaginationParams {
  page: number
  pageSize: number
  sortBy?: string
  sortOrder?: 'asc' | 'desc'
}
