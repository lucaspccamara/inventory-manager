<template>
  <q-page>
    <q-card>
      <q-card-section>
        <q-toolbar>
          <q-toolbar-title>Cadastro de Unidades</q-toolbar-title>
        </q-toolbar>
      </q-card-section>
      
      <q-card-section>
        <q-form @submit.prevent="salvarUnidade">
          <q-input v-model="nome" label="Nome da Unidade" required />
          <q-input v-model="sigla" label="Sigla" required />
          <q-btn type="submit" color="primary" label="Salvar" />
        </q-form>
      </q-card-section>
      
      <q-card-section>
        <q-list bordered>
          <q-item v-for="unidade in unidades" :key="unidade.id">
            <q-item-section>
              <q-item-label>{{ unidade.nome }} ({{ unidade.sigla }})</q-item-label>
            </q-item-section>
          </q-item>
        </q-list>
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { Unidades } from './models';

const nome = ref('');
const sigla = ref('');
const unidades = ref<Unidades[]>([]);

function salvarUnidade() {
  if (nome.value && sigla.value) {
    unidades.value.push({ id: Date.now(), nome: nome.value, sigla: sigla.value });
    nome.value = '';
    sigla.value = '';
  }
}
</script>
