<script setup lang="ts">

</script>

<template>
  <DefaultLayout>
    <router-view />
  </DefaultLayout>
</template>

<script setup>
import DefaultLayout from '@/layouts/DefaultLayout.vue'
</script>

<style>
/* pusty albo zrobić globalne style później */
body {
  margin: 0;
  background-color: #1e1e1e;
  color: white;
  font-family: 'Segoe UI', sans-serif;
}
</style>
