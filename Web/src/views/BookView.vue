<script setup>
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { ModalsContainer, useModal } from 'vue-final-modal';

import { getPlan, removePlan, additionalPagesRead } from '../utils/plans.js';
import { chooseColor } from '../utils/books.js';
import router from '../router';

import Menu from '../Components/Menu.vue';
import Footer from '../Components/Footer.vue';
import BookCover from '../Components/BookCover.vue';
import ModalPlan from '../Components/ModalPlan.vue';

function goBack() {
  router.push({path: '/books'});
}

var plan;

const fetchPlan = (async() => {
  plan = await getPlan(planId);

  id.value = plan.id;
  title.value = plan.title;
  author.value = plan.author;
  pages.value = plan.pages;
  pagesRead.value = plan.pagesRead;
  readingStart.value = plan.sessions[0].date;
  deadline.value = plan.deadline;
  time.value = plan.timeOfDay;

  const WEEKDAYS = ['Monday', 'Tueday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
  const WEEKDAY_IDX = [1,2,3,4,5,6,0];

  const wks = WEEKDAYS.map((x, i) => plan.weekdays[WEEKDAY_IDX[i]] ? x : null).filter(Boolean);

  schedule.value = wks.join(', ');
  cover.value = chooseColor(plan);
});

function editPlan() {
  const modal = useModal({
    component: ModalPlan,
    attrs: {
      title: 'Edit Plan',
      plan: plan,
      onConfirm: () => {
        modal.close();
        fetchPlan();
      }
    }
  });

  modal.open();
}

async function deletePlan() {
  await removePlan(plan.id);
  goBack();
}

async function submitAdditionalPagesRead() {
  await additionalPagesRead(plan.id, addPagesRead.value);
  fetchPlan();
}


function getReadingSessionsUrl() {
  return `/books/${planId}/sessions`;
}

const route = useRoute();
const planId = route.params.id;

const id = ref('');
const title = ref('');
const author = ref('');
const pages = ref('');
const pagesRead = ref('');
const readingStart = ref('');
const deadline = ref('');
const time = ref('');
const schedule = ref('');
const cover = ref('');

const addPagesRead = ref('');

fetchPlan();

</script>

<template>
  <div id="app" class="plan-background">
    <Menu/>
    <main>
      <ModalsContainer/>
      <div class="margin">
        <button class="cta-button" @click="goBack">Back</button>
        <button class="cta-button" @click="editPlan">Edit</button>
        <button class="cta-button" @click="deletePlan">Delete</button>
      </div>
      <div class="side-by-side">
        <table>
            <tr>
              <td class="data-key">Title</td>
              <td class="data-velue">{{ title }}</td>
            </tr>
            <tr>
              <td class="data-key">Author</td>
              <td class="data-value">{{ author }}</td>
            </tr>
            <tr>
              <td class="data-key">Pages read</td>
              <td class="data-value">{{ pagesRead }} / {{ pages }}</td>
            </tr>
            <tr>
              <td class="data-key">Reading start</td>
              <td class="data-value">{{ readingStart }}</td>
            </tr>
            <tr>
              <td class="data-key">Deadline</td>
              <td class="data-value">{{ deadline }}</td>
            </tr>
            <tr>
              <td class="data-key">Reading time</td>
              <td class="data-value">{{ time }}</td>
            </tr>
            <tr>
              <td class="data-key">Schedule</td>
              <td class="data-value">{{ schedule }}</td>
            </tr>

            <!-- ///NOTdone -->
            <tr>
              <td class="data-key">Additional Pages Read</td>
              <td class="data-value">
                <input type="number" v-model="addPagesRead" placeholder="Enter additional pages read">
                <button class="cta-button" @click="submitAdditionalPagesRead">Submit</button>
              </td>
            </tr>
        </table>
        <BookCover :title="title" :author="author" :cover="cover"/>
      </div>
      <div class="reading-sessions">
        <RouterLink :to="getReadingSessionsUrl()">Reading sessions</RouterLink>
      </div>
    </main>
  </div>
</template>

<style scroped>
.margin {
  margin-bottom: 10px;
}
.reading-sessions {
  margin-top: 20px;
  font-weight: bold;
  font-size: 25px;
}
.side-by-side {
  display: flex;
}
tr {
  box-sizing: border-box;
  min-width: 13rem;
}
td {
  border-bottom: .0625rem solid #e3e3e3;
}
td.data-key {
  font-weight: bold;
}
table {
  color: #f3f1f1;
  width: 80%;
  min-width: 13rem;
  font-size: 25px;
  padding-left: 1.8725rem;
  border-collapse: collapse;
  border-spacing: 0;
  margin-right: 50px;
}
</style>
