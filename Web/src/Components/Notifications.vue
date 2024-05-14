<script setup>

import { ref, } from 'vue';
import { useNotifications } from '../utils/notifications.js';

const TOAST_ANIM_ENTER = 'toast--anim-enter';
const TOAST_ANIM_ACTIVE = 'toast--anim-enter-active';
const TOAST_ANIM_DONE = 'toast--anim-enter-done';

const TOAST_CLASS_ERROR = '_error';
const TOAST_CLASS_WARNING = '_warning';
const TOAST_CLASS_INFO = '_success';

var toastMessage = ref('');
var toastTypeClass = ref(TOAST_CLASS_ERROR);
var toastAnimTypeClass = ref(TOAST_ANIM_ENTER);

var notifications = useNotifications();

notifications.value = (message, type) => {

  switch (type) {
  case 'error':
    toastTypeClass.value = TOAST_CLASS_ERROR;
    break;
  case 'warning':
    toastTypeClass.value = TOAST_CLASS_WARNING;
    break;
  case 'info':
    toastTypeClass.value = TOAST_CLASS_INFO;
  }

  toastMessage.value = message;
  toastAnimTypeClass.value = TOAST_ANIM_ACTIVE;

  setTimeout(() => {
    toastAnimTypeClass.value = TOAST_ANIM_DONE;

    setTimeout(() => {
      toastAnimTypeClass.value = TOAST_ANIM_ENTER;
    }, 3000);

  }, 500);
};

function close() {
  toastAnimTypeClass.value = TOAST_ANIM_ENTER;
}

</script setup>

<template>
  <div class="shared-toast-container">
    <div :class="[ 'shared-toast-wrapper', toastAnimTypeClass ]">
      <div :class="[ 'toast', toastTypeClass ]" @click="close">
        <div class="height-auto" aria-hidden="false">
          <div class="toast_content" style="transition: opacity 250ms ease 0ms;">
            <div class="_icon">
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
                <g fill="currentColor"><path fill="none" d="M12,4a8,8,0,1,0,8,8A8,8,0,0,0,12,4Zm0,14a1,1,0,1,1,1-1A1,1,0,0,1,12,18Zm1-4a1,1,0,0,1-2,0V7a1,1,0,0,1,2,0Z"></path><circle cx="12" cy="17" r="1"></circle><path d="M12,2A10,10,0,1,0,22,12,10,10,0,0,0,12,2Zm0,18a8,8,0,1,1,8-8A8,8,0,0,1,12,20Z"></path><path d="M12,6a1,1,0,0,0-1,1v7a1,1,0,0,0,2,0V7A1,1,0,0,0,12,6Z"></path></g>
              </svg>
            </div>
            <div class="_text-content">
              <div class="_content">{{ toastMessage }}</div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.height-auto {
  height: auto;
}
.shared-toast-container {
  position: fixed;
  right: 0;
  left: 0;
  top: 25px;
  pointer-events: none;
  z-index: 999999;

  max-height: 100px;

  > span {
    display: flex;
    flex-direction: column-reverse;
  }

  .rah-animating {
    transition: all 250ms ease-in-out;
  }
}
.shared-toast-wrapper {
  max-height: 200px;
  margin: 0 auto 14px auto;
  max-width: 650px;
  padding: 0 30px;
  width: 100%;
  transition: all 250ms ease;

  .toast {
    pointer-events: all;
    border-radius: 4px;
    overflow: hidden;
    background: #F1E4C3;
    border-radius: 4px;
    box-shadow: 0 22px 34px 0 rgba(0,16,34,.1);

    .toast_content {
      width: 100%;
      min-height: 58px;
      cursor: pointer;
      display: flex;
      flex-direction: row;

      ._icon {
        color: #ffffff;
        min-height: 60px;
        width: 60px;
        padding: 18px;
        position: relative;
        flex-shrink: 0;
        border-radius: 4px 0 0 4px;
        overflow: hidden;
        display: flex;
        justify-content: center;
        align-items: center;

        /* Fallback color */
        /*background: #ffa400; */

        svg,
        img {
          width: 36px;
          height: 36px;
          object-fit: contain;
          object-position: center;
        }
      }

      > ._text-content {
        display: flex;
        justify-content: center;
        align-items: center;
        padding: 12px;
        overflow: hidden;
        width: 100%;

        > ._content {
          font-style: normal;
          font-weight: normal;
          font-size: 18px;
          line-height: 18px;
          letter-spacing: 0.1px;
          color: #000000;
        }
      }
    }

    &._error ._icon {
      background: #f8423a;
    }

    &._warning ._icon {
      background: #ffa400;
    }

    &._success ._icon {
      background: #00ae42;
    }
  }

  &.toast--anim-enter {
    opacity: 0;
    margin-bottom: 0;
  }

  &.toast--anim-enter-active {
    opacity: 1;
    margin-bottom: 14px;
  }

  &.toast--anim-exit {
    opacity: 1;
  }

  &.toast--anim-exit-active {
    opacity: 0;
    margin-bottom: 0;
  }
}
</style>
