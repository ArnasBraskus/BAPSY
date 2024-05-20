import { notify } from './notifications.js';
import { getCookie } from './cookies.js';

const IMAGE_UPLOAD_ENDPOINT = '/api/resources/upload_image'
const IMAGE_GET_ENDPOINT = '/api/resources/image'

export async function uploadImage(file) {
  const token = getCookie('api-token');

  const response = await fetch(IMAGE_UPLOAD_ENDPOINT, {
    method: 'POST',
    headers: {
        'Content-type': 'multipart/form-data',
        'Authorization': `Bearer ${token}`
    },
    body: await file.arrayBuffer()
  });

  if (!response.ok) {
    notify('Failed to upload image!');
  }

  return response.json();
}

export function getImage(image) {
  return `${window.location.origin}/api/resources/image/${image}`;
}
