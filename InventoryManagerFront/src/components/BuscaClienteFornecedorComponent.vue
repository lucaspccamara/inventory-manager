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
  pageSize: 10,
  sortBy: 'nome',
  sortDesc: false
});

const response = ref<ApiResponse<ClienteFornecedorSelectDto>>({
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
  if (val.length <= 1) {
    response.value.data = []
    abort()
    return
  }
  
  // Limpar os campos de filtro antes de aplicar o novo valor
  request.value.filter.nome = '';
  request.value.filter.cpfCnpj = '';
  request.value.filter.email = '';
  request.value.filter.telefonecelular = '';

  setTimeout(() => {
    update(() => {
      const needle = val.toLowerCase();
      
      // Preencher o campo de acordo com o tipo de busca selecionado
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
  
      buscarClientesFornecedores();
    });
  }, 500);
};

const buscarClientesFornecedores = async () => {
  try {
    loading.value = true;

    const { data } = await api.post<ApiResponse<ClienteFornecedorSelectDto>>('/clientes/busca', request.value);

    response.value = data;
    pagination.value.rowsNumber = data.totalRecords;
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar dados!',
      color: 'negative'
    });
  } finally {
    loading.value = false;
  }
};

watch(() => props.modelValue, v => clienteFornecedorVModel.value = v);

watch(clienteFornecedorVModel, (v, oldVal) => {
  if (v !== oldVal) emit('update:modelValue', v);
});

watch(() => props.busca, () => {
  searchType.value = SearchOptionsClienteFornecedor[2].value as SearchType; // Valor padrão para o tipo de busca
  request.value.filter.searchType = searchType.value;
  clienteFornecedorVModel.value = null;
  response.value.data = [];
});
</script>

<style scoped lang="scss">
.clienteFornecedorSelect.digitando ::v-deep(.q-field__native > span) {
  display: none !important;
}
</style>
