<script setup>
import { ref } from 'vue';


import Menu from '../Components/Menu.vue';

import { getReports} from '../utils/reports.js';


var reportsInfo = ref([]);

var fetchReports = (async () => {
  reportsInfo.value = await getReports();
});

fetchReports();

</script>

<template>
    <div id="app" class="plan-background">
      <Menu/>
      <main>
        <h1>Reading sessions</h1>
        <div class="div-margin">
          <button class="cta-button" @click="goBack">Back</button>
        </div>
        <table class="Sessions-table">
          <tr class="Sessions-row">
            <th class="Sessions-header">Date</th>
            <th class="Sessions-header">Pages read</th>
            <th class="Sessions-header">pages read, %</th>
            <th class="Sessions-header">Sessions completed</th>
            <th class="Sessions-header">Sessions completed, %</th>
          </tr>
          <tr v-for="report in reportsInfo" class="sessions-row">
            <td class="sessions-data">{{ report.date }}</td>
            <td class="sessions-data">{{ report.totalPages }}</td>
            <td class="sessions-data">{{ report.percentagePages }}</td>
            <td class="sessions-data">{{ report.totalSessions }}</td>
            <td class="sessions-data">{{ report.percentageSesions }}</td>  
          </tr>
        </table>
      </main>
    </div>
  </template>
  

<style scoped>
.white {
  color: white;
}
.green {
  color: green;
}
.div-margin {
  margin-top: 20px;
  margin-bottom: 20px;
}
a.clickable:hover {
  cursor: pointer;
}
a {
  color: blue;
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