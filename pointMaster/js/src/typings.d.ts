type Point = {
    Id: number;
    Points: number;
    Turnout: number;
    postName: string;
    patruljeName: string;
    DateCreated: Date;
}

type Stat = {
    title: string;
    value: string;
}

type StatData = {
    stats: any;
    pointRatio: any;
    pointChartModels: any;
}