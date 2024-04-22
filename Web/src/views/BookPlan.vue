<script>
import { ModalsContainer, useModal } from 'vue-final-modal';
import ModalConfirmPlainCss from '../Components/ModalConfirmPlainCss.vue';
import { RouterView } from 'vue-router';
import Header from '../Components/Header.vue';
import Footer from '../Components/Footer.vue';
import { getPlans, removePlan } from '../utils/plans.js';
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
              <div class="container">
                <div class="select">
                  <select v-model="selectedPlan">
                    <option value="">Select a plan</option>
                    <option v-for="plan in plans" :key="plan.id" :value="plan">{{ plan.title }} - {{ plan.author }}</option>
                  </select>
                </div>
              </div>
            <template v-if="selectedPlan">
              <div class="info-container">
                <p> {{ selectedPlan.sessions }} </p>
                <div class="deadline">Deadline: {{ selectedPlan.deadline }} </div>
              </div>
              <div class="progress-container"> Progress bar</div>
              <div class="progress-container">
                <div role="progressbar" aria-valuenow="67" aria-valuemin="0" aria-valuemax="100" style="--value: 67"></div>
              </div>
              <div class="progress-container">
                <button class="edit-button" style="--clr:white" @click="showEditModal(selectedPlan)"><span>Edit</span></button>
                <button class="remove-button" style="--clr:white" @click="removePlan(selectedPlan.id)"><span>Remove</span></button>
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

.edit-button {
  position: relative;
  background: #444;
  color: #fff;
  text-decoration: none;
  text-transform: uppercase;
  border: none;
  letter-spacing: 0.1rem;
  font-size: 1rem;
  padding: 1rem 3rem;
  transition: 0.2s;
}

.edit-button:hover {
  letter-spacing: 0.2rem;
  padding: 1.1rem 3.1rem;
  background: var(--clr);
  color: var(--clr);
  animation: box 3s infinite;
}

.edit-button::before {
  content: "";
  position: absolute;
  inset: 2px;
  background: #d67c0d;
}

.edit-button span {
  position: relative;
  z-index: 1;
}


.remove-button {
  position: relative;
  background: #444;
  color: #fff;
  text-decoration: none;
  text-transform: uppercase;
  border: none;
  letter-spacing: 0.1rem;
  font-size: 1rem;
  padding: 1rem 3rem;
  transition: 0.2s;
}

.remove-button:hover {
  letter-spacing: 0.2rem;
  padding: 1.1rem 3.1rem;
  background: var(--clr);
  color: var(--clr);
  animation: box 3s infinite;
}

.remove-button::before {
  content: "";
  position: absolute;
  inset: 2px;
  background: #FF0000;
}

.remove-button span {
  position: relative;
  z-index: 1;
}

@keyframes move {
  0% {
    transform: translateX(0);
  }
  50% {
    transform: translateX(5px);
  }
  100% {
    transform: translateX(0);
  }
}

@keyframes box {
  0% {
    box-shadow: #27272c;
  }
  50% {
    box-shadow: 0 0 25px var(--clr);
  }
  100% {
    box-shadow: #27272c;
  }
}



select {
  /* Reset Select */
  float: right;
  outline: 10px red;
  border: 0;
  box-shadow: none;
  /* Personalize */
  flex: 1;
  padding: 0 1em;
  color: #000000;
  background-color: var(#2c3e50);
  background-image: none;
  cursor: pointer;
}
/* Remove IE arrow */
select::-ms-expand {
  display: none;
}
/* Custom Select wrapper */
.select {
  position: relative;
  display: flex;
  width: 20em;
  height: 3em;
  border-radius: .25em;
  overflow: hidden;
}
/* Arrow */
.select::after {
  content: '\25BC';
  position: absolute;
  top: 0;
  right: 0;
  padding: 1em;
  background-color: #5F6F52;
  transition: .25s all ease;
  pointer-events: none;
}
/* Transition */
.select:hover::after {
  color: #f39c12;
}

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
  position : absolute;
  left : 85%;
  margin-top: 50px;
  text-align: right;
}

.next-session {
  position : absolute;
  left: 4%;
  margin-top: 50px;
  text-align: left;
}

.progress-container {
  display: flex;
  justify-content: center;
  align-items: center;
    margin-top: 50px;
}

.container {
  position : absolute;
  left : 85%;
  transform :
  translate(-50%,-50%); 
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
</style>