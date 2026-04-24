<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { useNotificationsStore } from '../stores/notifications.store'
import AppSpinner from '@/components/AppSpinner.vue'

const notificationsStore = useNotificationsStore()
const isOpen = ref(false)
const bellRef = ref<HTMLElement | null>(null)

function toggle(): void {
  isOpen.value = !isOpen.value
  if (isOpen.value && !notificationsStore.hasItems && !notificationsStore.isLoading) {
    void notificationsStore.fetchAll()
  }
}

function handleClickOutside(event: MouseEvent): void {
  if (bellRef.value && !bellRef.value.contains(event.target as Node)) {
    isOpen.value = false
  }
}

async function handleItemClick(id: string): Promise<void> {
  await notificationsStore.markRead(id)
}

function formatDate(value: string): string {
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value))
}

onMounted(() => {
  document.addEventListener('mousedown', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('mousedown', handleClickOutside)
})
</script>

<template>
  <div ref="bellRef" class="notification-bell-wrapper">
    <button
      class="notification-bell"
      :aria-label="`Notificações${notificationsStore.unreadCount > 0 ? `, ${notificationsStore.unreadCount} não lidas` : ''}`"
      :aria-expanded="isOpen"
      aria-haspopup="listbox"
      @click="toggle"
    >
      <span class="bell-icon-wrap">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="22"
          height="22"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          aria-hidden="true"
        >
          <path d="M18 8A6 6 0 0 0 6 8c0 7-3 9-3 9h18s-3-2-3-9" />
          <path d="M13.73 21a2 2 0 0 1-3.46 0" />
        </svg>
        <span v-if="notificationsStore.unreadCount > 0" class="bell-badge" aria-hidden="true">
          {{ notificationsStore.unreadCount > 99 ? '99+' : notificationsStore.unreadCount }}
        </span>
      </span>
      <span class="bell-label">Notificações</span>
    </button>

    <div
      v-if="isOpen"
      class="notification-dropdown"
      role="listbox"
      aria-label="Lista de notificações"
    >
      <div class="dropdown-header">
        <span class="dropdown-title">Notificações</span>
      </div>

      <div v-if="notificationsStore.isLoading && !notificationsStore.hasItems" class="dropdown-state">
        <AppSpinner />
      </div>

      <p v-else-if="!notificationsStore.hasItems" class="dropdown-empty">
        Nenhuma notificação.
      </p>

      <ul v-else class="notification-list">
        <li
          v-for="item in notificationsStore.items"
          :key="item.id"
          :class="['notification-item', { 'notification-item--unread': !item.read }]"
          role="option"
          :aria-selected="!item.read"
          @click="handleItemClick(item.id)"
        >
          <p class="notification-message">{{ item.message }}</p>
          <time class="notification-date" :datetime="item.createdAt">
            {{ formatDate(item.createdAt) }}
          </time>
        </li>
      </ul>

      <div v-if="notificationsStore.hasNextPage" class="dropdown-load-more">
        <button
          class="load-more-btn"
          :disabled="notificationsStore.isLoading"
          @click.stop="notificationsStore.fetchAll(notificationsStore.nextPageToken ?? undefined)"
        >
          Carregar mais
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.notification-bell-wrapper {
  position: relative;
  flex: 1;
  display: flex;
  align-items: stretch;
}

.notification-bell {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: var(--space-1);
  background: none;
  border: none;
  cursor: pointer;
  padding: var(--space-2);
  color: var(--color-neutral-400);
  transition: color var(--transition-fast);
}

.notification-bell:hover {
  color: var(--color-primary-500);
}

.bell-icon-wrap {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: center;
}

.bell-badge {
  position: absolute;
  top: -4px;
  right: -6px;
  min-width: var(--space-4);
  height: var(--space-4);
  padding: 0 var(--space-1);
  background: var(--color-error);
  color: #fff;
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-bold);
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  line-height: 1;
}

.bell-label {
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
}

.notification-dropdown {
  position: absolute;
  bottom: calc(100% + var(--space-2));
  right: 0;
  width: 320px;
  max-height: 400px;
  overflow-y: auto;
  background: var(--color-surface);
  border: 1px solid var(--color-neutral-200);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-lg);
  z-index: 200;
}

.dropdown-header {
  padding: var(--space-3) var(--space-4);
  border-bottom: 1px solid var(--color-neutral-200);
}

.dropdown-title {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-semibold);
  color: var(--color-neutral-900);
}

.dropdown-state {
  display: flex;
  justify-content: center;
  padding: var(--space-6);
}

.dropdown-empty {
  padding: var(--space-6) var(--space-4);
  text-align: center;
  font-size: var(--font-size-sm);
  color: var(--color-neutral-500);
  margin: 0;
}

.notification-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.notification-item {
  padding: var(--space-3) var(--space-4);
  border-bottom: 1px solid var(--color-neutral-100);
  cursor: pointer;
  transition: background var(--transition-fast);
}

.notification-item:last-child {
  border-bottom: none;
}

.notification-item:hover {
  background: var(--color-neutral-50);
}

.notification-item--unread {
  background: var(--color-primary-50, #eff6ff);
}

.notification-item--unread:hover {
  background: var(--color-primary-100, #dbeafe);
}

.notification-message {
  font-size: var(--font-size-sm);
  color: var(--color-neutral-800);
  margin: 0 0 var(--space-1);
}

.notification-date {
  font-size: var(--font-size-xs);
  color: var(--color-neutral-400);
}

.dropdown-load-more {
  padding: var(--space-3) var(--space-4);
  border-top: 1px solid var(--color-neutral-200);
  display: flex;
  justify-content: center;
}

.load-more-btn {
  background: none;
  border: none;
  cursor: pointer;
  font-size: var(--font-size-sm);
  color: var(--color-primary-500);
  padding: 0;
  font-weight: var(--font-weight-medium);
}

.load-more-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}
</style>
