<script setup>
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const studentName = computed(() => route.query.studentName || 'Nguyễn Văn An');
const timer = ref('00:45:00');
const selectedAnswer = ref('');

const questions = [
  {
    id: 1,
    text: 'Remote Desktop là gì?',
    options: ['Một công cụ dùng để điều khiển máy tính từ xa', 'Một trình duyệt web', 'Một phần mềm soạn thảo văn bản']
  },
  {
    id: 2,
    text: 'Ứng dụng nào sau đây cần bị phát hiện trong phòng thi?',
    options: ['AnyDesk.exe', 'Notepad.exe', 'Chrome.exe']
  }
];
</script>

<template>
  <div class="min-h-screen bg-slate-50 px-4 py-10 text-slate-800">
    <div class="mx-auto max-w-4xl rounded-2xl border border-slate-200 bg-white p-8 shadow-sm">
      <div class="flex flex-wrap items-center justify-between gap-4">
        <div>
          <p class="text-sm font-semibold uppercase tracking-[0.3em] text-slate-500">Fake Exam</p>
          <h1 class="mt-3 text-3xl font-semibold">Phòng thi giả lập</h1>
        </div>
        <div class="rounded-xl border border-amber-200 bg-amber-50 px-4 py-2 text-sm font-medium text-amber-700">
          Thời gian còn lại: {{ timer }}
        </div>
      </div>

      <p class="mt-3 text-slate-600">Trang này mô phỏng phòng thi cho demo kiểm tra môi trường.</p>

      <div class="mt-8 grid gap-4 rounded-xl border border-slate-200 bg-slate-50 p-5 sm:grid-cols-2">
        <div>
          <p class="text-sm text-slate-500">Sinh viên</p>
          <p class="mt-1 font-semibold">{{ studentName }}</p>
        </div>
        <div>
          <p class="text-sm text-slate-500">Thời gian</p>
          <p class="mt-1 font-semibold">09:00 - 10:00</p>
        </div>
      </div>

      <div class="mt-8 space-y-6">
        <div v-for="question in questions" :key="question.id" class="rounded-xl border border-slate-200 p-6">
          <h2 class="text-xl font-semibold">{{ question.text }}</h2>
          <div class="mt-4 space-y-2">
            <label v-for="option in question.options" :key="option" class="flex items-center gap-3 rounded-lg border border-slate-200 px-3 py-2">
              <input v-model="selectedAnswer" :value="option" type="radio" name="question-1" class="h-4 w-4" />
              <span>{{ option }}</span>
            </label>
          </div>
        </div>
      </div>

      <div class="mt-8 flex justify-end">
        <button class="rounded-lg bg-emerald-600 px-4 py-2 text-sm font-medium text-white hover:bg-emerald-700">
          Nộp bài
        </button>
      </div>
    </div>
  </div>
</template>
