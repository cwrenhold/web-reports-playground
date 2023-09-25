class FilterValue < ActiveRecord::Base
    belongs_to :student_filter
    belongs_to :filter
end
