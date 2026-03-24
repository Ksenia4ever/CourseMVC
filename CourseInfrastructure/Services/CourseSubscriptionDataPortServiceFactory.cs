using CourseDomain.Model;
using static CourseInfrastructure.Services.IDataPortServiceFactory;
using static CourseInfrastructure.Services.IImportService;

namespace CourseInfrastructure.Services
{
    public class CourseSubscriptionDataPortServiceFactory : IDataPortServiceFactory<CourseAccount>
    {
        private readonly DbCourseContext _context;

        public CourseSubscriptionDataPortServiceFactory(DbCourseContext context)
        {
            _context = context;
        }

        public IImportService<CourseAccount> GetImportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new CourseSubscriptionImportService(_context);
            }

            throw new NotImplementedException(
                $"Імпорт для типу контенту {contentType} не реалізований.");
        }

        public IExportService<CourseAccount> GetExportService(string contentType)
        {
            if (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            {
                return new CourseSubscriptionExportService(_context);
            }

            throw new NotImplementedException(
                $"Експорт для типу контенту {contentType} не реалізований.");
        }
    }
}