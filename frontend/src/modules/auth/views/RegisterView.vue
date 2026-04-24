<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth.store'
import AuthFormWrapper from '../components/AuthFormWrapper.vue'
import AppInput from '@/components/AppInput.vue'
import AppButton from '@/components/AppButton.vue'

const router = useRouter()
const authStore = useAuthStore()

const name = ref('')
const email = ref('')
const password = ref('')
const errors = ref<Record<string, string>>({})
const isSubmitting = ref(false)

function validate(): boolean {
  errors.value = {}
  if (!name.value || name.value.trim().length < 2) {
    errors.value.name = 'Nome deve ter pelo menos 2 caracteres.'
  }
  if (!email.value) {
    errors.value.email = 'E-mail obrigatório.'
  } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
    errors.value.email = 'E-mail inválido.'
  }
  if (!password.value || password.value.length < 6) {
    errors.value.password = 'Senha deve ter pelo menos 6 caracteres.'
  }
  return Object.keys(errors.value).length === 0
}

async function submit(): Promise<void> {
  if (!validate()) return
  isSubmitting.value = true
  await authStore.register(name.value.trim(), email.value, password.value)
  isSubmitting.value = false
  if (authStore.isAuthenticated) {
    router.push({ name: 'feed' })
  }
}
</script>

<template>
  <AuthFormWrapper title="Criar conta">
    <form class="auth-form" @submit.prevent="submit">
      <div class="form-fields">
        <AppInput
          id="name"
          v-model="name"
          label="Nome"
          type="text"
          placeholder="Seu nome"
          :error="errors.name"
          :disabled="isSubmitting"
        />
        <AppInput
          id="email"
          v-model="email"
          label="E-mail"
          type="email"
          placeholder="seu@email.com"
          :error="errors.email"
          :disabled="isSubmitting"
        />
        <AppInput
          id="password"
          v-model="password"
          label="Senha"
          type="password"
          placeholder="Mínimo 6 caracteres"
          :error="errors.password"
          :disabled="isSubmitting"
        />
      </div>
      <p v-if="authStore.error" class="form-api-error">{{ authStore.error }}</p>
      <AppButton type="submit" variant="primary" :disabled="isSubmitting">
        {{ isSubmitting ? 'Criando conta...' : 'Criar conta' }}
      </AppButton>
    </form>
    <template #footer>
      Já tem conta?
      <RouterLink :to="{ name: 'login' }" class="auth-link">Entrar</RouterLink>
    </template>
  </AuthFormWrapper>
</template>

<style scoped>
.auth-form {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.form-fields {
  display: flex;
  flex-direction: column;
  gap: var(--space-4);
}

.form-api-error {
  font-size: var(--font-size-sm);
  color: var(--color-error);
  text-align: center;
  margin: 0;
}

.auth-link {
  color: var(--color-primary-500);
  font-weight: var(--font-weight-medium);
  text-decoration: none;
}

.auth-link:hover {
  color: var(--color-primary-600);
  text-decoration: underline;
}
</style>
