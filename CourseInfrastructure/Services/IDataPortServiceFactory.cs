using CourseDomain.Model;
using static CourseInfrastructure.Services.IImportService;

namespace CourseInfrastructure.Services
{
    public interface IDataPortServiceFactory
    {
        public interface IDataPortServiceFactory<TEntity>
        {
            IImportService<TEntity> GetImportService(string contentType);
            IExportService<TEntity> GetExportService(string contentType);
        }


    }
}
