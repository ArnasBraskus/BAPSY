<script>
import Footer from '../Components/Footer.vue';
import { markCompleted } from '../utils/confirmation.js';
import ModalConfirmation from '../Components/ModalConfirmation.vue';
import { ModalsContainer, useModal } from 'vue-final-modal';

const componentOptions = {
  components: {
    ModalsContainer,
    Footer
  },
  methods: {
    markCompletedPlan(planId, sesId) {
      markCompleted(planId, sesId);
    },
    async showModal() {
      const modal = useModal({
        component: ModalConfirmation,
        attrs: {
          title: 'Confirmation',
          onConfirm: () => {
            modal.close();
          }
        }
      });
      modal.open();
    }
  }
};

export default componentOptions;


</script>

<template>
  <div id="app" class="plan-background">
    <div class="box">
        <p>Did you read the assigned pages?</p>
        <button class="edit-button" style="--clr:white"><span>I did!</span></button>
        <button class="remove-button" style="--clr:white" @click="showModal()"><span>I read more or less</span></button>
    </div>
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
  margin-top: 250px;
  padding: 15px;

  background-color: #5F6F52;
  font-size:x-large;
  border-radius: 50px 50px 50px 50px;
 }
</style>

