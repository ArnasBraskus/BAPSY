import { apiDoPost } from './api.js';

export async function markCompleted(planId, sessionId) {
  const res = await apiDoPost('/api/mark-session-completed', {
    planId: planId,
    sessionId: sessionId,
  });
  return res.status == 200;
}

export async function markNotCompleted(planId, sessionId) {
  const res = await apiDoPost('/api/mark-session-not-completed', {
    planId: planId,
    sessionId: sessionId,
  });
  return res.status == 200;
}