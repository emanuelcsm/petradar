<script setup lang="ts">
import { computed } from 'vue'
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
  <article class="animal-card">
    <div class="image-wrapper">
      <img
        v-if="primaryImageUrl"
        :src="primaryImageUrl"
        :alt="`Foto de animal ${animal.status === 'Lost' ? 'perdido' : 'encontrado'}`"
        class="animal-image"
      />
      <div v-else class="image-placeholder" aria-hidden="true">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="40"
          height="40"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="1.5"
          stroke-linecap="round"
          stroke-linejoin="round"
          class="placeholder-icon"
        >
          <path d="M23 19a2 2 0 0 1-2 2H3a2 2 0 0 1-2-2V8a2 2 0 0 1 2-2h4l2-3h6l2 3h4a2 2 0 0 1 2 2z" />
          <circle cx="12" cy="13" r="4" />
        </svg>
      </div>

      <div class="badge-overlay">
        <AnimalStatusBadge :status="animal.status" />
      </div>
    </div>

    <div class="card-body">
      <p class="description">{{ animal.description }}</p>

      <div class="card-footer">
        <span class="location">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="12"
            height="12"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            stroke-width="2"
            stroke-linecap="round"
            stroke-linejoin="round"
            aria-hidden="true"
          >
            <path d="M21 10c0 7-9 13-9 13s-9-6-9-13a9 9 0 0 1 18 0z" />
            <circle cx="12" cy="10" r="3" />
          </svg>
          {{ approximateLocation }}
        </span>
        <span class="time">{{ relativeTime }}</span>
      </div>
    </div>
  </article>
</template>

<style scoped>
.animal-card {
  background: var(--color-surface);
  border-radius: var(--radius-lg);
  box-shadow: var(--shadow-md);
  border: 1px solid var(--color-neutral-200);
  overflow: hidden;
  transition: transform var(--transition-normal), box-shadow var(--transition-normal);
}

.animal-card:hover {
  transform: translateY(-2px);
  box-shadow: var(--shadow-lg);
}

.image-wrapper {
  position: relative;
  width: 100%;
  aspect-ratio: 4 / 3;
  background: var(--color-neutral-100);
  overflow: hidden;
}

.animal-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.image-placeholder {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.placeholder-icon {
  color: var(--color-neutral-400);
}

.badge-overlay {
  position: absolute;
  top: var(--space-3);
  right: var(--space-3);
}

.card-body {
  padding: var(--space-4);
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.description {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-medium);
  color: var(--color-neutral-800);
  line-height: var(--line-height-normal);
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  margin: 0;
}

.card-footer {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.location {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  font-size: var(--font-size-xs);
  color: var(--color-neutral-400);
}

.time {
  font-size: var(--font-size-xs);
  color: var(--color-neutral-400);
}
</style>
