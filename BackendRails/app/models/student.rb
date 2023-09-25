class Student < ActiveRecord::Base
    has_many :student_subjects
    has_many :subjects, through: :student_subjects
    has_many :student_grades
    has_many :grades, through: :student_grades
    has_many :student_filters
    has_many :filter_values, through: :student_filters
    has_many :filters, through: :filter_values
end
