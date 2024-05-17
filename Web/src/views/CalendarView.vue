<script setup>
import { ref, } from 'vue';
import Menu from '../Components/Menu.vue';

import { getToken } from '../utils/calendar.js';

async function getCalendarUrl() {
  const token = await getToken();

  return `${window.location.origin}/api/calendar/${token.userId}/export?t=${token.token}`;
}

const url = ref('');

(async() => {
  url.value = await getCalendarUrl();
})();

</script>

<template>
  <div id="app" class="plan-background">
    <Menu/>
    <main>
      <a :href="url">{{ url }}</a>
    </main>
  </div>
</template>
