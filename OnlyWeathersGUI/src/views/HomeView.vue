<!--umożliwia logowanie użytkownika,

pokazuje poglądową pogodę w losowych stolicach Europy (pobrane z publicznego endpointu),

umożliwia przejście do rejestracji-->

<template>
  <div class="container">

    <!-- LEWA STRONA: LOGOWANIE -->
    <div class="left">
      <h1>OnlyWeathers ☁️</h1>

      <div class="card">
        <h2>Login</h2>

        <!-- Formularz logowania -->
        <form @submit.prevent="loginUser" class="form-group">
          <input v-model="email" type="email" placeholder="Email" required />
          <input v-model="password" type="password" placeholder="Password" required />
          <div class="button-container">
            <button type="submit">Confirm</button>
          </div>
        </form>

        <!-- Komunikat o błędzie logowania -->
        <p v-if="error">{{ error }}</p>

        <!-- Linki: rejestracja i zapomniane hasło -->
        <div class="links">
          <router-link to="/register">Register</router-link>
          <span class="separator">|</span>
          <router-link to="/forgot-password">Forgot password?</router-link>
        </div>
      </div>
    </div>

    <!-- PRAWA STRONA: POGODA PUBLICZNA -->
    <div class="right">
      <h2>🌍 Random weather 🌍</h2>

      <!-- Widok ładowania danych pogodowych -->
      <div v-if="isLoading" class="loading">
        <div class="spinner"></div>
        <p>Loading weather data...</p>
      </div>

      <!-- Lista losowej pogody w stolicach -->
      <div v-else class="weather-grid">
        <div v-for="(item, index) in weatherList" :key="index" class="weather-card">

          <!-- Kolumna: kraj + flaga -->
          <div class="weather-column">
            <div class="country">
              <img :src="item.flag" alt="flag" class="flag-icon" />
              {{ item.country }}
            </div>
          </div>

          <!-- Kolumna: miasto -->
          <div class="weather-column">
            <div class="city">{{ item.city }}</div>
          </div>

          <!-- Kolumna: ikona pogody + opis + temperatura -->
          <div class="weather-column">
            <img :src="item.icon" alt="weather icon" class="weather-icon" />
            <div class="weather-details">
              <div>{{ item.description }}</div>
              <div class="temperature">
                {{ item.temperature !== null ? item.temperature.toFixed(1) + '°C' : '—' }}
              </div>
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
  import { login } from '@/services/authService' // funkcja logowania i zapisu tokena
  import api from '@/services/api' // axios z bazowym URL-em

  const email = ref('')
  const password = ref('')
  const error = ref('') // informacja o błędzie logowania
  const router = useRouter()

  // Lista danych pogodowych (dla prawego panelu)
  const weatherList = ref([])
  const isLoading = ref(true) // ładowanie danych

  // Gdy komponent zostanie zamontowany, pobierz dane pogodowe
  onMounted(async () => {
    try {
      const response = await api.get('/api/public-weather')
      weatherList.value = response.data
    } catch (error) {
      console.error('Error fetching public weather:', error)
    } finally {
      isLoading.value = false // wyłącz animację ładowania
    }
  })

  // Logowanie użytkownika po kliknięciu "Confirm"
  const loginUser = async () => {
    try {
      await login(email.value, password.value) // wysyła zapytanie do /auth/login
      error.value = ''
      router.push('/dashboard') // przekierowanie po zalogowaniu
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
  margin-right: 2rem;
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
  margin-right: 2rem;
}

.weather-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(450px, 1fr));
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

.loading {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 1rem;
  margin-top: 2rem;
}

.spinner {
  border: 4px solid #2e2e2e;
  border-top: 4px solid #4caf50;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.button-container {
  display: flex;
  justify-content: flex-start;
}


@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
