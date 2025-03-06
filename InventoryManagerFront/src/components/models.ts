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

export interface UnidadesConversoes {
  id: number;
  origem: Unidades;
  destino: Unidades;
  fator: number;
}

export interface Unidades {
  id: number;
  nome: string;
  sigla: string;
}

export interface Produtos {
  id: number | null;
  nome: string;
  unidadeCompra: Unidades | null;
  unidadesVenda: UnidadesConversoes[];
  menorUnidade: Unidades | null;
}

export interface UnidadeMedida {
  id: number;
  nome: string;
  sigla: string;
  status: boolean;
}

export const StatusOpcoesBoolean = [
  { label: 'Inativo', value: false },
  { label: 'Ativo', value: true },
  { label: 'Todos', value: null }
];
