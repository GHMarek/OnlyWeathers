

<template>
    <div class="container">
      <h2>Register</h2>
      <input v-model="email" type="email" placeholder="Email" />
      <input v-model="password" type="password" placeholder="Hasło" />
      <button @click="registerUser">Confirm</button>
      <p v-if="error">{{ error }}</p>
    </div>
  </template>
  
  <script setup>
  import { ref } from 'vue'
  import { register } from '@/services/authService'
  import { useRouter } from 'vue-router'

  const email = ref('')
  const password = ref('')
  const error = ref('')
  const router = useRouter()
  const registerUser = async () => {
  try {
    await register(email.value, password.value)
    error.value = ''
    alert('Successful!')
    router.push('/')
  } catch (err) {
    // Sprawdzasz czy backend zwrócił wiadomość
    if (err.response && err.response.data) {
      error.value = err.response.data; // np. "Invalid email format" albo "User already exists."
    } else {
      error.value = 'Registration failed. Please try again.'
    }
  }
}
  
  </script>
  