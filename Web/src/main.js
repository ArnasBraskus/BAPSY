import './assets/main.css';
import 'vue-final-modal/style.css';

import { createApp } from 'vue';
import { createVfm } from 'vue-final-modal';
import App from './App.vue';
import router from './router';
import vue3_google_login from 'vue3-google-login';
import config from './config.js';

const app = createApp(App);
const vfm = createVfm();

app.use(vfm);
app.use(router);
app.use(vue3_google_login, {
  clientId: config.googleSignInClientId
});

app.mount('#app');
