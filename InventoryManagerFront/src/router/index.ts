import { defineRouter } from '#q-app/wrappers';
import {
  createMemoryHistory,
  createRouter,
  createWebHashHistory,
  createWebHistory,
} from 'vue-router';
import routes from './routes';
import { apiStatus, checkApi } from 'boot/check-api'

export default defineRouter(function () {
  const createHistory = process.env.SERVER
    ? createMemoryHistory
    : (process.env.VUE_ROUTER_MODE === 'history' ? createWebHistory : createWebHashHistory);

  const Router = createRouter({
    scrollBehavior: () => ({ left: 0, top: 0 }),
    routes,
    history: createHistory(process.env.VUE_ROUTER_BASE),
  });

  let apiChecked = false

  Router.beforeEach(async (to, from, next) => {
    const isStartupPage = to.path === '/startup'
    
    if (!apiChecked && !isStartupPage) {
      apiStatus.value = 'checking'
      await checkApi()
      apiChecked = true
    }

    if (apiStatus.value === 'ready') {
      next()
    } else if (!isStartupPage) {
      next({ path: '/startup', query: { redirect: to.fullPath } })
    } else {
      next()
    }
  })

  return Router;
});
