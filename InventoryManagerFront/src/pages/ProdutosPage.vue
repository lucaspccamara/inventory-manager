<template>
  <q-page padding>
    <q-btn color="primary" label="Adicionar Produto" @click="novoProduto" />

    <q-table :rows="produtos" :columns="colunas" row-key="id" />

  </q-page>
  
  <q-dialog v-model="dialogCriarProduto" persistent class="lg-dialog">
    <CadastroProdutos />
  </q-dialog>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import CadastroProdutos from './CadastroProdutosPage.vue';

const dialogCriarProduto = ref(false);

const produtos = ref([
  { id: 1, nome: 'Produto A', estoque: 10, lote: 'L001' },
  { id: 2, nome: 'Produto B', estoque: 5, lote: 'L002' }
]);

const colunas = [
  { name: 'id', label: 'ID', field: 'id' },
  { name: 'nome', label: 'Nome', field: 'nome' },
  { name: 'estoque', label: 'Estoque', field: 'estoque' },
  { name: 'lote', label: 'Lote', field: 'lote' }
];

function novoProduto() {
  dialogCriarProduto.value = true;
}
</script>
