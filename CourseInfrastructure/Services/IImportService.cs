using CourseDomain.Model;

namespace CourseInfrastructure.Services
{
    public interface IImportService
    {
        public interface IImportService<TEntity>
        {
            Task ImportFromStreamAsync(Stream stream, CancellationToken cancellationToken);
        }

    }
}
