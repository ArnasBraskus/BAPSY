<script setup>
    import router from '../router'
    import { decodeCredential } from 'vue3-google-login'
    import { getCookie, setCookie } from '../utils/cookies.js'
    import { apiDoPostUnauthenticated, apiDoGet } from '../utils/api.js'

    async function loginCallback(res) {
        const response = await apiDoPostUnauthenticated('/api/auth/google', {jwttoken: res.credential});
        const data = await response.json();

        setCookie('api-token', data['token'], 1 * 24 * 60 * 60);

        router.push({path: 'app'});
    }
</script>

<template>
  <h1>LOGIN</h1>
  <GoogleLogin :callback="loginCallback"/>
</template>
