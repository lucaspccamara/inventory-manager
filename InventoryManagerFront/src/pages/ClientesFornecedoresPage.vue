<template>
  <q-page padding>
    <q-card>
      <q-card-section>
        <q-btn color="primary" label="Adicionar Novo" @click="novoClienteFornecedor" />
        <div class="row q-col-gutter-md">
          <div>
            <q-input v-model="filtro.nome" label="Buscar por Nome" dense outlined />
          </div>
          <div>
            <q-input v-model="filtro.cpfCnpj" label="Buscar por CPF/CNPJ" dense outlined />
          </div>
          <div>
            <q-select v-model="filtro.tipo" :options="[0, 1, 2]" label="Tipo" dense outlined />
          </div>
        </div>
      </q-card-section>
      <q-separator />
      <q-card-section>
        <q-table :rows="listaFiltrada" :columns="colunas" row-key="id">
          <template v-slot:body-cell(acoes)="props">
            <q-td :props="props">
              <q-btn icon="edit" color="primary" @click="editar(props.row)" />
              <q-btn icon="delete" color="negative" @click="excluir(props.row)" />
            </q-td>
          </template>
        </q-table>
      </q-card-section>
    </q-card>
  </q-page>

  <q-dialog v-model="dialogCriarClienteFornecedor" persistent class="lg-dialog">
    <CadastroClientesFornecedores />
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue';
import CadastroClientesFornecedores from './CadastroClientesFornecedoresPage.vue';

const dialogCriarClienteFornecedor = ref(false);

const filtro = ref({
  nome: '',
  cpfCnpj: '',
  tipo: 0
});

const colunas = [
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const, sortable: true },
  { name: 'cpfCnpj', label: 'CPF/CNPJ', field: 'cpfCnpj', align: 'left' as const, sortable: true },
  { name: 'telefone', label: 'Telefone', field: 'telefone', align: 'left' as const },
  { name: 'celular', label: 'Celular', field: 'celular', align: 'left' as const },
  { name: 'email', label: 'E-mail', field: 'email', align: 'left' as const },
  { name: 'endereco', label: 'Endereço', field: 'endereco', align: 'left' as const },
  { name: 'eCliente', label: 'Cliente', field: row => (row.eCliente ? 'Sim' : 'Não'), align: 'center' as const },
  { name: 'eFornecedor', label: 'Fornecedor', field: row => (row.eFornecedor ? 'Sim' : 'Não'), align: 'center' as const },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'center' as const }
];

const registros = ref([
  {
    id: 1,
    nome: 'Empresa ABC',
    cpfCnpj: '12.345.678/0001-99',
    telefone: '11 3333-4444',
    celular: '11 99999-8888',
    email: 'contato@empresaabc.com',
    endereco: 'Rua Exemplo, 123, São Paulo - SP',
    eCliente: true,
    eFornecedor: true
  }
]);

const listaFiltrada = computed(() => {
  return registros.value.filter(item => {
    const nomeMatch = item.nome.toLowerCase().includes(filtro.value.nome.toLowerCase());
    const cpfCnpjMatch = item.cpfCnpj.includes(filtro.value.cpfCnpj);
    const tipoMatch = filtro.value.tipo === 0 || item.tipo === filtro.value.tipo;
    return nomeMatch && cpfCnpjMatch && tipoMatch;
  });
});

const editar = (item) => {
  console.log('Editar:', item);
};

const excluir = (item) => {
  registros.value = registros.value.filter(i => i.id !== item.id);
};

function novoClienteFornecedor() {
  dialogCriarClienteFornecedor.value = true;
}
</script>
