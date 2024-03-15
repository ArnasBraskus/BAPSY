<script setup>
import router from '../router'
import { setCookie } from '../utils/cookies.js'
import { apiDoPostUnauthenticated, } from '../utils/api.js'
import Header from '../Components/Header.vue'
import Footer from '../Components/Footer.vue'

async function loginCallback(res) {
  const response = await apiDoPostUnauthenticated('/api/auth/google', { jwttoken: res.credential });
  const data = await response.json();

  setCookie('api-token', data['token'], data['validity']);

  router.push({ path: 'app' });
}
</script>

<template>
  <div id="app" class="image-container">
    <Header />

    <main class="darker">
      <GoogleLogin :callback="loginCallback" />
    </main>

    <Footer />
  </div>
</template>
