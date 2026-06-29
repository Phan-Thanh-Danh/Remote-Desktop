<script setup>
import { computed, onMounted, ref } from 'vue';
import { useRouter } from 'vue-router';
import {
  checkWithAgent,
  createSession,
  getSessionStatus
} from '../services/examGuardApi';

const router = useRouter();
const session = ref(null);
const status = ref(null);
const loading = ref(false);
const error = ref('');

const statusBadgeClass = computed(() => {
  switch (status.value?.status) {
    case 'Safe':
      return 'bg-green-100 text-green-800';
    case 'Unsafe':
      return 'bg-red-100 text-red-800';
    case 'Waiting':
      return 'bg-gray-100 text-gray-700';
    case 'Checking':
      return 'bg-blue-100 text-blue-800';
    default:
      return 'bg-slate-100 text-slate-700';
  }
});

const canEnterExam = computed(() => status.value?.safe === true);
const isNotCreated = computed(() => !session.value);

async function createGuardSession() {
  loading.value = true;
  error.value = '';

  try {
    const response = await createSession();
    session.value = response;
    status.value = {
      sessionId: response.sessionId,
      status: response.status,
      safe: response.safe,
      riskScore: response.riskScore,
      message: response.message,
      detectedApps: response.detectedApps || [],
      lastCheckedAt: response.lastCheckedAt
    };
  } catch (err) {
    error.value = err.message || 'Không thể tạo phiên kiểm tra.';
  } finally {
    loading.value = false;
  }
}

async function pollStatus() {
  if (!session.value?.sessionId) {
    error.value = 'Vui lòng tạo phiên kiểm tra trước.';
    return;
  }

  loading.value = true;
  error.value = '';

  try {
    const response = await getSessionStatus(session.value.sessionId);
    status.value = response;
  } catch (err) {
    error.value = err.message || 'Không thể lấy trạng thái.';
  } finally {
    loading.value = false;
  }
}

async function runAgentCheck() {
  if (!session.value?.sessionId) {
    error.value = 'Vui lòng tạo phiên kiểm tra trước.';
    return;
  }

  loading.value = true;
  error.value = '';

  try {
    await checkWithAgent(session.value.sessionId);
    status.value = await getSessionStatus(session.value.sessionId);
  } catch (err) {
    error.value = err.message || 'Không thể kiểm tra bằng ExamGuard Agent.';
  } finally {
    loading.value = false;
  }
}

function enterExam() {
  if (!canEnterExam.value) {
    return;
  }

  router.push({ name: 'fake-exam', query: { studentName: session.value?.studentName || 'Nguyễn Văn An' } });
}

onMounted(() => {
  createGuardSession();
});
</script>

<template>
  <div class="min-h-screen bg-slate-50 px-4 py-10 text-slate-800">
    <div class="mx-auto max-w-5xl space-y-6">
      <div class="rounded-2xl border border-slate-200 bg-white p-8 shadow-sm">
        <p class="text-sm font-semibold uppercase tracking-[0.3em] text-slate-500">ExamGuard Demo</p>
        <h1 class="mt-3 text-3xl font-semibold">Kiểm tra môi trường thi</h1>
        <p class="mt-3 max-w-2xl text-slate-600">
          Hệ thống cần xác nhận máy không chạy ứng dụng điều khiển từ xa trước khi vào phòng thi.
        </p>
      </div>

      <div class="grid gap-6 lg:grid-cols-[1.1fr_0.9fr]">
        <section class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div class="flex items-start justify-between gap-4">
            <div>
              <h2 class="text-xl font-semibold">Remote Desktop / Điều khiển từ xa</h2>
              <p class="mt-2 text-sm text-slate-600">Kiểm tra trạng thái bằng ExamGuard Agent local.</p>
            </div>
            <span v-if="status" :class="['rounded-full px-3 py-1 text-sm font-medium', statusBadgeClass]">
              {{ status.status }}
            </span>
            <span v-else class="rounded-full bg-slate-100 px-3 py-1 text-sm font-medium text-slate-700">NotCreated</span>
          </div>

          <div class="mt-6 space-y-4">
            <div class="grid gap-4 rounded-xl bg-slate-50 p-4 sm:grid-cols-2">
              <div>
                <p class="text-sm text-slate-500">Session ID</p>
                <p class="font-medium break-all">{{ session?.sessionId || 'Chưa tạo' }}</p>
              </div>
              <div>
                <p class="text-sm text-slate-500">Student</p>
                <p class="font-medium">{{ session?.studentName || 'Mock student' }}</p>
              </div>
            </div>

            <div class="rounded-xl border border-slate-200 p-4">
              <div class="flex items-center justify-between">
                <p class="text-sm text-slate-500">Risk score</p>
                <p class="text-2xl font-semibold">{{ status?.riskScore ?? 0 }}</p>
              </div>
              <p class="mt-2 text-sm text-slate-600">{{ status?.message || 'Chờ bắt đầu kiểm tra.' }}</p>
            </div>

            <div v-if="status?.detectedApps?.length" class="rounded-xl border border-red-200 bg-red-50 p-4">
              <p class="font-medium text-red-700">Ứng dụng phát hiện</p>
              <ul class="mt-2 list-disc space-y-1 pl-5 text-sm text-red-700">
                <li v-for="app in status.detectedApps" :key="app.name">
                  {{ app.name }} - {{ app.description }}
                </li>
              </ul>
            </div>
          </div>

          <div class="mt-6 flex flex-wrap gap-3">
            <button class="rounded-lg bg-slate-800 px-4 py-2 text-sm font-medium text-white hover:bg-slate-700" @click="createGuardSession">
              Tạo phiên kiểm tra
            </button>
            <button class="rounded-lg border border-emerald-600 px-4 py-2 text-sm font-medium text-emerald-700 hover:bg-emerald-50" @click="runAgentCheck">
              Kiểm tra bằng ExamGuard Agent
            </button>
            <button class="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-100" @click="pollStatus">
              Kiểm tra trạng thái
            </button>
            <button class="rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white disabled:cursor-not-allowed disabled:bg-blue-300" :disabled="!canEnterExam" @click="enterExam">
              Vào phòng thi
            </button>
          </div>
        </section>

        <aside class="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <h3 class="text-lg font-semibold">Hướng dẫn nhanh</h3>
          <ul class="mt-4 space-y-3 text-sm text-slate-600">
            <li>1. Tạo phiên kiểm tra để bắt đầu quy trình kiểm tra.</li>
            <li>2. Chạy ExamGuard Agent trên máy rồi bấm kiểm tra.</li>
            <li>3. Chỉ khi trạng thái Safe mới có thể vào phòng thi giả.</li>
          </ul>

          <div v-if="isNotCreated" class="mt-6 rounded-xl border border-slate-200 bg-slate-50 p-3 text-sm text-slate-600">
            Chưa có phiên kiểm tra. Nhấn “Tạo phiên kiểm tra” để bắt đầu.
          </div>
          <div v-if="error" class="mt-6 rounded-xl border border-red-200 bg-red-50 p-3 text-sm text-red-700">
            {{ error }}
          </div>
          <div v-if="loading" class="mt-6 text-sm text-slate-500">Đang xử lý...</div>
        </aside>
      </div>
    </div>
  </div>
</template>
