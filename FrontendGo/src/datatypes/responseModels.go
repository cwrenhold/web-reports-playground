package datatypes

import "github.com/cwrenhold/web-reports-playground/src/utils"

type ReportDataItem struct {
	FilterValue *string  `json:"filterValue"`
	GradeId     int      `json:"gradeId"`
	Label       string   `json:"label"`
	Points      *float32 `json:"points"`
	Count       int      `json:"count"`
}

type Report struct {
	ReportDataItems   []utils.Grouping[string, ReportDataItem] `json:"reportDataItems"`
	Subjects          []Subject                                `json:"subjects"`
	Filters           []Filter                                 `json:"filters"`
	SelectedSubjectId *int                                     `json:"selectedSubjectId"`
	SelectedFilterId  *int                                     `json:"selectedFilterId"`
}
