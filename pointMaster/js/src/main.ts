import { createApp } from "vue";
import HelloWorld from "./components/HelloWorld.vue";
import Stats from "./components/Stats.vue";

const app = createApp({});

app.component("hello-world", Stats);

(window as any).app = app;

app.mount("#app");