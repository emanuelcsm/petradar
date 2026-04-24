<script setup lang="ts">
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { ref, watch, onMounted, onUnmounted } from 'vue'

interface Props {
  latitude: number
  longitude: number
  zoom?: number
}

const props = defineProps<Props>()

const mapContainer = ref<HTMLDivElement | null>(null)

let map: L.Map | null = null
let marker: L.Marker | null = null

// DivIcon SVG avoids the bundler issue with Leaflet's default PNG assets.
// The fill literal is required — SVG fill attributes do not support CSS custom properties.
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

function initMap(): void {
  if (!mapContainer.value) return

  map = L.map(mapContainer.value, { zoomControl: true, attributionControl: true }).setView(
    [props.latitude, props.longitude],
    props.zoom ?? 15,
  )

  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    maxZoom: 19,
  }).addTo(map)

  marker = L.marker([props.latitude, props.longitude], { icon: pinIcon }).addTo(map)
}

watch(
  () => [props.latitude, props.longitude] as const,
  ([lat, lng]) => {
    if (!map || !marker) return
    const position: L.LatLngExpression = [lat, lng]
    marker.setLatLng(position)
    map.setView(position, map.getZoom())
  },
)

onMounted(initMap)

onUnmounted(() => {
  map?.remove()
  map = null
  marker = null
})
</script>

<template>
  <div ref="mapContainer" class="map-container" aria-label="Mapa com localização do animal" />
</template>

<style scoped>
.map-container {
  width: 100%;
  height: 240px;
  border-radius: var(--radius-lg);
  overflow: hidden;
  /* isolates Leaflet z-indexes from the rest of the layout */
  isolation: isolate;
}
</style>
