<script setup>
import Header from '../Components/Header.vue'
import Footer from '../Components/Footer.vue'
import { requestToken } from '../utils/auth.js'
import router from '../router';
import { GoogleLogin } from 'vue3-google-login';

async function loginCallback(res) {
  if (!await requestToken(res.credential))
    return;

  router.push({ path: 'plan' });
}
</script>

<template>
  <div id="app" class="image-container">
    <Header />

    <main class="darker ">
      <div class="home">
        <h1>Welcome to Our Book Reading Progress Tracker</h1>

        <p>This is a platform where you can keep track of your reading progress, discover new books, and more.</p>
      </div>
      <div>
        <div>
          <div class="login">
              <h1 style="margin-top:10px;">&#8203;Join Now&#8203;</h1>
              <GoogleLogin :callback="loginCallback" />
          </div>
        </div>
      </div>
    </main>

    <Footer />
  </div>
</template>

<style scoped>
.home {
  text-align: center;
  color: var(--quaternary-color);
  padding: 20px;
  margin-top: 20px;

  background-color: rgba(185, 148, 112, 0.75);
  font-size:x-large;
  border-radius: 50px 50px 50px 50px;
 }

 .login {
  display: inline-block;
  text-align: center;
  color: var(--quaternary-color);
  margin-top: 50px;
  padding: 15px;

  background-color: rgba(40, 39, 38, 0.75);
  font-size:x-large;
  border-radius: 50px 50px 50px 50px;
 }
</style>

