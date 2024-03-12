export function setCookie(name, value, secs) {
  let expires = '';

  if (secs) {
    const date = new Date();
    date.setTime(date.getTime() + secs * 1000);
    expires = date.toUTCString();
  }

  document.cookie = `${name}=${value}; expires=${expires}; path=/`;
}

export function getCookie(name) {
  let ret = null;

  document.cookie.split('; ').forEach(part => {
    if (part.startsWith(name + '=')) {
      ret = part.substring(name.length + 1);
      return;
    }
  });

  return ret;
}

export function deleteCookie(name) {
  document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/`;
}
