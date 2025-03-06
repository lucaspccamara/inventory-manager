<template>
  <q-card padding style="width: 700px; max-width: 80vw;">
    <q-card-section>
      <q-toolbar>
        <q-toolbar-title>Cadastro de Clientes e Fornecedores</q-toolbar-title>
      </q-toolbar>
      
      <q-form @submit="salvarCadastro" class="q-gutter-md">
        <q-input v-model="cadastro.nome" label="Nome" required />
        <q-input v-model="cadastro.cpfCnpj" label="CPF/CNPJ" required />
        <q-input v-model="cadastro.telefone" label="Telefone" />
        <q-input v-model="cadastro.celular" label="Celular" required />
        <q-input v-model="cadastro.email" label="E-mail" type="email" required />
        <q-input v-model="cadastro.endereco" label="Endereço" required />

        <q-checkbox v-model="cadastro.eCliente" label="Cliente" />
        <q-checkbox v-model="cadastro.eFornecedor" label="Fornecedor" />
        
        <q-btn type="submit" label="Salvar" color="primary" />
      </q-form>
    </q-card-section>
  </q-card>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const cadastro = ref({
  nome: '',
  cpfCnpj: '',
  telefone: '',
  celular: '',
  email: '',
  endereco: '',
  eCliente: false,
  eFornecedor: false
});

const salvarCadastro = () => {
  console.log("Cadastro salvo:", cadastro.value);
};
</script>
