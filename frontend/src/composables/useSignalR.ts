import * as signalR from '@microsoft/signalr'
import { ref, onUnmounted } from 'vue'
import { useAuthStore } from '@/modules/auth/stores/auth.store'

const _isConnected = ref(false)
let _connection: signalR.HubConnection | null = null
const _joinedRegions = new Set<string>()
let _refCount = 0

export function useSignalR() {
  _refCount++

  onUnmounted(() => {
    _refCount--
    if (_refCount === 0) {
      void _connection?.stop()
      _connection = null
      _isConnected.value = false
      _joinedRegions.clear()
    }
  })

  async function connect(): Promise<void> {
    if (_connection !== null) return

    const authStore = useAuthStore()

    _connection = new signalR.HubConnectionBuilder()
      .withUrl('/hubs/animals', {
        accessTokenFactory: () => authStore.token ?? '',
      })
      .withAutomaticReconnect()
      .build()

    _connection.onreconnected(async () => {
      for (const region of _joinedRegions) {
        await _connection?.invoke('JoinRegion', region)
      }
    })

    await _connection.start()
    _isConnected.value = true
  }

  async function disconnect(): Promise<void> {
    await _connection?.stop()
    _connection = null
    _isConnected.value = false
    _joinedRegions.clear()
  }

  async function joinRegion(regionKey: string): Promise<void> {
    _joinedRegions.add(regionKey)
    await _connection?.invoke('JoinRegion', regionKey)
  }

  async function leaveRegion(regionKey: string): Promise<void> {
    _joinedRegions.delete(regionKey)
    await _connection?.invoke('LeaveRegion', regionKey)
  }

  function on<T>(event: string, handler: (payload: T) => void): void {
    _connection?.on(event, handler)
  }

  function off(event: string): void {
    _connection?.off(event)
  }

  return { isConnected: _isConnected, connect, disconnect, joinRegion, leaveRegion, on, off }
}
