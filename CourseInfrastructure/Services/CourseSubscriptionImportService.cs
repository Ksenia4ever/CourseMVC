using ClosedXML.Excel;
using CourseDomain.Model;
using Microsoft.EntityFrameworkCore;
using static CourseInfrastructure.Services.IImportService;

namespace CourseInfrastructure.Services
{
    public class CourseSubscriptionImportService : IImportService<CourseAccount>
    {
        private readonly DbCourseContext _context;

        private const int StudentEmailColumn = 1;
        private const int StudentNameColumn = 2;
        private const int CourseTitleColumn = 3;
        private const int SubjectColumn = 4;
        private const int AuthorEmailColumn = 5;
        private const int AuthorNameColumn = 6;
        private const int SubscribedAtColumn = 7;
        private const int HasCertificateColumn = 8;

        public CourseSubscriptionImportService(DbCourseContext context)
        {
            _context = context;
        }

        public async Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken)
        {
            if (!stream.CanRead)
            {
                throw new ArgumentException("Дані не можуть бути прочитані", nameof(stream));
            }

            try
            {
                using var workbook = new XLWorkbook(stream);

                foreach (IXLWorksheet worksheet in workbook.Worksheets)
                {
                    foreach (var row in worksheet.RowsUsed().Skip(1))
                    {
                        await ImportRowAsync(row, cancellationToken);
                    }
                }

                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DataImportException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DataImportException("Помилка під час імпорту Excel-файлу.", ex);
            }
        }

        private async Task ImportRowAsync(IXLRow row, CancellationToken cancellationToken)
        {
            var studentEmail = GetStudentEmail(row);
            var studentName = GetStudentName(row);
            var courseTitle = GetCourseTitle(row);
            var subject = GetSubject(row);
            var authorEmail = GetAuthorEmail(row);
            var authorName = GetAuthorName(row);
            var subscribedAt = GetSubscribedAt(row);
            var hasCertificate = GetHasCertificate(row);

            if (string.IsNullOrWhiteSpace(studentEmail) ||
                string.IsNullOrWhiteSpace(studentName) ||
                string.IsNullOrWhiteSpace(courseTitle) ||
                string.IsNullOrWhiteSpace(authorEmail) ||
                string.IsNullOrWhiteSpace(authorName))
            {
                return;
            }

            var student = await GetOrCreateAccountAsync(studentEmail, studentName, cancellationToken);
            var author = await GetOrCreateAccountAsync(authorEmail, authorName, cancellationToken);
            var course = await GetOrCreateCourseAsync(courseTitle, subject, author.Id, cancellationToken);

            await GetOrCreateSubscriptionAsync(course.Id, student.Id, subscribedAt, cancellationToken);

            if (hasCertificate)
            {
                await EnsureCertificateAsync(course, student, cancellationToken);
            }
        }

        private async Task<Account> GetOrCreateAccountAsync(
            string email,
            string name,
            CancellationToken cancellationToken)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email, cancellationToken);

            if (account != null)
            {
                return account;
            }

            account = new Account
            {
                Name = name,
                Email = email
            };

            _context.Accounts.Add(account);
            await _context.SaveChangesAsync(cancellationToken);

            return account;
        }

        private async Task<Course> GetOrCreateCourseAsync(
            string courseTitle,
            string subject,
            int authorId,
            CancellationToken cancellationToken)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Title == courseTitle, cancellationToken);

            if (course != null)
            {
                return course;
            }

            course = new Course
            {
                Title = courseTitle,
                Description = $"Імпортований курс {courseTitle}",
                Subject = string.IsNullOrWhiteSpace(subject) ? "Без предмету" : subject,
                AuthorId = authorId,
                Created = DateOnly.FromDateTime(DateTime.Today),
                Modified = DateOnly.FromDateTime(DateTime.Today)
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync(cancellationToken);

            return course;
        }

        private async Task GetOrCreateSubscriptionAsync(
            int courseId,
            int accountId,
            DateTime subscribedAt,
            CancellationToken cancellationToken)
        {
            var subscription = await _context.CourseAccounts
                .FirstOrDefaultAsync(
                    ca => ca.CourseId == courseId && ca.AccountId == accountId,
                    cancellationToken);

            if (subscription != null)
            {
                return;
            }

            subscription = new CourseAccount
            {
                CourseId = courseId,
                AccountId = accountId,
                SubscribedAt = subscribedAt
            };

            _context.CourseAccounts.Add(subscription);
        }

        private async Task EnsureCertificateAsync(
            Course course,
            Account student,
            CancellationToken cancellationToken)
        {
            var certificateExists = await _context.Certificates.AnyAsync(c => c.CourseId == course.Id && c.AccountId == student.Id, cancellationToken);

            if (certificateExists)
            {
                return;
            }

            var certificate = new Certificate
            {
                Title = $"Сертифікат курсу {course.Title}",
                Description = $"Сертифікат для {student.Name} за курс {course.Title}",
                CourseId = course.Id,
                AccountId = student.Id,
                IssuedDate = DateTime.Now
            };

            _context.Certificates.Add(certificate);
        }

        private static string GetStudentEmail(IXLRow row)
        {
            return row.Cell(StudentEmailColumn).GetString().Trim();
        }

        private static string GetStudentName(IXLRow row)
        {
            return row.Cell(StudentNameColumn).GetString().Trim();
        }

        private static string GetCourseTitle(IXLRow row)
        {
            return row.Cell(CourseTitleColumn).GetString().Trim();
        }

        private static string GetSubject(IXLRow row)
        {
            return row.Cell(SubjectColumn).GetString().Trim();
        }

        private static string GetAuthorEmail(IXLRow row)
        {
            return row.Cell(AuthorEmailColumn).GetString().Trim();
        }

        private static string GetAuthorName(IXLRow row)
        {
            return row.Cell(AuthorNameColumn).GetString().Trim();
        }

        private static DateTime GetSubscribedAt(IXLRow row)
        {
            var text = row.Cell(SubscribedAtColumn).GetString().Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                return DateTime.Now;
            }

            if (DateTime.TryParse(text, out var subscribedAt))
            {
                return subscribedAt;
            }

            throw new DataImportException($"Невірний формат дати підписки у рядку {row.RowNumber()}.");
        }

        private static bool GetHasCertificate(IXLRow row)
        {
            var text = row.Cell(HasCertificateColumn).GetString().Trim();

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (bool.TryParse(text, out var hasCertificate))
            {
                return hasCertificate;
            }

            throw new DataImportException($"Невірне значення HasCertificate у рядку {row.RowNumber()}.");
        }
    }
}