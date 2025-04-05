<template>
  <q-page padding>
    <q-btn
    class="q-mb-md"
    color="primary"
    label="Adicionar Produto"
    @click="cadastroProduto(null)" />

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
      <div class="col-3">
        <q-input type="number" v-model="request.filter.id" label="Id:" outlined dense />
      </div>
      <div class="col">
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
      <div class="col flex justify-end">
        <q-btn label="Buscar" @click="buscarProdutos" color="primary" />
      </div>
    </div>

    <q-table
      :rows="response.data"
      :columns="colunas"
      row-key="id"
      :loading="loading"
      @request="atualizarPaginacao"
    >
      <template v-slot:body-cell-estoque="props">
        <q-td :props="props">
          <q-badge color="primary">
            {{ props.row.quantidade }}

            <q-tooltip class="bg-amber text-body2 text-black">
              <div v-for="(quantidade, index) in props.row.quantidadesPorUnidade" :key="index">
                <strong>{{ quantidade.unidade }}:</strong> {{ quantidade.quantidade }}
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
          <q-btn flat round icon="edit" @click="editarProduto(props.row.id)" />
          <q-btn flat round icon="delete" color="red" @click="confirmarDelecao(props.row.id)" :disable="!props.row.status" />
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
import { ProdutoTableDto, StatusOpcoesBoolean, SearchOptions, ApiRequest, ApiResponse } from '../components/models';
import CadastroProdutos from '../components/CadastroProdutosComponent.vue';
import ConfirmDialog from '../components/ConfirmDialogComponent.vue';

const idProduto = ref<number | null>(null);
const dialogCriarProduto = ref(false);
const idProdutoParaDelecao = ref<number | null>(null);
const dialogConfirmarDelecao = ref(false);

const request = ref<ApiRequest<{ id?: number | null; nome?: string; status?: boolean, searchType: string }>>({
  filter: { id: null, nome: '', status: true, searchType: 'contains' },
  page: 1,
  pageSize: 10,
  sortBy: 'nome',
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
  { name: 'id', label: 'ID', field: 'id', align: 'left' as const },
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const },
  { name: 'estoque', label: 'Estoque', field: 'quantidade', align: 'center' as const, style: 'width: 150px' },
  { name: 'status', label: 'Status', field: 'status', align: 'left' as const },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'center' as const, style: 'width: 100px' }
];

const buscarProdutos = async () => {
  try {
    loading.value = true;

    // Garantir que o campo `id` seja um número ou `null`
    if (String(request.value.filter.id) === '') {
      request.value.filter.id = null;
    }

    const { data } = await api.post<ApiResponse<ProdutoTableDto>>('/produtos/lista', request.value);

    response.value = data;
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

buscarProdutos();
</script>
