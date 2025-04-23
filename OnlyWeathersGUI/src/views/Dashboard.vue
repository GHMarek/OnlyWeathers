<template>
    <div class="container">
      <div class="left">
        <h1>OnlyWeathers ☁️</h1>
        <div class="card">
          <h2>Your Favorite Cities</h2>
          <div v-if="weatherList.length === 0">
            <p>No favorite cities yet.</p>
          </div>
          <div v-else>
            <div v-for="(item, index) in weatherList" :key="index" class="weather-card">
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
              <div class="weather-column">
                <div class="city">{{ item.city }}</div>
              </div>
            </div>
          </div>
        </div>
      </div>
  
      <div class="right">
        <h2>Add a City</h2>
        <input v-model="search" @input="searchCities" placeholder="Type city name..." />
        <ul v-if="suggestions.length > 0" class="suggestions">
          <li v-for="city in suggestions" :key="city" @click="addFavorite(city)">
            {{ city }}
          </li>
        </ul>
      </div>
    </div>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue'
  import api from '@/services/api'
  
  const weatherList = ref([])
  const search = ref('')
  const suggestions = ref([])
  
  const token = localStorage.getItem('token') // token zaciągany raz
  
  const loadFavorites = async () => {
    try {
      const response = await api.get('/api/users/favorites', {
        headers: { Authorization: `Bearer ${token}` }
      })
      weatherList.value = response.data
    } catch (err) {
      console.error('Error loading favorites:', err)
    }
  }
  
  const searchCities = async () => {
    if (search.value.length < 3) {
      suggestions.value = []
      return
    }
    try {
      const response = await api.get(`/api/users/cities?query=${search.value}`, {
        headers: { Authorization: `Bearer ${token}` }
      })
      suggestions.value = response.data
    } catch (err) {
      console.error('Error searching cities:', err)
    }
  }
  
  const addFavorite = async (city) => {
    try {
      await api.post('/api/users/favorites', { cityName: city }, {
        headers: { 
          Authorization: `Bearer ${token}`,
          'Content-Type': 'application/json'
        }
      })
      search.value = ''
      suggestions.value = []
      await loadFavorites()
    } catch (err) {
      console.error('Error adding favorite:', err)
    }
  }
  
  onMounted(loadFavorites)
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
    align-items: center;
    gap: 1rem;
  }
  
  .weather-icon {
    width: 50px;
    height: 50px;
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
  </style>
  