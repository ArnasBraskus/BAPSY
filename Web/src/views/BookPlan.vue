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
            <div id="app" class="image-container">
                <main class="darker ">
                    <div>
                        <div>
                            <div>
                                <h1>Plan management</h1>
                            </div>
                            <div>
                                <button class="cta-button" @click="showModal">
                                    Create a Plan
                                </button>
                                <div v-if="plans">
                                    <div v-for="plan in plans" :key="plan.id">
                                        <h2> {{ plan.title }} {{ plan.author }} <button class="edit-button" @click="showEditModal(plan)">Edit</button><button class="remove-button" @click="removePlan(plan.id)">Remove</button> </h2>
                                    </div>
                                </div>
                                
                                <ModalsContainer />
                            </div>
                        </div>
                        <RouterView />
                    </div>
                </main>
                <Footer />
            </div>
</div>
</template>

<style scoped>

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