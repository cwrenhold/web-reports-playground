using BackendCSharp.Data;
using Microsoft.AspNetCore.Mvc;

namespace BackendCSharp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RawDataController : Controller
{
    private readonly PlaygroundContext _playgroundContext;

    public RawDataController(PlaygroundContext playgroundContext)
    {
        _playgroundContext = playgroundContext;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var filters = this._playgroundContext.Filters.ToList();
        var filterValues = this._playgroundContext.FilterValues.ToList();
        var grades = this._playgroundContext.Grades.ToList();
        var students = this._playgroundContext.Students.ToList();
        var studentFilters = this._playgroundContext.StudentFilters.ToList();
        var studentGrades = this._playgroundContext.StudentGrades.ToList();
        var studentSubjects = this._playgroundContext.StudentSubjects.ToList();
        var subjects = this._playgroundContext.Subjects.ToList();

        var results = new {
            filters,
            filterValues,
            grades,
            students,
            studentFilters,
            studentGrades,
            studentSubjects,
            subjects
        };

        return this.Ok(results);
    }
}
