<script setup>
    import { ref } from 'vue';
    import router from '../router';
    import { getCookie } from '../utils/cookies.js'
    import { logout } from '../utils/auth.js'
    import { apiDoGet } from '../utils/api.js'

    function doLogout(e) {
        logout();
        router.push({path: '/'});
    }

    const apistatus = ref('');
    const name = ref('');
    const email = ref('');

    (async () => {
        const response = await apiDoGet('/api/profile');
        const data = await response.json();

        apistatus.value = response.statusText;
        name.value = data.name;
        email.value = data.email;
    })();
</script>

<template>
    <h1>APP</h1>
    <b>API: {{apistatus}}</b> <br/>
    <b>vardas: {{name}}</b> <br/>
    <b>el. paštas: {{email}}</b> <br/>
    <button @click="doLogout">Atsijungti</button>
</template>

