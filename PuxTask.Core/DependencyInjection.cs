using PuxTask.Abstract;
using PuxTask.Core;

namespace Microsoft.Extensions.DependencyInjection;
public static class DependencyInjection
{
    public static IServiceCollection AddProjectServices(this IServiceCollection services)
    {
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IRecordService, RecordService>();
        services.AddScoped<IReportService, ReportService>();
        return services;
    }
}
