<script setup>
import router from '../router'
import { decodeCredential } from 'vue3-google-login'
import { getCookie, setCookie } from '../utils/cookies.js'
    import { apiDoPostUnauthenticated, apiDoGet } from '../utils/api.js'
    import { RouterView } from 'vue-router'
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
    <div>
        <div id="app" class="image-container">
            <Header />

            <main class="darker ">
                <div>
                    <div>
                        <GoogleLogin :callback="loginCallback" />
                    </div>
                    <RouterView />
                </div>
            </main>
            <Footer />
        </div>
    </div>
</template>
