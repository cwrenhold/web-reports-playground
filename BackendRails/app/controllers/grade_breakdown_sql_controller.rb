class GradeBreakdownSqlController < ApplicationController
    def index
        connection = ActiveRecord::Base.connection

        @subjects = connection.execute('SELECT * FROM subjects ORDER BY name')
        @filters = connection.execute('SELECT * FROM filters ORDER BY name')

        @selected_subject_id = params[:selected_subject_id].to_i
        @selected_filter_id = params[:selected_filter_id].to_i

        query = "";

        if @selected_filter_id == 0
            query = <<~SQL
                SELECT
                    NULL AS filtervalue,
                    g.id AS gradeid,
                    g.points,
                    g.text AS label,
                    COUNT(*) AS count
                FROM student_grades sg
                INNER JOIN grades g ON sg.grade_id = g.id
                WHERE 1 = 1
            SQL
        else
            query = <<~SQL
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
            SQL
        end

        if @selected_subject_id != 0
            query += <<~SQL
                AND sg.subject_id = :subject_id
            SQL
        end

        grouping = if @selected_filter_id == 0
            " GROUP BY g.id"
        else
            " GROUP BY g.id, fv.text"
        end

        query += <<~SQL
                #{grouping}
            SQL

        print query

        @report_data = connection.execute(
            ApplicationRecord.sanitize_sql_array([
                query,
                {
                    subject_id: @selected_subject_id,
                    filter_id: @selected_filter_id
                }
            ])
        )

        @grouped_report_data = @report_data.group_by { |item| item["filtervalue"] }

        render erb: 'grade_breakdown/index'.to_sym
    end
end
