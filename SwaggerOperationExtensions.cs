using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Net;

namespace JusticeOne.Api.Swagger.Extensions
{
    public static class SwaggerOperationExtensions
    {
        public static OpenApiResponse WithCode(this OpenApiResponses responses, HttpStatusCode code)
        {
            var response = new OpenApiResponse();
            responses.Remove(((int)code).ToString());
            responses.Add(((int)code).ToString(), response);
            return response;
        }

        public static OpenApiResponse AndDescription(this OpenApiResponse response, string description)
        {
            response.Description = description;
            return response;
        }

        public static OpenApiResponse AndDefaultCreatedType(this OpenApiResponse response)
        {
            response.Content.Add("application/json", new OpenApiMediaType
            {
                Schema = new OpenApiSchema
                {
                    Type = "object",
                    Properties = new Dictionary<string, OpenApiSchema>()
                    {
                        {"id", new OpenApiSchema
                            {
                                Type = "string",
                                Format = "uuid",
                                Description = "Resource ID"
                            }
                        }
                    }
                }
            });

            return response;
        }

        public static OpenApiResponse AndResponseType(this OpenApiResponse response, OpenApiSchema schema)
        {
            response.Content.Add("application/json", new OpenApiMediaType
            {
                Schema = schema
            });

            return response;
        }
    }
}
