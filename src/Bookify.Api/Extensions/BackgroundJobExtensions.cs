using Bookify.Infrastructure.Outbox;
using Hangfire;

namespace Web.Api.Extensions;

internal static class BackgroundJobExtensions
{
    public static IApplicationBuilder UseBackgroundJobs(this WebApplication app)
    {
        IRecurringJobManager recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();

        recurringJobManager.AddOrUpdate<IProcessOutboxMessagesJob>(
            "outbox-processor",
            job => job.ProcessAsync(),
            app.Configuration["BackgroundJobs:Outbox:Schedule"]);

        return app;
    }
}
