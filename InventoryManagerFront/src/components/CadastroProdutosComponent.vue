<template>
  <q-card padding style="width: 700px; max-width: 80vw;">
    <q-card-section>
      <q-toolbar>
        <q-toolbar-title>Cadastro de Produtos</q-toolbar-title>
        <q-space />
        <q-btn dense flat icon="close" v-close-popup>
          <q-tooltip>Fechar</q-tooltip>
        </q-btn>
      </q-toolbar>

      <q-form @submit.prevent="salvarProduto" class="q-gutter-md">
        <q-input v-model="produto.nome" label="Nome do Produto" required />
        
        <q-select
          v-model="produto.unidadeCompra"
          :options="opcoesUnidades"
          label="Unidade de Compra"
          required
        />
        
        <q-list bordered separator>
          <q-item-label header>Unidades de Venda</q-item-label>
          <q-item v-for="(unidade, index) in produto.unidadesVenda" :key="index">
            <q-input v-model="unidade.origem.nome" label="Unidade" dense class="q-mr-md" />
            <q-input v-model.number="unidade.fator" label="Fator de Conversão" type="number" dense />
            <q-btn icon="delete" color="red" dense flat @click="removerUnidade(index)" />
          </q-item>
        </q-list>
        <!-- <q-btn label="Adicionar Unidade" @click="adicionarUnidade" flat color="primary" /> -->
        
        <q-select
          v-model="produto.menorUnidade"
          :options="produto.unidadesVenda.map(u => u.destino.nome)"
          label="Menor Unidade"
          required
        />
        
        <q-btn type="submit" label="Salvar Produto" color="primary" />
      </q-form>
    </q-card-section>
  </q-card>
</template>

<script setup lang="ts">
import { ref } from 'vue';
import type { Produtos } from './models';

const produto = ref<Produtos>({
  id: null,
  nome: '',
  unidadeCompra: null,
  unidadesVenda: [],
  menorUnidade: null
});

const opcoesUnidades = ['Caixa', 'Pacote', 'Unidade', 'Litro', 'Kg'];

// const adicionarUnidade = () => {
//   produto.value.unidadesVenda.push({ nome: '', fator: 1 });
// };

const removerUnidade = (index: number) => {
  produto.value.unidadesVenda.splice(index, 1);
};

const salvarProduto = () => {
  console.log("Produto salvo:", produto.value);
};
</script>
