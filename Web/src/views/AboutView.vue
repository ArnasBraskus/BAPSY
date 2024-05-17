<script setup>
import Footer from '../Components/Footer.vue';
</script>

<template>
  <div id="app" class="image-container">
    <header class="header">
        <div class="logo"></div>
        <nav>
            <RouterLink to="/">Home</RouterLink>
            <RouterLink to="/about">About</RouterLink>            
        </nav>
    </header>

    <main class="darker">
      <h1>About Page</h1>
      <p>Welcome to the About page!</p>
    </main>

    <Footer />
  </div>
</template>


<style scoped>
p {
  font-size: 16px;
  color: var(--quaternary-color);
}
</style>
