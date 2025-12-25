import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import i18n from './locales'
import App from './App.vue'
import './main.css'

const app = createApp(App)

app.use(createPinia())
app.use(i18n)
app.use(router)

// Initialize locale from localStorage or browser language
const locale = localStorage.getItem('locale') || navigator.language.split('-')[0] || 'en'
i18n.global.locale.value = locale
document.documentElement.lang = locale

app.mount('#app')
