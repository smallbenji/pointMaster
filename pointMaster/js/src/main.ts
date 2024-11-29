import { createApp } from "vue";
import Stats from "./components/Stats.vue";

const app = createApp({});

app.component("stats", Stats);

(window as any).app = app;

app.mount("#app");
