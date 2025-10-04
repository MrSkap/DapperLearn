using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using ILogger = Serilog.ILogger;

namespace DapperStudy.Api.Filters;

/// <summary>
///     Фильтр логгирования админских запросов.
/// </summary>
/// <remarks>Навешивать на эндпоинты требующие админские права.</remarks>
/// <example>
///     <code>[ServiceFilter(typeof(AdminFilter))]</code>
/// </example>
public class AdminFilter : IActionFilter
{
    private static readonly ILogger Logger = Log.ForContext<AdminFilter>();

    public void OnActionExecuting(ActionExecutingContext context)
    {
        Logger.Information("Admin {User} call endpoint {Endpoint} with body {Body}",
            context.HttpContext.User,
            context.HttpContext.Request.Path.Value,
            context.HttpContext.Request.Body);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.HttpContext.Response.StatusCode == 200)
        {
            Logger.Information("Admin {User} get response from endpoint {Endpoint} with body {Body}",
                context.HttpContext.User.Identity?.Name,
                context.HttpContext.Request.Path.Value,
                context.HttpContext.Response.Body);
            return;
        }

        Logger.Warning("Admin {User} get bad response from endpoint {Endpoint} with status code {Code}",
            context.HttpContext.User,
            context.HttpContext.Request.Path.Value,
            context.HttpContext.Response.StatusCode);
    }
}