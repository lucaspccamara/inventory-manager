<template>
  <q-layout view="lHh Lpr lFf">
    <q-header elevated>
      <q-toolbar>
        <q-toolbar-title>
          Sistema de Estoque
        </q-toolbar-title>

        <q-tabs align="right">
          <q-route-tab to="/" label="Resumo" icon="dashboard" />
          <q-route-tab to="/produtos" label="Produtos" icon="inventory" />
          <q-route-tab to="/clientes" label="Clientes e Fornecedores" icon="people" />
          <q-route-tab to="/entradas" label="Entradas" icon="add_business" />
          <q-route-tab to="/saidas" label="Saídas" icon="shopping_cart_checkout" />
          <q-route-tab to="/relatorios" label="Relatórios" icon="bar_chart" />
          <q-route-tab to="/configuracoes" label="Configurações" icon="settings" />
        </q-tabs>
      </q-toolbar>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
