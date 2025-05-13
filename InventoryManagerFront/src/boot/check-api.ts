import { api } from 'src/boot/axios'
import { ref } from 'vue'

export const apiStatus = ref<'checking' | 'ready' | 'error'>('checking')

export async function checkApi() {
  for (let i = 0; i < 30; i++) {
    try {
      await api.get('/health')
      apiStatus.value = 'ready'
      return
    } catch (err) {
      await new Promise(res => setTimeout(res, 1000))
    }
  }
  apiStatus.value = 'error'
}
