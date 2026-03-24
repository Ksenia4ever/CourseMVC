using ClosedXML.Excel;
using CourseDomain.Model;
using Microsoft.EntityFrameworkCore;

namespace CourseInfrastructure.Services
{
    public class CourseSubscriptionExportService : IExportService<CourseAccount>
    {
        private const string RootWorksheetName = "Subscriptions";

        private const int StudentEmailColumn = 1;
        private const int StudentNameColumn = 2;
        private const int CourseTitleColumn = 3;
        private const int SubjectColumn = 4;
        private const int AuthorEmailColumn = 5;
        private const int AuthorNameColumn = 6;
        private const int SubscribedAtColumn = 7;
        private const int HasCertificateColumn = 8;

        private static readonly IReadOnlyList<string> HeaderNames =
            new string[]
            {
                "StudentEmail",
                "StudentName",
                "CourseTitle",
                "Subject",
                "AuthorEmail",
                "AuthorName",
                "SubscribedAt",
                "HasCertificate"
            };

        private readonly DbCourseContext _context;

        public CourseSubscriptionExportService(DbCourseContext context)
        {
            _context = context;
        }

        public async Task WriteToAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanWrite)
            {
                throw new ArgumentException("У потік неможливо записати дані", nameof(stream));
            }

            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(RootWorksheetName);

            WriteHeader(worksheet);

            var subscriptions = await _context.CourseAccounts
                .Include(ca => ca.Account)
                .Include(ca => ca.Course)
                    .ThenInclude(c => c.Author)
                .OrderBy(ca => ca.CourseId)
                .ThenBy(ca => ca.AccountId)
                .ToListAsync(cancellationToken);

            var rowIndex = 2;

            foreach (var subscription in subscriptions)
            {
                var hasCertificate = await _context.Certificates.AnyAsync(
                    c => c.AccountId == subscription.AccountId &&
                         c.CourseId == subscription.CourseId,
                    cancellationToken);

                WriteSubscriptionRow(worksheet, subscription, rowIndex, hasCertificate);
                rowIndex++;
            }

            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(stream);
        }

        private static void WriteHeader(IXLWorksheet worksheet)
        {
            for (int columnIndex = 0; columnIndex < HeaderNames.Count; columnIndex++)
            {
                worksheet.Cell(1, columnIndex + 1).Value = HeaderNames[columnIndex];
            }

            worksheet.Row(1).Style.Font.Bold = true;
        }

        private static void WriteSubscriptionRow(
            IXLWorksheet worksheet,
            CourseAccount subscription,
            int rowIndex,
            bool hasCertificate)
        {
            worksheet.Cell(rowIndex, StudentEmailColumn).Value = subscription.Account?.Email ?? "";
            worksheet.Cell(rowIndex, StudentNameColumn).Value = subscription.Account?.Name ?? "";
            worksheet.Cell(rowIndex, CourseTitleColumn).Value = subscription.Course?.Title ?? "";
            worksheet.Cell(rowIndex, SubjectColumn).Value = subscription.Course?.Subject ?? "";
            worksheet.Cell(rowIndex, AuthorEmailColumn).Value = subscription.Course?.Author?.Email ?? "";
            worksheet.Cell(rowIndex, AuthorNameColumn).Value = subscription.Course?.Author?.Name ?? "";
            worksheet.Cell(rowIndex, SubscribedAtColumn).Value = subscription.SubscribedAt.ToString("yyyy-MM-dd");
            worksheet.Cell(rowIndex, HasCertificateColumn).Value = hasCertificate ? "true" : "false";
        }
    }
}