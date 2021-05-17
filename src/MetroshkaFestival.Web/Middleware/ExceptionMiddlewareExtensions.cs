using System;
using System.Linq;
using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace MetroshkaFestival.Web.Middleware
{
    public static class ExceptionMiddlewareExtensions
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings;

        static ExceptionMiddlewareExtensions()
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            JsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            };
        }

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
                                    {
                                        appError.Run(async context =>
                                                    {
                                                        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                                                        context.Response.ContentType = "application/json";

                                                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                                                        if (contextFeature != null)
                                                        {
                                                            var response = HandleError(contextFeature, context);

                                                            Log.Error($"URL {context.Request.GetDisplayUrl()}", contextFeature.Error);

                                                            await context.Response.WriteAsync(response);
                                                        }
                                                    });
                                    });
        }

        private static string HandleError(IExceptionHandlerFeature contextFeature, HttpContext context)
        {
            string response;
            HttpStatusCode statusCode;
            switch (contextFeature.Error)
            {
                case ValidationException validationException:
                {
                    var validationFailure = validationException.Errors.First();
                    if (!string.IsNullOrEmpty(validationFailure.ErrorCode))
                    {
                        statusCode = HttpStatusCode.UnprocessableEntity;
                        var errors = validationException.Errors
                                                        .Where(x => !string.IsNullOrEmpty(x.ErrorCode))
                                                        .Select(x => new ValidationErrorModel
                                                         {
                                                             Field = x.PropertyName,
                                                             Message = x.ErrorMessage
                                                         })
                                                        .ToArray();
                        response = GetMultipleErrorsSerializedObject(errors);
                    }
                    else
                    {
                        statusCode = HttpStatusCode.BadRequest;
                        response = GetSingleErrorSerializedObject(validationFailure.ErrorMessage);
                    }

                    break;
                }
                case ArgumentException _:
                    statusCode = HttpStatusCode.BadRequest;
                    response = GetSingleErrorSerializedObject(contextFeature.Error.Message);
                    break;
                case ApplicationException applicationException:
                    statusCode = HttpStatusCode.OK;
                    response = GetSingleErrorSerializedObject(applicationException.Message);
                    break;
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    response = GetSingleErrorSerializedObject(contextFeature.Error.Message);
                    break;
            }

            context.Response.StatusCode = (int) statusCode;
            return response;
        }

        private static string GetSingleErrorSerializedObject(string errorMessage)
        {
            return SerializeObject(new
            {
                Success = false,
                Error = errorMessage
            });
        }

        private static string GetMultipleErrorsSerializedObject(ValidationErrorModel[] validationErrorModels)
        {
            return SerializeObject(new
            {
                Success = false,
                Errors = validationErrorModels.GroupBy(x => x.Field)
                                              .Select(x => new
                                               {
                                                   Field = x.Key,
                                                   Messages = x.Select(y => y.Message)
                                               })
            });
        }

        private static string SerializeObject<TValue>(TValue value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        private class ValidationErrorModel
        {
            public string Field { get; set; }
            public string Message { get; set; }
        }
    }
}