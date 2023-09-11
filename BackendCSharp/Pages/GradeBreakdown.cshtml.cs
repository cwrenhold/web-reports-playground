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

    public void OnGet()
    {
        this.Subjects = this._playgroundContext.Subjects.OrderBy(x => x.Name).ToList();
        this.Filters = this._playgroundContext.Filters.OrderBy(x => x.Name).ToList();
    }
}
