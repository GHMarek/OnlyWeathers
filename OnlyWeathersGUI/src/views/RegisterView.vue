<template>
  <div class="container">
    <div class="card">
      <h2>Register</h2>

      <!-- Formularz rejestracji -->
      <form @submit.prevent="registerUser" class="form-group">
        <input v-model="email" type="email" placeholder="Email" required />
        <input v-model="password" type="password" placeholder="Password" required />
        <div class="button-container">
          <button type="submit">Confirm</button>
        </div>
      </form>

      <!-- Komunikat o błędzie -->
      <p v-if="error" class="error">{{ error }}</p>

      <!-- Link powrotny do logowania -->
      <div class="links">
        <router-link to="/">Back to login</router-link>
      </div>
    </div>
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

// Obsługa rejestracji użytkownika
const registerUser = async () => {
  try {
    await register(email.value, password.value)
    error.value = ''
    alert('Registration successful!')
    router.push('/') // przekierowanie do logowania
  } catch (err) {
    if (err.response?.data) {
      error.value = err.response.data // np. "User already exists"
    } else {
      error.value = 'Registration failed. Please try again.'
    }
  }
}
</script>

<style scoped>
.container {
  min-height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(to right, #222, #333);
  color: white;
  font-family: 'Segoe UI', sans-serif;
  padding: 2rem;
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
  text-align: center;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

input {
  padding: 0.5rem;
  border: none;
  border-radius: 5px;
}

.button-container {
  display: flex;
  justify-content: flex-start;
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

.links {
  font-size: 0.9rem;
}

.error {
  color: #ff6b6b;
  font-size: 0.9rem;
}
</style>
