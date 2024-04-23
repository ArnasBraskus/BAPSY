<script>
import { VueFinalModal } from 'vue-final-modal'
import { markNotCompleted } from '../utils/confirmation.js';

export default {
  components: {
    VueFinalModal
  },
  data() {
    return {
      errors: [],
      pages: null
    };
  },
  methods: {
    checkForm() {
      this.errors = [];

      if(this.pages == 0 || (this.pages)) {
        markNotCompleted(planId, sesId)
        .then(() => {
            this.$emit('confirm');
            window.location.reload();
        });
      } 
    },
    clearErrors() {
      this.errors = [];
    },
    cancelForm() {
      this.$emit('confirm');
    },
  },
    title: {
      type: String,
      default: 'Default Title'
    }
};

</script>

<template>
  <VueFinalModal
    class="confirm-modal"
    content-class="confirm-modal-content"
    overlay-transition="vfm-fade"
    content-transition="vfm-fade"
    @close="clearErrors"
  >
      <h1>{{ title }}</h1>
      <slot />
      <p>
        <label for="page"> How many pages? </label>
        <input type="number" name="pages" v-model="pages">
      </p> 
    <p><button @click="checkForm">Confirm</button>  <button @click="cancelForm">Cancel</button></p>
  </VueFinalModal>
</template>

<style>
.confirm-modal {
  display: flex;
  justify-content: center;
  align-items: center;
}
.confirm-modal-content {
  display: flex;
  flex-direction: column;
  padding: 1rem;
  background: #fff;
  border-radius: 0.5rem;
}
.confirm-modal-content > * + *{
  margin: 0.5rem 0;
}
.confirm-modal-content h1 {
  font-size: 1.375rem;
}
.confirm-modal-content button {
  margin: 0.25rem 0 0 auto;
  padding: 0 8px;
  border: 1px solid;
  border-radius: 0.5rem;
}
.dark .confirm-modal-content {
  background: #000000;
}
</style>