<script setup lang="ts">
import { onMounted } from 'vue'
import { useAnimalsStore } from '../stores/animals.store'
import { useGeolocation } from '@/composables/useGeolocation'
import AnimalCard from '../components/AnimalCard.vue'
import AppSpinner from '@/components/AppSpinner.vue'
import AppEmptyState from '@/components/AppEmptyState.vue'
import AppButton from '@/components/AppButton.vue'

const FEED_RADIUS_KM = 10

const animalsStore = useAnimalsStore()
const geo = useGeolocation()

onMounted(async () => {
  await geo.requestLocation()
  if (geo.latitude.value !== null && geo.longitude.value !== null) {
    await animalsStore.fetchNearby(geo.latitude.value, geo.longitude.value, FEED_RADIUS_KM)
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
    <h1 class="feed-title">Animais próximos</h1>

    <div v-if="geo.isLoading.value" class="state-center">
      <AppSpinner />
      <p class="state-message">Obtendo sua localização…</p>
    </div>

    <div v-else-if="geo.error.value" class="state-center">
      <p class="state-message error">{{ geo.error.value }}</p>
      <AppButton variant="secondary" @click="retryLocation">Tentar novamente</AppButton>
    </div>

    <template v-else>
      <div v-if="animalsStore.isLoading && !animalsStore.hasItems" class="state-center">
        <AppSpinner />
        <p class="state-message">Buscando animais…</p>
      </div>

      <div v-else-if="animalsStore.error" class="state-center">
        <p class="state-message error">{{ animalsStore.error }}</p>
        <AppButton variant="secondary" @click="retryFeed">Tentar novamente</AppButton>
      </div>

      <AppEmptyState
        v-else-if="!animalsStore.hasItems"
        message="Nenhum animal registrado próximo a você."
        cta-label="Recarregar"
        @action="retryFeed"
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
.feed-view {
  padding: var(--space-4);
  max-width: 640px;
  margin: 0 auto;
}

.feed-title {
  font-size: var(--font-size-2xl);
  font-weight: var(--font-weight-bold);
  color: var(--color-neutral-900);
  margin-bottom: var(--space-6);
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

.state-message {
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
  text-align: center;
}

.state-message.error {
  color: var(--color-error);
}

.load-more {
  display: flex;
  justify-content: center;
  margin-top: var(--space-6);
}
</style>
