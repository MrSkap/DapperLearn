using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using ILogger = Serilog.ILogger;

namespace DapperStudy.Api.Filters;

/// <summary>
///     Фильтр необработанных ошибок.
/// </summary>
/// <remarks>
///     Обрабатывает ошибки и выставляет нужный коды ошибки.
///     Для Development среды пишет стек трейс и логгирует ошибки более подробно.
/// </remarks>
public class BadResponseFilter : IExceptionFilter
{
    private static readonly ILogger Logger = Log.ForContext<AdminFilter>();
    private readonly IWebHostEnvironment _environment;

    public BadResponseFilter(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.ExceptionHandled) return;

        if (_environment.IsDevelopment())
            Logger.Error(context.Exception,
                "Catch unhandled error.\n User: {User}\n Endpoint: {Endpoint} \n Request: {Request}",
                context.HttpContext.User.Identity?.Name,
                context.HttpContext.Request.Path.Value,
                context.HttpContext.Response.Body);
        else
            Logger.Error(context.Exception, "Catch unhandled error.\n Endpoint: {Endpoint}",
                context.HttpContext.User.Identity?.Name);


        var problemDetails = GetProblemDetails(context);

        context.Result = new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };

        context.ExceptionHandled = true;
    }

    private ProblemDetails GetProblemDetails(ExceptionContext context)
    {
        var problemDetails = ProcessCustomExceptions(context) ?? GetDefaultProblemDetails(context);

        if (!_environment.IsDevelopment()) return problemDetails;

        problemDetails.Detail = context.Exception.ToString();
        problemDetails.Extensions.Add("stackTrace", context.Exception.StackTrace);

        return problemDetails;
    }

    private ProblemDetails GetDefaultProblemDetails(ExceptionContext context)
    {
        return new ProblemDetails
        {
            Title = "Unhandled error",
            Status = (int)HttpStatusCode.InternalServerError,
            Instance = context.HttpContext.Request.Path
        };
    }

    private ProblemDetails? ProcessCustomExceptions(ExceptionContext context)
    {
        return context.Exception switch
        {
            ArgumentException argEx => new ProblemDetails
            {
                Title = "Wrong request arguments", Status = (int)HttpStatusCode.BadRequest, Detail = argEx.Message
            },
            KeyNotFoundException notFoundEx => new ProblemDetails
            {
                Title = "Key not found", Status = (int)HttpStatusCode.BadRequest, Detail = notFoundEx.Message
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Title = "Unauthorized",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "Not enough rights to execute"
            },
            NotImplementedException => new ProblemDetails
            {
                Title = "Not implemented",
                Status = (int)HttpStatusCode.BadRequest,
                Detail = "The feature is not implemented"
            },
            _ => null
        };
    }
}