import { setCookie, getCookie, deleteCookie } from './cookies.js';
import { apiDoPostUnauthenticated } from './api.js';
import { googleLogout } from 'vue3-google-login';

const API_TOKEN_COOKIE = 'api-token';

export function isLoggedIn() {
  return getCookie(API_TOKEN_COOKIE) != null;
}

export async function requestToken(credential) {
  const response = await apiDoPostUnauthenticated('/api/auth/google', { jwttoken: credential });
  const data = await response.json();

  setCookie(API_TOKEN_COOKIE, data.token, data.validity);

  return true;
}

export function logout() {
  googleLogout();
  deleteCookie(API_TOKEN_COOKIE);
}
