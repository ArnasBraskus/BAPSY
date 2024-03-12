<script setup>
    import { ref } from 'vue';
    import router from '../router';
    import { getCookie } from '../utils/cookies.js'
    import { logout } from '../utils/auth.js'
    import { apiDoGet } from '../utils/api.js'
    import { RouterView } from 'vue-router'
    import Footer from '../Components/Footer.vue'

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
        <header class="header">
            <div class="logo"></div>
            <nav>
                <RouterLink to="/">Home</RouterLink>
                <RouterLink to="/about">About</RouterLink>
                <RouterLink to="/plan">Plan</RouterLink>
                <RouterLink to="/books">Books</RouterLink>
                <RouterLink to="/settings">Settings</RouterLink>
                <!-- dar neveikia normaliai
    <button v-if="isLoggedIn" @click="doLogout">Log Out</button>
    <button v-else @click="doLogin">Log In</button>
    -->
            </nav>
        </header>
            <div id="app" class="image-container">
                <main class="darker ">
                    <div>
                        <div>
                            <div>
                                <h1>Welcome to the App page!</h1>
                            </div>
                            <div class="center">

                                <div class="info">
                                    <b>API: {{apistatus}}</b> <br />
                                    <b>Vardas: {{name}}</b> <br />
                                    <b>El. paštas: {{email}}</b> <br />

                                </div>
                            </div>
                            <button class="cta-button"@click="doLogout">Logout</button>
                        </div>
                        <RouterView />
                    </div>
                </main>
                <Footer />
            </div>
</div>
</template>

<style scoped>
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
