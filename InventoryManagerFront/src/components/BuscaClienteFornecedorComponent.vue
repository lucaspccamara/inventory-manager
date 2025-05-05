<template>
  <q-btn-group spread outline>
    <q-select
      v-if="campoOcultado"
      v-model="searchType"
      :options="SearchOptionsClienteFornecedor"
      label="Tipo de Busca:"
      outlined
      dense
      emit-value
      map-options
      @update:model-value="(val) => onSearchTypeChange(val)"
      class="col-4"
    >
      <template v-slot:option="scope">
        <q-item v-if="scope.opt.group" v-bind="scope.itemProps" :clickable="false" class="q-pl-xs">
          <q-item-section>
            <q-separator v-if="scope.index > 0" class="q-my-xs" />
            <div class="text-bold text-primary">{{ scope.opt.label }}</div>
          </q-item-section>
        </q-item>
        <q-item v-else clickable v-bind="scope.itemProps">
          <q-item-section>{{ scope.opt.label }}</q-item-section>
        </q-item>
      </template>
    </q-select>
    <q-select
      v-model="clienteFornecedorVModel"
      :options="response.data"
      option-value="id"
      option-label="nome"
      :label="labelDinamico"
      :hint="hintDinamico"
      :rules="props.rules"
      outlined
      dense
      use-input
      clearable
      input-debounce="0"
      :loading="loading"
      @filter="filterFn"
      @input-value="onInputValue"
      @update:model-value="onClienteFornecedorSelect"
      @virtual-scroll="onVirtualScroll"
      ref="clienteFornecedorSelectRef"
      class="col clienteFornecedorSelect"
      :class="{ digitando: digitando }"
      @focus="digitando = true"
      @blur="digitando = false"
    />
  </q-btn-group>
</template>

<script setup lang="ts">
import { ref, nextTick, watch, computed } from 'vue';
import { Notify, QSelect } from 'quasar';
import { api } from 'src/boot/axios';
import { 
  ClienteFornecedorSelectDto,
  TipoClienteFornecedor,
  TipoClienteFornecedorType,
  SearchOptionsClienteFornecedor,
  SearchType,
  ApiRequest,
  ApiResponse
} from '../components/models';

const props = defineProps<{
  modelValue: ClienteFornecedorSelectDto | null,
  busca: 'clientes' | 'fornecedores',
  ocultarTipoBusca?: boolean,
  hint?: boolean,
  rules?: Array<(val: ClienteFornecedorSelectDto | null) => true | string>
}>();

const emit = defineEmits(['update:modelValue', 'select']);

// Setup de busca de cliente e fornecedores
const listaTipos = computed(() => {
  if (props.busca === 'clientes') {
    return [TipoClienteFornecedor[0].value, TipoClienteFornecedor[2].value]; // Clientes
  } else {
    return [TipoClienteFornecedor[1].value, TipoClienteFornecedor[2].value]; // Fornecedores
  }
});

const searchType = ref<SearchType>(SearchOptionsClienteFornecedor[2].value as SearchType); // Valor padrão para o tipo de busca

const onSearchTypeChange = (val: SearchType) => {
  searchType.value = val;
  request.value.filter.searchType = val;
  clienteFornecedorVModel.value = null;
  response.value.data = [];
};

const request = ref<ApiRequest<{ nome?: string; cpfCnpj?: string; email?: string; telefonecelular?: string; listaTipos?: TipoClienteFornecedorType[] | null; status?: boolean, searchType: string }>>({
  filter: { nome: '', cpfCnpj: '', email: '', telefonecelular: '', listaTipos: listaTipos.value, status: true, searchType: searchType.value },
  page: 1,
  pageSize: 15,
  sortBy: 'Nome',
  sortDesc: false
});

const response = ref<ApiResponse<ClienteFornecedorSelectDto>>({
  data: [],
  totalRecords: 0,
  page: 1,
  pageSize: 15,
  totalPages: 1
});

const loading = ref(false);

// Setup do campo de busca
const clienteFornecedorVModel = ref<ClienteFornecedorSelectDto | null>(props.modelValue ?? null);
const clienteFornecedorSelectRef = ref<QSelect | null>(null);
const digitando = ref(false);

const labelDinamico = computed(() =>
  (props.busca === 'clientes' ? 'Cliente:' : 'Fornecedor:')
);

const hintDinamico = computed(() => {
  if (props.hint && clienteFornecedorVModel.value === null) {
    if (props.busca === 'clientes') {
      return 'Selecione o cliente';
    } else {
      return 'Selecione o fornecedor';
    }
  }
  return undefined;
});

const campoOcultado = computed(() => {
  return !(props.ocultarTipoBusca && clienteFornecedorVModel.value !== null)
});

const clienteFornecedorMask = (valor: string) => {
  switch (searchType.value) {
    case 'cpfCnpj':
      return valor.length <= 14 ? '###.###.###-###' : '##.###.###/####-##';
    case 'telefoneCelular':
      return valor.length <= 14 ? '(##) ####-####' : '(##) #####-####';
    default:
      return null;
  }
};

function aplicarMascara(valor: string, mascara: string): string {
  const numeros = valor.replace(/\D/g, '');
  let resultado = '';
  let numeroIndex = 0;

  for (let i = 0; i < mascara.length && numeroIndex < numeros.length; i++) {
    if (mascara[i] === '#') {
      resultado += numeros[numeroIndex];
      numeroIndex++;
    } else {
      resultado += mascara[i];
    }
  }
  return resultado;
}

const onInputValue = (valor: string) => {
  const mask = clienteFornecedorMask(valor);
  if (!mask) {
    return;
  }
  const valorMascarado = aplicarMascara(valor, mask);

  // Só atualiza se realmente mudou para evitar loops
  if (valorMascarado !== valor) {
    nextTick(() => {
      clienteFornecedorSelectRef.value?.updateInputValue(valorMascarado, true);
    });
  }
};

const onClienteFornecedorSelect = () => {
  nextTick(() => {
    // Acessa o input interno do QSelect e remove o foco
    const input = clienteFornecedorSelectRef.value?.$el.querySelector('input');
    if (input) input.blur();
    digitando.value = false; // Remove o estado de digitação
  });
  emit('select', clienteFornecedorVModel.value);
};

const filterFn = async (val: string, update: (val: any) => void, abort: () => void) => {
  // Limpar os campos de filtro antes de aplicar o novo valor
  request.value.filter.nome = '';
  request.value.filter.cpfCnpj = '';
  request.value.filter.email = '';
  request.value.filter.telefonecelular = '';

  // Limpar a lista de resultados antes de aplicar o novo valor
  response.value.data = [];
  response.value.totalRecords = 0;
  response.value.page = 1;
  response.value.pageSize = 15;
  response.value.totalPages = 1;

  // Resetar a paginação para a primeira página
  request.value.page = 1;

  // Preencher o campo de acordo com o tipo de busca selecionado
  const needle = val.toLowerCase();
  switch (request.value.filter.searchType) {
    case 'contains':
    case 'startsWith':
    case 'endsWith':
      request.value.filter.nome = needle || '';
      break;
    case 'cpfCnpj':
      request.value.filter.cpfCnpj = needle || '';
      break;
    case 'email':
      request.value.filter.email = needle || '';
      break;
    case 'telefoneCelular':
      request.value.filter.telefonecelular = needle || '';
      break;
  }

  await buscarClientesFornecedores(false);
  update(() => {});
};

const buscarClientesFornecedores = async (append = false) => {
  try {
    loading.value = true;
    const { data } = await api.post<ApiResponse<ClienteFornecedorSelectDto>>('/clientes/busca', request.value);

    if (append) {
      response.value.data = [...response.value.data, ...data.data];
    } else {
      response.value.data = data.data;
    }
    response.value.totalRecords = data.totalRecords;
    response.value.page = data.page;
    response.value.pageSize = data.pageSize;
    response.value.totalPages = data.totalPages;
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar dados!',
      color: 'negative'
    });
  } finally {
    loading.value = false;
  }
};

const onVirtualScroll = ({ index }: { index: number }) => {
  // Se já está carregando ou não há mais páginas, não faz nada
  if (loading.value || response.value.page * response.value.pageSize >= response.value.totalRecords) return;

  // Se o usuário chegou perto do fim da lista, carrega mais
  if (index >= response.value.data.length - 1) {
    request.value.page++;
    buscarClientesFornecedores(true);
  }
}

watch(() => props.modelValue, v => clienteFornecedorVModel.value = v);

watch(clienteFornecedorVModel, (v, oldVal) => {
  if (v !== oldVal) emit('update:modelValue', v);
});

watch(() => props.busca, () => {
  // Atualiza o tipo de busca padrão
  searchType.value = SearchOptionsClienteFornecedor[2].value as SearchType;
  request.value.filter.searchType = searchType.value;

  // Atualiza os tipos permitidos conforme o novo tipo de busca
  request.value.filter.listaTipos = props.busca === 'clientes'
    ? [TipoClienteFornecedor[0].value, TipoClienteFornecedor[2].value]
    : [TipoClienteFornecedor[1].value, TipoClienteFornecedor[2].value];

  // Limpa filtros específicos
  request.value.filter.nome = '';
  request.value.filter.cpfCnpj = '';
  request.value.filter.email = '';
  request.value.filter.telefonecelular = '';
  request.value.filter.status = true;

  // Reseta paginação
  request.value.page = 1;
  request.value.pageSize = 15;

  // Limpa seleção e resposta
  clienteFornecedorVModel.value = null;
  response.value.data = [];
  response.value.totalRecords = 0;
  response.value.page = 1;
  response.value.pageSize = 15;
  response.value.totalPages = 1;
});
</script>

<style scoped lang="scss">
.clienteFornecedorSelect.digitando ::v-deep(.q-field__native > span) {
  display: none !important;
}
</style>
