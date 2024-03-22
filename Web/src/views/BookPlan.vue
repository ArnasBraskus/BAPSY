<script setup lang="ts">
import { ModalsContainer, useModal } from 'vue-final-modal'
import ModalConfirmPlainCss from '../Components/ModalConfirmPlainCss.vue'
import { RouterView } from 'vue-router'
import Header from '../Components/Header.vue'
import Footer from '../Components/Footer.vue'


const { open, close } = useModal({
    component: ModalConfirmPlainCss,
    attrs: {
        title: 'Plan',
        onConfirm() {
            close()
        },
    },
})
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