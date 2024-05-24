
namespace Core
{
    public class ReportApi : ApiBase
    {
        private readonly Reports Reports;
        private const string ReportNotFound = "Report not found.";
        private const string AuthorizationPolicyName = "Users";

        public ReportApi(Users users, Reports reports) : base(users)
        {
            this.Reports = reports;
        }
        internal class ListReportsResponseJson
        {
            public ICollection<int> Ids { get; }
            public ListReportsResponseJson(ICollection<int> ids)
            {
                Ids = ids;
            }
        }

        public IResult ListReports(HttpContext context)
        {
            User user = GetUser(context);

            var id = Reports.GenerateReportsIfNeeded(user.Id);

            ICollection<int> ids = Reports.FindReportsByUser(user.Id);

            return Results.Ok(new ListReportsResponseJson(ids));
        }

        internal class GetReportResponseJson
        {
            public int Id { get; set; }
            public int userid { get; set; }
            public int TotalPages { get; set; }
            public int PercentagePages { get; set; }

            public int TotalSessions { get; set; }

            public int PercentageSessions { get; set; }
            public DateTime Date { get; set; } 
        }

        public IResult GetReports(HttpContext context, int id)
        {
            User user = GetUser(context);

           Report report = Reports.FindReport(id);
            

            if (report == null || report.UserId != user.Id)
                return Results.BadRequest(new ErrorResponse { Error = ReportNotFound });

            return Results.Ok(new GetReportResponseJson
            {
                Id = id,
                userid = user.Id,
                TotalPages = report.TotalPages,
                PercentagePages = report.PercentagePages,
                TotalSessions = report.TotalSessions,
                PercentageSessions = report.PercentageSessions,
                Date = report.Date
            });
        }

        public override void Map(WebApplication app)
        {
            app.MapGet("/reports/list", ListReports).RequireAuthorization(AuthorizationPolicyName);
            app.MapGet("/reports/get/{id}", GetReports).RequireAuthorization(AuthorizationPolicyName);
        }
    }
}
