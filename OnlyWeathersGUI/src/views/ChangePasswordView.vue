<template>
    <div class="container">
      <h1>Forgot Password 🔐</h1>
      <div class="card">
        <input v-model="currentPassword" type="password" placeholder="Current password" />
        <input v-model="newPassword" type="password" placeholder="New password" />
        <button @click="submitPasswordChange">Change Password</button>
        <p v-if="error">{{ error }}</p>
        <p v-if="success">{{ success }}</p>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue'
  import { changePassword } from '@/services/authService'
  
  const currentPassword = ref('')
  const newPassword = ref('')
  const error = ref('')
  const success = ref('')
  
  const submitPasswordChange = async () => {
    try {
      await changePassword(currentPassword.value, newPassword.value)
      error.value = ''
      success.value = 'Password changed successfully!'
    } catch {
      error.value = 'Password change failed.'
      success.value = ''
    }
  }
  </script>
  
  <style scoped>
  .container {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    background: linear-gradient(to right, #222, #333);
    color: white;
    font-family: 'Segoe UI', sans-serif;
  }
  
  .card {
    background-color: #2e2e2e;
    padding: 2rem;
    border-radius: 10px;
    box-shadow: 0 0 10px #000;
    display: flex;
    flex-direction: column;
    gap: 1rem;
    width: 300px;
  }
  
  input {
    padding: 0.5rem;
    border: none;
    border-radius: 5px;
  }
  
  button {
    padding: 0.5rem;
    background-color: #4caf50;
    color: white;
    border: none;
    border-radius: 5px;
    cursor: pointer;
  }
  
  button:hover {
    background-color: #45a049;
  }
  </style>
  