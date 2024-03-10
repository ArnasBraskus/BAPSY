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
    <div>
    <div>
        <h1>Welcome to the App page!</h1>
    </div>
    <div class="center">
    
    <div class="info">
    <b>API: {{apistatus}}</b> <br/>
    <b>Vardas: {{name}}</b> <br/>
    <b>El. paštas: {{email}}</b> <br/>
    
    </div>    
    </div>
    <button @click="doLogout">Atsijungti</button>
    </div>
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
        margin: 10px;
        transition: background-color 0.3s ease;
    }

    button:hover {
        background-color: rgba(0, 0, 0, 0.5);
    }

    button:active {
        background-color: #004080;
    }
    .info{
        text-align: left;
        color: var(--quaternary-color);
        padding: 20px;
        margin-top: 20px;
    }
    .center{
        display: flex;
        justify-content: center;
        align-items: center;
        align-content: center;
    }
</style>