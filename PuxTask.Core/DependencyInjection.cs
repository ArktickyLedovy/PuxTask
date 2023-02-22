using Microsoft.Extensions.DependencyInjection;
using PuxTask.Abstract;

namespace PuxTask.Core
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services) {
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IRecordService, RecordService>();
            services.AddScoped<IReportService, ReportService>();
            return services;
        }
    }
}
