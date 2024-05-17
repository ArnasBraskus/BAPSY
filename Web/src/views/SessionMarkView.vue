<script setup>
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import router from '../router';
import Menu from '../Components/Menu.vue';
import BookCover from '../Components/BookCover.vue';
import { getSession, markSession } from '../utils/sessions.js';
import { getPlan } from '../utils/plans.js';
import { chooseColor } from '../utils/books.js';

const route = useRoute();
const planId = Number(route.params.planId);
const sessionId = Number(route.params.id);

const date = ref('');
const goal = ref('');
const actual = ref('');
const title = ref('');
const author = ref('');
const cover = ref('');

function goBack() {
  router.push({path: `/books/${planId}/sessions`});
}

async function doMarkSession() {
  markSession(sessionId, actual.value);
  goBack();
}

var fetchSession = async () => {
  const session = await getSession(sessionId);

  date.value = session.date;
  goal.value = session.goal;

  if (session.actual != 0) {
    actual.value = session.actual;
  }
  else {
    actual.value = goal.value;
  }

  const plan = await getPlan(planId);

  title.value = plan.title;
  author.value = plan.author;
  cover.value = chooseColor(plan);
};

fetchSession();

</script>

<template>
  <div id="app" class="plan-background">
    <Menu/>
    <main>
      <div style="text-align: center">
          <h1>Session {{ date }}</h1>
          <div style="margin-bottom: 50px; margin-top: 20px">
            <BookCover :title="title" :author="author" :cover="cover"/>
          </div>
          <input v-model="actual" type="number" min="0">
          <button class="cta-button" @click="doMarkSession">Ok</button>
      </div>
    </main>
  </div>
</template>

<style scoped>
input {
  font-size: 27px;
  width: 20%;
  padding-bottom: 7px;
}
.side-by-side {
  display: flex;
}
input {
}
</style>
