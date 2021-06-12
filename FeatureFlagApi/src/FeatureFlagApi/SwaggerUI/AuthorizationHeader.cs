using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureFlagApi.SwaggerUI
{
	/// <summary>
	/// For Swagger UI we want to allow a JWT Bearer token to be set for all API requests as
	/// an OPTIONAL header
	/// </summary>
	public class AuthorizationHeader : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			operation.Parameters ??= new List<OpenApiParameter>();

			operation.Parameters.Add(new OpenApiParameter()
			{
				Name = "Authorization",
				In = ParameterLocation.Header,
				Description = "A valid JWT Bearer token",
				Required = false // set to false if this is optional
			});
		}
	}
}
