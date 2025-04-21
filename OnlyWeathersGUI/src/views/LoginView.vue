<template>
    <div>
      <h2>Login</h2>
      <input v-model="email" placeholder="Email" />
      <input v-model="password" type="password" placeholder="Password" />
      <button @click="loginUser">Log in</button>
      <p v-if="error">{{ error }}</p>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue';
  import { login } from '@/services/authService';
  
  const email = ref('');
  const password = ref('');
  const error = ref('');
  
  const loginUser = async () => {
    try {
      await login(email.value, password.value);
      error.value = '';
      alert('Logged in!');
    } catch (err) {
      error.value = 'Invalid credentials';
    }
  };
  </script>
  