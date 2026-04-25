<script setup lang="ts">
import { ref, computed, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useAnimalsStore } from '../stores/animals.store'
import { useGeolocation } from '@/composables/useGeolocation'
import AnimalMap from '../components/AnimalMap.vue'
import AnimalMapPicker from '../components/AnimalMapPicker.vue'
import AppButton from '@/components/AppButton.vue'
import AppSpinner from '@/components/AppSpinner.vue'

interface FileUploadState {
  id: string
  file: File
  previewUrl: string
  status: 'pending' | 'uploading' | 'success' | 'error'
  mediaId: string | null
  errorMessage: string | null
}

type SubmitPhase = 'idle' | 'uploading' | 'creating'
type LocationMode = 'gps' | 'manual'

const router = useRouter()
const animalsStore = useAnimalsStore()
const geo = useGeolocation()

const description = ref('')
const files = ref<FileUploadState[]>([])
const fileInputRef = ref<HTMLInputElement | null>(null)
const locationMode = ref<LocationMode>('gps')
const latitude = ref<number | null>(null)
const longitude = ref<number | null>(null)
const formErrors = ref<Record<string, string>>({})
const submitPhase = ref<SubmitPhase>('idle')
const submitError = ref<string | null>(null)

let fileIdCounter = 0

const hasLocation = computed(() => latitude.value !== null && longitude.value !== null)

const isSubmitting = computed(
  () => submitPhase.value === 'uploading' || submitPhase.value === 'creating',
)

const submitButtonLabel = computed(() => {
  if (submitPhase.value === 'uploading') return 'Enviando fotos…'
  if (submitPhase.value === 'creating') return 'Publicando…'
  return 'Publicar'
})

function goBack(): void {
  router.push({ name: 'feed' })
}

function triggerFileInput(): void {
  fileInputRef.value?.click()
}

function onFilesSelected(event: Event): void {
  const input = event.target as HTMLInputElement
  if (!input.files) return
  Array.from(input.files).forEach((file) => {
    files.value.push({
      id: String(++fileIdCounter),
      file,
      previewUrl: URL.createObjectURL(file),
      status: 'pending',
      mediaId: null,
      errorMessage: null,
    })
  })
  input.value = ''
}

function removeFile(id: string): void {
  const index = files.value.findIndex((f) => f.id === id)
  if (index === -1) return
  const [removed] = files.value.splice(index, 1)
  URL.revokeObjectURL(removed.previewUrl)
}

async function retryFileUpload(fileState: FileUploadState): Promise<void> {
  fileState.status = 'uploading'
  fileState.errorMessage = null
  const mediaId = await animalsStore.uploadMedia(fileState.file)
  if (mediaId !== null) {
    fileState.mediaId = mediaId
    fileState.status = 'success'
  } else {
    fileState.status = 'error'
    fileState.errorMessage = 'Falha no upload. Tente novamente.'
  }
}

async function requestGps(): Promise<void> {
  await geo.requestLocation()
  if (geo.latitude.value !== null && geo.longitude.value !== null) {
    latitude.value = geo.latitude.value
    longitude.value = geo.longitude.value
  }
}

function switchToGps(): void {
  locationMode.value = 'gps'
}

function switchToManual(): void {
  locationMode.value = 'manual'
}

function onLocationPicked(coords: { latitude: number; longitude: number }): void {
  latitude.value = coords.latitude
  longitude.value = coords.longitude
}

function validate(): boolean {
  formErrors.value = {}
  if (!description.value.trim()) {
    formErrors.value.description = 'Descrição obrigatória.'
  }
  if (!hasLocation.value) {
    formErrors.value.location = 'Localização obrigatória. Use GPS ou marque no mapa.'
  }
  return Object.keys(formErrors.value).length === 0
}

async function submit(): Promise<void> {
  if (!validate()) return
  submitError.value = null

  const pendingFiles = files.value.filter((f) => f.status !== 'success')
  if (pendingFiles.length > 0) {
    submitPhase.value = 'uploading'
    for (const fileState of files.value) {
      if (fileState.status === 'success') continue
      fileState.status = 'uploading'
      fileState.errorMessage = null
      const mediaId = await animalsStore.uploadMedia(fileState.file)
      if (mediaId !== null) {
        fileState.mediaId = mediaId
        fileState.status = 'success'
      } else {
        fileState.status = 'error'
        fileState.errorMessage = 'Falha no upload.'
      }
    }
    if (files.value.some((f) => f.status === 'error')) {
      submitPhase.value = 'idle'
      return
    }
  }

  submitPhase.value = 'creating'
  const mediaIds = files.value.filter((f) => f.mediaId !== null).map((f) => f.mediaId!)

  await animalsStore.createAnimal(
    description.value.trim(),
    latitude.value!,
    longitude.value!,
    mediaIds,
  )

  if (animalsStore.createError) {
    submitError.value = animalsStore.createError
    submitPhase.value = 'idle'
  } else {
    router.push({ name: 'feed' })
  }
}

onUnmounted(() => {
  files.value.forEach((f) => URL.revokeObjectURL(f.previewUrl))
})
</script>

<template>
  <div class="post-view">
    <header class="post-header">
      <button class="back-btn" type="button" aria-label="Voltar para o feed" @click="goBack">
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
      </button>
      <h1 class="post-title">Publicar animal</h1>
    </header>

    <form class="post-form" novalidate @submit.prevent="submit">
      <section class="form-section">
        <label for="description" class="field-label">
          Descrição <span class="required" aria-hidden="true">*</span>
        </label>
        <textarea
          id="description"
          v-model="description"
          class="textarea-field"
          :class="{ 'textarea-field--error': formErrors.description }"
          placeholder="Ex: Cachorro preto de porte médio, coleira azul, encontrado próximo à praça…"
          rows="4"
          maxlength="500"
          :disabled="isSubmitting"
        />
        <span v-if="formErrors.description" class="field-error" role="alert">
          {{ formErrors.description }}
        </span>
      </section>

      <section class="form-section">
        <div class="section-header">
          <h2 class="section-title">Fotos</h2>
          <button
            type="button"
            class="add-photo-btn"
            :disabled="isSubmitting"
            @click="triggerFileInput"
          >
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="14"
              height="14"
              viewBox="0 0 24 24"
              fill="none"
              stroke="currentColor"
              stroke-width="2.5"
              stroke-linecap="round"
              stroke-linejoin="round"
              aria-hidden="true"
            >
              <line x1="12" y1="5" x2="12" y2="19" />
              <line x1="5" y1="12" x2="19" y2="12" />
            </svg>
            Adicionar
          </button>
        </div>

        <input
          ref="fileInputRef"
          type="file"
          multiple
          accept="image/*"
          class="visually-hidden"
          aria-hidden="true"
          tabindex="-1"
          @change="onFilesSelected"
        />

        <ul v-if="files.length > 0" class="files-grid">
          <li v-for="fileState in files" :key="fileState.id" class="file-item">
            <div class="file-preview">
              <img
                :src="fileState.previewUrl"
                :alt="`Pré-visualização de ${fileState.file.name}`"
                class="file-thumb"
              />

              <div v-if="fileState.status === 'uploading'" class="file-overlay">
                <AppSpinner />
              </div>

              <div
                v-else-if="fileState.status === 'success'"
                class="file-overlay file-overlay--success"
                aria-label="Upload concluído"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="22"
                  height="22"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="3"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  aria-hidden="true"
                >
                  <polyline points="20 6 9 17 4 12" />
                </svg>
              </div>

              <div
                v-else-if="fileState.status === 'error'"
                class="file-overlay file-overlay--error"
                :aria-label="fileState.errorMessage ?? 'Erro no upload'"
              >
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="22"
                  height="22"
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  stroke-width="3"
                  stroke-linecap="round"
                  stroke-linejoin="round"
                  aria-hidden="true"
                >
                  <line x1="18" y1="6" x2="6" y2="18" />
                  <line x1="6" y1="6" x2="18" y2="18" />
                </svg>
              </div>
            </div>

            <div class="file-meta">
              <button
                v-if="fileState.status === 'error'"
                type="button"
                class="file-retry-btn"
                @click="retryFileUpload(fileState)"
              >
                Tentar novamente
              </button>
              <button
                v-if="fileState.status !== 'uploading' && fileState.status !== 'success'"
                type="button"
                class="file-remove-btn"
                :aria-label="`Remover ${fileState.file.name}`"
                @click="removeFile(fileState.id)"
              >
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
                  <line x1="18" y1="6" x2="6" y2="18" />
                  <line x1="6" y1="6" x2="18" y2="18" />
                </svg>
              </button>
            </div>
          </li>
        </ul>
      </section>

      <section class="form-section">
        <h2 class="section-title">
          Localização <span class="required" aria-hidden="true">*</span>
        </h2>

        <div class="location-tabs" role="group" aria-label="Modo de localização">
          <button
            type="button"
            class="tab-btn"
            :class="{ 'tab-btn--active': locationMode === 'gps' }"
            :aria-pressed="locationMode === 'gps'"
            @click="switchToGps"
          >
            GPS
          </button>
          <button
            type="button"
            class="tab-btn"
            :class="{ 'tab-btn--active': locationMode === 'manual' }"
            :aria-pressed="locationMode === 'manual'"
            @click="switchToManual"
          >
            Manual
          </button>
        </div>

        <div v-if="locationMode === 'gps'" class="gps-section">
          <AppButton
            type="button"
            variant="secondary"
            :disabled="geo.isLoading.value || isSubmitting || geo.isPermanentlyDenied.value"
            @click="requestGps"
          >
            <AppSpinner v-if="geo.isLoading.value" />
            <span>{{ geo.isLoading.value ? 'Obtendo localização…' : 'Usar minha localização' }}</span>
          </AppButton>

          <p v-if="geo.isPermanentlyDenied.value" class="location-blocked-note" role="alert">
            Localização bloqueada nas configurações do navegador. Use o modo
            <button type="button" class="inline-link" @click="switchToManual">Manual</button>
            para marcar a posição no mapa.
          </p>
          <p v-else-if="geo.error.value" class="field-error" role="alert">
            {{ geo.error.value }}
          </p>

          <p v-if="hasLocation && !geo.isPermanentlyDenied.value" class="coords-display">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="13"
              height="13"
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
            {{ latitude?.toFixed(4) }}, {{ longitude?.toFixed(4) }}
          </p>

          <div v-if="hasLocation" class="map-preview">
            <AnimalMap :latitude="latitude ?? 0" :longitude="longitude ?? 0" />
          </div>
        </div>

        <div v-else class="manual-section">
          <AnimalMapPicker :latitude="latitude" :longitude="longitude" @pick="onLocationPicked" />
          <p v-if="hasLocation" class="coords-display">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              width="13"
              height="13"
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
            {{ latitude?.toFixed(4) }}, {{ longitude?.toFixed(4) }}
          </p>
        </div>

        <span v-if="formErrors.location" class="field-error" role="alert">
          {{ formErrors.location }}
        </span>
      </section>

      <div class="form-footer">
        <p v-if="submitError" class="submit-error" role="alert">{{ submitError }}</p>
        <AppButton type="submit" variant="primary" :disabled="isSubmitting">
          <AppSpinner v-if="isSubmitting" />
          <span>{{ submitButtonLabel }}</span>
        </AppButton>
      </div>
    </form>
  </div>
</template>

<style scoped>
.post-view {
  max-width: 640px;
  margin: 0 auto;
  padding: var(--space-4);
}

.post-header {
  display: flex;
  align-items: center;
  gap: var(--space-3);
  padding: var(--space-4) 0 var(--space-6);
}

.back-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  width: var(--space-10);
  height: var(--space-10);
  border: none;
  background: transparent;
  border-radius: var(--radius-md);
  color: var(--color-neutral-600);
  cursor: pointer;
  transition: background-color var(--transition-fast), color var(--transition-fast);
}

.back-btn:hover {
  background: var(--color-neutral-100);
  color: var(--color-neutral-800);
}

.post-title {
  font-size: var(--font-size-xl);
  font-weight: var(--font-weight-semibold);
  color: var(--color-neutral-900);
  margin: 0;
}

.post-form {
  display: flex;
  flex-direction: column;
  gap: var(--space-6);
}

.form-section {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.field-label {
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-neutral-800);
}

.required {
  color: var(--color-error);
}

.textarea-field {
  padding: var(--space-3);
  border: 1px solid var(--color-neutral-200);
  border-radius: var(--radius-sm);
  font-size: var(--font-size-base);
  font-family: var(--font-family-base);
  color: var(--color-neutral-800);
  background: var(--color-surface);
  resize: vertical;
  min-height: 120px;
  line-height: var(--line-height-normal);
  transition: border-color var(--transition-fast);
  outline: none;
}

.textarea-field:focus {
  border-color: var(--color-primary-500);
}

.textarea-field:disabled {
  background: var(--color-neutral-100);
  color: var(--color-neutral-400);
  cursor: not-allowed;
}

.textarea-field--error {
  border-color: var(--color-error);
}

.field-error {
  font-size: var(--font-size-sm);
  color: var(--color-error);
}

.section-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.section-title {
  font-size: var(--font-size-base);
  font-weight: var(--font-weight-semibold);
  color: var(--color-neutral-800);
  margin: 0;
}

.add-photo-btn {
  display: inline-flex;
  align-items: center;
  gap: var(--space-1);
  padding: var(--space-2) var(--space-3);
  border: 1px solid var(--color-primary-500);
  border-radius: var(--radius-sm);
  background: transparent;
  color: var(--color-primary-500);
  font-size: var(--font-size-sm);
  font-family: var(--font-family-base);
  font-weight: var(--font-weight-medium);
  cursor: pointer;
  transition: background-color var(--transition-fast);
}

.add-photo-btn:hover:not(:disabled) {
  background: var(--color-primary-50);
}

.add-photo-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.files-grid {
  list-style: none;
  padding: 0;
  margin: 0;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: var(--space-3);
}

.file-item {
  display: flex;
  flex-direction: column;
  gap: var(--space-1);
}

.file-preview {
  position: relative;
  aspect-ratio: 1;
  border-radius: var(--radius-md);
  overflow: hidden;
  background: var(--color-neutral-100);
}

.file-thumb {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}

.file-overlay {
  position: absolute;
  inset: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgb(0 0 0 / 0.45);
  color: var(--color-surface);
}

.file-overlay--success {
  background: rgb(22 163 74 / 0.75);
}

.file-overlay--error {
  background: rgb(220 38 38 / 0.75);
}

.file-meta {
  display: flex;
  align-items: center;
  justify-content: space-between;
  min-height: var(--space-6);
}

.file-retry-btn {
  font-size: var(--font-size-xs);
  color: var(--color-primary-600);
  background: transparent;
  border: none;
  cursor: pointer;
  padding: 0;
  font-family: var(--font-family-base);
  text-decoration: underline;
}

.file-remove-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: var(--space-6);
  height: var(--space-6);
  border: none;
  background: transparent;
  color: var(--color-neutral-400);
  cursor: pointer;
  border-radius: var(--radius-sm);
  margin-left: auto;
  transition: color var(--transition-fast);
}

.file-remove-btn:hover {
  color: var(--color-error);
}

.location-tabs {
  display: grid;
  grid-template-columns: 1fr 1fr;
  border: 1px solid var(--color-neutral-200);
  border-radius: var(--radius-sm);
  overflow: hidden;
}

.tab-btn {
  padding: var(--space-2) var(--space-4);
  border: none;
  background: var(--color-surface);
  color: var(--color-neutral-600);
  font-size: var(--font-size-sm);
  font-family: var(--font-family-base);
  font-weight: var(--font-weight-medium);
  cursor: pointer;
  transition: background-color var(--transition-fast), color var(--transition-fast);
}

.tab-btn--active {
  background: var(--color-primary-500);
  color: var(--color-surface);
}

.gps-section {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.manual-section {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
}

.location-blocked-note {
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
  margin: 0;
}

.inline-link {
  background: none;
  border: none;
  padding: 0;
  font-size: inherit;
  font-family: inherit;
  color: var(--color-primary-600);
  cursor: pointer;
  text-decoration: underline;
}

.coords-display {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
  margin: 0;
}

.map-preview {
  border-radius: var(--radius-lg);
  overflow: hidden;
}

.form-footer {
  display: flex;
  flex-direction: column;
  gap: var(--space-3);
  padding-bottom: var(--space-4);
}

.submit-error {
  font-size: var(--font-size-sm);
  color: var(--color-error);
  text-align: center;
  margin: 0;
}
</style>
