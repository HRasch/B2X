import { createApp } from 'vue'
import { createPinia } from 'pinia'
import router from './router'
import { setupAuthMiddleware } from './middleware/auth'
import App from './App.vue'
import './main.css'

const app = createApp(App)

app.use(createPinia())
app.use(router)

// Setup auth middleware
setupAuthMiddleware(router)

app.mount('#app')
