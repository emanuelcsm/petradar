<script setup lang="ts">
import { computed } from 'vue'
import AppCard from '@/components/AppCard.vue'
import AnimalStatusBadge from './AnimalStatusBadge.vue'
import type { AnimalCardDto } from '@/types/api.types'

interface Props {
  animal: AnimalCardDto
}

const props = defineProps<Props>()

const primaryImageUrl = computed(() => props.animal.media[0]?.url ?? null)

const approximateLocation = computed(() => {
  const lat = props.animal.latitude.toFixed(2)
  const lng = props.animal.longitude.toFixed(2)
  return `${lat}, ${lng}`
})

const relativeTime = computed(() => {
  const diff = Date.now() - new Date(props.animal.createdAt).getTime()
  const minutes = Math.floor(diff / 60_000)
  if (minutes < 60) return `há ${minutes} min`
  const hours = Math.floor(minutes / 60)
  if (hours < 24) return `há ${hours}h`
  const days = Math.floor(hours / 24)
  return `há ${days}d`
})
</script>

<template>
  <AppCard>
    <div class="card-inner">
      <div class="image-wrapper">
        <img
          v-if="primaryImageUrl"
          :src="primaryImageUrl"
          :alt="`Foto de animal ${animal.status === 'Lost' ? 'perdido' : 'encontrado'}`"
          class="animal-image"
        />
        <div v-else class="image-placeholder" aria-hidden="true">
          <span class="placeholder-icon">🐾</span>
        </div>
      </div>

      <div class="card-body">
        <div class="card-header">
          <AnimalStatusBadge :status="animal.status" />
          <span class="time">{{ relativeTime }}</span>
        </div>

        <p class="description">{{ animal.description }}</p>

        <p class="location">
          <span class="location-label">Localização aprox.:</span>
          {{ approximateLocation }}
        </p>
      </div>
    </div>
  </AppCard>
</template>

<style scoped>
.card-inner {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.image-wrapper {
  width: 100%;
  aspect-ratio: 16 / 9;
  overflow: hidden;
  border-radius: var(--radius-md);
  background: var(--color-neutral-100);
}

.animal-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.image-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.placeholder-icon {
  font-size: var(--font-size-3xl);
}

.card-body {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.time {
  font-size: var(--font-size-xs);
  color: var(--color-neutral-400);
}

.description {
  font-size: var(--font-size-sm);
  color: var(--color-neutral-800);
  line-height: var(--line-height-normal);
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.location {
  font-size: var(--font-size-xs);
  color: var(--color-neutral-600);
}

.location-label {
  font-weight: var(--font-weight-medium);
}
</style>
