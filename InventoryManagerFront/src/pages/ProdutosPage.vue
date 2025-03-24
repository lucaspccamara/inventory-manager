<template>
  <q-page padding>
    <q-btn
    class="q-mb-md"
    color="primary"
    label="Adicionar Produto"
    @click="cadastroProduto(null)" />

    <div class="row q-col-gutter-md">

    </div>

    <q-table :rows="produtos" :columns="colunas" row-key="id" />

    <q-dialog v-model="dialogCriarProduto" persistent class="lg-dialog">
      <CadastroProdutos 
        :idProduto="idProduto"
        @atualizarLista=""
        @fecharDialog="dialogCriarProduto = false"
      />
    </q-dialog>
  </q-page>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import CadastroProdutos from '../components/CadastroProdutosComponent.vue';

const dialogCriarProduto = ref(false);
const idProduto = ref<number | null>(null);

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

function cadastroProduto(id: number | null) {
  idProduto.value = id;
  dialogCriarProduto.value = true;
}
</script>
