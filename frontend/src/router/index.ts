import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '@/modules/auth/stores/auth.store'

declare module 'vue-router' {
  interface RouteMeta {
    public?: boolean
  }
}

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      redirect: '/feed',
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('@/modules/auth/views/LoginView.vue'),
      meta: { public: true },
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/modules/auth/views/RegisterView.vue'),
      meta: { public: true },
    },
    {
      path: '/feed',
      name: 'feed',
      component: () => import('@/modules/animals/views/FeedView.vue'),
    },
    {
      path: '/post',
      name: 'post-animal',
      component: () => import('@/modules/animals/views/PostAnimalView.vue'),
    },
    {
      path: '/animals/:id',
      name: 'animal-detail',
      component: () => import('@/modules/animals/views/AnimalDetailView.vue'),
    },
  ],
})

router.beforeEach(to => {
  const auth = useAuthStore()

  if (!to.meta.public && !auth.isAuthenticated) {
    return { name: 'login', query: { redirect: to.fullPath } }
  }

  if (to.meta.public && auth.isAuthenticated) {
    return { name: 'feed' }
  }
})

export default router
