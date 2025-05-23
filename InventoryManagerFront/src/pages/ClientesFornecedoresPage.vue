<template>
  <q-page padding>
    <div class="col flex justify-between q-mb-md">
      <q-btn
        color="primary"
        label="Cadastrar Cliente/Fornecedor"
        @click="novoClienteFornecedor"
      />
  
      <q-btn
        label="Buscar"
        @click="buscarClientesFornecedores"
        color="primary"
      />
    </div>

    <div class="row q-col-gutter-md q-mb-md">
      <div class="col-5">
        <q-btn-group spread outline>
          <q-select
            v-model="request.filter.searchType"
            :options="SearchOptions"
            label="Tipo de Busca:"
            outlined
            dense
            emit-value
            map-options
            class="col"
          />
          <q-input v-model="request.filter.nome" label="Nome:" outlined dense class="col-9" />
        </q-btn-group>
      </div>
      <div class="col-2">
        <q-input
          v-model="request.filter.cpfCnpj"
          label="CPF/CNPJ:"
          outlined
          dense
          :mask="cpfCnpjMask"
          :rules="[val => validarCpfCnpj(val)]"
          lazy-rules
          ref="cpfCnpjInput"
          @update:model-value="validarDuranteInput"
        />
      </div>
      <div class="col-1">
        <q-select
          v-model="request.filter.tipo"
          :options="TipoClienteFornecedor"
          label="Tipo:"
          outlined
          dense
          emit-value
          map-options
        />
      </div>
      <div class="col-1">
        <q-select
          v-model="request.filter.status"
          :options="StatusOpcoesBoolean"
          label="Status:"
          outlined
          dense
          emit-value
          map-options
        />
      </div>
    </div>

    <q-table
      v-model:pagination="pagination"
      :rows="response.data"
      :columns="colunas"
      row-key="id"
      :loading="loading"
      @request="atualizarPaginacao"
      loading-label="Carregando..."
      rows-per-page-label="Registros por página"
      :pagination-label="(start, end, total) => `${ start }-${ end } de ${ total }`"
      no-data-label="Nenhum registro encontrado"
      no-results-label="Nenhum resultado encontrado"
    >
      <template v-slot:body-cell-tipo="props">
        <q-td :props="props">
          {{ TipoClienteFornecedor.find(tipo => tipo.value === props.row.tipo)?.label }}
        </q-td>
      </template>
      <template v-slot:body-cell-status="props">
        <q-td :props="props">
          {{ props.row.status ? 'Ativo' : 'Inativo' }}
        </q-td>
      </template>
      <template v-slot:body-cell-acoes="props">
        <q-td :props="props">
          <q-btn flat round icon="edit" @click="editarClienteFornecedor(props.row.id)" />
          <q-btn flat round icon="delete" color="red" @click="confirmarDelecao(props.row.id)" />
        </q-td>
      </template>
    </q-table>

    <q-dialog v-model="dialogCriarClienteFornecedor" persistent class="lg-dialog">
      <CadastroClientesFornecedores
        :idClienteFornecedor="idClienteFornecedor"
        @atualizarLista="buscarClientesFornecedores"
        @fecharDialog="dialogCriarClienteFornecedor = false"
      />
    </q-dialog>

    <ConfirmDialog
      v-model="dialogConfirmarDelecao"
      message="Tem certeza que deseja excluir este cliente/fornecedor?"
      @isConfirmado="deletarClienteFornecedor"
    />
  </q-page>
</template>

<script setup lang="ts">
import { ref, watch, computed, nextTick } from 'vue';
import { Notify, QInput } from 'quasar';
import { api } from '../boot/axios';
import { ClienteFornecedorTableDto, TipoClienteFornecedor, StatusOpcoesBoolean, SearchOptions, ApiRequest, ApiResponse, TipoClienteFornecedorType } from '../components/models';
import CadastroClientesFornecedores from '../components/CadastroClientesFornecedoresComponent.vue';
import ConfirmDialog from '../components/ConfirmDialogComponent.vue';

const cpfCnpjInput = ref<QInput | null>(null);
const cpfCnpjMask = computed(() => {
  const valor = request.value.filter.cpfCnpj ?? '';
  return valor.length <= 14 ? '###.###.###-###' : '##.###.###/####-##'; // Mascara de CPF com um caractere a mais para permitir a mudança de máscara
});

const validarCpfCnpj = (valor: string) => {
  if (!valor) return true;
  const tamanho = valor.length;
  if (tamanho === 14 || tamanho === 18) {
    return true; // CPF (10 dígitos) ou CNPJ (14 dígitos) é válido
  }
  return 'CPF deve ter 11 dígitos ou CNPJ deve ter 14 dígitos';
};

const validarDuranteInput = async () => {
  await nextTick(); // Aguarda o Vue atualizar o valor do campo
  if (cpfCnpjInput.value && cpfCnpjInput.value.hasError) {
    cpfCnpjInput.value.validate(); // Revalida o campo somente se houver erro
  }
};

const idClienteFornecedor = ref<number | null>(null);
const dialogCriarClienteFornecedor = ref(false);
const idClienteFornecedorParaDelecao = ref<number | null>(null);
const dialogConfirmarDelecao = ref(false);

const request = ref<ApiRequest<{ nome?: string; cpfCnpj?: string; tipo?: TipoClienteFornecedorType | null; status?: boolean, searchType: string }>>({
  filter: { nome: '', cpfCnpj: '', tipo: TipoClienteFornecedor[3].value, status: true, searchType: 'contains' },
  page: 1,
  pageSize: 10,
  sortBy: 'nome',
  sortDesc: false
});

const response = ref<ApiResponse<ClienteFornecedorTableDto>>({
  data: [],
  totalRecords: 0,
  page: 1,
  pageSize: 10,
  totalPages: 1
});

const loading = ref(false);

const pagination = ref({
  page: 1,
  rowsPerPage: 10,
  rowsNumber: 0
});

const colunas = [
  { name: 'id', label: 'ID', field: 'id', align: 'left' as const, style: 'width: 100px' },
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const },
  { name: 'cpfCnpj', label: 'CPF/CNPJ', field: 'cpfCnpj', align: 'left' as const },
  { name: 'contato', label: 'Telefone/Celular', field: 'contato', align: 'left' as const },
  { name: 'email', label: 'E-mail', field: 'email', align: 'left' as const },
  { name: 'tipo', label: 'Tipo', field: 'tipo', align: 'left' as const },
  { name: 'status', label: 'Status', field: 'status', align: 'center' as const },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'center' as const, style: 'width: 100px' }
];

const buscarClientesFornecedores = async () => {
  try {
    loading.value = true;

    const { data } = await api.post('/clientes/lista', request.value);

    response.value = data;

    // Atualiza a paginação do q-table
    pagination.value.page = data.page;
    pagination.value.rowsPerPage = data.pageSize;
    pagination.value.rowsNumber = data.totalRecords;
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar clientes/fornecedores!',
      color: 'negative'
    });
  } finally {
    loading.value = false;
  }
};

const atualizarPaginacao = (props: { pagination: any }) => {
  const { page, rowsPerPage } = props.pagination;
  request.value.page = page;
  request.value.pageSize = rowsPerPage;
  buscarClientesFornecedores();
};

const novoClienteFornecedor = () => {
  idClienteFornecedor.value = null;
  dialogCriarClienteFornecedor.value = true;
};

const editarClienteFornecedor = (id: number) => {
  idClienteFornecedor.value = id;
  dialogCriarClienteFornecedor.value = true;
};

const confirmarDelecao = (id: number) => {
  idClienteFornecedorParaDelecao.value = id;
  dialogConfirmarDelecao.value = true;
};

const deletarClienteFornecedor = async (isConfirmado: boolean) => {
  if (isConfirmado && idClienteFornecedorParaDelecao.value !== null) {
    try {
      await api.delete(`/clientes/${idClienteFornecedorParaDelecao.value}`);
      Notify.create({
        message: 'Cliente/Fornecedor excluído com sucesso!',
        color: 'info'
      });
      buscarClientesFornecedores();
    } catch (error) {
      Notify.create({
        message: 'Erro ao excluir cliente/fornecedor!',
        color: 'negative'
      });
    } finally {
      dialogConfirmarDelecao.value = false;
    }
  } else {
    dialogConfirmarDelecao.value = false;
  }
};

watch(() => request.value.filter, () => {
  request.value.page = 1;
}, { deep: true });

buscarClientesFornecedores();
</script>