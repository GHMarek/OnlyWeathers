<template>
  <!-- Główna sekcja widoku -->
  <div class="container"> 
  
      <h2>Change Password</h2>

      <div class="card">
        <!-- Pole na obecne hasło -->
        <input v-model="currentPassword" type="password" placeholder="Current password" />
        
        <!-- Pole na nowe hasło -->
        <input v-model="newPassword" type="password" placeholder="New password" />
        
        <!-- Przycisk zmieniający hasło -->
        <button @click="submitPasswordChange">Change Password</button>

        <!-- Komunikat o błędzie -->
        <p v-if="error">{{ error }}</p>

        <!-- Komunikat o sukcesie -->
        <p v-if="success">{{ success }}</p>
      </div>



  </div>
</template>


  
<script setup>
  import { ref } from 'vue' // ref tworzy zmienne reaktywne (reagujące na zmiany)
  import { changePassword } from '@/services/authService' // import funkcji

  // Pola formularza – dane z inputów
  const currentPassword = ref('')
  const newPassword = ref('')

  // Komunikaty stanu
  const error = ref('')
  const success = ref('')

  // Funkcja wywoływana po kliknięciu przycisku
  const submitPasswordChange = async () => {
    try {
      await changePassword(currentPassword.value, newPassword.value)
      error.value = ''
      success.value = 'Password changed successfully!'
    } catch (err) {
      success.value = ''

      if (err.response?.status === 401) {
        error.value = 'You are not authorized. Please log in again.'
      } else if (err.response?.status === 400) {
        error.value = err.response.data || 'Invalid current password.'
      } else {
        error.value = 'Something went wrong. Please try again.'
      }
    }
  }

</script>
  
<style scoped>
  .container {
    min-height: 100vh;
    display: flex;
    flex-direction: column;
    justify-content: start;
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

  .back-link {
  align-self: flex-start;
  margin-bottom: 1rem;
  color: #6bdee2;
  text-decoration: none;
  font-weight: bold;
  }

  .back-link:hover {
    text-decoration: underline;
  }

  .logo {
    font-size: 2rem;
    margin-bottom: 1rem;
  }
  
</style>
  