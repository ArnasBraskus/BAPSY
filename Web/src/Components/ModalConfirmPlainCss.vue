<script>
import { VueFinalModal } from 'vue-final-modal'

export default {
  components: {
    VueFinalModal
  },
  data() {
    return {
      errors: [],
      name: null,
      page: null,
      date: null,
      day1: null,
      day2: null,
      day3: null,
      day4: null,
      day5: null,
      day6: null,
      day7: null,
      hour: null
    };
  },
  methods: {
    checkForm() {
      if (this.name && this.page && this.date && (this.day1 || this.day2 || this.day3 || this.day4 || this.day5  || this.day6
        || this.day7) && this.hour) {
        this.$emit('confirm');
      } else {
        this.errors = [];
        if (!this.name) this.errors.push("Book name required.");
        if (!this.page) this.errors.push("Page count required.");
        if (!this.date) this.errors.push("Deadline date required.");
        //if (currentDate > this.date) this.errors.push("Deadline date is incorrect.")
        //neveikia patikrinimas ar data nera per sena.
        if (!this.day1 && !this.day2 && !this.day3 && !this.day4 && !this.day5  && !this.day6
        && !this.day7) this.errors.push("Days required.")
        if (!this.hour) this.errors.push("Hour required.")
      }
    },
    clearErrors() {
      this.errors = [];
    }
  },
  props: {
    title: {
      type: String,
      default: 'Default Title'
    }
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
      <p v-if="errors.length">
        <b>Please correct the following error(s):</b>
        <ul>
          <li v-for="error in errors">{{ error }}</li>
        </ul>
      </p>
      <slot />
      <p> 
        <label for="name"> Book name </label> 
        <input type="text" name="name" id="name" v-model="name"> 
      </p>
      <p>
        <label for="page"> How many pages? </label>
        <input type="number" name="page" id="page" v-model="page">
      </p> 
      <p>
        <label for="format"> Book Format </label>
        <select>
         <option value="a4">a4</option>
         <option value="a5">a5</option>
         <option value="a5">a6</option>
      </select>
     </p> 
      <p>
        <label for="date"> Deadline date </label>
         <input type="date" name="page" id="page" v-model="date"/>
      </p>
      <p> 
        <label for="days"> Select which days to read </label>
      <fieldset>
      <div class="day">
      <label>
        <input type="checkbox" value="1" name="days" id="day1" v-model="day1"> Monday
      </label>
      <label>
        <input type="checkbox" value="2" name="days" id="day2" v-model="day2"> Tuesday
      </label>
      <label>
        <input type="checkbox" value="3" name="days" id="day3" v-model="day3"> Wednesday
      </label>
      <label>
        <input type="checkbox" value="4" name="days" id="day4" v-model="day4"> Thursday
      </label>
      <label>
        <input type="checkbox" value="5" name="days" id="day5" v-model="day5"> Friday
      </label>
      <label>
        <input type="checkbox" value="6" name="days" id="day6" v-model="day6"> Saturday
      </label>
      <label>
        <input type="checkbox" value="7" name="days" id="day7" v-model="day7"> Sunday
      </label>
    </div>
    </fieldset>
    </p>
    <p>
      <label for="hour"> Input what hour you prefer to read </label>
      <input type="time" name="hour" id="hour" v-model="hour">
    </p>
    <button @click="checkForm">
      Confirm
    </button>
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