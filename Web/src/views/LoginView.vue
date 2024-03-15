<script setup>
import router from '../router'
import { requestToken } from '../utils/auth.js'
import Header from '../Components/Header.vue'
import Footer from '../Components/Footer.vue'

async function loginCallback(res) {
  if (!await requestToken(res.credential))
    return;

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
