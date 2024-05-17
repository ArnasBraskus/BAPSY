import { apiDoGet, apiDoPost } from './api.js';

export async function getSession(id) {
  const res = await apiDoGet(`/api/sessions/get/${id}`);
  const data = await res.json();

  return data;
}

export async function getSessionNoAuth(id, token) {
  const res = await apiDoGet(`/api/sessions/get_noauth/${id}?token=${token}`);
  const data = await res.json();

  return data;
}

export async function getSessions(planId) {
  const res = await apiDoGet(`/api/sessions/list/${planId}`);
  const data = await res.json();

  return data.sessions;
}

export async function markSession(id, pagesRead) {
  const res = await apiDoPost('/api/sessions/mark', {
    id: id,
    pagesRead: pagesRead
  });

  const data = await res.json();

  return data;
}

export async function markSessionNoAuth(id, pagesRead, token) {
  const res = await apiDoPost('/api/sessions/mark_noauth', {
    sessionId: id,
    pagesRead: pagesRead,
    token: token
  });

  const data = await res.json();

  return data;
}
