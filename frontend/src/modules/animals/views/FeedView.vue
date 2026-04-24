<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAnimalsStore } from '../stores/animals.store'
import { useGeolocation } from '@/composables/useGeolocation'
import { useSignalR } from '@/composables/useSignalR'
import AnimalCard from '../components/AnimalCard.vue'
import AppSpinner from '@/components/AppSpinner.vue'
import AppEmptyState from '@/components/AppEmptyState.vue'
import AppButton from '@/components/AppButton.vue'
import type { AnimalPostedEventDto } from '@/types/api.types'

const FEED_RADIUS_KM = 10

const router = useRouter()
const animalsStore = useAnimalsStore()
const geo = useGeolocation()
const signalR = useSignalR()

let _currentRegionKey: string | null = null

function buildRegionKey(lat: number, lng: number): string {
  const latRounded = (Math.round(lat * 10) / 10).toFixed(1)
  const lngRounded = (Math.round(lng * 10) / 10).toFixed(1)
  return `region:${latRounded}:${lngRounded}`
}

onMounted(async () => {
  await geo.requestLocation()
  if (geo.latitude.value !== null && geo.longitude.value !== null) {
    await animalsStore.fetchNearby(geo.latitude.value, geo.longitude.value, FEED_RADIUS_KM)

    if (signalR.isConnected.value) {
      _currentRegionKey = buildRegionKey(geo.latitude.value, geo.longitude.value)
      await signalR.joinRegion(_currentRegionKey)
      signalR.on<AnimalPostedEventDto>('animal-posted', (payload) => {
        void animalsStore.handleAnimalPostedEvent(payload)
      })
    }
  }
})

onUnmounted(() => {
  signalR.off('animal-posted')
  if (_currentRegionKey !== null) {
    void signalR.leaveRegion(_currentRegionKey)
  }
})

async function retryLocation(): Promise<void> {
  await geo.requestLocation()
  if (geo.latitude.value !== null && geo.longitude.value !== null) {
    await animalsStore.fetchNearby(geo.latitude.value, geo.longitude.value, FEED_RADIUS_KM)
  }
}

async function retryFeed(): Promise<void> {
  if (geo.latitude.value !== null && geo.longitude.value !== null) {
    await animalsStore.fetchNearby(geo.latitude.value, geo.longitude.value, FEED_RADIUS_KM)
  }
}

function goToPost(): void {
  router.push({ name: 'post-animal' })
}

async function loadMore(): Promise<void> {
  if (!animalsStore.hasNextPage || animalsStore.isLoading) return
  if (geo.latitude.value !== null && geo.longitude.value !== null) {
    await animalsStore.fetchNearby(
      geo.latitude.value,
      geo.longitude.value,
      FEED_RADIUS_KM,
      animalsStore.nextPageToken ?? undefined,
    )
  }
}
</script>

<template>
  <div class="feed-view">
    <header class="feed-header">
      <div class="feed-brand">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="28"
          height="28"
          viewBox="0 0 24 24"
          fill="currentColor"
          class="brand-icon"
          aria-hidden="true"
        >
          <path d="M4.5 11a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5zm15 0a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5zM7 6.5a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5zm10 0a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5zM12 22c3.5 0 7-2.5 7-7 0-2.5-1.5-4.5-3.5-5.5-.5-.3-1-.5-1.5-.7-.5-.2-1-.3-2-.3s-1.5.1-2 .3c-.5.2-1 .4-1.5.7C6.5 10.5 5 12.5 5 15c0 4.5 3.5 7 7 7z"/>
        </svg>
        <span class="brand-name">PetRadar</span>
      </div>
      <p class="feed-subtitle">Animais perdidos na sua região</p>
    </header>

    <div v-if="geo.isLoading.value" class="state-center">
      <AppSpinner />
      <p class="state-message">Obtendo sua localização…</p>
    </div>

    <div v-else-if="geo.error.value" class="state-center">
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="48"
        height="48"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="1.5"
        stroke-linecap="round"
        stroke-linejoin="round"
        class="state-icon state-icon--warning"
        aria-hidden="true"
      >
        <circle cx="12" cy="12" r="10" />
        <line x1="12" y1="8" x2="12" y2="12" />
        <line x1="12" y1="16" x2="12.01" y2="16" />
      </svg>
      <p class="state-message">{{ geo.error.value }}</p>
      <AppButton variant="secondary" @click="retryLocation">Tentar novamente</AppButton>
    </div>

    <template v-else>
      <ul v-if="animalsStore.isLoading && !animalsStore.hasItems" class="feed-list" aria-label="Carregando animais">
        <li v-for="i in 3" :key="i" class="feed-item">
          <div class="skeleton-card" aria-hidden="true">
            <div class="skeleton-image" />
            <div class="skeleton-body">
              <div class="skeleton-line skeleton-line--wide" />
              <div class="skeleton-line skeleton-line--narrow" />
              <div class="skeleton-footer">
                <div class="skeleton-line skeleton-line--short" />
                <div class="skeleton-line skeleton-line--short" />
              </div>
            </div>
          </div>
        </li>
      </ul>

      <div v-else-if="animalsStore.error" class="state-center">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="48"
          height="48"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="1.5"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="state-icon state-icon--error"
          aria-hidden="true"
        >
          <circle cx="12" cy="12" r="10" />
          <line x1="15" y1="9" x2="9" y2="15" />
          <line x1="9" y1="9" x2="15" y2="15" />
        </svg>
        <p class="state-message state-message--error">{{ animalsStore.error }}</p>
        <AppButton variant="secondary" @click="retryFeed">Tentar novamente</AppButton>
      </div>

      <AppEmptyState
        v-else-if="!animalsStore.hasItems"
        message="Nenhum animal registrado próximo a você. Seja o primeiro a publicar!"
        cta-label="Publicar animal"
        @action="goToPost"
      />

      <template v-else>
        <ul class="feed-list">
          <li v-for="animal in animalsStore.items" :key="animal.id" class="feed-item">
            <AnimalCard :animal="animal" />
          </li>
        </ul>

        <div v-if="animalsStore.hasNextPage" class="load-more">
          <AppButton
            variant="secondary"
            :disabled="animalsStore.isLoading"
            @click="loadMore"
          >
            <AppSpinner v-if="animalsStore.isLoading" />
            <span v-else>Carregar mais</span>
          </AppButton>
        </div>
      </template>
    </template>
  </div>
</template>

<style scoped>
@keyframes skeleton-pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.45; }
}

.feed-view {
  padding: var(--space-4);
  max-width: 640px;
  margin: 0 auto;
}

.feed-header {
  padding: var(--space-6) 0 var(--space-5);
  border-bottom: 1px solid var(--color-neutral-200);
  margin-bottom: var(--space-5);
}

.feed-brand {
  display: flex;
  align-items: center;
  gap: var(--space-2);
  margin-bottom: var(--space-1);
}

.brand-icon {
  color: var(--color-primary-500);
  flex-shrink: 0;
}

.brand-name {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  color: var(--color-primary-500);
  line-height: var(--line-height-tight);
}

.feed-subtitle {
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
  margin: 0;
}

.feed-list {
  list-style: none;
  padding: 0;
  margin: 0;
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.feed-item {
  display: block;
}

.state-center {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-4);
  padding: var(--space-12) var(--space-4);
}

.state-icon {
  opacity: 0.5;
}

.state-icon--warning {
  color: var(--color-warning);
  opacity: 1;
}

.state-icon--error {
  color: var(--color-error);
  opacity: 1;
}

.state-message {
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
  text-align: center;
  margin: 0;
}

.state-message--error {
  color: var(--color-error);
}

.load-more {
  display: flex;
  justify-content: center;
  margin-top: var(--space-6);
  padding-bottom: var(--space-4);
}

.skeleton-card {
  background: var(--color-surface);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  border: 1px solid var(--color-neutral-200);
  overflow: hidden;
}

.skeleton-image {
  width: 100%;
  aspect-ratio: 4 / 3;
  background: var(--color-neutral-200);
  animation: skeleton-pulse 1.5s ease-in-out infinite;
}

.skeleton-body {
  padding: var(--space-4);
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.skeleton-line {
  height: var(--space-4);
  border-radius: var(--radius-sm);
  background: var(--color-neutral-200);
  animation: skeleton-pulse 1.5s ease-in-out infinite;
}

.skeleton-line--wide   { width: 80%; }
.skeleton-line--narrow { width: 55%; }
.skeleton-line--short  { width: 35%; }

.skeleton-footer {
  display: flex;
  justify-content: space-between;
}

@media (min-width: 768px) {
  .feed-list {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: var(--space-5);
  }
}
</style>
