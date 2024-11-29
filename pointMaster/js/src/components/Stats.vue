<template>
    <div class="stat-dashboard">
        <div class="stat-panel">
            <h1>Statistikker</h1>
            <div class="stats">
                <div
                    class="stat"
                    v-for="(data, index) in StatData"
                    :key="index"
                >
                    <p class="stat-title">{{ data.Title }}</p>
                    <p class="stat-value">{{ data.Value }}</p>
                </div>
            </div>
        </div>
        <div class="stat-chart">
            <p class="stat-title">Point over tid</p>
            <div id="chart"></div>
            <p class="stat-title">Point ratio</p>
            <div id="pie"></div>
        </div>
    </div>
</template>

<script lang="ts">
import ApexCharts, { ApexOptions } from "apexcharts";
import axios from "axios";
import { defineComponent, onMounted, ref } from "vue";

export default defineComponent({
    name: "Stats",
    setup() {
        const StatData = ref<Stat[]>([]);

        const GetStats = async () => {
            const data = (await axios.get("/api/getstats")).data as Stat[];

            StatData.value = data;
        };

        const initializeChart = async () => {
            const options: ApexOptions = {
                chart: {
                    type: "line",
                    toolbar: {
                        show: true,
                    },
                },
                xaxis: {
                    type: "datetime",
                },
                series: [],
            };

            const chart = new ApexCharts(
                document.querySelector("#chart"),
                options
            );
            chart.render();

            const data = (await axios.get("/api/GetPointsOverTime")).data;

            chart.updateOptions({
                series: data,
            } as ApexOptions);
        };

        const initializePieChart = async () => {
            const options: ApexOptions = {
                chart: {
                    type: "pie",
                    toolbar: {
                        show: true,
                    },
                },
                series: [],
            };

            const chart = new ApexCharts(
                document.querySelector("#pie"),
                options
            );
            chart.render();

            const data = (await axios.get("/api/pointratio")).data;

            chart.updateOptions({
                series: data.data,
                labels: data.names,
            } as ApexOptions);
        };

        onMounted(async () => {
            await GetStats();
            await initializeChart();
            await initializePieChart();
        });

        return {
            StatData,
        };
    },
});
</script>

<style lang="scss">
.stat-dashboard {
    display: flex;
    justify-content: space-between;
    flex-direction: row;

    @media (max-width: 768px) {
        flex-direction: column;
    }
}

.stat-panel {
    @media (max-width: 768px) {
        margin-bottom: 2rem;
    }
}

.stat-chart {
    flex-grow: 1;
}

.stats {
    display: grid;
    grid-template-columns: repeat(4, 1fr);
    gap: 2rem;

    @media (max-width: 1000px) {
        grid-template-columns: repeat(2, 1fr);
    }

    .stat {
        background-color: rgba(0, 0, 0, 0.1);
        padding: 10px;
        height: 7rem;
        width: 10rem;
        border-radius: 5px;
        text-align: center;
        box-shadow: 0px 0px 5px rgba(0, 0, 0, 0.1);
        margin: auto;
    }

    &-title {
    font-weight: 700;
    font-size: 1.25rem;
    margin: 0;
    }

    &-value {
    font-weight: 400;
    font-size: 1.5rem;
}
}
</style>
