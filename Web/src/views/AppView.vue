<script setup>
    import { ref } from 'vue';
    import router from '../router';
    import { getCookie } from '../utils/cookies.js'
    import { logout, getProfileInfo } from '../utils/auth.js'

    function doLogout(e) {
        logout();
        router.push({path: '/'});
    }

    const profile = getProfileInfo();

    const name = ref(profile.name);
    const email = ref(profile.email);
</script>

<template>
    <h1>APP</h1>
    <b>vardas: {{name}}</b> <br/>
    <b>el. paštas: {{email}}</b> <br/>
    <button @click="doLogout">Atsijungti</button>
</template>
