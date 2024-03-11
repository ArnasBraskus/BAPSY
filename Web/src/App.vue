<script setup>
import { RouterView } from 'vue-router'
import Header from './Components/Header.vue'
import Footer from './Components/Footer.vue'
</script>

<template>

                    <RouterView />
</template>

<style>
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
</style>