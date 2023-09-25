class Grade < ActiveRecord::Base
    has_many :student_grades
    has_many :students, through: :student_grades
end
