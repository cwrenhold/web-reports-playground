package main

import (
	"context"
	"encoding/json"
	"sort"
	"syscall/js"
	"time"

	"github.com/cwrenhold/web-reports-playground/src/datatypes"
	"github.com/cwrenhold/web-reports-playground/src/utils"
	fetch "marwan.io/wasm-fetch"
)

var loadedData datatypes.ApiResponse

func loadDataFromUrl(url string) (bool, error) {
	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	response, err := fetch.Fetch(url, &fetch.Opts{
		Signal: ctx,
		Method: fetch.MethodGet,
	})

	if err != nil {
		return false, err
	}

	if response.Status != 200 {
		return false, nil
	}

	jsData := response.Body

	var apiResponse datatypes.ApiResponse
	err = json.Unmarshal([]byte(jsData), &apiResponse)
	if err != nil {
		return false, err
	}

	loadedData = apiResponse

	return true, nil
}

func jsValueToNullableInt(value js.Value) *int {
	if value.IsNull() {
		return nil
	}

	result := value.Int()
	return &result
}

func generateReportDataItems(apiResponse datatypes.ApiResponse, selectedSubjectId, selectedFilterId *int) []utils.Grouping[string, datatypes.ReportDataItem] {
	filteredGrades := apiResponse.StudentGrades

	if selectedSubjectId != nil {
		filteredGrades = utils.Filter[datatypes.StudentGrade](filteredGrades, func(item datatypes.StudentGrade) bool {
			return item.SubjectId == *selectedSubjectId
		})
	}

	var displayResults []datatypes.ReportDataItem

	if selectedFilterId == nil {
		resultsByGradeId := utils.GroupBy[datatypes.StudentGrade, int](filteredGrades, func(item datatypes.StudentGrade) int {
			return item.GradeId
		})

		displayResults = utils.Map[utils.Grouping[int, datatypes.StudentGrade], datatypes.ReportDataItem](resultsByGradeId, func(group utils.Grouping[int, datatypes.StudentGrade]) datatypes.ReportDataItem {
			grade := utils.Filter[datatypes.Grade](apiResponse.Grades, func(item datatypes.Grade) bool {
				return item.Id == group.Key
			})[0]

			return datatypes.ReportDataItem{
				GradeId:     grade.Id,
				Points:      grade.Points,
				Label:       grade.Text,
				Count:       len(group.Items),
				FilterValue: nil,
			}
		})
	} else {
		studentsWithFilterValueIds := utils.Filter[datatypes.StudentFilter](apiResponse.StudentFilters, func(item datatypes.StudentFilter) bool {
			return item.FilterId == *selectedFilterId
		})

		resultsWithFilterValueIds := utils.Map[datatypes.StudentGrade, FilterBreakdownIds](filteredGrades, func(item datatypes.StudentGrade) FilterBreakdownIds {
			studentFilterValue := utils.Find[datatypes.StudentFilter](studentsWithFilterValueIds, func(filterValue datatypes.StudentFilter) bool {
				return filterValue.StudentId == item.StudentId
			})

			return FilterBreakdownIds{
				gradeId:       item.GradeId,
				filterValueId: studentFilterValue.FilterValueId,
			}
		})

		resultsByGradeId := utils.GroupByComplex[FilterBreakdownIds, FilterBreakdownIds](resultsWithFilterValueIds, func(item FilterBreakdownIds) FilterBreakdownIds {
			return item
		}, func(a FilterBreakdownIds, b FilterBreakdownIds) bool {
			return a.gradeId == b.gradeId && a.filterValueId == b.filterValueId
		})

		displayResults = utils.Map[utils.Grouping[FilterBreakdownIds, FilterBreakdownIds], datatypes.ReportDataItem](resultsByGradeId, func(group utils.Grouping[FilterBreakdownIds, FilterBreakdownIds]) datatypes.ReportDataItem {
			grade := utils.Find[datatypes.Grade](apiResponse.Grades, func(item datatypes.Grade) bool {
				return item.Id == group.Key.gradeId
			})

			filterValue := utils.Find[datatypes.FilterValue](apiResponse.FilterValues, func(item datatypes.FilterValue) bool {
				return item.Id == group.Key.filterValueId
			})

			var filterValueText string
			if filterValue == nil {
				filterValueText = ""
			} else {
				filterValueText = filterValue.Text
			}

			return datatypes.ReportDataItem{
				GradeId:     grade.Id,
				Points:      grade.Points,
				Label:       grade.Text,
				Count:       len(group.Items),
				FilterValue: &filterValueText,
			}
		})
	}

	displayResultsComparer := func(i, j int) bool {
		// Compare the FilterValue fields first
		a := displayResults[i]
		b := displayResults[j]

		if a.FilterValue == nil && b.FilterValue == nil {
			// If the FilterValue fields are equal, compare the Points fields
			if a.Points == nil && b.Points == nil {
				// If both Points fields are nil, return true
				return true
			} else if a.Points == nil {
				// If only the Points field of the first element is nil, return false
				return true
			} else if b.Points == nil {
				// If only the Points field of the second element is nil, return true
				return false
			} else {
				// If both Points fields are not nil, compare them
				return *a.Points < *b.Points
			}
		} else if a.FilterValue == nil {
			// If only the FilterValue field of the first element is nil, return false
			return false
		} else if b.FilterValue == nil {
			// If only the FilterValue field of the second element is nil, return true
			return true
		} else if *a.FilterValue != *b.FilterValue {
			// If the FilterValue fields are not equal, compare them
			return *a.FilterValue < *b.FilterValue
		} else {
			// If the FilterValue fields are equal, compare the Points fields
			if a.Points == nil && b.Points == nil {
				// If both Points fields are nil, return true
				return true
			} else if a.Points == nil {
				// If only the Points field of the first element is nil, return false
				return true
			} else if b.Points == nil {
				// If only the Points field of the second element is nil, return true
				return false
			} else {
				// If both Points fields are not nil, compare them
				return *a.Points < *b.Points
			}
		}
	}

	sort.Slice(displayResults, displayResultsComparer)

	groupedDisplayResults := utils.GroupBy[datatypes.ReportDataItem, string](displayResults, func(item datatypes.ReportDataItem) string {
		if item.FilterValue == nil {
			return ""
		} else {
			return *item.FilterValue
		}
	})

	return groupedDisplayResults
}

type FilterBreakdownIds struct {
	gradeId       int
	filterValueId int
}

func main() {
	js.Global().Set("loadDataFromUrl", js.FuncOf(func(this js.Value, args []js.Value) interface{} {
		url := args[0].String()

		handler := js.FuncOf(func(this js.Value, args []js.Value) interface{} {
			resolve := args[0]

			go func() {
				success, _ := loadDataFromUrl(url)

				resolve.Invoke(success)
			}()

			return nil
		})

		promiseConstructor := js.Global().Get("Promise")
		return promiseConstructor.New(handler)
	}))

	js.Global().Set("processData", js.FuncOf(func(this js.Value, args []js.Value) interface{} {
		selectedSubjectId := jsValueToNullableInt(args[0])
		selectedFilterId := jsValueToNullableInt(args[1])

		result := datatypes.Report{
			ReportDataItems: []utils.Grouping[string, datatypes.ReportDataItem]{},
			Subjects:        loadedData.Subjects,
			Filters:         loadedData.Filters,
		}

		result.ReportDataItems = generateReportDataItems(loadedData, selectedSubjectId, selectedFilterId)

		result.Subjects = append([]datatypes.Subject{datatypes.Subject{Id: nil, Name: "All"}}, result.Subjects...)
		result.SelectedSubjectId = selectedSubjectId

		result.Filters = append([]datatypes.Filter{datatypes.Filter{Id: nil, Name: "None"}}, result.Filters...)
		result.SelectedFilterId = selectedFilterId

		jsonData, err := json.Marshal(result)
		if err != nil {
			return js.Global().Get("Error").New(err.Error())
		}

		return string(jsonData)
	}))

	select {}
}
