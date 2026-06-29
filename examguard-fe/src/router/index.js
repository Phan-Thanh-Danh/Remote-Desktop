import { createRouter, createWebHistory } from 'vue-router';
import EnvironmentCheckView from '../views/EnvironmentCheckView.vue';
import FakeExamView from '../views/FakeExamView.vue';

const routes = [
  { path: '/', redirect: '/environment-check' },
  { path: '/environment-check', name: 'environment-check', component: EnvironmentCheckView },
  { path: '/fake-exam', name: 'fake-exam', component: FakeExamView }
];

const router = createRouter({
  history: createWebHistory(),
  routes
});

export default router;
