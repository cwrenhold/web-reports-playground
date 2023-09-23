import { Chart, registerables } from 'chart.js';
import { ApiResponse } from './DataTypes';
import './style.css';

async function fetchData(): Promise<ApiResponse> {
  const response = await fetch('https://localhost:8003/api/rawdata');
  const data = await response.json();
  return data;
}

const chartsCollection: any[] = [];

const subjectsSelector = document.getElementById('subject') as HTMLSelectElement;
const filtersSelector = document.getElementById('filter') as HTMLSelectElement;

async function initialise() {
  Chart.register(...registerables);

  const data = await fetchData();

  const subjects = data.subjects.sort((a, b) => a.name.localeCompare(b.name));
  const filters = data.filters.sort((a, b) => a.name.localeCompare(b.name));

  subjects.splice(0, 0, { id: null, name: 'All' });
  filters.splice(0, 0, { id: null, name: 'None' });

  subjects.forEach(subject => {
    const option = document.createElement('option');
    option.value = subject.id?.toString() ?? '';
    option.text = subject.name;
    subjectsSelector.appendChild(option);
  });
  filters.forEach(filter => {
    const option = document.createElement('option');
    option.value = filter.id?.toString() ?? '';
    option.text = filter.name;
    filtersSelector.appendChild(option);
  });

  generateCharts(data);

  subjectsSelector.addEventListener('change', () => generateCharts(data));
  filtersSelector.addEventListener('change', () => generateCharts(data));
}

function generateCharts(dataSet: ApiResponse) {
  const selectedSubject = subjectsSelector.value === '' ? null : parseInt(subjectsSelector.value);
  const selectedFilter = filtersSelector.value === '' ? null : parseInt(filtersSelector.value);

  const chartsContainer = document.getElementById('charts') as HTMLDivElement;

  // Clear any existing charts
  chartsContainer.innerHTML = '';
  chartsCollection.length = 0;

  let students = dataSet.students;
  let studentGrades = dataSet.studentGrades;

  if (selectedSubject !== null) {
    studentGrades = studentGrades.filter(grade => grade.subjectId === selectedSubject);
  }

  let displayResults: ReportDataItem[] = [];

  if (selectedFilter === null) {
    const resultsByGradeId = groupBy(studentGrades, grade => grade.gradeId);

    displayResults = resultsByGradeId.map(result => {
      const grade = dataSet.grades.find(grade => grade.id === result.key);

      return {
        filterValue: null,
        gradeId: result.key,
        label: grade?.text ?? '',
        points: grade?.points ?? null,
        count: result.items.length
      };
    });
  } else {
    const studentsWithFilterValueIds =
      dataSet.studentFilters.filter(filter => filter.filterId === selectedFilter).map(filter => ({
        studentId: filter.studentId,
        filterValueId: filter.filterValueId
      }));

    const resultsWithFilterValueIds =
      studentGrades.map(grade => {
        const studentFilterValue = studentsWithFilterValueIds.find(studentFilter => studentFilter.studentId === grade.studentId);

        return {
          gradeId: grade.gradeId,
          filterValueId: studentFilterValue?.filterValueId
        };
      })

    const resultsByGradeId = groupBy(
      resultsWithFilterValueIds,
      grade => ({
        gradeId: grade.gradeId,
        filterValueId: grade.filterValueId
      }),
      (a, b) => a.gradeId === b.gradeId && a.filterValueId === b.filterValueId);

    displayResults = resultsByGradeId.map(result => {
      const grade = dataSet.grades.find(grade => grade.id === result.key.gradeId);

      const filterValue = dataSet.filterValues.find(filterValue => filterValue.id === result.key.filterValueId);

      return {
        filterValue: filterValue?.text ?? null,
        gradeId: result.key.gradeId,
        label: grade?.text ?? '',
        points: grade?.points ?? null,
        count: result.items.length
      };
    });
  }

  displayResults.sort(displayResultsComparer);

  const groupedDisplayResults = groupBy(displayResults, result => result.filterValue ?? '');

  for(const group of groupedDisplayResults) {
    renderGroupAsChart(group, chartsContainer);
  }
}

interface ReportDataItem {
  filterValue: string | null;
  gradeId: number;
  label: string;
  points: number | null;
  count: number;
}

initialise();

function displayResultsComparer(a: ReportDataItem, b: ReportDataItem): number {
  // Sort by filter value, then by grade points
  if (a.filterValue === b.filterValue) {
    if (a.points === null && b.points === null) {
      return 0;
    } else if (a.points === null) {
      return -1;
    } else if (b.points === null) {
      return 1;
    } else {
      return a.points - b.points;
    }
  } else {
    if (a.filterValue === null && b.filterValue === null) {
      return 0;
    } else if (a.filterValue === null) {
      return -1;
    } else if (b.filterValue === null) {
      return 1;
    } else {
      return a.filterValue.localeCompare(b.filterValue);
    }
  }
}

function renderGroupAsChart(group: { key: string; items: ReportDataItem[]; }, chartsContainer: HTMLDivElement) {
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

function groupBy<TObject, TKey>(
  array: TObject[],
  selector: (item: TObject) => TKey,
  equalityComparer: (a: TKey, b: TKey) => boolean = (a, b) => a === b
): { key: TKey; items: TObject[] }[] {
  const results: { key: TKey; items: TObject[] }[] = [];

  array.forEach(item => {
    const key = selector(item);
    const existing = results.find(result => equalityComparer(result.key, key));
    if (existing) {
      existing.items.push(item);
    } else {
      results.push({ key, items: [item] });
    }
  });

  return results;
}
