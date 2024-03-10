<template>
  <div class="home">
    <h1>Welcome to Our Book Reading Progress Tracker</h1>

    <p>This is a platform where you can keep track of your reading progress, discover new books, and more.</p>

    <RouterLink to="/login" class="cta-button">Login to Get Started</RouterLink>
  </div>
</template>



<style scoped>
.home {
  text-align: center;
  color: var(--quaternary-color);
  padding: 20px;
  margin-top: 20px;
}

.cta-button {
  display: inline-block;
  margin-top: 20px;
  padding: 10px 20px;
  background-color: var(--secondary-color);
  color: #fff;
  text-decoration: none;
  border-radius: 5px;
  transition: background-color 0.3s ease;
}


.cta-button:hover {
  background-color: rgba(0, 0, 0, 0.3);
}
.cta-button:active {
        background-color: #004080;
    }

</style>
