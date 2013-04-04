$(function($) {
    'use strict';

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
        var regexS = "[\\?&]" + name + "=([^&#]*)";
        var regex = new RegExp(regexS);
        var results = regex.exec(window.location.search);
        if(results == null)
            return "";
        else
            return decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    function createChart() {
        return polyjs.chart(getChartSpec());
    };
    
    function updateChart(chart) {
        chart.make(getChartSpec());
    }

    function getChartSpec() {
        var sizes = polyjs.data.url('./dumps/' +
                                    getParameterByName('src') +
                                    '.csv');
        return {
            layer: {
                data: sizes,
                type: "bar",
                x: "bin(Size, 15)",
                y: "count(Size)",
            },
            title: "Frequency of Packet Sizes",
            dom: "chart",
            width: 1300,
            height: 600,
            guide: {
                x: { title: 'Packet Size' },
                y: { title: 'Frequency' }
            }
        };
    }

    var chart = createChart();
    setInterval(function() { updateChart(chart); }, 1000);
});
