import { apiDoGet, } from './api.js';

export async function getEvents() {
  const res = await apiDoGet('/api/calendar/events');
  const data = await res.json();

  return data;
}
export async function getToken() {
  const res = await apiDoGet('/api/calendar/token');
  const data = await res.json();

  return data;
}
