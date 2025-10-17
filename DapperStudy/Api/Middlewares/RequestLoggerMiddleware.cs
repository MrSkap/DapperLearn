using System.Diagnostics;
using System.Text;
using Serilog;
using ILogger = Serilog.ILogger;

namespace DapperStudy.Api.Middlewares;

/// <summary>
///     Миддлевара для логирования всех запросов. Пишет все в Verbose.
/// </summary>
/// <remarks>Большие запросы пропускает, просто пишет их тип, если получится.</remarks>
public class RequestLoggerMiddleware
{
    private static readonly ILogger Logger = Log.ForContext<RequestLoggerMiddleware>();
    private readonly RequestDelegate _next;

    public RequestLoggerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = await FormatRequest(context.Request);

        // Логируем входящий запрос
        Logger.Verbose("""
                       Incoming Request:
                       Method: {Method}
                       Path: {Path}
                       QueryString: {QueryString}
                       Headers: {Headers}
                       Body: {Body}
                       Timestamp: {Timestamp}
                       """,
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            FormatHeaders(context.Request.Headers),
            request,
            DateTime.UtcNow);

        // Сохраняем оригинальный поток ответа
        var originalResponseBody = context.Response.Body;

        using var responseBody = new MemoryStream();
        context.Response.Body = responseBody;

        try
        {
            await _next(context);
            stopwatch.Stop();
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            HandleExceptionAsync(context, ex, stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            // Логируем исходящий ответ
            var response = await FormatResponse(context.Response);

            Logger.Verbose("""
                           Outgoing Response:
                           Status Code: {StatusCode}
                           Headers: {ResponseHeaders}
                           Body: {ResponseBody}
                           Duration: {Duration}ms
                           Timestamp: {ResponseTimestamp}
                           """,
                context.Response.StatusCode,
                FormatHeaders(context.Response.Headers),
                response,
                stopwatch.ElapsedMilliseconds,
                DateTime.UtcNow);

            // Возвращаем поток ответа на место
            await responseBody.CopyToAsync(originalResponseBody);
            context.Response.Body = originalResponseBody;
        }
    }

    private async Task<string> FormatRequest(HttpRequest request)
    {
        request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(request.ContentLength ?? 0)];

        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);

        // Возвращаем поток в начало для последующего чтения
        request.Body.Position = 0;

        // Не логируем слишком большие тела или бинарные данные
        if (bodyAsText.Length > 2048 || IsBinaryContent(request.ContentType))
            return GetSkipString(bodyAsText.Length, request.ContentType);

        return string.IsNullOrEmpty(bodyAsText) ? "[Empty]" : bodyAsText;
    }

    private async Task<string> FormatResponse(HttpResponse response)
    {
        response.Body.Seek(0, SeekOrigin.Begin);

        var text = await new StreamReader(response.Body).ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        if (text.Length > 2048 || IsBinaryContent(response.ContentType))
            return GetSkipString(text.Length, response.ContentType);

        return string.IsNullOrEmpty(text) ? "[Empty]" : text;
    }

    private string FormatHeaders(IHeaderDictionary headers)
    {
        var headerList = headers.Select(header =>
            $"{header.Key}: {string.Join(", ", header.Value.ToString())}");

        return string.Join("; ", headerList);
    }

    private bool IsBinaryContent(string? contentType)
    {
        if (string.IsNullOrEmpty(contentType))
            return false;

        var binaryTypes = new[]
        {
            "image/", "video/", "audio/", "application/octet-stream",
            "application/pdf", "application/zip", "application/gzip"
        };

        return binaryTypes.Any(contentType.StartsWith);
    }

    private void HandleExceptionAsync(HttpContext context, Exception exception, long duration)
    {
        Log.Error(exception, """
                               Request failed:
                               Method: {Method}
                               Path: {Path}
                               Duration: {Duration}ms
                               Error: {ErrorMessage}
                               StackTrace: {StackTrace}
                               """,
            context.Request.Method,
            context.Request.Path,
            duration,
            exception.Message,
            exception.StackTrace);
    }

    private string GetSkipString(int length, string? contentType)
    {
        return $"[Body skipped - Length: {length}, Content-Type: {contentType}]";
    }
}