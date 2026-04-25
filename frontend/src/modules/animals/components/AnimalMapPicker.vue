<script setup lang="ts">
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { ref, onMounted, onUnmounted } from 'vue'

interface Props {
  latitude?: number | null
  longitude?: number | null
}

interface PickedLocation {
  latitude: number
  longitude: number
}

const props = withDefaults(defineProps<Props>(), {
  latitude: null,
  longitude: null,
})

const emit = defineEmits<{
  pick: [location: PickedLocation]
}>()

const mapContainer = ref<HTMLDivElement | null>(null)

let map: L.Map | null = null
let marker: L.Marker | null = null

const DEFAULT_LAT = -23.5505
const DEFAULT_LNG = -46.6333
const DEFAULT_ZOOM = 12

// Same icon style as AnimalMap for visual consistency.
// SVG fill literals are required — SVG fill does not support CSS custom properties.
const pinIcon = L.divIcon({
  html: `<svg xmlns="http://www.w3.org/2000/svg" width="28" height="40" viewBox="0 0 28 40">
    <path d="M14 0C6.268 0 0 6.268 0 14c0 10.5 14 26 14 26S28 24.5 28 14C28 6.268 21.732 0 14 0z" fill="#f97316"/>
    <circle cx="14" cy="14" r="6" fill="white"/>
  </svg>`,
  className: '',
  iconSize: [28, 40],
  iconAnchor: [14, 40],
  popupAnchor: [0, -40],
})

function emitPosition(lat: number, lng: number): void {
  emit('pick', { latitude: lat, longitude: lng })
}

function placeMarker(lat: number, lng: number): void {
  if (!map) return
  if (marker) {
    marker.setLatLng([lat, lng])
  } else {
    marker = L.marker([lat, lng], { icon: pinIcon, draggable: true }).addTo(map)
    marker.on('dragend', () => {
      const pos = marker!.getLatLng()
      emitPosition(pos.lat, pos.lng)
    })
  }
  emitPosition(lat, lng)
}

function initMap(): void {
  if (!mapContainer.value) return

  const initialLat = props.latitude ?? DEFAULT_LAT
  const initialLng = props.longitude ?? DEFAULT_LNG

  map = L.map(mapContainer.value, { zoomControl: true, attributionControl: true }).setView(
    [initialLat, initialLng],
    DEFAULT_ZOOM,
  )

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    maxZoom: 19,
  }).addTo(map)

  map.on('click', (e: L.LeafletMouseEvent) => {
    placeMarker(e.latlng.lat, e.latlng.lng)
  })

  if (props.latitude !== null && props.longitude !== null) {
    placeMarker(props.latitude, props.longitude)
  }
}

onMounted(initMap)

onUnmounted(() => {
  map?.remove()
  map = null
  marker = null
})
</script>

<template>
  <div class="map-picker">
    <p class="map-hint" aria-live="polite">
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
      Clique no mapa para definir a localização
    </p>
    <div
      ref="mapContainer"
      class="map-container"
      aria-label="Mapa para seleção de localização. Clique para marcar o ponto."
    />
  </div>
</template>

<style scoped>
.map-picker {
  display: flex;
  flex-direction: column;
  gap: var(--space-2);
}

.map-hint {
  display: flex;
  align-items: center;
  gap: var(--space-1);
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
  margin: 0;
}

.map-container {
  width: 100%;
  height: 280px;
  border-radius: var(--radius-lg);
  border: 1px solid var(--color-neutral-200);
  overflow: hidden;
  /* isolates Leaflet z-indexes from the rest of the layout */
  isolation: isolate;
}
</style>
