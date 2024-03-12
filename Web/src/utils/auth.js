import { getCookie, deleteCookie } from './cookies.js';
import { decodeCredential } from 'vue3-google-login';
import { googleLogout } from 'vue3-google-login';

export function isLoggedIn() {
  return getCookie('api-token') != null;
}

export function logout() {
  googleLogout();
  deleteCookie('api-token');
}
