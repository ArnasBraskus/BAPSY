import { apiDoGet } from './api.js';

export async function getUserProfile() {
  const res = await apiDoGet('/api/user/profile');
  const data = await res.json();

  return data;
}

