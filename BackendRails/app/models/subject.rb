class Subject < ActiveRecord::Base
    has_many :student_subjects
    has_many :students, through: :student_subjects
    has_many :student_grades
    has_many :grades, through: :student_grades
end
