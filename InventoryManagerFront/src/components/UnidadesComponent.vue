<template>
  <q-page padding>
    <q-card>
      <q-card-section>
        <div class="row q-col-gutter-md">
          <div class="col-4">
            <q-input v-model="request.filter.nome" label="Nome:" outlined dense />
          </div>
          <div class="col-4">
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
        />
      </q-card-section>
    </q-card>
  </q-page>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue';
import { api } from '../boot/axios';
import { UnidadeMedida, StatusOpcoesBoolean, ApiRequest, ApiResponse } from './models';

const request = ref<ApiRequest<{ nome?: string; sigla?: string; status?: boolean }>>({
  filter: { nome: '', sigla: '', status: true },
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
  { name: 'status', label: 'Status', field: 'status', align: 'left' as const }
];

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
