<template>
    <div>
      <header class="topbar">
        <h1 class="logo">OnlyWeathers ☁️</h1>
  
        <!-- Jeśli użytkownik jest zalogowany, pokaż panel użytkownika -->
        <div class="links" v-if="isLoggedIn">
            <details class="dropdown-container">
                <summary class="user-info">
                    {{ email }} ⮟
                </summary>
                <div class="dropdown-menu">
                    <router-link to="/dashboard">Dashboard</router-link>
                    <router-link to="/change-password">Change Password</router-link>
                    <button @click="logout">Logout</button>
                </div>
            </details>
        </div>
      </header>
  
      <main>
        <slot />
      </main>
    </div>
  </template>
  
  <script setup>
    import { useRouter } from 'vue-router'
    import { computed } from 'vue'

    // Wspólny reaktywny stan aplikacji
    import { token, email } from '@/services/authState'
    import { watchEffect } from 'vue'

    const router = useRouter()

    // Czy użytkownik jest zalogowany?
    const isLoggedIn = computed(() => !!token.value)

    // Po odświeżeniu – ustaw e-mail na nowo z tokena
    watchEffect(() => {
    if (token.value && !email.value) {
        try {
        const jwt = JSON.parse(atob(token.value.split('.')[1]))
        email.value = jwt['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'] || ''
        } catch {
        // Jeśli token jest uszkodzony – usuń
        token.value = null
        email.value = ''
        localStorage.removeItem('token')
        }
    }
    })

    // Wylogowanie: czyści token z pamięci i stanu
    const logout = () => {
    localStorage.removeItem('token')
    token.value = null
    email.value = ''
    router.push('/')
    }
</script>

  
<style scoped>
  .topbar {
    background-color: #2e2e2e;
    color: white;
    padding: 1rem 2rem;
    display: flex;
    justify-content: space-between;
    align-items: center;
    box-shadow: 0 0 5px #000;
    font-size: 1rem;
  }
  
  .logo {
    font-size: 1.5rem;
    font-weight: bold;
    color: #fff;
  }
  
  .links {
    display: flex;
    gap: 1rem;
    align-items: center;
  }
  
  .links a,
  .links button {
    color: #6bdee2;
    text-decoration: none;
    background: none;
    border: none;
    font-weight: bold;
    cursor: pointer;
  }
  
  .links button:hover,
  .links a:hover {
    text-decoration: underline;
  }
  
  .user-info {
    color: #aaa;
  }

/* Styl samego triggera dropdowna (e-mail użytkownika) */
.dropdown-container summary {
  cursor: pointer;
  font-weight: bold;
  color: #6bdee2;
  list-style: none; /* usuwa strzałkę z boku */
  display: flex;
  align-items: center;
  gap: 0.3rem;
}

/* Ukryj domyślny trójkąt <summary> w niektórych przeglądarkach */
.dropdown-container summary::-webkit-details-marker {
  display: none;
}

/* Styl menu rozwijanego */
.dropdown-menu {
  position: absolute;
  top: 100%;        /* zaraz pod summary */
  right: 0;         /* wyrównanie do prawej */
  background-color: #2e2e2e;
  padding: 0.75rem 1rem;
  border-radius: 10px;
  box-shadow: 0 0 8px rgba(0, 0, 0, 0.5);
  z-index: 100;
  min-width: 180px;
  display: flex;
  flex-direction: column;
}

/* Styl linków i przycisków w menu */
.dropdown-menu a,
.dropdown-menu button {
  color: #6bdee2;
  font-weight: bold;
  background: none;
  border: none;
  text-align: left;
  padding: 0.4rem 0;
  cursor: pointer;
  font-size: 0.95rem;
}

/* Hover efekt */
.dropdown-menu a:hover,
.dropdown-menu button:hover {
  color: #ffffff;
  text-decoration: underline;
}
.dropdown-container {
  position: relative; /* umożliwia poprawne pozycjonowanie menu */
  display: inline-block;
}

</style>
  