using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NashAssetManagement.Domain.Constants;
using NashAssetManagement.Infrastructure.Jwt;

namespace NashAssetManagement.WebAPI.Configuration
{
    public static class AuthServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services)
        {
            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
        {
            // Configure Default Authentication Schema
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })

            // Configure Challenges And Forbidden Exceptions + Get Access Token from Cookie
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Cookies[JwtTokenConstants.CookieAccessToken];
                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine($"[JWT Diagnostics] Token validation failed: {context.Exception.Message}");
                            return Task.CompletedTask;
                        },

                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.Clear();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.Headers.Append("WWW-Authenticate", "Bearer");
                            context.Response.ContentType = "application/problem+json";
                            var problem = new ProblemDetails
                            {
                                Title = "Unauthorized",
                                Status = StatusCodes.Status401Unauthorized,
                                Type = "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                                Detail = "Authentication is required to access this resource."
                            };
                            await context.Response.WriteAsJsonAsync(problem);
                        },

                        OnForbidden = async context =>
                        {
                            context.Response.Clear();
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/problem+json";
                            var problem = new ProblemDetails
                            {
                                Title = "Forbidden",
                                Status = StatusCodes.Status403Forbidden,
                                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.3",
                                Detail = "You do not have permission to perform this action."
                            };
                            await context.Response.WriteAsJsonAsync(problem);
                        }
                    };
                });

            // Configure Jwt Validation Handler
            services
                .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<JwtOptions>>((libOptions, jwtOptions) =>
                {
                    libOptions.MapInboundClaims = false;
                    libOptions.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Value.Issuer,
                        ValidAudience = jwtOptions.Value.Audience,
                        ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecretKey)),
                        ClockSkew = TimeSpan.Zero, // expired access token + refreshtoken will be invalidated immediately
                        NameClaimType = JwtTokenConstants.Username,
                        RoleClaimType = JwtTokenConstants.Roles
                    };
                });

            return services;
        }
    }
}