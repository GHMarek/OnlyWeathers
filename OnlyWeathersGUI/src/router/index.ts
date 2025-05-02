import { createRouter, createWebHistory } from 'vue-router'

// Import synchroniczny – dla strony głównej i dashboardu (często używane)
import HomeView from '@/views/HomeView.vue'
import Dashboard from '@/views/Dashboard.vue'

// Router Vue 3 z historią przeglądarki (czyli ładne adresy bez #)
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),

  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView,
    },
    {
      path: '/register',
      name: 'register',
      component: () => import('@/views/RegisterView.vue'), // lazy loading
    },
    {
      path: '/change-password',
      name: 'change-password',
      component: () => import('@/views/ChangePasswordView.vue'),
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: () => import('@/views/ForgotPasswordView.vue'),
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: Dashboard,
    },
    // ✨ fallback 404 (opcjonalnie)
    {
      path: '/:pathMatch(.*)*',
      redirect: '/',
    },
  ],
})

export default router
