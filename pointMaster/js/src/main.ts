import { createApp } from "vue";
import HelloWorld from "./components/HelloWorld.vue";
import Stats from "./components/Stats.vue";
import Test from "./components/Test.vue";

const app = createApp({});

app.component("stats", Stats);

(window as any).app = app;

app.mount("#app");
