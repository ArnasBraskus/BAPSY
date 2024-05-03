<script setup>
import router from '../router';

const props = defineProps(['id', 'title', 'author', 'cover']);

function openPlan() {
  if (!props.id)
    return;

  router.push({path: `/books/${props.id}`});
}

</script>

<template>
  <div class="cover" @click="openPlan">
    <div v-if="author" class="cover-text">
      <span>{{ author }}</span>
    </div>
    <div v-if="title" class="cover-text">
      <span>{{ title }}</span>
    </div>
  </div>
</template>

<style>
.bookcase {
  display: grid;
  grid-template-columns: repeat(auto-fill, 300px);
  grid-gap: 30px;
}
</style>

<style scoped>
.cover-text {
  color: #f3f1f1;
  font-family: sans-serif;
  font-size: 40px;
  text-shadow: 1px 1px 10px rgba(0,0,0,0.8);
  text-align: center;
  padding-left: 10px;
  padding-right: 10px;
}
.cover {
  background: linear-gradient(to right, rgb(60, 13, 20) 3px, rgba(255, 255, 255, 0.5) 5px, rgba(255, 255, 255, 0.25) 7px, rgba(255, 255, 255, 0.25) 10px, transparent 12px, transparent 16px, rgba(255, 255, 255, 0.25) 17px, transparent 22px), v-bind('cover');
  background-repeat: no-repeat;
  background-size: cover;
  box-shadow: 0 0 5px -1px black, inset -1px 1px 2px rgba(255, 255, 255, 0.5);
  border-radius: 5px;
  width: 300px;
  height: 450px;
  margin: auto;
  padding-left: 15px;
}
.cover img {
  width: 100%;
  height: 100%;
  object-fit: fill;
  overflow: hidden;
}
.cover:hover {
  opacity: 0.8;
  cursor: pointer;
}

.cover-completion {
  display: none;
}

.cover:hover > .cover-text {
  opacity: 0.8;
}

.cover:hover > .cover-completion {
  display: inline;
  color: #f3f1f1;
  font-family: sans-serif;
  font-size: 100px;
  text-shadow: 1px 1px 10px rgba(0,0,0,0.8);
}
</style>
