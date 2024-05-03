<script setup>
import { ref, inject } from 'vue';
import router from '../router';
import { logout } from '../utils/auth.js';
import { apiDoGet } from '../utils/api.js';
import Footer from '../Components/Footer.vue';
import Menu from '../Components/Menu.vue';

</script>

<template>
  <div id="app" class="plan-background">
    <Menu/>
    <main>
    </main>
  </div>
</template>
