<script setup>
import { ref } from 'vue';
import Menu from '../Components/Menu.vue';

import { getToken } from '../utils/calendar.js';

async function getCalendarUrl() {
  const token = await getToken();
  return `${window.location.origin}/api/calendar/${token.userId}/export?t=${token.token}`;
}

const url = ref('');

(async () => {
  url.value = await getCalendarUrl();
})();

const googleCalendarUrl = ref('https://calendar.google.com/calendar/r/settings/export');

function goToGoogleCalendar() {
  window.open(googleCalendarUrl.value, '_blank', 'noopener,noreferrer');
}

function goToUrl() {
  window.open(url.value, '_blank', 'noopener,noreferrer');
}
</script>

<template>
  <div id="app" class="plan-background">
    <Menu/>    
    <main>
      <br>
      <button class="cta-button" @click="goToUrl">Download calendar file</button>
      <br>
      <button class="cta-button" @click="goToGoogleCalendar">Go to Google Calendar</button>
      <br>      
    </main>
  </div>
</template>
