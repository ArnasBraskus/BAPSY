<script>
import { ModalsContainer, useModal } from 'vue-final-modal';
import ModalConfirmPlainCss from '../Components/ModalConfirmPlainCss.vue';
import { RouterView } from 'vue-router';
import Header from '../Components/Header.vue';
import Footer from '../Components/Footer.vue';
import { getPlans, addPlan, removePlan, editPlan, getPlan } from '../utils/plans.js';

const componentOptions = {
  components: {
    ModalsContainer,
    RouterView,
    Header,
    Footer
  },
  data() {
    return {
      plans: null
    };
  },
  mounted() {
    this.fetchPlans();
  },
  methods: {
    removePlan(planId) {
        this.plans = removePlan(planId);
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
                <RouterLink to="/">Home</RouterLink>
                <RouterLink to="/about">About</RouterLink>
                <RouterLink to="/plan">Plan</RouterLink>
                <RouterLink to="/books">Books</RouterLink>
                <RouterLink to="/settings">Settings</RouterLink>
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
                                        <h2>{{ plan.title }} {{ plan.author }} <button class="remove-button" @click="removePlan(plan.id)">Remove</button> </h2>
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