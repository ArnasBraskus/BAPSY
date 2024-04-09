<script>
import { ModalsContainer, useModal } from 'vue-final-modal';
import ModalConfirmPlainCss from '../Components/ModalConfirmPlainCss.vue';
import { RouterView } from 'vue-router';
import Header from '../Components/Header.vue';
import Footer from '../Components/Footer.vue';
import { getPlans, addPlan, removePlan, editPlan, getPlan } from '../utils/plans.js';
import { logout } from '../utils/auth.js';
import { apiDoGet } from '../utils/api.js'
import router from '../router';


const componentOptions = {
  components: {
    ModalsContainer,
    RouterView,
    Header,
    Footer
  },
  data() {
    return {
      plans: null,
      selectedPlan: null,
      email : ''
    };
  },
  mounted() {
    this.fetchPlans();
    this.fetchUserProfile();
  },
  methods: {
    doLogout(){
      logout();
      router.push({path: '/'});
    },
    async fetchUserProfile() {
      try {
        const response = await apiDoGet('/api/user/profile');
        const data = await response.json();
        this.email = data.email;
      } catch (error) {
        console.error('Error fetching user profile:', error);
      }
    },
    removePlan(planId) {
        removePlan(planId)
        .then(() => {
          this.plans = this.plans.filter(plan => plan.id !== planId);
        })
        .catch(error => {
          console.error('Error removing plan:', error);
        });
    },
    async fetchPlans() {
      try {
        this.plans = await getPlans();
      } catch (error) {
        console.error('Error fetching plans:', error);
      }
    },
    async showModal() {
    const modal = useModal({
    component: ModalConfirmPlainCss,
        attrs: {
            title: 'Plan',
            onConfirm: () => {
            modal.close();
        }
    }
    });
        modal.open();
    },
    showEditModal(plan) {
      const modal = useModal({
        component: ModalConfirmPlainCss,
      attrs: {
        title: 'Edit Plan',
        plan: plan,
        onConfirm: () => {
          modal.close();
         }
        }
      });
      modal.open();
    }
  }
};

export default componentOptions;
</script>

<template>
  <div>
    <header class="header">
      <div class="logo"></div>
      <nav>
        <RouterLink to="/plan">Plan</RouterLink>
        <RouterLink to="/books">Books</RouterLink>
        <RouterLink to="/settings">Settings</RouterLink>
        {{ email }}
        <button @click="doLogout()">Logout</button>
      </nav>
    </header>
    <div id="app" class="plan-background">
      <main>
        <div>
          <div>
            <h1 class="plan-h1">Plan management</h1>
          </div>
          <div>
            <button class="cta-button" @click="showModal">Create a Plan</button>
          <div v-if="plans && plans.length > 0">
              <select v-model="selectedPlan" style="float: left; margin-right: 10px;">
              <option value="">Select a plan</option>
              <option v-for="plan in plans" :key="plan.id" :value="plan">{{ plan.title }} - {{ plan.author }}</option>
              </select>
            <template v-if="selectedPlan">
              <h1 class="emoji-container">📚</h1>
              <div class="info-container">
                <div class="next-session"> Next Session: ???</div>
                <div class="deadline">Deadline: {{ selectedPlan.deadline }} </div>
              </div>
              <div class="progress-container"> Progress bar</div>
              <div class="progress-container">
                <div role="progressbar" aria-valuenow="67" aria-valuemin="0" aria-valuemax="100" style="--value: 67"></div>
              </div>
              <div class="progress-container">
                <button class="edit-button" @click="showEditModal(selectedPlan)">Edit</button>
                <button class="remove-button" @click="removePlan(selectedPlan.id)">Remove</button>
              </div>
            </template>
          </div>
          <ModalsContainer />
          <RouterView />
          </div>
        </div>
      </main>
      <Footer />
    </div>
  </div>
</template>

<style scoped>
@keyframes progress {
  0% { --percentage: 0; }
  100% { --percentage: var(--value); }
}

@property --percentage {
  syntax: '<number>';
  inherits: true;
  initial-value: 0;
}

.info-container {
  display: flex;
  justify-content: space-between;
}

.deadline {
  text-align: right;
}

.next-session {
  text-align: left;
}

.emoji-container {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-right: 120px;
  margin-bottom: 10px;
}

.progress-container {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 10px;
}


[role="progressbar"] {
  --percentage: var(--value);
  --primary: #369;
  --secondary: #adf;
  --size: 100px;
  animation: progress 2s 0.5s forwards;
  width: var(--size);
  aspect-ratio: 1;
  border-radius: 50%;
  position: relative;
  overflow: hidden;
  display: grid;
  place-items: center;
}

[role="progressbar"]::before {
  content: "";
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: conic-gradient(var(--primary) calc(var(--percentage) * 1%), var(--secondary) 0);
  mask: radial-gradient(white 55%, transparent 0);
  mask-mode: alpha;
  -webkit-mask: radial-gradient(#0000 55%, #000 0);
  -webkit-mask-mode: alpha;
}

[role="progressbar"]::after {
  counter-reset: percentage var(--value);
  content: counter(percentage) '%';
  font-family: Helvetica, Arial, sans-serif;
  font-size: calc(var(--size) / 5);
  color: var(--primary);
}

.plan-h1 {
  color: #000000;
}
    .home {
        text-align: center;
        color: var(--quaternary-color);
        padding: 20px;
        margin-top: 20px;
        background-color: rgba(185, 148, 112, 0.75);
        font-size: x-large;
        border-radius: 50px 50px 50px 50px;
    }
</style>