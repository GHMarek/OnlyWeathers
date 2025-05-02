<template>
  <div class="container">
    <!-- LEWA KOLUMNA – lista ulubionych -->
    <div class="left">
      <div class="card">
        <h2>Your Favorite Cities</h2>

        <!-- Pokazuje spinner podczas ładowania danych z API -->
        <div v-if="isLoading" class="loading">
          <div class="spinner"></div>
          <p>Loading your favorite weather data...</p>
        </div>

        <!-- Gdy nie ma żadnych ulubionych miast -->
        <div v-else-if="weatherList.length === 0">
          <p>No favorite cities yet.</p>
        </div>

        <!-- Gdy są ulubione miasta z pogodą -->
        <div v-else>
          <!-- Iteracja po liście pogody dla ulubionych miast -->
          <div v-for="(item, index) in weatherList" :key="index" class="weather-card">
            
            <!-- Kolumna z ikoną i szczegółami pogody -->
            <div class="weather-column">
              <img
                :src="item.icon || '/icons/no-data.png'"
                :alt="item.description || 'No data'"
                class="weather-icon"
              />
              <div class="weather-details">
                <!-- Opis warunków pogodowych -->
                <div>{{ item.description || 'No weather data' }}</div>
                <!-- Temperatura lub myślnik jeśli brak -->
                <div class="temperature">
                  {{ item.temperature !== null ? item.temperature.toFixed(1) + '°C' : '—' }}
                </div>
              </div>
            </div>

            <!-- Kolumna z miastem i aliasem -->
            <div class="weather-column city-info">
              <div class="city">
                <!-- Alias użytkownika, jeśli ustawiony -->
                <span v-if="item.alias" class="alias-strong">{{ item.alias }}</span>
                <!-- Nazwa miasta, jeśli brak aliasu -->
                <span v-else>{{ item.city }}</span>
                <!-- Oryginalna nazwa miasta w nawiasie (gdy alias istnieje) -->
                <div v-if="item.alias" class="alias-original">({{ item.city }})</div>
              </div>

              <!-- Formularz zmiany aliasu, widoczny tylko gdy edytujemy ten wpis -->
              <div v-if="editingId === item.id" class="edit-alias-form">
                <input
                  v-model="editingAlias"
                  class="alias-input"
                  placeholder="Enter alias..."
                />
                <button class="save-btn" @click="submitAlias(item.id)">💾</button>
              </div>
            </div>

            <!-- Kolumna z przyciskami akcji: edytuj i usuń -->
            <div class="weather-column action-buttons">
              <!-- Przycisk edycji aliasu -->
              <button class="edit-btn" @click="startEditing(item.id, item.alias)">✏️</button>
              <!-- Przycisk usunięcia miasta z ulubionych -->
              <button class="delete-btn" @click="deleteFavorite(item.id)">❌</button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- PRAWA KOLUMNA – wyszukiwanie i dodawanie miast -->
    <div class="right">
      <h2>Add a City</h2>

      <!-- Pole do wpisywania nazwy miasta -->
      <input v-model="search" @input="searchCities" placeholder="Type city name..." autocomplete="off" />

      <!-- Podpowiedzi miast z API -->
      <ul v-if="suggestions.length > 0" class="suggestions">
        <li
          v-for="city in suggestions"
          :key="city.city"
          @click="selectCity(city)"
        >
          {{ city.city }}, {{ city.region }} {{ city.country }}
        </li>
      </ul>

      <!-- Przycisk dodania miasta do ulubionych -->
      <button
        v-if="search.trim().length > 0"
        @click="addFavorite(search)"
        class="add-btn"
      >
        Add to favorites
      </button>

      <!-- Komunikat o błędzie (np. limit, duplikat) -->
      <p v-if="errorMsg" class="error">{{ errorMsg }}</p>
    </div>
  </div>
</template>


<script setup>
import { ref, onMounted } from 'vue'
import api from '@/services/api'
import { useRouter } from 'vue-router'

const router = useRouter()
// Zmienne do przechowywania stanu komponentu
let debounceTimeout = null

// Zmienne reaktywne (powiązane z danymi formularzy i odpowiedzi API)
const errorMsg = ref('')
const weatherList = ref([])
const search = ref('')
const suggestions = ref([])

const token = localStorage.getItem('token')

const editingId = ref(null) // ID miasta, którego alias edytujemy
const editingAlias = ref('') // nowy alias

const email = ref('')

const isLoading = ref(true)



onMounted(() => {
  const token = localStorage.getItem('token')
  if (!token) {
    router.push('/')
    return
  }

  try {
    const jwt = JSON.parse(atob(token.split('.')[1]))
    email.value = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
    loadFavorites()
  } catch (err) {
    console.error('Invalid token', err)
    router.push('/')
  }
})



// Ładowanie ulubionych miast z pogodą
const loadFavorites = async () => {
  isLoading.value = true
  try {
    const response = await api.get('/api/favorites/weather', {
      headers: { Authorization: `Bearer ${token}` }
    })
    weatherList.value = response.data
  } catch (err) {
    console.error('Error loading favorites:', err)
  } finally {
    isLoading.value = false
  }
}


// Usuwanie ulubionego miasta
const deleteFavorite = async (id) => {
  try {
    await api.delete(`/api/favorites/${id}`, {
      headers: { Authorization: `Bearer ${token}` }
    })
    await loadFavorites()
  } catch (err) {
    console.error('Error deleting favorite:', err)
  }
}

// Szukanie miast po wpisaniu 3+ liter
const searchCities = () => {
  clearTimeout(debounceTimeout)

  if (search.value.trim().length < 3) {
    suggestions.value = []
    return
  }

  debounceTimeout = setTimeout(async () => {
    try {
      const response = await api.get(`/api/favorites/cities?query=${search.value}`, {
        headers: { Authorization: `Bearer ${token}` }
      })
      suggestions.value = response.data
    } catch (err) {
      console.error('Error searching cities:', err)
    }
  }, 300)
}


// Dodanie miasta do ulubionych
const addFavorite = async (cityName) => {
  if (!cityName) return
  try {
    await api.post('/api/favorites', { cityName }, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    })
    search.value = ''
    suggestions.value = []
    errorMsg.value = ''
    await loadFavorites()
  } catch (err) {
    console.error('Error adding favorite:', err)
    errorMsg.value = err.response?.data || 'An unexpected error occurred.'
  }
}

// Kliknięcie podpowiedzi w liście
const selectCity = (city) => {
  search.value = city.city
  suggestions.value = []
}

// Rozpoczęcie edycji aliasu
const startEditing = (id, alias) => {
  editingId.value = id
  editingAlias.value = alias
}

// Zatwierdzenie aliasu
const submitAlias = async (id) => {
  try {
    await updateAlias(id, editingAlias.value)
    editingId.value = null
    editingAlias.value = ''
    await loadFavorites()
  } catch (err) {
    console.error('Error submitting alias:', err)
  }
}

// Wysłanie aliasu do API
const updateAlias = async (id, alias) => {
  try {
    await api.put(`/api/favorites/${id}/alias`, { alias }, {
      headers: {
        Authorization: `Bearer ${token}`,
        'Content-Type': 'application/json'
      }
    })
  } catch (err) {
    console.error('Error updating alias:', err)
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
  gap: 6rem;
  padding: 3rem;
}

.left {
  display: flex;
  flex-direction: column;
  gap: 3rem;
  width: 400px;
}

.card {
  background-color: #2e2e2e;
  padding: 2rem;
  border-radius: 10px;
  box-shadow: 0 0 10px #000;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.right {
  flex: 1;
}

.weather-card {
  background-color: #2e2e2e;
  padding: 1rem;
  border-radius: 10px;
  box-shadow: 0 0 5px #000;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 1rem;
  margin-bottom: 1rem;
}

.city-info {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.3rem;
}

.weather-column {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.weather-icon {
  width: 50px;
  height: 50px;
}

.city {
  font-weight: bold;
}

.temperature {
  font-size: 1.2rem;
}

.suggestions {
  background-color: #2e2e2e;
  border-radius: 5px;
  box-shadow: 0 0 5px #000;
  list-style: none;
  padding: 0.5rem;
  margin-top: 0.5rem;
}

.suggestions li {
  padding: 0.3rem;
  cursor: pointer;
}

.suggestions li:hover {
  background-color: #444;
}

.add-btn {
  margin-top: 1rem;
  padding: 0.5rem 1rem;
  background-color: #6bdee2;
  color: black;
  font-weight: bold;
  border: none;
  border-radius: 5px;
  cursor: pointer;
}

.add-btn:hover {
  background-color: #5ad3db;
}

.delete-btn {
  background: transparent;
  border: none;
  color: red;
  font-size: 1.2rem;
  cursor: pointer;
}

.error {
  color: #ff6b6b;
  margin-top: 0.5rem;
  font-size: 0.9rem;
}

.logo {
  text-align: center;
  font-size: 2rem;
  margin-top: 1rem;
  color: #ffffff;
}

.topbar {
  max-width: 1200px;
  width: 100%;
  margin: 1rem auto;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: #2e2e2e;
  padding: 1rem 2rem;
  border-radius: 10px;
  box-shadow: 0 0 5px #000;
  font-size: 1rem;
}


.user-info {
  color: #aaa;
}

.change-link {
  color: #6bdee2;
  text-decoration: none;
  font-weight: bold;
}

.change-link:hover {
  text-decoration: underline;
}

.alias-input {
  background-color: #1f1f1f;
  border: 1px solid #444;
  border-radius: 5px;
  padding: 0.3rem 0.5rem;
  color: white;
  margin-top: 0.3rem;
  width: 100%;
}

.alias-edit {
  margin-top: 0.3rem;
}

.alias-display {
  display: flex;
  align-items: center;
  gap: 0.3rem;
  font-size: 0.85rem;
  color: #ccc;
}

.edit-btn, .save-btn {
  background: none;
  border: none;
  color: #6bdee2;
  cursor: pointer;
  font-size: 1rem;
}

.alias-strong {
  font-weight: bold;
  font-size: 1.1rem;
  color: #6bdee2;
}


.alias-original {
  font-size: 0.85rem;
  color: #aaa;
}

.action-buttons {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
  align-items: flex-end;
}

.edit-alias-form {
  display: flex;
  gap: 0.3rem;
  margin-top: 0.3rem;
}

.spinner {
  border: 4px solid #2e2e2e;
  border-top: 4px solid #4caf50;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  animation: spin 1s linear infinite;
}

</style>
