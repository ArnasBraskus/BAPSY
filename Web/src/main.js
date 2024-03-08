import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import vue3_google_login from 'vue3-google-login'
import config from './config.js'

const app = createApp(App)

app.use(router)
app.use(vue3_google_login, {
    clientId: config.googleSignInClientId
});

app.mount('#app')
