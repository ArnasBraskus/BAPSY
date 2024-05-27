<script setup>
import Footer from '../Components/Footer.vue';
import { requestToken } from '../utils/auth.js';
import router from '../router';
import { GoogleLogin } from 'vue3-google-login';

async function loginCallback(res) {
  if (!await requestToken(res.credential))
    return;

  router.push({ path: '/app' }).then(() => {
    window.location.reload();
  });
}
</script>

<template>
  <div id="app" class="image-container">
    <header class="header">
        <div class="logo"></div>
        <nav>
            <RouterLink to="/">Home</RouterLink>
            <RouterLink to="/about">About</RouterLink>            
        </nav>
    </header>

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

<style>
main {
  justify-content: center;
  text-align: center;
}
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
header {
  background-color: #333;
  padding: 20px;
  color: #FFFFEC;
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: var(--primary-color);
}
header h1 {
  margin: 0;
  font-size: 24px;
}

header nav {
  display: flex;
  gap: 20px;
  align-self: right;
  padding-right: 20px;
}

header nav a {
  color: #fff;
  text-decoration: none;
  font-size: 16px;
  transition: color 0.3s ease;
}

header nav a:hover {
  color: var(--secondary-color);
}

header button {
  background-color: var(--primary-color);
  font-size: 16px;
  color: #fff;
  border: none;
  border-radius: 2px;
  cursor: pointer;
  transition: color 0.3s ease;
}

header button:hover {
  color: var(--secondary-color);
}
button {
    align-self: unset;
    background-color: var(--secondary-color);
    color: #fff;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 16px;
    transition: background-color 0.3s ease;
}

button:hover {
    background-color: rgba(0, 0, 0, 0.2);
}

button:active {
    background-color: #004080;
}
</style>
