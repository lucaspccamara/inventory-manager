<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-toolbar-title>
          Sistema de Estoque
        </q-toolbar-title>

        <q-tabs align="right">
          <q-route-tab to="/" label="Resumo" />
          <q-route-tab to="/produtos" label="Produtos" />
          <q-route-tab to="/clientes" label="Clientes e Fornecedores" />
          <q-route-tab to="/entradas" label="Entradas" />
          <q-route-tab to="/saidas" label="Saídas" />
          <q-route-tab to="/relatorios" label="Relatórios" />
          <q-route-tab to="/configuracoes" label="Configurações" />
        </q-tabs>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
