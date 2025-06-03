using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Contracts;
using NLog;
using HalisahaOtomasyon.Extensions;
using HalisahaOtomasyon.ActionFilters;
using Service.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Load NLog configuration
LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));

// Configure services using extensions
builder.Services.ConfigureRedis(builder.Configuration);

builder.Services.ConfigureCors();

builder.Services.ConfigureISSIntegration();

builder.Services.ConfigureLoggerService();

builder.Services.ConfigureRepositoryManager();

builder.Services.ConfigureCodeGenerator();

builder.Services.ConfigureServiceManager();


// builder.Services.ConfigureSqlContext(builder.Configuration); SQLite içindi
builder.Services.ConfigureMySqlContext(builder.Configuration);
//"MySqlConnection": "server=shinkansen.proxy.rlwy.net;port=50868;database=railway;user=root;password=dxWpjBoGmtRRdlULSudVnMjvioaomEYm;"

builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddAutoMapper(typeof(Program));


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


builder.Services.AddScoped<ValidationFilterAttribute>();


builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = false;
    config.ReturnHttpNotAcceptable = true;
    config.InputFormatters.RemoveType<TextInputFormatter>(); // text/plain'i devre dışı bırak
    config.OutputFormatters.RemoveType<TextOutputFormatter>();
    config.OutputFormatters.RemoveType<XmlDataContractSerializerOutputFormatter>();
}).AddApplicationPart(typeof(HalisahaOtomasyonPresentation.AssemblyReference).Assembly);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    //c.EnableAnnotations();
    //c.OperationFilter<DefaultResponseContentTypeFilter>();

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Halısaha API",
        Version = "v1",
        Description = "Halısaha otomasyonunun apisi"
    });

    // JWT Authentication
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: 'Bearer eyJhbGciOiJIUzI1NiIs...'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});



var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerManager>();
app.ConfigureExceptionHandler(logger);


if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Halısaha API V1");

        //c.DefaultModelRendering(Swashbuckle.AspNetCore.SwaggerUI.ModelRendering.Model);
    });
}

if (app.Environment.IsProduction())
    app.UseHsts();

// var logger = app.Services.GetRequiredService<ILoggerManager>();
// app.ConfigureExceptionHandler(logger);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<NotificationHub>("/notificationhub");

app.Run();