<script setup>
import Footer from '../Components/Footer.vue';
import { ref } from 'vue';
import { useRoute } from 'vue-router';
import { markCompleted } from '../utils/confirmation.js';
import { chooseColor } from '../utils/books.js';
import { notify } from '../utils/notifications.js';
import { getSessionNoAuth, markSessionNoAuth } from '../utils/sessions.js';
import ModalConfirmation from '../Components/ModalConfirmation.vue';
import { ModalsContainer, useModal } from 'vue-final-modal';
import BookCover from '../Components/BookCover.vue';

var session;

const title = ref('');
const author = ref('');
const cover = ref('');
const goal = ref('');

function showModal() {
  const modal = useModal({
    component: ModalConfirmation,
    attrs: {
      title: 'Confirmation',
      onConfirm: (pages) => {
        if (pages == null)
          return;

        markRead(pages);

        modal.close();
      }
    }
  });
  modal.open();
}

function markReadAll() {
  (async() => {
    markSessionNoAuth(sessionId, session.goal, token).then(() => {
      notify('Pages marked successfully!', 'info');
    });
  })();
}

function markRead(pages) {
  (async() => {
    await markSessionNoAuth(sessionId, pages, token).then(() => {
      notify('Pages marked successfully!', 'info');
    });
  })();
}

const fetchSession = (async() => {
  session = await getSessionNoAuth(sessionId, token);

  title.value = session.metadata.title;
  author.value = session.metadata.author;
  cover.value = chooseColor(session.metadata);
  goal.value = session.goal;
});

const route = useRoute();
const sessionId = Number(route.params.id);
const token = route.query.t;

fetchSession();

</script>

<template>
  <div id="app" class="plan-background">
    <div class="box">
      <p>Did you read the assigned pages ({{ goal }})?</p>
        <button class="edit-button" style="--clr:white" @click="markReadAll()"><span>I did!</span></button>
        <button class="remove-button" style="--clr:white" @click="showModal()"><span>I read more or less</span></button>
    </div>
    <BookCover :title="title" :author="author" :cover="cover"/>
    <ModalsContainer />
    <Footer />
  </div>
</template>

<style scoped>

.remove-button {
  position: relative;
  background: #444;
  color: #fff;
  text-decoration: none;
  text-transform: uppercase;
  border: none;
  letter-spacing: 0.1rem;
  font-size: 1rem;
  padding: 1rem 3rem;
  transition: 0.2s;
}

.remove-button:hover {
  letter-spacing: 0.2rem;
  padding: 1.1rem 3.1rem;
  background: var(--clr);
  color: var(--clr);
  animation: box 3s infinite;
}

.remove-button::before {
  content: "";
  position: absolute;
  inset: 2px;
  background: #FF0000;
}

.remove-button span {
  position: relative;
  z-index: 1;
}

.edit-button {
  position: relative;
  background: #444;
  color: #fff;
  text-decoration: none;
  text-transform: uppercase;
  border: none;
  letter-spacing: 0.1rem;
  font-size: 1rem;
  padding: 1rem 3rem;
  transition: 0.2s;
}

.edit-button:hover {
  letter-spacing: 0.2rem;
  padding: 1.1rem 3.1rem;
  background: var(--clr);
  color: var(--clr);
  animation: box 3s infinite;
}

.edit-button::before {
  content: "";
  position: absolute;
  inset: 2px;
  background: #d67c0d;
}

.edit-button span {
  position: relative;
  z-index: 1;
}

.box {
  display: inline-block;
  text-align: center;
  color: var(--quaternary-color);
  padding: 15px;

  background-color: #5F6F52;
  font-size:x-large;
  border-radius: 50px 50px 50px 50px;
 }
</style>

