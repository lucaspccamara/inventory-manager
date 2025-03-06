import type { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    component: () => import('layouts/MainLayout.vue'),
    children: [
      { path: '', component: () => import('pages/ResumoPage.vue') },
      { path: 'produtos', component: () => import('pages/ProdutosPage.vue') },
      { path: 'clientes', component: () => import('pages/ClientesFornecedoresPage.vue') },
      { path: 'entradas', component: () => import('pages/EntradasPage.vue') },
      { path: 'saidas', component: () => import('pages/SaidasPage.vue') },
      { path: 'relatorios', component: () => import('pages/RelatoriosPage.vue') },
      { path: 'configuracoes', component: () => import('pages/ConfiguracoesPage.vue') }
    ],
  }
];

export default routes;
