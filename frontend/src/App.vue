<script setup lang="ts">
import { RouterView, RouterLink } from 'vue-router'
import { useAuthStore } from '@/modules/auth/stores/auth.store'

const authStore = useAuthStore()
</script>

<template>
  <div class="app-layout">
    <div :class="['app-content', { 'app-content--nav': authStore.isAuthenticated }]">
      <RouterView />
    </div>

    <nav
      v-if="authStore.isAuthenticated"
      class="bottom-nav"
      aria-label="Navegação principal"
    >
      <div class="bottom-nav-inner">
      <RouterLink :to="{ name: 'feed' }" class="nav-item" aria-label="Feed">
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
          <path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z" />
          <polyline points="9 22 9 12 15 12 15 22" />
        </svg>
        <span class="nav-label">Feed</span>
      </RouterLink>

      <RouterLink :to="{ name: 'post-animal' }" class="nav-item" aria-label="Publicar animal">
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
          <circle cx="12" cy="12" r="10" />
          <line x1="12" y1="8" x2="12" y2="16" />
          <line x1="8" y1="12" x2="16" y2="12" />
        </svg>
        <span class="nav-label">Publicar</span>
      </RouterLink>
      </div>
    </nav>
  </div>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-content {
  flex: 1;
}

.app-content--nav {
  padding-bottom: var(--space-16);
}

.bottom-nav {
  position: fixed;
  bottom: 0;
  left: 0;
  right: 0;
  height: var(--space-16);
  background: var(--color-surface);
  border-top: 1px solid var(--color-neutral-200);
  box-shadow: 0 -2px 8px rgb(0 0 0 / 0.06);
  z-index: 100;
}

.bottom-nav-inner {
  max-width: 640px;
  margin: 0 auto;
  height: 100%;
  display: flex;
  align-items: stretch;
}

.nav-item {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: var(--space-1);
  color: var(--color-neutral-400);
  text-decoration: none;
  transition: color var(--transition-fast);
}

.nav-item:hover {
  color: var(--color-primary-500);
}

.nav-item.router-link-active {
  color: var(--color-primary-500);
}

.nav-label {
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
}
</style>
