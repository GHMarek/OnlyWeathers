<template>
  <div class="container">
    <h1>OnlyWeathers ☁️</h1>
    <div class="card">
      <h2>Logowanie</h2>
      <input v-model="email" type="email" placeholder="Email" />
      <input v-model="password" type="password" placeholder="Hasło" />
      <button @click="loginUser">Zaloguj</button>
      <p v-if="error">{{ error }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { login } from '@/services/authService'

const email = ref('')
const password = ref('')
const error = ref('')

const loginUser = async () => {
  try {
    await login(email.value, password.value)
    error.value = ''
    alert('Zalogowano!')
  } catch {
    error.value = 'Nieprawidłowe dane logowania.'
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
