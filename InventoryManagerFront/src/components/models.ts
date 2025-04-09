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

export interface ClienteFornecedorTableDto {
  id: number;
  nome: string;
  cpfCnpj: string;
  email: string;
  contato: string;
  tipo: TipoClienteFornecedorType;
  status: boolean;
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

export const StatusOpcoesBoolean = [
  { label: 'Inativo', value: false },
  { label: 'Ativo', value: true },
  { label: 'Todos', value: null }
];

export const TipoClienteFornecedor = [
  { label: 'Cliente', value: 1 },
  { label: 'Fornecedor', value: 2 },
  { label: 'Cliente/Fornecedor', value: 3 },
  { label: 'Todos', value: null }
] as const;

export type TipoClienteFornecedorType = typeof TipoClienteFornecedor[number]['value'];
