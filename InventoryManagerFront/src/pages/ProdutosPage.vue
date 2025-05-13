<template>
  <q-page padding>
    <div class="col flex justify-between q-mb-md">
      <q-btn
        color="primary"
        label="Cadastrar Produto"
        @click="cadastroProduto(null)"
      />

      <q-btn label="Buscar" @click="buscarProdutos" color="primary" />
    </div>

    <div class="row q-col-gutter-md q-mb-md">
      <div :class="selecionavel ? 'col-2' : 'col-1'">
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
      <div :class="selecionavel ? 'col-10' : 'col-5'">
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
      <div v-if="!selecionavel" class="col-1">
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
      <template v-slot:body-cell-nome="props">
        <q-td :props="props">
          <div style="display: flex;">
            <div class="ellipsis" style="flex: 1 1 0%; max-width: 500px;" :title="props.row.nome">
              {{ props.row.nome }}
            </div>
          </div>
        </q-td>
      </template>
      <template v-slot:body-cell-estoque="props">
        <q-td :props="props">
          <q-badge :color="props.row.quantidade < 0 ? 'red' : 'primary'">
            {{ formatarQuantidade(props.row.quantidade) }}

            <q-tooltip class="bg-amber text-body2 text-black">
              <div v-for="(quantidade, index) in props.row.quantidadesPorUnidade" :key="index">
                <strong>{{ quantidade.unidade }}:</strong> {{ formatarQuantidade(quantidade.quantidade) }}
              </div>
            </q-tooltip>
          </q-badge>
        </q-td>
      </template>
      <template v-slot:body-cell-status="props">
        <q-td :props="props">
          {{ props.row.status ? 'Ativo' : 'Inativo' }}
        </q-td>
      </template>
      <template v-slot:body-cell-acoes="props">
        <q-td :props="props">
          <q-btn
            v-if="selecionavel"
            flat
            round
            icon="check"
            color="primary"
            @click="selecionarProduto(props.row.id)"
          />
          <template v-else>
            <q-btn flat round icon="edit" @click="editarProduto(props.row.id)" />
            <q-btn flat round icon="delete" color="red" @click="confirmarDelecao(props.row.id)" :disable="!props.row.status" />
          </template>
        </q-td>
      </template>
    </q-table>

    <q-dialog v-model="dialogCriarProduto" persistent class="lg-dialog">
      <CadastroProdutos 
        :idProduto="idProduto"
        @atualizarLista="buscarProdutos"
        @fecharDialog="dialogCriarProduto = false"
      />
    </q-dialog>

    <ConfirmDialog
      v-model="dialogConfirmarDelecao"
      message="Tem certeza que deseja excluir este produto?"
      @isConfirmado="deletarProduto"
    />
  </q-page>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { Notify } from 'quasar';
import { api } from '../boot/axios';
import { Produto, ProdutoTableDto, StatusOpcoesBoolean, SearchOptions, ApiRequest, ApiResponse } from '../components/models';
import CadastroProdutos from '../components/CadastroProdutosComponent.vue';
import ConfirmDialog from '../components/ConfirmDialogComponent.vue';

const props = defineProps<{
  selecionavel?: boolean
}>();

const emit = defineEmits(['selecionar']);

const selecionarProduto = async (produtoId: number) => {
  const produto = ref<Produto>({
    id: null,
    nome: '',
    descricao: '',
    quantidade: 0,
    status: true,
    unidadeCompra: null,
    unidadesVenda: [],
    menorUnidade: null
  });

  try {
    const { data } = await api.get<Produto>(`/produtos/${produtoId}`);
    produto.value = data;
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar produto!',
      color: 'negative'
    });
  } finally {
    emit('selecionar', produto.value);
  }
}

const idProduto = ref<number | null>(null);
const dialogCriarProduto = ref(false);
const idProdutoParaDelecao = ref<number | null>(null);
const dialogConfirmarDelecao = ref(false);

const request = ref<ApiRequest<{ id?: number | null; nome?: string; status?: boolean, searchType: string }>>({
  filter: { id: null, nome: '', status: true, searchType: 'contains' },
  page: 1,
  pageSize: 10,
  sortBy: 'id',
  sortDesc: false
});

const response = ref<ApiResponse<ProdutoTableDto>>({
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
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const, style: 'width: 500px' },
  { name: 'estoque', label: 'Estoque', field: 'quantidade', align: 'center' as const },
  { name: 'status', label: 'Status', field: 'status', align: 'center' as const },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'center' as const, style: 'width: 100px' }
];

const formatarQuantidade = (quantidade: number): string => {
  return quantidade.toLocaleString('pt-BR');
};

const buscarProdutos = async () => {
  try {
    loading.value = true;

    const { data } = await api.post<ApiResponse<ProdutoTableDto>>('/produtos/lista', request.value);

    response.value = data;

    // Atualiza a paginação do q-table
    pagination.value.page = data.page;
    pagination.value.rowsPerPage = data.pageSize;
    pagination.value.rowsNumber = data.totalRecords;
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar produtos!',
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
  buscarProdutos();
};

const cadastroProduto = (id: number | null) => {
  idProduto.value = id;
  dialogCriarProduto.value = true;
}

const editarProduto = (id: number) => {
  cadastroProduto(id);
};

const confirmarDelecao = (id: number) => {
  idProdutoParaDelecao.value = id;
  dialogConfirmarDelecao.value = true;
};

const deletarProduto = async (isConfirmado: boolean) => {
  if (isConfirmado && idProdutoParaDelecao.value !== null) {
    try {
      await api.delete(`/produtos/${idProdutoParaDelecao.value}`).finally(() => {
        Notify.create({
          message: 'Produto deletado com sucesso!',
          color: 'info'
        });
      });
      buscarProdutos();
    } catch (error) {
      Notify.create({
        message: 'Erro ao deletar produto!',
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

watch(() => request.value.filter.id, (newValue) => {
  request.value.filter.id = newValue ? Number(newValue) : null;
}, { deep: true });

buscarProdutos();
</script>
