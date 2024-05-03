<script setup>
import { ref } from 'vue';
import { logout } from '../utils/auth.js';
import { getUserProfile } from '../utils/user.js';
import router from '../router';

function doLogout() {
  logout();
  router.push({path: '/'});
}

const name = ref(localStorage.getItem('name'));
const email = ref(localStorage.getItem('email'));

(async () => {
  const profile = await getUserProfile();

  localStorage.setItem('name', profile.name);
  localStorage.setItem('email', profile.email);

  name.value = profile.name;
  email.value = profile.email;
})();

</script>

<template>
    <header class="header">
      <nav class="nav">
        <div class="nav-left">
          <ul>
            <li><span style="margin-right: 15px">BookQuest</span></li>
            <li><RouterLink to="/app">Home</RouterLink></li>
            <li><RouterLink to="/books">Books</RouterLink></li>
            <li><RouterLink to="/calendar">Calendar</RouterLink></li>
            <li><RouterLink to="/reports">Reports</RouterLink></li>
          </ul>
        </div>
        <div class="nav-right">
          <ul>
            <li><button @click="doLogout()">Logout</button></li>
            <li><a href="#">{{ name }}</a></li>
          </ul>
        </div>
      </nav>
    </header>
</template>

<style scoped>
li:has( > a.router-link-active) {
  border-bottom: solid 3px transparent;
  border-bottom-color: white;
}

header {
  background-color: #333;
  color: #FFFFEC;
  text-align: center;
  background-color: var(--primary-color);
}

header h1 {
  margin: 0;
  font-size: 24px;
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

nav {
  padding-left: 10px;
}
nav ul {
  list-style-type: none;
  margin: 0;
  padding: 0;
  overflow: hidden;
}
nav h1 a span {
  display: block;
}
.nav-left {
  float: left;
}
.nav-left ul li {
  padding: 8px;
  margin-right: 10px;
  float: left;
}
.nav-right ul li {
  padding: 8px;
  margin-right: 10px;
  float: right;
}
</style>
