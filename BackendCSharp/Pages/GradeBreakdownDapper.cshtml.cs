using BackendCSharp.Data;
using BackendCSharp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using Npgsql;

namespace BackendCSharp.Pages;

public class GradeBreakdownDapperModel : PageModel
{
    private readonly PlaygroundContext _playgroundContext;
    private readonly ILogger<IndexModel> _logger;
    private readonly string _connectionString;

    public GradeBreakdownDapperModel(PlaygroundContext playgroundContext, ILogger<IndexModel> logger)
    {
        _playgroundContext = playgroundContext;
        _logger = logger;

        this._connectionString = this._playgroundContext.Database.GetConnectionString() ?? string.Empty;
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
        this.Subjects = this.LoadSubjects();
        this.Filters = this.LoadFilters();

        string query = "";

        if (this.SelectedFilterId is null)
        {
            query = @"
                SELECT
                    NULL AS filtervalue,
                    g.id AS gradeid,
                    g.points,
                    g.text AS label,
                    COUNT(*) AS count
                FROM student_grades sg
                INNER JOIN grades g ON sg.grade_id = g.id
                WHERE 1 = 1
            ";
        }
        else
        {
            query = @"
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
                AND sf.filter_id = @filter_id

                INNER JOIN filter_values fv
                ON sf.filter_value_id = fv.id

                WHERE 1 = 1
            ";
        }

        if (this.SelectedSubjectId is not null)
        {
            query += " AND sg.subject_id = @subject_id";
        }

        string grouping = "";
        if (this.SelectedFilterId is null)
        {
            grouping = " GROUP BY g.id";
        }
        else
        {
            grouping = " GROUP BY fv.text, g.id";
        }

        query += @"
            GROUP BY " + grouping + @"
            ORDER BY " + grouping + @"
        ";

        using IDbConnection connection = new NpgsqlConnection(this._connectionString);

        var results = connection.Query<ReportDataItem>(
            query,
            new
            {
                subject_id = this.SelectedSubjectId,
                filter_id = this.SelectedFilterId
            }).ToList();

        this.ReportData = results.ToList();

        this.GroupReportData = this.ReportData
            .GroupBy(rd => rd.FilterValue)
            .ToList();
    }

    private List<Filter> LoadFilters()
    {
        using IDbConnection connection = new NpgsqlConnection(this._connectionString);

        var filters = connection.Query<Filter>("SELECT * FROM Filters ORDER BY Name").ToList();

        return filters;
    }

    private List<Subject> LoadSubjects()
    {
        using IDbConnection connection = new NpgsqlConnection(this._connectionString);

        var subjects = connection.Query<Subject>("SELECT * FROM Subjects ORDER BY Name").ToList();

        return subjects;
    }
}
