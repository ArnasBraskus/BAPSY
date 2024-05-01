import { ref } from 'vue';

const _notify = ref((message) => { console.log(message);});

export function notify(message, type) {
  _notify.value(message, type);
}

export function useNotifications() {
  return _notify;
}
