class GradeBreakdownController < ApplicationController
    def index
        @subjects = Subject.all.order(:name)
        @filters = Filter.all.order(:name)

        @selected_subject_id = params[:selected_subject_id].to_i
        @selected_filter_id = params[:selected_filter_id].to_i

        student_grades = StudentGrade.all

        if @selected_subject_id != 0
            student_grades = student_grades.where(subject_id: @selected_subject_id)
        end

        results = []

        if @selected_filter_id == 0
            results = student_grades.includes(:grade).to_a.group_by(&:grade_id).map { |key, group| 
                {
                    grade_id: key,
                    label: group.first.grade.text,
                    points: group.first.grade.points,
                    count: group.count,
                    filter_value: nil
                }
            }
        else
            results = student_grades
                .includes(:grade, student: :student_filters)
                .where(student_filters: { filter_id: @selected_filter_id })
                .to_a
                .group_by { |student_grade| [student_grade.grade_id, student_grade.student.student_filters.first.filter_value_id] }
                .map { |key, group|
                    {
                        grade_id: key[0],
                        label: group.first.grade.text,
                        points: group.first.grade.points,
                        count: group.count,
                        filter_value: key[1]
                    }
                }

            filter_value_ids = results.map { |item| item[:filter_value] }.uniq
            filter_values = FilterValue.where(id: filter_value_ids)

            results.each do |item|
                filter_value = filter_values.find { |filter_value| filter_value.id == item[:filter_value] }
                item[:filter_value] = filter_value.text
            end
        end

        @report_data = results.sort_by { |item| [item[:filter_value], item[:points] || -Float::INFINITY] }

        @grouped_report_data = @report_data.group_by { |item| item[:filter_value] }

        print @grouped_report_data

        render erb: 'grade_breakdown/index'.to_sym
    end
end
