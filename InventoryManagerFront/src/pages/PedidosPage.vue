<template>
  <q-page padding>
    <div class="row items-center q-mb-md">
      <div class="col-auto flex items-center">
        <q-btn
          :color="props.tipoMovimentacao == 'entrada' ? 'primary' : 'secondary'"
          :label="props.tipoMovimentacao == 'entrada' ? 'Registrar Compra' : 'Registrar Venda'"
          @click="cadastroPedido(null)"
          class="q-mr-md"
        />
  
        <q-btn
          v-if="props.tipoMovimentacao == 'saida'"
          color="warning"
          label="Gerar Orçamento"
          @click="cadastroPedido(null, true)"
        />
      </div>
      <div class="col text-right">
        <q-btn
          label="Buscar"
          @click="buscarPedidos"
          color="primary"
        />
      </div>
    </div>

    <div class="row q-col-gutter-md q-mb-md">
      <div class="col-1">
        <q-input
          v-model="request.filter.id"
          label="Id:"
          maxlength="15"
          mask="###.###.###.###"
          reverse-fill-mask
          unmasked-value
          outlined
          dense
        />
      </div>
      <div class="col-3">
        <BuscaClienteFornecedor
          v-model="clienteFornecedor"
          :busca="tipoMovimentacao == 'entrada' ? 'fornecedores' : 'clientes'"
        />
      </div>
      <div class="col-1">
        <q-input
          v-model="dataInicio"
          label="Data Início:"
          type="date"
          outlined
          dense
        />
      </div>
      <div class="col-1">
        <q-input
          v-model="dataFim"
          label="Data Final:"
          type="date"
          outlined
          dense
        />
      </div>
      <div class="col-1">
        <q-select
          v-model="request.filter.status"
          :options="statusOpcoes"
          label="Status:"
          outlined
          dense
          emit-value
          map-options
        />
      </div>
    </div>
    
    <q-table
      :rows="response.data"
      :columns="colunas"
      row-key="id"
      :table-row-class-fn="getBgStatusColor"
      :loading="loading"
      @request="atualizarPaginacao"
    >
      <template v-slot:body-cell-data="props">
        <q-td :props="props">
          {{ new Date(props.row.data).toLocaleDateString('pt-BR') }}
        </q-td>
      </template>
      <template v-slot:body-cell-total="props">
        <q-td :props="props">
          R$ {{ formatarPreco(props.row.total) }}
        </q-td>
      </template>
      <template v-slot:body-cell-status="props">
        <q-td :props="props">
          {{ getStatusLabel(props.row.status) }}
        </q-td>
      </template>
      <template v-slot:body-cell-acoes="props">
        <q-td :props="props">
          <q-btn
            flat
            round
            icon="print"
            @click="editarPedido(props.row.id, props.row.status === 0)"
          >
            <q-tooltip>Imprimir</q-tooltip>
          </q-btn>
          <q-btn
            flat
            round
            icon="edit"
            @click="editarPedido(props.row.id, props.row.status === 0)"
            :disable="props.row.status === 1 || props.row.status === 3 || props.row.status === 5"
          >
            <q-tooltip>Editar</q-tooltip>
          </q-btn>
          <q-btn
            v-if="props.row.status === 1 || props.row.status === 3 || props.row.status === 5"
            flat
            round
            color="dark"
            icon="restore_page"
            @click="restaurarPedido(props.row.id)"
          >
            <q-tooltip>Restaurar</q-tooltip>
          </q-btn>
          <q-btn
            v-else
            flat
            round
            color="negative"
            icon="delete"
            @click="dialogConfirmarDelecao = true; idPedidoParaDelecao = props.row.id"
          >
            <q-tooltip>Cancelar</q-tooltip>
          </q-btn>
        </q-td>
      </template>
    </q-table>

    <q-dialog v-model="dialogCadastroPedido" persistent class="lg-dialog">
      <CadastroPedidos
        :idPedido="idPedido"
        :tipoMovimentacao="cadastroPedidoTipo"
        @atualizarLista="buscarPedidos"
        @fecharDialog="dialogCadastroPedido = false"
      />
    </q-dialog>

    <ConfirmDialog
      v-model="dialogConfirmarDelecao"
      message="Tem certeza que deseja cancelar este pedido?"
      @isConfirmado="deletarPedido"
    />
  </q-page>
</template>

<script setup lang="ts">
import { ref, watch, computed, onMounted } from 'vue';
import { Notify } from 'quasar';
import { api } from 'src/boot/axios';
import { 
  PedidoTableDto,
  StatusOpcoesEntrada,
  StatusOpcoesEntradaType,
  StatusOpcoesSaida,
  StatusOpcoesSaidaType,
  StatusPedidoLabelColor,
  ClienteFornecedorSelectDto,
  ApiRequest,
  ApiResponse
} from '../components/models';

import BuscaClienteFornecedor from '../components/BuscaClienteFornecedorComponent.vue';
import CadastroPedidos from '../components/CadastroPedidosComponent.vue';
import ConfirmDialog from '../components/ConfirmDialogComponent.vue';

const props = defineProps<{ tipoMovimentacao: 'entrada' | 'saida' }>();

const idPedido = ref<number | null>(null);
const dialogCadastroPedido = ref(false);
const cadastroPedidoTipo = ref<'entrada' | 'saida' | 'orcamento'>('entrada');
const idPedidoParaDelecao = ref<number | null>(null);
const dialogConfirmarDelecao = ref(false);

// Setup de datas
const dataInicio = ref('');
const dataFim = ref('');

function setDefaultDates() {
  const today = new Date();
  dataInicio.value = new Date(today.getFullYear(), today.getMonth() - 1, today.getDate()).toISOString().split('T')[0] || '';
  dataFim.value = new Date(today.getFullYear(), today.getMonth(), today.getDate()).toISOString().split('T')[0] || '';

  request.value.filter.dataInicio = dataInicio.value ? new Date(dataInicio.value) : null;
  request.value.filter.dataFim = dataFim.value ? new Date(dataFim.value) : null;
};

// Prop do campo de busca de clientes e fornecedores
const clienteFornecedor = ref<ClienteFornecedorSelectDto | null>(null);

// Setup de status
const statusOpcoes = ref<typeof StatusOpcoesEntrada | typeof StatusOpcoesSaida>();

// Setup de busca de pedidos
const request = ref<ApiRequest<{ id?: number | null; clienteFornecedorId?: number | null; dataInicio: Date | null; dataFim: Date | null; status?: StatusOpcoesEntradaType | StatusOpcoesSaidaType | null, tipo: number }>>({
  filter: { id: null, clienteFornecedorId: null, dataInicio: null, dataFim: null, status: props.tipoMovimentacao === 'entrada' ? StatusOpcoesEntrada[2].value : StatusOpcoesSaida[3].value, tipo: props.tipoMovimentacao === 'entrada' ? 1 : 2 },
  page: 1,
  pageSize: 10,
  sortBy: 'id',
  sortDesc: false
});

const response = ref<ApiResponse<PedidoTableDto>>({
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

const clienteFornecedorNome = computed(() =>
  props.tipoMovimentacao === 'entrada' ? 'Fornecedor' : 'Cliente'
);

const colunas = computed(() => [
  { name: 'id', label: 'ID', field: 'id', align: 'left' as const, style: 'width: 100px' },
  { name: 'clienteFornecedorNome', label: clienteFornecedorNome.value, field: 'clienteFornecedorNome', align: 'left' as const },
  { name: 'data', label: 'Data', field: 'data', align: 'center' as const, style: 'width: 120px' },
  { name: 'total', label: 'Total', field: 'total', align: 'left' as const, style: 'width: 120px' },
  { name: 'status', label: 'Status', field: 'status', align: 'center' as const, style: 'width: 150px' },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'center' as const, style: 'width: 160px' }
]);

const buscarPedidos = async () => {
  try {
    loading.value = true;

    const { data } = await api.post('/pedidos/lista', request.value);
    
    response.value = data;
    pagination.value.rowsNumber = data.totalRecords;
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar pedidos',
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
  buscarPedidos();
};

const cadastroPedido = (id: number | null, isOrcamento: boolean = false) => {
  if (isOrcamento) {
    cadastroPedidoTipo.value = 'orcamento';
  } else {
    cadastroPedidoTipo.value = props.tipoMovimentacao;
  }

  idPedido.value = id;
  dialogCadastroPedido.value = true;
}

const editarPedido = (id: number, isOrcamento: boolean = false) => {
  cadastroPedido(id, isOrcamento);
};

const deletarPedido = async (isConfirmado: boolean) => {
  if (isConfirmado && idPedidoParaDelecao.value !== null) {
    try {
      await api.delete(`/pedidos/${idPedidoParaDelecao.value}`).finally(() => {
        Notify.create({
          message: 'Pedido deletado com sucesso!',
          color: 'info'
        });
      });
      buscarPedidos();
    } catch (error) {
      Notify.create({
        message: 'Erro ao deletar pedido!',
        color: 'negative'
      });
    } finally {
      dialogConfirmarDelecao.value = false;
    }
  } else {
    dialogConfirmarDelecao.value = false;
  }
};

const restaurarPedido = async (id: number) => {
  try {
    await api.put(`/pedidos/${id}/restaurar`).then(() => {
    }).finally(() => {
      Notify.create({
        message: 'Pedido restaurado com sucesso!',
        color: 'info'
      });
    });
    buscarPedidos();
  } catch (error) {
    Notify.create({ message: 'Erro ao restaurar pedido!', color: 'negative' });
  }
};

const formatarPreco = (valor: number) => {
  return valor.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
};

const getBgStatusColor = (row: any) => {
  const statusObj = StatusPedidoLabelColor.find(s => s.value === row.status);
  return statusObj?.color || 'bg-grey';
}

const getStatusLabel = (status: number) => {
  const statusObj = StatusPedidoLabelColor.find(s => s.value === status);
  return statusObj?.label || '';
}

watch(() => request.value.filter, () => {
  request.value.page = 1;
}, { deep: true });

watch(() => request.value.filter.id, (newValue) => {
  request.value.filter.id = newValue ? Number(newValue) : null;
}, { deep: true });

watch(clienteFornecedor, (newValue) => {
  request.value.filter.clienteFornecedorId = newValue ? Number(newValue.id) : null;
}, { deep: true });

watch(dataInicio, (newValue) => {
  request.value.filter.dataInicio = newValue ? new Date(newValue) : null;
}, { deep: true });

watch(dataFim, (newValue) => {
  request.value.filter.dataFim = newValue ? new Date(newValue) : null;
}, { deep: true });

watch(() => props.tipoMovimentacao, () => { 
  statusOpcoes.value = props.tipoMovimentacao === 'entrada' ? StatusOpcoesEntrada : StatusOpcoesSaida; // Atualiza as opções de status
  cadastroPedidoTipo.value = props.tipoMovimentacao; // Atualiza o tipo de movimentação para o cadastro
  clienteFornecedor.value = null; 
  request.value.filter.id = null;
  request.value.filter.clienteFornecedorId = null;
  request.value.filter.status = props.tipoMovimentacao === 'entrada' ? StatusOpcoesEntrada[2].value : StatusOpcoesSaida[3].value; // Valor padrão para o status
  request.value.filter.tipo = props.tipoMovimentacao === 'entrada' ? 1 : 2; // Valor padrão para o tipo
  response.value = { data: [], totalRecords: 0, page: 1, pageSize: 10, totalPages: 1 };
  setDefaultDates();
  buscarPedidos();
}, { immediate: true });

onMounted(() => {
  setDefaultDates();
  buscarPedidos();
});
</script>
