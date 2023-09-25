class Filter < ActiveRecord::Base
    has_many :filter_values
    has_many :student_filters
    has_many :students, through: :student_filters
    has_many :grades, through: :filter_values
end
