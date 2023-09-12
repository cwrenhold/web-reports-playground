using BackendCSharp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BackendCSharp.Pages;

public class GradeBreakdownModel : PageModel
{
    private readonly PlaygroundContext _playgroundContext;
    private readonly ILogger<IndexModel> _logger;

    public GradeBreakdownModel(PlaygroundContext playgroundContext, ILogger<IndexModel> logger)
    {
        _playgroundContext = playgroundContext;
        _logger = logger;
    }

    public int? SelectedSubjectId { get; set; }

    public int? SelectedFilterId { get; set; }

    public List<Subject> Subjects { get; private set; } = new List<Subject>();

    public List<Filter> Filters { get; private set; } = new List<Filter>();

    public List<ReportDataItem> ReportData { get; private set; }

    public void OnGet()
    {
        this.Subjects = this._playgroundContext.Subjects.OrderBy(x => x.Name).ToList();
        this.Filters = this._playgroundContext.Filters.OrderBy(x => x.Name).ToList();

        var results =
            from sg in this._playgroundContext.StudentGrades
            join grd in this._playgroundContext.Grades on sg.GradeId equals grd.Id
            group sg by sg.GradeId into g
            select new ReportDataItem
            {
                GradeId = g.Key,
                Label = g.First().Grade.Text,
                Count = g.Count()
            };

        this.ReportData = results.ToList();
    }

    public class ReportDataItem
    {
        public int GradeId { get; set; }

        public string Label { get; set; } = string.Empty;

        public int Count { get; set; }
    }
}
