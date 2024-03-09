import { getCookie } from './cookies.js';

export async function apiDoPostUnauthenticated(endpoint, data) {
    return fetch(endpoint, {
        method: 'POST',
        body: JSON.stringify(data),
        headers: {
            'Content-type': 'application/json; charset=UTF-8'
        }
    });
}

export function apiDoPost(endpoint, data) {
    const token = getCookie('api-token');

    return fetch(endpoint, {
        method: 'POST',
        body: JSON.stringify(data),
        headers: {
            'Content-type': 'application/json; charset=UTF-8',
            'Authorization': `Bearer ${token}`
        }
    });
}

export async function apiDoGet(endpoint) {
    const token = getCookie('api-token');

    return fetch(endpoint, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`
        }
    });
}
