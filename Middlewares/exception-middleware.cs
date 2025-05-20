using System.Net;
using System.Text.Json;


namespace api_econsulta.Middlewares
{
    public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;
        
       
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro não tratado ocorreu durante a execução da requisição");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            
            var response = new 
            {
                error = new 
                {
                    message = "Ocorreu um erro ao processar sua solicitação.",
                    traceId = context.TraceIdentifier
                }
            };

          
            context.Response.StatusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                InvalidOperationException => (int)HttpStatusCode.BadRequest,
                _ => (int)HttpStatusCode.InternalServerError,
            };

      
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, _jsonOptions));
        }
    }
}