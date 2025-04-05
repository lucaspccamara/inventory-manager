<template>
  <q-page>
    <q-card>
      <q-card-section>
        <q-toolbar>
          <q-toolbar-title>Conversões de Unidades</q-toolbar-title>
        </q-toolbar>
      </q-card-section>
      
      <q-card-section>
        <q-form @submit.prevent="salvarConversao">
          <q-select v-model="origem" :options="opcoesUnidades" label="Unidade de Origem" required />
          <q-select v-model="destino" :options="opcoesUnidades" label="Unidade de Destino" required />
          <q-input v-model.number="fator" type="number" label="Fator de Conversão" required />
          <q-btn type="submit" color="primary" label="Salvar" />
        </q-form>
      </q-card-section>
      
      <q-card-section>
        <q-list bordered>
          <q-item v-for="(conversao, index) in conversoes" :key="index">
            <q-item-section>
              <q-item-label>{{ conversao.origem }} → {{ conversao.destino }} ({{ conversao.fator }})</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { UnidadeConversao } from './models';

const origem = ref(null);
const destino = ref(null);
const fator = ref(1);
const conversoes = ref<UnidadeConversao[]>([]);
const opcoesUnidades = ref(["kg", "g", "l", "ml", "m", "cm"]);

function salvarConversao() {
  console.log('Salvando conversão:', {
    origem: origem.value,
    destino: destino.value,
    fator: fator.value
  });
}
</script>
