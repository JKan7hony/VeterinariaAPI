using System.Net;
using System.Text.Json;

namespace VeterinariaAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continúa con la ejecución de la petición
                await _next(context);
            }
            catch (Exception ex)
            {
                // Registra el error en la consola o archivo log
                _logger.LogError(ex, "Excepción no controlada capturada por GlobalExceptionMiddleware.");

                // Genera la respuesta estandarizada
                await ManejarExcepcionAsync(context, ex);
            }
        }

        private static Task ManejarExcepcionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var respuesta = new
            {
                status = context.Response.StatusCode,
                error = "Error Interno del Servidor",
                message = "Ocurrió un error inesperado al procesar la solicitud.",
                detalle = exception.Message
            };

            var jsonOpciones = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            return context.Response.WriteAsync(JsonSerializer.Serialize(respuesta, jsonOpciones));
        }
    }
}