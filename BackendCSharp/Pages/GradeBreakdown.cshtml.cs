using BackendCSharp.Data;
using BackendCSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BackendCSharp.Pages;

public partial class GradeBreakdownModel : PageModel
{
    private readonly PlaygroundContext _playgroundContext;
    private readonly ILogger<IndexModel> _logger;

    public GradeBreakdownModel(PlaygroundContext playgroundContext, ILogger<IndexModel> logger)
    {
        _playgroundContext = playgroundContext;
        _logger = logger;
    }

    [FromQuery]
    public int? SelectedSubjectId { get; set; }

    [FromQuery]
    public int? SelectedFilterId { get; set; }

    public List<Subject> Subjects { get; private set; } = new List<Subject>();

    public List<Filter> Filters { get; private set; } = new List<Filter>();

    public List<ReportDataItem> ReportData { get; private set; } = new();

    public List<IGrouping<string?, ReportDataItem>> GroupReportData { get; private set; } = new();

    public void OnGet()
    {
        this.Subjects = this._playgroundContext.Subjects.OrderBy(x => x.Name).ToList();
        this.Filters = this._playgroundContext.Filters.OrderBy(x => x.Name).ToList();

        var students = this._playgroundContext.Students.AsQueryable();
        var studentGrades = this._playgroundContext.StudentGrades.AsQueryable();

        if (this.SelectedSubjectId is not null)
        {
            studentGrades = studentGrades
                .Where(sg => sg.SubjectId == this.SelectedSubjectId);
        }

        IQueryable<ReportDataItem> results;
        
        if (this.SelectedFilterId is null)
        {
            results = 
                from sg in studentGrades
                join std in students on sg.StudentId equals std.Id
                join grd in this._playgroundContext.Grades on sg.GradeId equals grd.Id
                group sg by sg.GradeId into g
                select new ReportDataItem
                {
                    FilterValue = null,
                    GradeId = g.Key,
                    Label = g.First().Grade.Text,
                    Points = g.First().Grade.Points,
                    Count = g.Count()
                };
        }
        else
        {
            results = 
                from sg in studentGrades
                join std in students on sg.StudentId equals std.Id
                join grd in this._playgroundContext.Grades on sg.GradeId equals grd.Id
                join sf in this._playgroundContext.StudentFilters.Where(sf => sf.FilterId == this.SelectedFilterId)
                    on sg.StudentId equals sf.StudentId
                join fv in this._playgroundContext.FilterValues on sf.FilterValueId equals fv.Id
                group sg by new { sf.FilterValueId, fv.Text, sg.GradeId } into g
                select new ReportDataItem
                {
                    FilterValue = g.Key.Text,
                    GradeId = g.Key.GradeId,
                    Label = g.First().Grade.Text,
                    Points = g.First().Grade.Points,
                    Count = g.Count()
                };
        }

        this.ReportData = results
            .ToList()
            .OrderBy(rd => rd.FilterValue)
            .ThenByDescending(rd => rd.Points != null)
            .ThenByDescending(rd => rd.Points)
            .ToList();

        this.GroupReportData = this.ReportData
            .GroupBy(rd => rd.FilterValue)
            .ToList();
    }
}
