<template>
  <q-page padding>
    <q-card>
      <q-card-section>
        <q-btn
          class="q-mb-md"
          color="primary"
          label="Adicionar Nova"
          @click="cadastroUnidade(null)"
        />
        <div class="row q-col-gutter-md">
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
            <q-input v-model="request.filter.sigla" label="Sigla:" outlined dense />
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
            <q-btn label="Buscar" @click="carregarUnidades" color="primary" />
          </div>
        </div>
      </q-card-section>

      <q-card-section>
        <q-table
          v-model:pagination="pagination"
          :rows="response.data"
          :columns="colunas"
          row-key="id"
          :loading="loading"
          @request="atualizarPaginacao"
        >
          <template v-slot:body-cell-status="props">
            <q-td :props="props">
              {{ props.row.status ? 'Ativo' : 'Inativo' }}
            </q-td>
          </template>
          <template v-slot:body-cell-acoes="props">
            <q-td :props="props">
              <q-btn flat round icon="edit" @click="editarUnidade(props.row.id)" />
              <q-btn flat round icon="delete" color="red" @click="deletarUnidade(props.row.id)" :disable="!props.row.status" />
            </q-td>
          </template>
        </q-table>
      </q-card-section>
    </q-card>
  </q-page>

  <q-dialog v-model="dialogCriarUnidadeMedida" persistent class="lg-dialog">
    <CadastroUnidades
      :idUnidade="idUnidade"
      @atualizarLista="carregarUnidades"
      @fecharDialog="dialogCriarUnidadeMedida = false"
    />
  </q-dialog>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { api } from '../boot/axios';
import { UnidadeMedida, StatusOpcoesBoolean, SearchOptions, ApiRequest, ApiResponse } from './models';
import CadastroUnidades from './CadastroUnidadesComponent.vue';

const idUnidade = ref<number | null>(null);
const dialogCriarUnidadeMedida = ref(false);

const request = ref<ApiRequest<{ nome?: string; sigla?: string; status?: boolean, searchType: string }>>({
  filter: { nome: '', sigla: '', status: true, searchType: 'contains' },
  page: 1,
  pageSize: 10,
  sortBy: 'id',
  sortDesc: false
});

const response = ref<ApiResponse<UnidadeMedida>>({
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
  { name: 'nome', label: 'Nome', field: 'nome', align: 'left' as const },
  { name: 'sigla', label: 'Sigla', field: 'sigla', align: 'left' as const },
  { name: 'status', label: 'Status', field: 'status', align: 'left' as const },
  { name: 'acoes', label: 'Ações', field: 'acoes', align: 'right' as const }
];

function cadastroUnidade(id: number | null) {
  idUnidade.value = id;
  dialogCriarUnidadeMedida.value = true;
}

const editarUnidade = (id: number) => {
  cadastroUnidade(id);
};

const deletarUnidade = async (id: number) => {
  try {
    await api.delete(`/unidades/${id}`);
    carregarUnidades();
  } catch (error) {
    console.error('Erro ao deletar unidade:', error);
  }
};

const carregarUnidades = async () => {
  try {
    loading.value = true;
    const { data } = await api.post<ApiResponse<UnidadeMedida>>('/unidades/lista', request.value);

    response.value = data;
    pagination.value.rowsNumber = data.totalRecords;
  } catch (error) {
    console.error('Erro ao carregar unidades:', error);
  } finally {
    loading.value = false;
  }
};

const atualizarPaginacao = (props: { pagination: any }) => {
  const { page, rowsPerPage } = props.pagination;
  request.value.page = page;
  request.value.pageSize = rowsPerPage;
  carregarUnidades();
};

watch(() => request.value.filter, () => {
  request.value.page = 1;
}, { deep: true });

carregarUnidades();
</script>
