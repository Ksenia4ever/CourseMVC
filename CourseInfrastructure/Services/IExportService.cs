using CourseDomain.Model;

namespace CourseInfrastructure.Services
{
  public interface IExportService<TEntity>
    {
        Task WriteToAsync(Stream stream, CancellationToken cancellationToken);
    }

}
