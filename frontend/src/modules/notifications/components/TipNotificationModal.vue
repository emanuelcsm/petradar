<script setup lang="ts">
import AppModal from '@/components/AppModal.vue'
import type { TipPayloadDto } from '@/types/api.types'

interface Props {
  open: boolean
  tip: TipPayloadDto
}

defineProps<Props>()
defineEmits<{ close: [] }>()
</script>

<template>
  <AppModal :open="open" @close="$emit('close')">
    <div class="tip-modal">
      <h2 class="tip-modal-title">Dica recebida</h2>
      <p class="tip-modal-sender">De: <strong>{{ tip.senderName }}</strong></p>
      <p class="tip-modal-message">{{ tip.tipMessage }}</p>
      <button class="tip-modal-close" @click="$emit('close')">Fechar</button>
    </div>
  </AppModal>
</template>

<style scoped>
.tip-modal {
  width: min(480px, 90vw);
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.tip-modal-title {
  font-size: var(--font-size-lg);
  font-weight: var(--font-weight-semibold);
  color: var(--color-neutral-900);
  margin: 0;
}

.tip-modal-sender {
  font-size: var(--font-size-sm);
  color: var(--color-neutral-600);
  margin: 0;
}

.tip-modal-message {
  font-size: var(--font-size-base);
  color: var(--color-neutral-800);
  line-height: var(--line-height-relaxed);
  background: var(--color-neutral-50);
  border-radius: var(--radius-md);
  padding: var(--space-4);
  margin: 0;
}

.tip-modal-close {
  align-self: flex-end;
  background: none;
  border: 1px solid var(--color-neutral-300);
  border-radius: var(--radius-md);
  padding: var(--space-2) var(--space-4);
  font-size: var(--font-size-sm);
  font-weight: var(--font-weight-medium);
  color: var(--color-neutral-700);
  cursor: pointer;
  transition: background var(--transition-fast);
}

.tip-modal-close:hover {
  background: var(--color-neutral-100);
}
</style>
