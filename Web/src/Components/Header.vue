<script setup>

</script>

<template>
    <header class="header">
        <div class="logo"></div>
        <nav>
            <RouterLink to="/">Home</RouterLink>
            <RouterLink to="/about">About</RouterLink>            
        </nav>
    </header>
</template>

<style scoped>
button {
    align-self: unset;
    background-color: var(--secondary-color);
    color: #fff;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-size: 16px;
    transition: background-color 0.3s ease;
}

button:hover {
    background-color: rgba(0, 0, 0, 0.2);
}

button:active {
    background-color: #004080;
}
</style>
