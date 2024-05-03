<script setup>

import { ref } from 'vue';
import router from '../router';
import Menu from '../Components/Menu.vue';
import BookCover from '../Components/BookCover.vue';
import { getPlans } from '../utils/plans.js';
import { chooseColor } from '../utils/books.js';

import { ModalsContainer, useModal } from 'vue-final-modal';
import ModalPlan from '../Components/ModalPlan.vue';

const booksReading = ref([]);
const booksCompleted = ref([]);

const fetchPlans = (async() => {
  booksReading.value = [];
  booksCompleted.value = [];

  const plans = await getPlans();

  plans.forEach(plan => {
    booksReading.value.push({
      id: plan.id,
      title: plan.title,
      author: plan.author,
      cover: chooseColor(plan)
    });
  });
});

function showModal() {
  const modal = useModal({
    component: ModalPlan,
    attrs: {
      title: 'Plan',
      onConfirm: () => {
        modal.close();
        fetchPlans();
      }
    }
  });

  modal.open();
}

fetchPlans();

</script>

<template>
  <div id="app" class="plan-background">
    <Menu/>
    <main>
      <ModalsContainer/>
      <h1 class="info-text">Currently reading ({{ booksReading.length }})</h1>
      <div class="bookcase">
        <BookCover v-for="book in booksReading" :id="book.id" :title="book.title" :author="book.author" :cover="book.cover"/>
      </div>
      <div class="top">
        <button class="cta-button" @click="showModal">Add Book</button>
      </div>
      <h1 class="info-text">Completed ({{ booksCompleted.length }})</h1>
      <div class="bookcase">
        <BookCover v-for="book in booksCompleted" :id="book.id" :title="book.title" :author="book.author" :cover="book.cover"/>
      </div>
    </main>
  </div>
</template>

<style scoped>
.top {
  margin-top: 20px;
  margin-bottom: 20px;
}
h1.info-text {
  text-align: left;
  margin-top: 10px;
  margin-bottom: 20px;
  color: #f3f1f1;
}
</style>
