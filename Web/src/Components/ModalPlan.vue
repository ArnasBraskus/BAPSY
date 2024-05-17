<script>
import { addPlan, editPlan } from '../utils/plans.js';
import { VueFinalModal } from 'vue-final-modal';

export default {
  components: {
    VueFinalModal
  },
  data() {
    return {
      errors: [],
      formData: {
        bookTitle: '',
        author: '',
        day1: null,
        day2: null,
        day3: null,
        day4: null,
        day5: null,
        day6: null,
        day7: null,
        pages: null,
        deadline: null,
        timeOfDay: null
      }
    };
  },
  methods: {
    checkForm() {
      this.errors = [];

      if (!this.formData.bookTitle) this.errors.push('Book title required.');
      if (!this.formData.author) this.errors.push('Author required.');
      if (!this.formData.pages) this.errors.push('Page count required.');
      if (this.formData.pages <= 0 && (this.formData.pages)) this.errors.push('Page count cannot be negative.');
      if (!this.formData.deadline) this.errors.push('Deadline date required.');
      if (new Date() > new Date(this.formData.deadline)) this.errors.push('Deadline date cannot be older than today.');
      if (!this.formData.day1 && !this.formData.day2 && !this.formData.day3 && !this.formData.day4 && !this.formData.day5 && !this.formData.day6 && !this.formData.day7) {
        this.errors.push('At least one day required.');
      }
      if (!this.formData.timeOfDay) this.errors.push('Hour required.');

      if (this.errors.length === 0) {
        if (this.plan){
          let weekdays = [];
          weekdays.push(this.formData.day7, this.formData.day1, this.formData.day2, this.formData.day3, this.formData.day4, this.formData.day5, this.formData.day6);
          const convertedWeekdays = weekdays.map(day => !!day);
          editPlan(this.plan.id, this.formData.bookTitle, this.formData.author, this.formData.pages, this.formData.deadline, convertedWeekdays, this.formData.timeOfDay)
            .then(() => {
              this.$emit('confirm');
            })
            .catch(error => {
              console.error('Error editing plan:', error);
            });
        } else {
          let weekdays = [];
          weekdays.push(this.formData.day7, this.formData.day1, this.formData.day2, this.formData.day3, this.formData.day4, this.formData.day5, this.formData.day6);
          const convertedWeekdays = weekdays.map(day => !!day);
          addPlan(this.formData.bookTitle, this.formData.author, this.formData.pages, this.formData.deadline, convertedWeekdays, this.formData.timeOfDay)
            .then(() => {
              this.$emit('confirm');
            })
            .catch(error => {
              console.error('Error adding plan:', error);
            });
        }
      }
    },
    clearErrors() {
      this.errors = [];
    },
    cancelForm() {
      this.$emit('confirm');
    },
  },
  props: {
    plan: {
      type: Object,
      default: null
    },
    title: {
      type: String,
      default: 'Default Title'
    }
  },
  watch: {
    plan: {
      handler(editPlan) {
        if (editPlan) {
          this.formData = {
            bookTitle: editPlan.title,
            author: editPlan.author,
            pages: editPlan.pages,
            deadline: editPlan.deadline,
            day1: editPlan.weekdays[1],
            day2: editPlan.weekdays[2],
            day3: editPlan.weekdays[3],
            day4: editPlan.weekdays[4],
            day5: editPlan.weekdays[5],
            day6: editPlan.weekdays[6],
            day7: editPlan.weekdays[0],
            timeOfDay: editPlan.timeOfDay
          };
        } else {
          this.formData = {
            bookTitle: '',
            author: '',
            pages: null,
            deadline: null,
            day1: null,
            day2: null,
            day3: null,
            day4: null,
            day5: null,
            day6: null,
            day7: null,
            timeOfDay: null
          };
        }
      },
      immediate: true
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
        <b> Please correct the following error(s): </b>
        <ul>
          <li v-for="error in errors">{{ error }}</li>
        </ul>
      </p>
      <slot />
      <p> 
        <label for="title"> Book title </label> 
        <input type="text" name="bookTitle"  v-model="formData.bookTitle"> 
      </p>
      <p> 
        <label for="title"> Book author </label> 
        <input type="text" name="author" v-model="formData.author"> 
      </p>
      <p>
        <label for="page"> How many pages? </label>
        <input type="number" name="pages"  v-model="formData.pages">
      </p> 
      <p>
        <label for="date"> Deadline date </label>
        <input type="date" name="deadline" v-model="formData.deadline"/>
      </p>
      <p> 
        <label for="days"> Select which days to read </label>
      <fieldset>
      <div class="day">
      <label>
        <input type="checkbox" name="days" id="day1" v-model="formData.day1"> Monday
      </label>
      <label>
        <input type="checkbox" name="days" id="day2" v-model="formData.day2"> Tuesday
      </label>
      <label>
        <input type="checkbox" name="days" id="day3" v-model="formData.day3"> Wednesday
      </label>
      <label>
        <input type="checkbox" name="days" id="day4" v-model="formData.day4"> Thursday
      </label>
      <label>
        <input type="checkbox" name="days" id="day5" v-model="formData.day5"> Friday
      </label>
      <label>
        <input type="checkbox" name="days" id="day6" v-model="formData.day6"> Saturday
      </label>
      <label>
        <input type="checkbox" name="days" id="day7" v-model="formData.day7"> Sunday
      </label>
    </div>
    </fieldset>
    <p>
      <label for="hour"> Input what hour you prefer to read </label>
      <input type="time" name="timeOfDay" id="timeOfDay" v-model="formData.timeOfDay">
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
