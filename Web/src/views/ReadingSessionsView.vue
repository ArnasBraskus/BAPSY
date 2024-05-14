<script setup>
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import router from '../router';

import Footer from '../Components/Footer.vue';
import Menu from '../Components/Menu.vue';

import { getPlan } from '../utils/plans.js';
import { getSessions, markSession } from '../utils/sessions.js';

const route = useRoute();
const planId = route.params.id;

var sessionInfo = ref([]);

var fetchSessions = (async () => {
  sessionInfo.value = await getSessions(planId);
});

function goBack() {
  router.push({path: `/books/${planId}`});
}

function goToSession(id) {
  router.push({path: `/books/${planId}/sessions/${id}`});
}

function doMarkSession(id, actual) {
  markSession(id, actual);
  fetchSessions();
}

fetchSessions();

</script>

<template>
  <div id="app" class="plan-background">
    <Menu/>
    <main>
      <h1>Reading sessions</h1>
      <div class="div-margin">
        <button class="cta-button" @click="goBack">Back</button>
      </div>
      <table class="sessions-table">
        <tr class="sessions-row">
          <th class="sessions-header">Date</th>
          <th class="sessions-header">Pages</th>
          <th class="sessions-header">Status</th>
        </tr>
        <tr v-for="session in sessionInfo" class="sessions-row">
          <td class="sessions-data"><a href="#" @click="goToSession(session.id)">{{ session.date }}</a></td>
          <td class="sessions-data">{{ session.actual }} / {{ session.goal }}</td>
          <td class="sessions-data">
            <a v-if="session.actual > 0" class="green">✓</a>
            <a v-else href="#" @click="doMarkSession(session.id, session.goal)" class="white">✓</a>
          </td>

        </tr>
      </table>
    </main>
  </div>
</template>

<style scoped>
.white{
  color: white;
}
.green {
  color: green;
}
.div-margin {
  margin-top: 20px;
  margin-bottom: 20px;
}
.sessions-table {
  width: 1000px;
  color: white;
  font-size: 25px;
}
.sessions-row {
  text-align: left;
}
.sessions-header, .sessions-data {
  border-bottom: .0625rem solid #e3e3e3;
}
</style>
