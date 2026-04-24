import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import { authService } from '../services/auth.service'
import type { UserMeDto, LoginResponseDto } from '@/types/api.types'

const TOKEN_KEY = 'auth_token'

function decodeJwtPayload(token: string): Record<string, unknown> | null {
  try {
    return JSON.parse(atob(token.split('.')[1]))
  } catch {
    return null
  }
}

function isTokenExpired(token: string): boolean {
  const payload = decodeJwtPayload(token)
  if (!payload || typeof payload.exp !== 'number') return true
  return payload.exp < Math.floor(Date.now() / 1000)
}

function parseUserFromToken(token: string): UserMeDto | null {
  const payload = decodeJwtPayload(token)
  if (!payload) return null
  const id = payload.sub as string | undefined
  const name = payload.name as string | undefined
  const email = payload.email as string | undefined
  if (!id || !email) return null
  return { id, name: name ?? '', email }
}

export const useAuthStore = defineStore('auth', () => {
  const storedToken = localStorage.getItem(TOKEN_KEY)
  const initialToken = storedToken && !isTokenExpired(storedToken) ? storedToken : null

  if (storedToken && !initialToken) {
    localStorage.removeItem(TOKEN_KEY)
  }

  const token = ref<string | null>(initialToken)
  const user = ref<UserMeDto | null>(initialToken ? parseUserFromToken(initialToken) : null)

  const isAuthenticated = computed(() => token.value !== null)

  function persistAuth(data: LoginResponseDto): void {
    token.value = data.token
    user.value = { id: data.userId, name: data.name, email: data.email }
    localStorage.setItem(TOKEN_KEY, data.token)
  }

  async function login(email: string, password: string): Promise<void> {
    const response = await authService.login(email, password)
    persistAuth(response.data)
  }

  async function register(name: string, email: string, password: string): Promise<void> {
    const response = await authService.register(name, email, password)
    persistAuth(response.data)
  }

  function logout(): void {
    token.value = null
    user.value = null
    localStorage.removeItem(TOKEN_KEY)
  }

  return { token, user, isAuthenticated, login, register, logout }
})
