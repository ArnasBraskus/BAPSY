<script setup>
import { RouterView } from 'vue-router'
import Header from '../Components/Header.vue'
import Footer from '../Components/Footer.vue'
</script>

<template>
    <div>
        <Header />
        <div id="app" class="image-container">

            <main class="darker ">
                <div>
                    <div class="home">
                        <h1>Welcome to Our Book Reading Progress Tracker</h1>

                        <p>This is a platform where you can keep track of your reading progress, discover new books, and more.</p>

                        <RouterLink to="/login" class="cta-button">Login to Get Started</RouterLink>
                    </div>
                    <RouterView />
                </div>
            </main>
        </div>
        <Footer />
    </div>
</template>

<style scoped>
#app {
        display: flex;
        flex-direction: column;
        background-size: auto;
        min-height: 100vh;
    }

    .image-container {
        background-image: url('https://images.unsplash.com/photo-1457369804613-52c61a468e7d?q=80&w=2070&auto=format&fit=crop&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D');
        background-size: cover;
        background-position: center;
        position: relative;
        top: 0;
        left: 0;
        right: 0;
        bottom: 0;
 }
 .home {
  text-align: center;
  color: var(--quaternary-color);
  padding: 20px;
  margin-top: 20px;
}

.cta-button {
  display: inline-block;
  margin-top: 20px;
  padding: 10px 20px;
  background-color: var(--secondary-color);
  color: #fff;
  text-decoration: none;
  border-radius: 5px;
  transition: background-color 0.3s ease;
}


.cta-button:hover {
  background-color: rgba(0, 0, 0, 0.3);
}
.cta-button:active {
        background-color: #004080;
    }

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
