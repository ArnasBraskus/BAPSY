import { getCookie, deleteCookie } from './cookies.js'
import { decodeCredential } from 'vue3-google-login'
import { googleLogout } from 'vue3-google-login'

export function isLoggedIn() {
    return getCookie('g-signin-credential') != null;
}

export function getProfileInfo() {
    const credential = getCookie('g-signin-credential');
    const decoded = decodeCredential(credential);

    return {
        name: decoded.name,
        email: decoded.email
    }
}

export function logout() {
    googleLogout();
    deleteCookie('g-signin-credential');
}
