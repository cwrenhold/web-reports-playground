const chartsCollection = [];

let response = null;
let data = null;
let stringifiedData = null;

const rawDataUrl = 'http://localhost:8002/api/rawdata';

const go = new Go();

async function loadWasm() {
    const result = await WebAssembly.instantiateStreaming(fetch("dist/tiny_main.wasm"), go.importObject);
    go.run(result.instance);
}

async function initialiseReport() {
    const subjects = document.getElementById('subject');
    const filters = document.getElementById('filter');

    renderReport(subjects, filters);

    subjects.addEventListener('change', () => {
        renderReport(subjects, filters);
    });

    filters.addEventListener('change', () => {
        renderReport(subjects, filters);
    });
}

loadWasm().then(async () => {
    const report = document.getElementById('report');
    const loading = document.getElementById('loading');

    report.style.display = 'none';
    loading.style.display = 'block';
    await loadDataFromUrl(rawDataUrl);
    report.style.display = 'block';
    loading.style.display = 'none';

    initialiseReport();
});

function renderReport(subjects, filters) {
    const chartsContainer = document.getElementById('charts');
    chartsContainer.innerHTML = '';

    const selectedSubjectId = subjects.value == '' || subjects.value == "null" ? null : parseInt(subjects.value);
    const selectedFilterId = filters.value == '' || filters.value == "null" ? null : parseInt(filters.value);

    const processedObject = window.processData(selectedSubjectId, selectedFilterId);
    const responseData = JSON.parse(processedObject);

    subjects.innerHTML = '';

    responseData.subjects.forEach(subject => {
        const option = document.createElement('option');
        option.value = subject.id;
        option.innerText = subject.name;

        if (responseData.selectedSubjectId == subject.id) {
            option.selected = true;
        }

        subjects.appendChild(option);
    });

    filters.innerHTML = '';

    responseData.filters.forEach(filter => {
        const option = document.createElement('option');
        option.value = filter.id;
        option.innerText = filter.name;

        if (responseData.selectedFilterId == filter.id) {
            option.selected = true;
        }

        filters.appendChild(option);
    });

    if (responseData.reportDataItems) {
        for (const group of responseData.reportDataItems) {
            renderGroupAsChart(group, chartsContainer);
        }
    }
}

function renderGroupAsChart(group, chartsContainer) {
    const chartContainer = document.createElement('div');
    chartContainer.style.height = '500px';
    chartContainer.style.float = 'left';

    if (group.key !== '') {
        const chartTitle = document.createElement('h3');
        chartTitle.innerText = group.key;

        chartContainer.appendChild(chartTitle);
    }

    const chartDiv = document.createElement('div');
    chartDiv.style.height = '450px';
    chartContainer.appendChild(chartDiv);

    const chartCanvas = document.createElement('canvas');
    chartCanvas.id = `chart-${group.key}`;
    chartDiv.appendChild(chartCanvas);

    chartsContainer.appendChild(chartContainer);

    const chart = new Chart(chartCanvas, {
        type: 'doughnut',
        data: {
        labels: group.items.map(item => item.label),
        datasets: [{
            label: '# of Grades',
            data: group.items.map(item => item.count),
            borderWidth: 1
        }]
        },
    });

    chartsCollection.push(chart);
}
