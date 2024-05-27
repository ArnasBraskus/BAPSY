<script setup>
import { ref } from 'vue';


import Menu from '../Components/Menu.vue';

import { getReports} from '../utils/reports.js';


var reportsInfo = ref([]);

var fetchReports = (async () => {
  let reports = await getReports();
  reportsInfo.value = reports.map(report => {
    let date = new Date(report.date);
    let formattedDate = date.toISOString().split('T')[0];
    return {
      ...report,
      date: formattedDate
    };
  });
});

fetchReports();

</script>

<template>
    <div id="app" class="plan-background">
      <Menu/>
      <main>
        <h1>Reports</h1>
        <table class="Sessions-table">
          <tr class="Sessions-row">
            <th class="Sessions-header">Date</th>
            <th class="Sessions-header">Pages read</th>
            <th class="Sessions-header">Page completion %</th>
            <th class="Sessions-header">Sessions completed</th>
            <th class="Sessions-header">Session completion %</th>
          </tr>
          <tr v-for="report in reportsInfo" class="sessions-row">
            <td class="sessions-data">{{ report.date }}</td>
            <td class="sessions-data">{{ report.totalPages }}</td>
            <td>                       <div class="eb-progress-bar html" :style="{ '--value': report.percentagePages, '--col': '#FF5089' }">
              <progress :id="'progress-' + report.id" min="0" max="100" :value="report.percentagePages"></progress>
            </div></td>
            <td class="sessions-data">{{ report.totalSessions }} </td>
            <td>                       <div class="eb-progress-bar html" :style="{ '--value': report.percentageSessions, '--col': '#FF5089' }">
              <progress :id="'progress-' + report.id" min="0" max="100" :value="report.percentageSessions"></progress>
            </div></td>
          </tr>
        </table>
      </main>
    </div>
  </template>
  

<style scoped>
.eb-progress-bar {
  border-bottom: .0625 solid #e3e3e3;
  padding: 10px;
  padding-left: 50px;
  --size: 4.5;
  --inner-bg: #f2f2f2;
  --primary-color: var(--col);
  --secondary-color: #dfe0e0;
  display: flex;
  justify-content: center;
  align-items: center;
  width: var(--size);
  height: var(--size);
  font-size: calc(var(--size) / 5);
  color: var(--primary-color);
  background: radial-gradient(
      closest-side,
      var(--inner-bg) 79%,
      transparent 80% 100%
    ),
    conic-gradient(
      var(--primary-color) calc(var(--eb-progress-value) * 1%),
      var(--secondary-color) 0
    );
  border-radius: 50%;
}
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
  width: 100%;
  max-width: 1000px;
  margin: 0 auto;
  color: white;
  font-size: 25px;
  border-collapse: collapse;
}
.sessions-row {
  text-align: left;
}
.sessions-header, .sessions-data {
  border-bottom: .0625rem solid #e3e3e3;
  padding: 10px;
  padding-left: 50px;
}
</style>