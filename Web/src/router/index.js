import { createRouter, createWebHistory } from 'vue-router';
import { isLoggedIn } from '../utils/auth.js';

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: () => import('../views/HomeView.vue'),
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
      path: '/books',
      name: 'books',
      component: () => import('../views/BooksView.vue'),
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/calendar',
      name: 'calendar',
      component: () => import('../views/CalendarView.vue'),
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
      path: '/books/:id',
      name: 'book',
      component: () => import('../views/BookView.vue'),
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/books/:id/sessions',
      name: 'book_sessions',
      component: () => import('../views/ReadingSessionsView.vue'),
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/books/:planId/sessions/:id',
      name: 'book_session_mark',
      component: () => import('../views/SessionMarkView.vue'),
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/confirmation',
      name: 'confirmation',
      component: () => import('../views/Confirmation.vue'),
    }
  ]
});

router.beforeEach((to, from) => {
  if (to.meta.requiresAuth && !isLoggedIn()) {
    return {
      path: '/',
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
