<script setup>
    import router from '../router'
    import { decodeCredential } from 'vue3-google-login'
    import { getCookie, setCookie } from '../utils/cookies.js'

    function loginCallback(res) {
        setCookie('g-signin-credential', res.credential, 7 * 24 * 60 * 60);
        router.push({path: 'app'});
    }
</script>

<template>
  <h1>LOGIN</h1>
  <GoogleLogin :callback="loginCallback"/>
</template>
