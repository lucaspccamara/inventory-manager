<template>
  <q-page padding>
    <q-btn color="primary" label="Registrar Compra" />
    
    <q-table :rows="entradas" :columns="colunas" row-key="id" />
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const entradas = ref([
  { id: 1, produto: 'Produto A', quantidade: 50, fornecedor: 'Fornecedor X' },
  { id: 2, produto: 'Produto B', quantidade: 30, fornecedor: 'Fornecedor Y' }
]);

const colunas = [
  { name: 'id', label: 'ID', field: 'id' },
  { name: 'produto', label: 'Produto', field: 'produto' },
  { name: 'quantidade', label: 'Quantidade', field: 'quantidade' },
  { name: 'fornecedor', label: 'Fornecedor', field: 'fornecedor' }
];
</script>
