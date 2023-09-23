package datatypes

type Filter struct {
	Id   *int   `json:"id"`
	Name string `json:"name"`
}

type FilterValue struct {
	Id       int    `json:"id"`
	Text     string `json:"text"`
	FilterId int    `json:"filterId"`
}

type Grade struct {
	Id     int      `json:"id"`
	Text   string   `json:"text"`
	Points *float32 `json:"points"`
}

type Student struct {
	Id        int    `json:"id"`
	FirstName string `json:"firstName"`
	LastName  string `json:"lastName"`
}

type StudentFilter struct {
	Id            int `json:"id"`
	StudentId     int `json:"studentId"`
	FilterId      int `json:"filterId"`
	FilterValueId int `json:"filterValueId"`
}

type StudentGrade struct {
	Id        int `json:"id"`
	StudentId int `json:"studentId"`
	SubjectId int `json:"subjectId"`
	GradeId   int `json:"gradeId"`
}

type StudentSubject struct {
	Id        int `json:"id"`
	StudentId int `json:"studentId"`
	SubjectId int `json:"subjectId"`
}

type Subject struct {
	Id   *int   `json:"id"`
	Name string `json:"name"`
}

type ApiResponse struct {
	Filters         []Filter         `json:"filters"`
	FilterValues    []FilterValue    `json:"filterValues"`
	Grades          []Grade          `json:"grades"`
	Students        []Student        `json:"students"`
	StudentFilters  []StudentFilter  `json:"studentFilters"`
	StudentGrades   []StudentGrade   `json:"studentGrades"`
	StudentSubjects []StudentSubject `json:"studentSubjects"`
	Subjects        []Subject        `json:"subjects"`
}
