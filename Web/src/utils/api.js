import { getCookie } from './cookies.js';
import { notify } from './notifications.js';

function checkResponse(response) {
  if (!response.ok) {
    notify('Couldn\'t reach backend server');
  }
}

export async function apiDoGetUnauthenticated(endpoint) {
  const response = await fetch(endpoint, {
      method: 'GET'
  });

  checkResponse(response);

  return response;
}

export async function apiDoPostUnauthenticated(endpoint, data) {
  const response = await fetch(endpoint, {
    method: 'POST',
    body: JSON.stringify(data),
    headers: {
      'Content-type': 'application/json; charset=UTF-8'
    }
  });

  checkResponse(response);

  return response;
}

export async function apiDoPost(endpoint, data) {
  const token = getCookie('api-token');

  const response = await fetch(endpoint, {
    method: 'POST',
    body: JSON.stringify(data),
    headers: {
      'Content-type': 'application/json; charset=UTF-8',
      'Authorization': `Bearer ${token}`
    }
  });

  checkResponse(response);

  return response;
}

export async function apiDoGet(endpoint) {
  const token = getCookie('api-token');

  const response = await fetch(endpoint, {
      method: 'GET',
      headers: {
        'Authorization': `Bearer ${token}`
      }
  });

  checkResponse(response);

  return response;
}
