<script setup>
import Header from '../Components/Header.vue'
import Footer from '../Components/Footer.vue'
</script>

<template>
  <div id="app" class="image-container">
    <Header />

    <main class="darker ">
      <div>
        <div class="home">
          <h1>Welcome to Our Book Reading Progress Tracker</h1>

          <p>This is a platform where you can keep track of your reading progress, discover new books, and more.</p>
          <button class="cta-button" @click="$router.push('login')">Login to Get Started</button>
          <!--  <RouterLink to="/login" class="cta-button">Login </RouterLink> -->
        </div>
      </div>
    </main>

    <Footer />
  </div>
</template>

<style scoped>
.home {
  text-align: center;
  color: var(--quaternary-color);
  padding: 20px;
  margin-top: 20px;

  background-color: rgba(185, 148, 112, 0.75);
  font-size:x-large;
  border-radius: 50px 50px 50px 50px;
 }
</style>
