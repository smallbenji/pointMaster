<template>
    <div class="stat-dashboard">
        <div>
            <h1>Stats</h1>
            <div class="stats">
                <div class="stat" v-for="(data, index) in StatData" :key="index">
                    <p class="stat-title">{{ data.Title }}</p>
                    <p class="stat-value">{{ data.Value }}</p>
                </div>
            </div>
        </div>
        <div>
            <p class="stat-title">Point over tid</p>
            <div id="chart"></div>
        </div>
    </div>
</template>

<script lang="ts">
import ApexCharts, { ApexOptions } from 'apexcharts';
import axios from 'axios';
import { defineComponent, onMounted, ref } from 'vue';


export default defineComponent({
    name: "Stats",
    setup() {
        const StatData = ref<Stat[]>([]);

        const GetStats = async () => {
            const data = (await axios.get("/api/getstats")).data as Stat[]

            StatData.value = data;
        };

        const initializeChart = async () => {
            const options: ApexOptions = {
                chart: {
                    type: "line",
                    toolbar: {
                        show: false,
                    },
                },
                xaxis: {
                    type: "datetime"
                },
                series: []
            };

            const chart = new ApexCharts(document.querySelector("#chart"), options);
            chart.render();

            const data = (await axios.get("/api/GetPointsOverTime")).data;

            chart.updateOptions({
                series: data
            } as ApexOptions);
        };

        onMounted(async () => {
            await GetStats();
            await initializeChart();
        });

        return {
            StatData
        }
    }
})

</script>

<style lang="css">
.stat-dashboard {
    display: flex;
    height: 60vh;
    justify-content: space-between;
}

#chart {
    height: 300px;
    width: 500px;
}

.stats {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 2rem;
    max-width: 50vw;
}

.stat {
    background-color: rgba(0, 0, 0, 0.1);
    padding: 10px;
    height: 7rem;
    width: 10rem;
    border-radius: 5px;
    text-align: center;
    box-shadow: 0px 0px 5px rgba(0,0,0,0.1);
}

.stat-title {
    font-weight: 700;
    font-size: 1.25rem;
}

.stat .stat-value {
    font-size: 2rem;
}
</style>