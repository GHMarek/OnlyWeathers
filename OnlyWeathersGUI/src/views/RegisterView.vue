

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
    } catch {
      error.value = 'Rejestracja nieudana (email już istnieje?).'
    }
  }
  </script>
  