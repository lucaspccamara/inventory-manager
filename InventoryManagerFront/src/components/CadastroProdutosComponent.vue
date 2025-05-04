<template>
  <q-card padding style="width: 900px; max-width: 80vw;">
    <q-card-section>
      <q-toolbar>
        <q-toolbar-title>{{ props.idProduto == null ? "Cadastrar" : "Editar" }} Produto</q-toolbar-title>
        <q-space />
        <q-btn dense flat icon="close" v-close-popup>
          <q-tooltip>Fechar</q-tooltip>
        </q-btn>
      </q-toolbar>

      <q-form @submit.prevent="salvarProduto" class="row q-col-gutter-md">
        <div class="col-12">
          <q-input
            v-model="produto.nome"
            label="Nome do Produto *"
            bottom-slots
            counter
            maxlength="255"
            :rules="[ val => val && val.length > 0 || 'Campo obrigatório' ]"
          />
        </div>

        <div class="col-12">
          <q-input
            v-model="produto.descricao"
            label="Descrição"
            type="textarea"
            autogrow
            bottom-slots
            counter
            maxlength="500"
          />
        </div>

        <div class="col-6">
          <q-select
            :model-value="produto.unidadeCompra"
            :options="opcoesUnidades"
            option-value="id"
            option-label="nome"
            label="Unidade de Compra *"
            :rules="[ val => val !== null || 'Campo obrigatório' ]"
            @scroll="handleScroll"
            @update:model-value="onUnidadeCompraChange"
          />
        </div>

        <div class="col-6">
          <q-btn-group spread outline>
            <q-select
              :model-value="produto.menorUnidade"
              :options="opcoesUnidades"
              option-value="id"
              option-label="nome"
              label="Menor Unidade de Venda *"
              :rules="[ val => val !== null || 'Campo obrigatório' ]"
              @scroll="handleScroll"
              @update:model-value="onMenorUnidadeChange"
              class="col-8"
            />
            <q-input
              v-model.number="produto.quantidade"
              type="number"
              label="Quantidade *"
              :input-style="{ color: produto.quantidade < 0 ? 'red' : '' }"
              :rules="[ val => val !== '' || 'Campo obrigatório' ]"
              :disable="!produto.menorUnidade"
              :readonly="produto.id !== null && !quantidadeEditavel"
              @dblclick="habilitarEdicaoQuantidade"
              class="col"
            >
              <q-tooltip class="bg-amber text-black text-body2" v-if="produto.id !== null && !quantidadeEditavel">
                Para editar, clique duas vezes no campo.
              </q-tooltip>
            </q-input>
          </q-btn-group>
        </div>

        <div class="col-12">
          <q-list bordered separator v-if="produto.unidadeCompra && produto.menorUnidade">
            <q-item-label header class="row justify-between items-center">
              <strong>Unidades de Venda</strong>
              <q-btn
                label="Adicionar Unidade"
                @click="adicionarUnidade()"
                color="primary"
                :disable="!produto.nome.trim() || !produto.unidadeCompra || !produto.menorUnidade || unidadeEmEdicao"
              />
            </q-item-label>
            <q-item v-for="(unidade, index) in produto.unidadesVenda" :key="index">
              <div class="row full-width items-center">
                <span>Preço Padrão:&nbsp;</span>
  
                <div v-if="unidade.editing">
                  <CurrencyInput
                    class="input-size-content"
                    v-model="unidade.precoPadrao"
                  />
                </div>
                <strong v-else>R$ {{ formatarPreco(unidade.precoPadrao) }}</strong>
  
                <span>&nbsp;| 1&nbsp;</span>
  
                <div v-if="unidade.editing && unidade.origem.id !== produto.unidadeCompra.id && unidade.origem.id !== produto.menorUnidade.id">
                  <q-select
                    class="input-size-content unidade-select"
                    v-model="unidade.origem"
                    :options="opcoesUnidades"
                    option-value="id"
                    option-label="nome"
                    dense
                    rounded
                    outlined
                    :error="unidade.origem.id == 0"
                  />
                </div>
                <strong v-else>{{ unidade.origem.nome }} ({{ unidade.origem.sigla }})</strong>
  
                <div class="row items-center" v-if="unidade.origem.id !== produto.menorUnidade.id">
                  <span>&nbsp;contém&nbsp;</span>
    
                  <div v-if="unidade.editing">
                    <q-input
                      class="input-size-content"
                      v-model.number="unidade.fator"
                      type="number"
                      dense
                      rounded
                      outlined
                    />
                  </div>
                  <span v-else>{{ unidade.fator }}</span>
                  
                  <strong>&nbsp;{{ produto.menorUnidade.nome }} ({{ produto.menorUnidade.sigla }})</strong>
                </div>
  
                <div class="col" align="right">
                  <q-btn
                    :icon="unidade.editing ? 'save' : 'edit'"
                    :color="unidade.editing ? 'green' : 'primary'"
                    dense
                    flat
                    @click="salvarEditarUnidade(unidade)"
                  />
                  <q-btn
                    v-if="unidade.editing"
                    icon="close"
                    color="red"
                    dense
                    flat
                    @click="restaurarValoresOriginais(index)"
                  />
                  <q-btn
                    v-if="!unidade.editing"
                    icon="delete"
                    color="red"
                    dense
                    flat
                    @click="removerUnidade(index)"
                    :disable="(produto.unidadesVenda[index]?.origem?.id === produto.unidadeCompra?.id && produto.unidadesVenda[index]?.destino?.id === produto.menorUnidade?.id) || (produto.unidadesVenda[index]?.origem?.id === produto.menorUnidade?.id)"
                  />
                </div>
              </div>
            </q-item>
          </q-list>
        </div>

        <div class="col-12">
          <q-list bordered separator v-if="produto.unidadeCompra && produto.menorUnidade">
            <q-item-label header><strong>Quantidade por Unidade de Venda</strong></q-item-label>
            <q-item v-for="(info, index) in informacoesQuantidades" :key="index">
              <div class="row full-width items-center">
                <span>{{ info.unidade }}: <span :class="{ 'text-negative': produto.quantidade < 0 }">{{ formatarQuantidade(info.quantidade) }}</span></span>
              </div>
            </q-item>
          </q-list>
        </div>

        <div class="col-3">
          <q-toggle v-model="produto.status" :label="produto.status ? 'Status: Ativo' : 'Status: Inativo'" :disable="props.idProduto == null" />
        </div>

        <div class="col flex justify-end">
          <q-btn
            type="submit"
            label="Salvar Produto"
            color="primary"
            :disable="!produto.nome.trim() || !produto.unidadeCompra || !produto.menorUnidade || unidadeEmEdicao"
          />
        </div>
      </q-form>
    </q-card-section>
  </q-card>

  <ConfirmDialog
    v-model="confirmDialogVisible"
    :mensagem="'Tem certeza que deseja alterar a ' + (unidadeAlterada === 'unidadeCompra' ? 'Unidade de Compra' : 'Menor Unidade de Venda') + '?'"
    @isConfirmado="confirmarAlteracaoUnidade"
  />
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { api } from 'src/boot/axios';
import { Notify } from 'quasar';
import { Produto, ProdutoCreateDto, UnidadeMedida, UnidadeConversao, ApiRequest, ApiResponse } from './models';
import ConfirmDialog from './ConfirmDialogComponent.vue';
import CurrencyInput from './CurrencyInput.vue';

const props = defineProps<{ idProduto: number | null }>();
const emit = defineEmits(['atualizarLista', 'fecharDialog']);

const requestUnidades = ref<ApiRequest<{ nome?: string; sigla?: string; status?: boolean, searchType: string }>>({
  filter: { nome: '', sigla: '', status: true, searchType: 'contains' },
  page: 1,
  pageSize: 10,
  sortBy: 'id',
  sortDesc: false
});

const responseUnidades = ref<ApiResponse<UnidadeMedida>>({
  data: [],
  totalRecords: 0,
  page: 1,
  pageSize: 10,
  totalPages: 1
});

const paginationUnidades = ref({
  page: 1,
  rowsPerPage: 10,
  rowsNumber: 0
});

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

const editingUnidadeConversao = ref<UnidadeConversao>({ 
  id: null,
  origem: {
    id: 0,
    nome: 'Selecione',
    sigla: '',
    status: true
  },
  destino: {
    id: 0,
    nome: 'Selecione',
    sigla: '',
    status: true
  },
  fator: 1 ,
  precoPadrao: 0,
  original: null,
});

const opcoesUnidades = ref<UnidadeMedida[]>([]);
const unidadeTemporaria = ref<UnidadeMedida | null>(null);
const confirmDialogVisible = ref(false);
const carregandoProduto = ref(false);
const unidadeAlterada = ref<null | 'unidadeCompra' | 'menorUnidade'>(null);
const quantidadeEditavel = ref(false);

const unidadeEmEdicao = computed(() => {
  return produto.value.unidadesVenda.some(unidade => unidade.editing);
});

const habilitarEdicaoQuantidade = () => {
  if (produto.value.id !== null) {
    quantidadeEditavel.value = true;
  }
};

const informacoesQuantidades = computed(() => {
  let restante = produto.value.quantidade;
  const resultados: { unidade: string; quantidade: number }[] = [];

  // Ordenar as unidades de venda pelo fator (do maior para o menor)
  const unidadesOrdenadas = [...produto.value.unidadesVenda].sort((a, b) => b.fator - a.fator);

  for (const unidade of unidadesOrdenadas) {
    // Calcular a quantidade para a unidade atual
    const quantidade = Math.trunc(restante / unidade.fator);
    restante = restante - quantidade * unidade.fator;

    resultados.push({
      unidade: `${unidade.origem.nome} (${unidade.origem.sigla})`,
      quantidade
    });
  }

  // Adicionar o restante na menor unidade
  if (restante !== 0) {
    resultados.push({
      unidade: `${produto.value.menorUnidade?.nome} (${produto.value.menorUnidade?.sigla})`,
      quantidade: restante
    });
  }

  return resultados;
});

const formatarQuantidade = (quantidade: number): string => {
  return quantidade.toLocaleString('pt-BR');
};

const salvarEditarUnidade = (unidade: UnidadeConversao) => {
  if (unidade.origem.id == 0) {
    Notify.create({
      message: 'Por favor, selecione uma unidade válida.',
      color: 'negative'
    });
    return;
  }

  if (!unidade.editing) {
    // Salvar os valores originais antes de habilitar a edição
    unidade.original = {
      id: unidade.id,
      origem: { ...unidade.origem },
      destino: { ...unidade.destino },
      fator: unidade.fator,
      precoPadrao: unidade.precoPadrao,
      original: null
    };
  }
  
  unidade.editing = !unidade.editing;

  // Ordenar as unidades de venda do maior fator para o menor
  produto.value.unidadesVenda.sort((a, b) => b.fator - a.fator);
};

const removerUnidade = (index: number) => {
  produto.value.unidadesVenda.splice(index, 1);
};

const adicionarUnidade = () => {
  const novaUnidade = {
    id: editingUnidadeConversao.value.id,
    origem: { ...editingUnidadeConversao.value.origem },
    destino: {
      id: produto.value.menorUnidade?.id ?? null,
      nome: produto.value.menorUnidade?.nome ?? '',
      sigla: produto.value.menorUnidade?.sigla ?? '',
      status: true
    },
    fator: editingUnidadeConversao.value.fator,
    precoPadrao: editingUnidadeConversao.value.precoPadrao,
    editing: true,
    original: null // Inicializar como null, pois não há valores originais ainda
  };

  adicionarOuAtualizarUnidade(novaUnidade);

  // Resetar o estado de edição
  editingUnidadeConversao.value = {
    id: null,
    origem: { id: 0, nome: 'Selecione', sigla: '', status: true },
    destino: { id: 0, nome: 'Selecione', sigla: '', status: true },
    fator: 1,
    precoPadrao: 0,
    editing: true,
    original: null // Inicializar como null, pois não há valores originais ainda
  };
};

const atualizarUnidadesVenda = () => {
  if (carregandoProduto.value) return;

  if (produto.value.unidadeCompra && produto.value.menorUnidade) {
    // Remover unidades antigas que não correspondem mais à unidade de compra ou menor unidade
    produto.value.unidadesVenda = produto.value.unidadesVenda.filter(unidade => {
      return (
        (unidade.origem.id === produto.value.unidadeCompra?.id && unidade.destino.id === produto.value.menorUnidade?.id) ||
        (unidade.origem.id === produto.value.menorUnidade?.id && unidade.destino.id === produto.value.menorUnidade?.id)
      );
    });

    // Unidade de Venda 1: Unidade de Compra -> Menor Unidade
    const unidadeVenda1 = {
      id: null,
      origem: {
        id: produto.value.unidadeCompra.id,
        nome: produto.value.unidadeCompra.nome,
        sigla: produto.value.unidadeCompra.sigla,
        status: true
      },
      destino: {
        id: produto.value.menorUnidade.id,
        nome: produto.value.menorUnidade.nome,
        sigla: produto.value.menorUnidade.sigla,
        status: true
      },
      fator: 1,
      precoPadrao: 0,
      editing: true,
      original: null // Inicializar como null, pois não há valores originais ainda
    };

    // Unidade de Venda 2: Menor Unidade -> Menor Unidade
    const unidadeVenda2 = {
      id: null,
      origem: {
        id: produto.value.menorUnidade.id,
        nome: produto.value.menorUnidade.nome,
        sigla: produto.value.menorUnidade.sigla,
        status: true
      },
      destino: {
        id: produto.value.menorUnidade.id,
        nome: produto.value.menorUnidade.nome,
        sigla: produto.value.menorUnidade.sigla,
        status: true
      },
      fator: 1,
      precoPadrao: 0,
      editing: true,
      original: null // Inicializar como null, pois não há valores originais ainda
    };

    // Adicionar ou substituir as unidades de venda
    adicionarOuAtualizarUnidade(unidadeVenda1);
    adicionarOuAtualizarUnidade(unidadeVenda2);
  }
};

const adicionarOuAtualizarUnidade = (novaUnidade: UnidadeConversao) => {
  // Verificar se já existe uma unidade de venda igual
  const unidadeExistenteIndex = produto.value.unidadesVenda.findIndex(
    u => u.origem.id === novaUnidade.origem.id && u.destino.id === novaUnidade.destino.id
  );

  if (unidadeExistenteIndex !== -1) {
    // Atualizar a unidade existente
    produto.value.unidadesVenda.splice(unidadeExistenteIndex, 1, {
      ...novaUnidade,
      original: {
        id: produto.value.unidadesVenda[unidadeExistenteIndex]?.id ?? null,
        origem: produto.value.unidadesVenda[unidadeExistenteIndex]?.origem ?? { id: null, nome: '', sigla: '', status: true },
        destino: produto.value.unidadesVenda[unidadeExistenteIndex]?.destino ?? { id: null, nome: '', sigla: '', status: true },
        fator: produto.value.unidadesVenda[unidadeExistenteIndex]?.fator ?? 1,
        precoPadrao: produto.value.unidadesVenda[unidadeExistenteIndex]?.precoPadrao ?? 0,
        editing: produto.value.unidadesVenda[unidadeExistenteIndex]?.editing ?? false,
        original: produto.value.unidadesVenda[unidadeExistenteIndex]?.original ?? null
      }, // Salvar os valores originais
      editing: true // Certifique-se de que a unidade não está em edição ao atualizar
    });
  } else {
    // Adicionar nova unidade
    produto.value.unidadesVenda.push({
      ...novaUnidade,
      original: { ...novaUnidade }, // Salvar os valores originais
      editing: true // Inicializar como não editando
    });
  }

  // Ordenar as unidades de venda do maior fator para o menor
  produto.value.unidadesVenda.sort((a, b) => b.fator - a.fator);
};

const restaurarValoresOriginais = (index: number) => {
  const unidade = produto.value.unidadesVenda[index];
  if (unidade?.original) {
    produto.value.unidadesVenda[index] = {
      ...unidade.original,
      editing: false // Sair do modo de edição
    };
  }
};

const onUnidadeCompraChange = (novaUnidadeCompra: UnidadeMedida) => {
  if (produto.value.id !== null) {
    // Exibir diálogo de confirmação durante a edição
    unidadeTemporaria.value = novaUnidadeCompra;
    unidadeAlterada.value = 'unidadeCompra';
    confirmDialogVisible.value = true;
    //editingUnidadeConversao.value.origem = novaUnidadeCompra;
  } else {
    // Alterar diretamente durante o cadastro
    produto.value.unidadeCompra = novaUnidadeCompra;
    atualizarUnidadesVenda();
  }
};

const onMenorUnidadeChange = (novaMenorUnidade: UnidadeMedida) => {
  if (produto.value.id !== null) {
    // Exibir diálogo de confirmação durante a edição
    unidadeTemporaria.value = novaMenorUnidade;
    unidadeAlterada.value = 'menorUnidade';
    confirmDialogVisible.value = true;
    //editingUnidadeConversao.value.destino = novaMenorUnidade;
  } else {
    // Alterar diretamente durante o cadastro
    produto.value.menorUnidade = novaMenorUnidade;
    atualizarUnidadesVenda();
  }
};

const confirmarAlteracaoUnidade = (isConfirmado: boolean) => {
  if (isConfirmado) {
    if (unidadeAlterada.value === 'unidadeCompra') {
      produto.value.unidadeCompra = unidadeTemporaria.value;
    } else if (unidadeAlterada.value === 'menorUnidade') {
      produto.value.menorUnidade = unidadeTemporaria.value;
    }
    atualizarUnidadesVenda();
    confirmDialogVisible.value = false;
    unidadeTemporaria.value = null;
    unidadeAlterada.value = null;
  } else {
    confirmDialogVisible.value = false
  }
};

const carregarProduto = async (id: number) => {
  try {
    carregandoProduto.value = true;
    const { data } = await api.get<Produto>(`/produtos/${id}`);
    produto.value = data;
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar produto!',
      color: 'negative'
    });
  } finally {
    carregandoProduto.value = false;
  }
};

const salvarProduto = async () => {
  const protudoDto = {
    id: produto.value.id,
    nome: produto.value.nome,
    descricao: produto.value.descricao,
    quantidade: produto.value.quantidade,
    status: produto.value.status,
    unidadeCompraId: produto.value.unidadeCompra?.id,
    menorUnidadeId: produto.value.menorUnidade?.id,
    unidadesVenda: produto.value.unidadesVenda.map(u => ({
      id: u.id,
      unidadeMedidaId: u.origem.id,
      menorUnidadeId: u.destino.id,
      fator: u.fator,
      precoPadrao: u.precoPadrao
    }))
  } as ProdutoCreateDto;

  if (protudoDto.id === null) {
    // Salvar novo produto
    try {
      console.log('Produto DTO:', protudoDto);
      await api.post('/produtos', protudoDto).then((response) => {
        if (response.status === 201) {
          Notify.create({message: 'Cadastro salvo com sucesso', color: 'positive' });
          emit('atualizarLista');
          emit('fecharDialog');
        }
      });
    } catch (error) {
      Notify.create({ 
        message: 'Erro ao salvar cadastro',
        color: 'negative'
      });
    }
  } else {
    // Atualizar produto
    try {
      await api.put(`/produtos/${protudoDto.id}`, protudoDto).then((response) => {
        if (response.status === 204) {
          Notify.create({message: 'Cadastro atualizado com sucesso', color: 'positive' });
          emit('atualizarLista');
          emit('fecharDialog');
        }
      });
    } catch (error) {
      Notify.create({ 
        message: 'Erro ao atualizar cadastro',
        color: 'negative'
      });
    }
  }
};

const formatarPreco = (valor: number) => {
  return valor.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
};

const carregarUnidades = async () => {
  try {
    const { data } = await api.post<ApiResponse<UnidadeMedida>>('/unidades/lista', requestUnidades.value);

    responseUnidades.value = data;
    paginationUnidades.value.rowsNumber = data.totalRecords;

    // Adicionar as novas unidades à lista de opções
    data.data.forEach((unidade) => {
      if (!opcoesUnidades.value.some(u => u.id === unidade.id)) {
        opcoesUnidades.value.push(unidade);
      }
    });

    // Incrementar a página para a próxima consulta
    if (paginationUnidades.value.page < data.totalPages) {
      paginationUnidades.value.page++;
    } else {
      // Desabilitar a consulta se não houver mais páginas
      paginationUnidades.value.page = -1;
    }
  } catch (error) {
    Notify.create({
      message: 'Erro ao carregar unidades!',
      color: 'negative'
    });
  }
};

const handleScroll = (event: Event) => {
  const target = event.target as HTMLElement;
  if (target.scrollHeight - target.scrollTop === target.clientHeight) {
    if (paginationUnidades.value.page !== -1) {
      carregarUnidades();
    }
  }
};

carregarUnidades();

onMounted(() => {
  if (props.idProduto) {
    carregarProduto(props.idProduto);
  }
});
</script>
