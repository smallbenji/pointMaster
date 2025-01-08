<template>
    <div class="grid">
        <div class="title">
            <h1>Statistikker</h1>
        </div>
        <div class="stats">
            <div
                v-for="(data, index) in statData"
                class="box"
            >
                <p class="header">{{ data.title }}</p>
                <p class="content">{{ data.value }}</p>
            </div>
        </div>
        <div class="line-chart">
            <div id="line-chart"></div>
        </div>
        <div class="pie-chart">
            <div id="pie-chart"></div>
        </div>
    </div>
</template>

<script lang="ts">
import ApexCharts, { ApexOptions } from "apexcharts";
import axios from "axios";
import { defineComponent, onMounted, ref } from "vue";
import { HubConnectionBuilder, LogLevel } from "@aspnet/signalr";

export default defineComponent({
    name: "Stats",
    setup() {
        const statData = ref<Stat[]>([]);

        const lineChartOptions: ApexOptions = {
            chart: {
                type: "line",
                toolbar: {
                    show: true,
                },
            },
            stroke: {
                curve: "smooth"
            },
            xaxis: {
                type: "datetime",
            },
            series: [],
        }

        const pieChartOptions: ApexOptions = {
            chart: {
                type: "pie",
                toolbar: {
                    show: true,
                },
            },
            series: [],
        };

        var lineChart: ApexCharts;
        var pieChart: ApexCharts;

        const initalizeCharts = async () => {

            lineChart = new ApexCharts(
                document.querySelector("#line-chart"),
                lineChartOptions
            );
            lineChart.render();
            
            pieChart = new ApexCharts(
                document.querySelector("#pie-chart"),
                pieChartOptions
            );
            pieChart.render();
        }

        const initializeHub = async () => {

            const connection = new HubConnectionBuilder().withUrl("../DataHub").configureLogging(LogLevel.Information).build();
    
            connection.start();

            connection.on("ReceiveMessage", async (data: StatData) => {
                connection.invoke("SendData");
                console.log(data);
            })

            connection.on("StatData", (a:StatData) => {
                console.log(a);
                updateData(a);
            })
        }

        const updateData = (data: StatData) => {
            lineChart.updateOptions({
                series: data.pointChartModels,
            } as ApexOptions);

            pieChart.updateOptions({
                series: data.pointRatio.data,
                labels: data.pointRatio.names,
            });

            statData.value = (data.stats as Stat[]);
        }

            
        onMounted(async () => {
            await initalizeCharts()
            await initializeHub()
        });

        return {
            statData,
        };
    },
});
</script>

<style lang="scss">
    .grid {
        display: grid;
        grid-template-areas: 
        "a a"
        "b b"
        "c d";
        height: 100%;
        width: 100%;
    }

    @media (max-width: 640px) {
        .grid {
            grid-template-areas: 
            "a"
            "b"
            "c"
            "d";
        }
    }
    
    .title {
        grid-area: a;
    }
    
    .stats {
        grid-area: b;
        display: flex;
        justify-content: space-between;

        @media (max-width: 640px) {
            flex-direction: column;
            gap: 1rem;
            margin: auto;
            margin-bottom: 2rem;
            margin-top: 2rem;
        }

        margin-top: 2rem;
        margin-bottom: 6rem;

        .box {
            height: 6rem;
            width: 12rem;
            padding: 1rem;
            background-color: rgba(0,0,0,.2);
            border: solid rgba(0,0,0,0.3) 1px;
            border-radius: 5px;
            text-align: center;
            box-shadow: 5px 5px 5px rgba(0,0,0,.1);

            .header {
                font-size: 1.25rem;
            }
            .content {
                font-size: 1rem;
            }
        }
    }

    .line-chart {
        grid-area: c;
    }

    .pie-chart {
        grid-area: d;
    }
</style>
