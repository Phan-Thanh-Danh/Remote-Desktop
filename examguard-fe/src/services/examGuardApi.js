const API_ORIGIN = 'http://localhost:5204';
const API_BASE_URL = `${API_ORIGIN}/api/exam-guard`;
const AGENT_BASE_URL = 'http://127.0.0.1:17891';

async function request(path, options = {}) {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: {
      'Content-Type': 'application/json',
      ...(options.headers || {})
    },
    ...options
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || 'Request failed');
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export async function createSession() {
  return request('/sessions', { method: 'POST' });
}

export async function getSessionStatus(sessionId) {
  return request(`/sessions/${sessionId}/status`);
}

export async function checkWithAgent(sessionId) {
  const response = await fetch(`${AGENT_BASE_URL}/check`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      sessionId,
      apiBaseUrl: API_ORIGIN
    })
  });

  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(errorText || 'ExamGuard Agent request failed');
  }

  return response.json();
}

export async function reportSession(sessionId, payload) {
  return request(`/sessions/${sessionId}/report`, {
    method: 'POST',
    body: JSON.stringify(payload)
  });
}

export async function heartbeat(sessionId, message) {
  return request(`/sessions/${sessionId}/heartbeat`, {
    method: 'POST',
    body: JSON.stringify({ message })
  });
}
