//INTERFACES
export interface ApiRequest<T = any> {
  filter: T;
  page: number;
  pageSize: number;
  sortBy: string;
  sortDesc: boolean;
}

export interface ApiResponse<T = any> {
  data: T[];
  totalRecords: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ClienteFornecedor {
  id: number | null;
  nome: string;
  cpfCnpj: string;
  email: string;
  telefone: string;
  celular: string;
  endereco: string;
  tipo: TipoClienteFornecedorType;
  status: boolean;
}

export interface ClienteFornecedorSelectDto {
  id: number;
  nome: string;
}

export interface ClienteFornecedorTableDto {
  id: number;
  nome: string;
  cpfCnpj: string;
  email: string;
  contato: string;
  tipo: TipoClienteFornecedorType;
  status: boolean;
}

export interface Pedido {
  id: number | null;
  clienteFornecedor: ClienteFornecedorSelectDto | null;
  observacao: string;
  status: number;
  data: Date;
  total: number;
  itens: ProdutoPedidoDto[];
}

export interface PedidoCreateDto {
  id: number | null;
  clienteFornecedorId: number | null;
  status: number;
  observacao: string;
  data: Date;
  itens: ProdutoPedidoCreateDto[];
}

export interface PedidoUpdateDto {
  id: number | null;
  clienteFornecedorId: number | null;
  status: number;
  observacao: string;
  data: Date;
  itens: ProdutoPedidoCreateDto[];
  itensAdicionados: ProdutoPedidoCreateDto[];
  itensModificados: ProdutoPedidoCreateDto[];
  itensRemovidos: number[];
}

export interface PedidoTableDto {
  id: number;
  fornecedor: string;
  total: number;
  data: string;
  status: number;
}

export interface Produto {
  id: number | null;
  nome: string;
  descricao: string;
  quantidade: number;
  status: boolean;
  unidadeCompra: UnidadeMedida | null;
  unidadesVenda: UnidadeConversao[];
  menorUnidade: UnidadeMedida | null;
}

export interface ProdutoPedidoDto {
  id: number | null;
  pedidoId: number | null;
  produtoId: number;
  nome: string;
  quantidade: number;
  unidadesVenda: UnidadeConversao[] | null;
  unidadeVendaSelecionada: UnidadeMedida | null;
  precoUnitario: number;
  valorTotal: number;
}

export interface ProdutoPedidoCreateDto {
  id: number | null;
  pedidoId: number | null;
  produtoId: number;
  produtoUnidadeVendaId: number;
  fatorConversao: number;
  quantidade: number;
  precoUnitario: number;
}

export interface ProdutoTableDto {
  id: number;
  nome: string;
  quantidade: number;
  status: boolean;
}

export interface ProdutoCreateDto {
  id: number | null;
  nome: string;
  descricao: string;
  quantidade: number;
  status: boolean;
  unidadeCompraId: number;
  unidadesVenda: UnidadeConversaoCreateDto[];
  menorUnidadeId: number;
}

export interface UnidadeConversao {
  id: number | null;
  origem: UnidadeMedida;
  destino: UnidadeMedida;
  fator: number;
  precoPadrao: number;
  editing?: boolean;
  original: UnidadeConversao | null;
}

export interface UnidadeConversaoCreateDto {
  id: number | null;
  unidadeMedidaId: number;
  menorUnidadeId: number;
  fator: number;
  precoPadrao: number;
}

export interface UnidadeMedida {
  id: number | null;
  nome: string;
  sigla: string;
  status: boolean;
}

//ENUMS
export const SearchOptions = [
  { label: 'Exato', value: 'exact' },
  { label: 'Contém', value: 'contains' },
  { label: 'Começa com', value: 'startsWith' },
  { label: 'Termina com', value: 'endsWith' }
];

export const SearchOptionsClienteFornecedor = [
  {
    label: 'Nome',
    group: true, // Indica que é um cabeçalho de grupo
    value: ''
  },
  { label: 'Exato', value: 'exact' },
  { label: 'Contém', value: 'contains' },
  { label: 'Começa com', value: 'startsWith' },
  { label: 'Termina com', value: 'endsWith' },
  {
    label: 'Outros',
    group: true, // Indica que é um cabeçalho de grupo
    value: ''
  },
  { label: 'CPF/CNPJ', value: 'cpfCnpj' },
  { label: 'Email', value: 'email' },
  { label: 'Telefone/Celular', value: 'telefoneCelular' }
] as const;

export const StatusOpcoesBoolean = [
  { label: 'Inativo', value: false },
  { label: 'Ativo', value: true },
  { label: 'Todos', value: null }
];

export const StatusOpcoesEntrada = [
  { label: 'Compra', value: 4 },
  { label: 'Cancelado', value: 5 },
  { label: 'Todos', value: null }
]  as const;

export const StatusOpcoesSaida = [
  { label: 'Orçamento', value: 0 },
  { label: 'Venda', value: 2 },
  { label: 'Cancelado', value: 3 },
  { label: 'Todos', value: null }
]  as const;

export const StatusPedidoLabelColor = [
  { label: 'Orçamento', color: 'bg-yellow-1', value: 0 },                // Amarelo médio
  { label: 'Orçamento Cancelado', color: 'bg-yellow-2', value: 1 },      // Amarelo claro
  { label: 'Venda', color: 'bg-green-1', value: 2 },                    // Verde médio
  { label: 'Venda Cancelada', color: 'bg-green-2', value: 3 },          // Verde claro
  { label: 'Compra', color: 'bg-blue-1', value: 4 },                   // Azul médio
  { label: 'Compra Cancelada', color: 'bg-blue-2', value: 5 }          // Azul claro
] as const;

export const TipoClienteFornecedor = [
  { label: 'Cliente', value: 1 },
  { label: 'Fornecedor', value: 2 },
  { label: 'Cliente/Fornecedor', value: 3 },
  { label: 'Todos', value: null }
] as const;

export type SearchType = typeof SearchOptionsClienteFornecedor[number]['value'];

export type StatusOpcoesEntradaType = typeof StatusOpcoesEntrada[number]['value'];

export type StatusOpcoesSaidaType = typeof StatusOpcoesSaida[number]['value'];

export type TipoClienteFornecedorType = typeof TipoClienteFornecedor[number]['value'];