<template>
  <div class="fullscreen flex flex-center column">
    <q-spinner v-if="status === 'checking'" color="primary" size="40px" />
    <div v-if="status === 'checking'" class="q-mt-md text-grey-7">
      Verificando conexão com a API local...
    </div>

    <template v-if="status === 'error'">
      <q-icon name="warning" color="negative" size="40px" />
      <div class="q-mt-md text-negative">
        Não foi possível conectar à API local.
      </div>
      <q-btn class="q-mt-md" label="Tentar novamente" color="primary" @click="recheck" />
      <q-btn flat label="Fechar aplicativo" class="q-mt-sm" color="grey" @click="closeApp" />
    </template>
  </div>
</template>

<script setup lang="ts">
import { apiStatus, checkApi } from 'boot/check-api'
import { onMounted, watch } from 'vue'
import { useRouter, useRoute } from 'vue-router'

const router = useRouter()
const route = useRoute()
const status = apiStatus

// Guarda a rota original que o usuário tentou acessar antes de cair em /startup
const originalTarget = route.query.redirect as string || '/'

async function recheck() {
  apiStatus.value = 'checking'
  await checkApi()
}

function closeApp() {
  window.close()
}

onMounted(async () => {
  apiStatus.value = 'checking'
  await checkApi()
})

// Redireciona para a rota original quando a API estiver pronta
watch(status, (val) => {
  if (val === 'ready') {
    router.replace(originalTarget)
  }
})
</script>
