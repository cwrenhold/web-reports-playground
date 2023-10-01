package main

import (
	"html/template"
	"log"
	"net/http"
	"strconv"

	"github.com/cwrenhold/web-reports-playground/initializers"
	"github.com/cwrenhold/web-reports-playground/utils"
)

type IdAndName struct {
	Id   int
	Name string
}

type ReportDataItem struct {
	FilterValue *string
	GradeId     int
	Points      *float64
	Label       string
	Count       int
}

type ReportData struct {
	Subjects          []IdAndName
	Filters           []IdAndName
	ReportData        []utils.Grouping[string, ReportDataItem]
	SelectedSubjectId int
	SelectedFilterId  int
}

func main() {
	http.HandleFunc("/", gradeBreakdown)

	initializers.ConnectToDB()

	log.Panicln(http.ListenAndServe(":8005", nil))
	http.ListenAndServe(":8005", nil)
}

func gradeBreakdown(w http.ResponseWriter, r *http.Request) {
	tmpl, err := template.ParseFiles("templates/gradeBreakdown.html")

	if err != nil {
		log.Println(err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	var selectedSubjectId int
	var selectFilterId int

	queryParameters := r.URL.Query()

	if len(queryParameters["subject"]) > 0 {
		parsedId, err := strconv.Atoi(queryParameters["subject"][0])
		if err != nil {
			selectedSubjectId = 0
		} else {
			selectedSubjectId = parsedId
		}
	}

	if len(queryParameters["filter"]) > 0 {
		parsedId, err := strconv.Atoi(queryParameters["filter"][0])
		if err != nil {
			selectFilterId = 0
		} else {
			selectFilterId = parsedId
		}
	}

	loadedData, err := loadGradeBreakdown(selectedSubjectId, selectFilterId)

	if err != nil {
		log.Println(err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	subjects, err := loadSubjects()

	if err != nil {
		log.Println(err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	filters, err := loadFilters()

	if err != nil {
		log.Println(err)
		http.Error(w, "Internal Server Error", http.StatusInternalServerError)
		return
	}

	groupedData := utils.GroupBy(loadedData, func(item ReportDataItem) string {
		if item.FilterValue == nil {
			return ""
		} else {
			return *item.FilterValue
		}
	})

	reportData := ReportData{
		Subjects:          subjects,
		Filters:           filters,
		ReportData:        groupedData,
		SelectedSubjectId: selectedSubjectId,
		SelectedFilterId:  selectFilterId,
	}

	tmpl.Execute(w, reportData)
}

func loadSubjects() ([]IdAndName, error) {
	sql := "SELECT id, name FROM subjects ORDER BY name"

	rows, err := initializers.SqlxDB.Queryx(sql)

	if err != nil {
		return nil, err
	}

	defer rows.Close()

	var subjects []IdAndName

	for rows.Next() {
		var subject IdAndName

		err := rows.StructScan(&subject)

		if err != nil {
			return nil, err
		}

		subjects = append(subjects, subject)
	}

	return subjects, nil
}

func loadFilters() ([]IdAndName, error) {
	sql := "SELECT id, name FROM filters ORDER BY name"

	rows, err := initializers.SqlxDB.Queryx(sql)

	if err != nil {
		return nil, err
	}

	defer rows.Close()

	var filters []IdAndName

	for rows.Next() {
		var filter IdAndName

		err := rows.StructScan(&filter)

		if err != nil {
			return nil, err
		}

		filters = append(filters, filter)
	}

	return filters, nil
}

func loadGradeBreakdown(selectedSubjectId, selectedFilterId int) ([]ReportDataItem, error) {
	var sql string

	if selectedFilterId == 0 {
		sql = `
			SELECT
				NULL AS filtervalue,
				g.id AS gradeid,
				g.points,
				g.text AS label,
				COUNT(*) AS count
			FROM student_grades sg
			INNER JOIN grades g ON sg.grade_id = g.id
			WHERE 1 = 1
		`
	} else {
		sql = `
			SELECT
				fv.text AS filtervalue,
				g.id AS gradeid,
				g.points,
				g.text AS label,
				COUNT(*) AS count
			FROM student_grades sg

			INNER JOIN grades g
			ON sg.grade_id = g.id

			INNER JOIN student_filters sf
			ON sg.student_id = sf.student_id
			AND sf.filter_id = :filter_id

			INNER JOIN filter_values fv
			ON sf.filter_value_id = fv.id

			WHERE 1 = 1
		`
	}

	if selectedSubjectId != 0 {
		sql += " AND sg.subject_id = :subject_id"
	}

	var grouping string
	if selectedFilterId == 0 {
		grouping = "g.id"
	} else {
		grouping = "fv.text, g.id"
	}

	sql += `
		GROUP BY ` + grouping + `
		ORDER BY ` + grouping + `
	`

	parameters := map[string]interface{}{
		"subject_id": selectedSubjectId,
		"filter_id":  selectedFilterId,
	}

	rows, err := initializers.SqlxDB.NamedQuery(sql, parameters)

	if err != nil {
		return nil, err
	}

	defer rows.Close()

	var reportData []ReportDataItem

	for rows.Next() {
		var reportItem ReportDataItem

		err := rows.StructScan(&reportItem)

		if err != nil {
			return nil, err
		}

		reportData = append(reportData, reportItem)
	}

	return reportData, nil
}
