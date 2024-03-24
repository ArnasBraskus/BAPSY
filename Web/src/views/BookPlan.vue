<script setup lang="ts">
import { ModalsContainer, useModal } from 'vue-final-modal'
import ModalConfirmPlainCss from '../Components/ModalConfirmPlainCss.vue'
import { RouterView } from 'vue-router'
import Header from '../Components/Header.vue'
import Footer from '../Components/Footer.vue'
import router from '../router';
import { logout } from '../utils/auth.js';
import { ref } from 'vue';
import { apiDoGet } from '../utils/api.js'



const { open, close } = useModal({
    component: ModalConfirmPlainCss,
    attrs: {
        title: 'Plan',
        onConfirm() {
            close()
        },
    },
})

function doLogout(e) {
  logout();
  router.push({path: '/'});
}

const email = ref('');

(async () => {
  const response = await apiDoGet('/api/user/profile');
  const data = await response.json();
  
  email.value = data.email;
})();

</script>

<template>
     <div>
        <header class="header">
            <div class="logo"></div>
            <nav>
                <RouterLink to="/plan">Plan</RouterLink>
                <RouterLink to="/books">Books</RouterLink>
                <RouterLink to="/settings">Settings</RouterLink>
                {{email}}
                <button @click="doLogout">Logout</button>
            </nav>
        </header>
            <div id="app" class="image-container">
                <main class="darker ">
                    <div>
                        <div>
                            <div>
                                <h1>Plan management</h1>
                            </div>
                            <div>
                                <VButton class="cta-button" @click="open">
                                    Create a Plan
                                </VButton>
                                <ModalsContainer />
                            </div>
                        </div>
                        <RouterView />
                    </div>
                </main>
                <Footer />
            </div>
</div>
</template>

<style scoped>

    .home {
        text-align: center;
        color: var(--quaternary-color);
        padding: 20px;
        margin-top: 20px;
        background-color: rgba(185, 148, 112, 0.75);
        font-size: x-large;
        border-radius: 50px 50px 50px 50px;
    }
</style>