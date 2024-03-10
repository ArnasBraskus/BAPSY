<script setup>
import router from '../router';
import { logout } from '../utils/auth.js';
import { ref } from 'vue';

const isLoggedIn = ref(false);

function doLogout(e) {
    logout();
    isLoggedIn.value = false;
    router.push({ path: '/' });
}

function doLogin(e) {
    isLoggedIn.value = true;
    router.push({ path: '/login' });
}
</script>

<template>
    <header class="header">
        <div class="logo"></div>
        <nav>
            <RouterLink to="/">Home</RouterLink>
            <RouterLink to="/about">About</RouterLink>

            <!-- dar neveikia normaliai
            <button v-if="isLoggedIn" @click="doLogout">Log Out</button>
            <button v-else @click="doLogin">Log In</button>
            -->

        </nav>
    </header>
</template>

<style scoped>
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
