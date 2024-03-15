import { createRouter, createWebHistory } from 'vue-router';
import { isLoggedIn } from '../utils/auth.js';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../views/HomeView.vue')
    },
    {
      path: '/login',
      name: 'login',
      component: () => import('../views/LoginView.vue'),
      meta: {
        hideForAuth: true
      }
    },
    {
      path: '/app',
      name: 'app',
      component: () => import('../views/AppView.vue'),
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/about',
      name: 'about',
      component: () => import('../views/AboutView.vue'),
    },
    {
      path: '/plan',
      name: 'plan',
      component: () => import('../views/BookPlan.vue'),
    }
  ]
});

router.beforeEach((to, from) => {
  if (to.meta.requiresAuth && !isLoggedIn()) {
    return {
      path: '/login',
      query: {redirect: to.fullPath}
    };
  }

  if (to.meta.hideForAuth && isLoggedIn()) {
    return {
      path: '/app'
    };
  }
});

export default router;
