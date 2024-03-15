<script setup lang="ts">
import { VueFinalModal } from 'vue-final-modal'

defineProps<{
  title?: string
}>()

const emit = defineEmits<{
  (e: 'confirm'): void
}>()
</script>

<template>
  <VueFinalModal
    class="confirm-modal"
    content-class="confirm-modal-content"
    overlay-transition="vfm-fade"
    content-transition="vfm-fade"
  >
    <h1>{{ title }}</h1>
    <slot />
    <p> Book name   
    </p> 
    <input />
    <p> How many pages?
    </p>
    <input />  
    <p> Book format </p> 
    <select>
         <option value="a4">a4</option>
         <option value="a5">a5</option>
         <option value="a5">a6</option>
    </select>
    <p> Deadline date</p>
    <input type="date"/>
    <p> Select which days to read </p>
    <fieldset>
    <div class="days">
    <label>
      <input type="checkbox" value="1" name="days"> Monday
    </label>
    <label>
      <input type="checkbox" value="2" name="days"> Tuesday
    </label>
    <label>
      <input type="checkbox" value="3" name="days"> Wednesday
    </label>
    <label>
      <input type="checkbox" value="4" name="days"> Thursday
    </label>
    <label>
      <input type="checkbox" value="5" name="days"> Friday
    </label>
    <label>
      <input type="checkbox" value="6" name="days"> Saturday
    </label>
    <label>
      <input type="checkbox" value="0" name="days"> Sunday
    </label>
    </div>
    </fieldset>
    <p>Input what hour you prefer to read</p>
    <input type="time">
    <button @click="emit('confirm')">
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