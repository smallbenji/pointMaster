<template>
    <div class="stat-dashboard">
        <div>
            <h1>Stats</h1>
            <div class="stats" v-for="data in StatData">
                <div>

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
import { defineComponent, onMounted } from 'vue';


export default defineComponent({
    name: 'HelloWorld',
    setup() {
        var StatData: Stat[] = [];

        const GetStats = async () => {
            var data = (await axios.get("/api/getstats")).data as Stat[];

            StatData = data;
        }

        onMounted(async () => {
            await GetStats();
            await Chart();
        });
    }
});

const Chart = async () => {
    var options: ApexOptions = {
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

    console.log(data);

    chart.updateOptions({
        series: data
    } as ApexOptions)
}
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
}

.stat-title {
    font-weight: 700;
    font-size: 1.25rem;
}

.stat .stat-value {
    font-size: 2rem;
}
</style>