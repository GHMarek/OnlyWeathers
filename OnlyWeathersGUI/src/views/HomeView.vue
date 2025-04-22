<template>
  <div class="container">
    <div class="left">
      <h1>OnlyWeathers ☁️</h1>
      <div class="card">
        <h2>Login</h2>
        <input v-model="email" type="email" placeholder="Email" />
        <input v-model="password" type="password" placeholder="Password" />
        <button @click="loginUser">Confirm</button>
        <p v-if="error">{{ error }}</p>
        <div class="links">
          <router-link to="/register">Register</router-link>
          <span class="separator">|</span>
          <router-link to="/forgot-password">Forgot password?</router-link>
        </div>
      </div>
    </div>

    <div class="right">
      <h2>🌍 Random weather 🌍</h2>
      <div class="weather-grid">
        <div v-for="(item, index) in weatherList" :key="index" class="weather-card">
          
          <!-- Country + Flag -->
          <div class="weather-column">
            <div class="country">
              <img :src="`https://flagcdn.com/w40/${item.countryCode.toLowerCase()}.png`" alt="flag" class="flag-icon" />
              {{ item.country }}
            </div>
          </div>

          <!-- City -->
          <div class="weather-column">
            <div class="city">{{ item.city }}</div>
          </div>

          <!-- Weather Icon + Description + Temp -->
          <div class="weather-column">
            <img 
              :src="`http://openweathermap.org/img/wn/${item.icon}@2x.png`" 
              :alt="item.description" 
              class="weather-icon" 
            />
            <div class="weather-details">
              <div>{{ item.description }}</div>
              <div class="temperature">{{ item.temperature }}°C</div>
            </div>
          </div>

        </div>
      </div>
    </div>
  </div>
</template>



<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { login } from '@/services/authService'
import api from '@/services/api'

//console.log('API URL:', import.meta.env.VITE_API_URL) // 👀 sprawdź czy poprawny

const email = ref('')
const password = ref('')
const error = ref('')
const router = useRouter()

const weatherList = ref([])

onMounted(async () => {

  try {
    const response = await api.get('/api/public-weather')
    console.log('Weather response:', response.data)
    weatherList.value = response.data
  } catch (error) {
    console.error('Error fetching public weather:', error)
  }
})

const loginUser = async () => {
  try {
    await login(email.value, password.value)
    error.value = ''
    router.push('/dashboard')
  } catch {
    error.value = 'Incorrect login data.'
  }
}
</script>


<style scoped>
.container {
  min-height: 100vh;
  display: flex;
  flex-direction: row; 
  justify-content: center;
  align-items: flex-start;
  background: linear-gradient(to right, #222, #333);
  color: white;
  font-family: 'Segoe UI', sans-serif;
  gap: 4rem;
  padding: 2rem;
}

.left {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: flex-start; 
  gap: 2rem;
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

.right {
  flex: 1;
}

.weather-grid {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.weather-card {
  background-color: #2e2e2e;
  padding: 1rem;
  border-radius: 10px;
  box-shadow: 0 0 5px #000;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.weather-column {
  display: flex;
  flex-direction: row; /* zamiana na poziome */
  align-items: center;
  gap: 1rem;
}

/* Każda sekcja osobno w kolumnie */
.weather-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

.weather-icon {
  width: 50px;
  height: 50px;
}

.flag-icon {
  width: 24px;
  height: 16px;
}

.city {
  font-weight: bold;
}

.temperature {
  font-size: 1.2rem;
}

.links {
  display: flex;
  justify-content: center;
  gap: 0.5rem;
  font-size: 0.9rem;
}

.separator {
  color: #05ff1a;
}


</style>
