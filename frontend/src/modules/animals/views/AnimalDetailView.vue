<script setup lang="ts">
import { computed, onMounted, ref } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAnimalsStore } from '../stores/animals.store'
import { useAuthStore } from '@/modules/auth/stores/auth.store'
import AnimalStatusBadge from '../components/AnimalStatusBadge.vue'
import AppSpinner from '@/components/AppSpinner.vue'
import AppButton from '@/components/AppButton.vue'

const route = useRoute()
const router = useRouter()
const animalsStore = useAnimalsStore()
const authStore = useAuthStore()

const isMarking = ref(false)

const canMarkFound = computed(
  () =>
    animalsStore.selectedAnimal?.status === 'Lost' &&
    authStore.user?.id === animalsStore.selectedAnimal?.userId,
)

const primaryImageUrl = computed(() => animalsStore.selectedAnimal?.media[0]?.url ?? null)

const approximateLocation = computed(() => {
  const a = animalsStore.selectedAnimal
  if (!a) return ''
  return `${a.latitude.toFixed(2)}, ${a.longitude.toFixed(2)}`
})

function formatDate(value: string): string {
  return new Intl.DateTimeFormat('pt-BR', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  }).format(new Date(value))
}

async function handleMarkFound(): Promise<void> {
  if (!animalsStore.selectedAnimal) return
  isMarking.value = true
  await animalsStore.markFoundById(animalsStore.selectedAnimal.id)
  isMarking.value = false
}

onMounted(() => {
  void animalsStore.fetchById(route.params.id as string)
})
</script>

<template>
  <div class="detail-view">
    <button class="back-btn" aria-label="Voltar" @click="router.back()">
      <svg
        xmlns="http://www.w3.org/2000/svg"
        width="20"
        height="20"
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        stroke-width="2"
        stroke-linecap="round"
        stroke-linejoin="round"
        aria-hidden="true"
      >
        <polyline points="15 18 9 12 15 6" />
      </svg>
      <span>Voltar</span>
    </button>

    <div v-if="animalsStore.isLoadingDetail" class="state-center">
      <AppSpinner />
    </div>

    <div v-else-if="animalsStore.detailError" class="state-center">
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
        class="state-icon"
        aria-hidden="true"
      >
        <circle cx="12" cy="12" r="10" />
        <line x1="15" y1="9" x2="9" y2="15" />
        <line x1="9" y1="9" x2="15" y2="15" />
      </svg>
      <p class="state-message">{{ animalsStore.detailError }}</p>
      <AppButton variant="secondary" @click="animalsStore.fetchById(route.params.id as string)">
        Tentar novamente
      </AppButton>
    </div>

    <template v-else-if="animalsStore.selectedAnimal">
      <div class="image-wrapper">
        <img
          v-if="primaryImageUrl"
          :src="primaryImageUrl"
          :alt="`Foto de animal ${animalsStore.selectedAnimal.status === 'Lost' ? 'perdido' : 'encontrado'}`"
          class="animal-image"
        />
        <div v-else class="image-placeholder" aria-hidden="true">
          <svg
            xmlns="http://www.w3.org/2000/svg"
            width="56"
            height="56"
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
      </div>

      <div class="detail-body">
        <div class="detail-header">
          <AnimalStatusBadge :status="animalsStore.selectedAnimal.status" />
        </div>

        <p class="description">{{ animalsStore.selectedAnimal.description }}</p>

        <dl class="meta-list">
          <div class="meta-item">
            <dt class="meta-label">Localização aproximada</dt>
            <dd class="meta-value">
              <svg
                xmlns="http://www.w3.org/2000/svg"
                width="14"
                height="14"
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
            </dd>
          </div>

          <div class="meta-item">
            <dt class="meta-label">Publicado em</dt>
            <dd class="meta-value">
              <time :datetime="animalsStore.selectedAnimal.createdAt">
                {{ formatDate(animalsStore.selectedAnimal.createdAt) }}
              </time>
            </dd>
          </div>
        </dl>

        <div v-if="canMarkFound" class="action-area">
          <AppButton
            variant="primary"
            :disabled="isMarking"
            @click="handleMarkFound"
          >
            <AppSpinner v-if="isMarking" />
            <span v-else>Marcar como encontrado</span>
          </AppButton>
        </div>
      </div>
    </template>
  </div>
</template>

<style scoped>
.detail-view {
  max-width: 640px;
  margin: 0 auto;
  padding-bottom: var(--space-16);
}

.back-btn {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  background: none;
  border: none;
  cursor: pointer;
  color: var(--color-neutral-600);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  padding: var(--space-4);
  transition: color var(--transition-fast);
}

.back-btn:hover {
  color: var(--color-primary-500);
}

.state-center {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: var(--space-4);
  padding: var(--space-12) var(--space-4);
}

.state-icon {
  color: var(--color-error);
}

.state-message {
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
  text-align: center;
  margin: 0;
}

.image-wrapper {
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

.detail-body {
  padding: var(--space-5) var(--space-4);
  display: flex;
  flex-direction: column;
  gap: var(--space-5);
}

.detail-header {
  display: flex;
  align-items: center;
  gap: var(--space-3);
}

.description {
  font-size: var(--font-size-base);
  color: var(--color-neutral-800);
  line-height: var(--line-height-relaxed);
  margin: 0;
}

.meta-list {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
  margin: 0;
}

.meta-item {
  display: flex;
  flex-direction: column;
  gap: var(--space-1);
}

.meta-label {
  font-size: var(--font-size-xs);
  font-weight: var(--font-weight-medium);
  color: var(--color-neutral-400);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.meta-value {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
  margin: 0;
}

.action-area {
  padding-top: var(--space-2);
}
</style>
