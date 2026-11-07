using CoreGoDelivery.Api.Conveters;
using CoreGoDelivery.Api.Swagger;
using CoreGoDelivery.Application;
using CoreGoDelivery.Infrastructure.Database.Services;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Diagnostics;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var jwtAuthority = builder.Configuration["Jwt:Authority"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var requireHttpsMetadata = builder.Configuration.GetValue("Jwt:RequireHttpsMetadata", !builder.Environment.IsDevelopment());

if (!Uri.TryCreate(jwtAuthority, UriKind.Absolute, out var authorityUri) || string.IsNullOrWhiteSpace(jwtAudience))
{
    throw new InvalidOperationException("Jwt:Authority and Jwt:Audience must be configured for the trusted identity provider.");
}

if (!builder.Environment.IsDevelopment() && (authorityUri.Scheme != Uri.UriSchemeHttps || !requireHttpsMetadata))
{
    throw new InvalidOperationException("Production JWT authority and metadata must use HTTPS.");
}

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.DefaultBufferSize = 4096;

    options.JsonSerializerOptions.Converters.Add(new TrimStringJsonConverter());
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.Converters.Add(new CustomDateTimeConverter());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtAuthority;
        options.Audience = jwtAudience;
        options.RequireHttpsMetadata = requireHttpsMetadata;
        options.MapInboundClaims = false;
        options.IncludeErrorDetails = builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = $"Core Motorcycle, Rental, Deliverier - {builder.Environment.EnvironmentName}",
        Version = "v1"
    });
    c.CustomSchemaIds(type => type.ToString());
    c.OperationFilter<DefaultValuesOperation>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Paste an access token issued by the configured OAuth/OIDC provider.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Type = SecuritySchemeType.Http
    });
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
builder.Services.AddApplication(builder.Configuration);

builder.WebHost.ConfigureKestrel(options =>
{
    var kestrelPort = builder.Configuration.GetValue<int?>("Kestrel:Port");
    if (kestrelPort is not > 0 or > 65535)
    {
        throw new InvalidOperationException("Kestrel:Port must be configured with a valid TCP port.");
    }

    options.ListenAnyIP(kestrelPort.Value);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await ExecutePendingMigration.ExecuteAsync(app.Services);
}

app.UseRouting();
app.Use(async (context, next) =>
{
    var requestLogger = context.RequestServices
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("CoreGoDelivery.Api.Requests");
    var endpoint = context.GetEndpoint()?.DisplayName ?? "unmatched";
    var traceId = context.TraceIdentifier;
    var elapsed = Stopwatch.StartNew();
    context.Response.Headers["X-Correlation-ID"] = traceId;

    try
    {
        await next();
    }
    finally
    {
        elapsed.Stop();
        var statusCode = context.Response.StatusCode;
        var level = statusCode >= StatusCodes.Status500InternalServerError
            ? LogLevel.Error
            : LogLevel.Information;

        requestLogger.Log(
            level,
            "HTTP {HttpMethod} {Endpoint} completed with {StatusCode} in {ElapsedMs} ms TraceId={TraceId}",
            context.Request.Method,
            endpoint,
            statusCode,
            elapsed.ElapsedMilliseconds,
            traceId);
    }
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Logger.LogInformation("Starting application in {EnvironmentName}", app.Environment.EnvironmentName);
await app.RunAsync();
