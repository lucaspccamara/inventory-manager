<template>
  <q-page padding>
    <q-btn color="primary" label="Gerar Pedido" />
    <q-btn color="secondary" label="Gerar Orçamento" />

    <q-table :rows="saidas" :columns="colunas" row-key="id" />
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const saidas = ref([
  { id: 1, produto: 'Produto A', quantidade: 2, cliente: 'Cliente 1' },
  { id: 2, produto: 'Produto B', quantidade: 5, cliente: 'Cliente 2' }
]);

const colunas = [
  { name: 'id', label: 'ID', field: 'id' },
  { name: 'produto', label: 'Produto', field: 'produto' },
  { name: 'quantidade', label: 'Quantidade', field: 'quantidade' },
  { name: 'cliente', label: 'Cliente', field: 'cliente' }
];
</script>
