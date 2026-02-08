using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using SHSDP.API.DbContexts;
using SHSDP.API.Services;
using SHSDP.API.Swagger;

namespace SHSDP.API;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "TopSchool Core API", Version = "v1" });

            // Set the comments path for the Swagger JSON and UI.
            String xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            String xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);

            // Add Bearer token authentication
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "Custom",
                In = ParameterLocation.Header,
                Description = "Enter your valid token in the text input below."
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });

            // Register custom operation filter for authorization responses
            c.OperationFilter<AuthorizeResponsesOperationFilter>();

            // Register custom schema filter for enums to be represented as strings
            c.SchemaFilter<EnumAsStringSchemaFilter>();
        });

        #region Custom Services
        builder.Services.AddAuthentication("LoginTokenScheme")
            .AddScheme<AuthenticationSchemeOptions, LoginTokenAuthenticationHandlerSvc>("LoginTokenScheme", options => { });
        builder.Services.AddAuthorization();
        #endregion
        
        #region Database Contexts
        builder.Services.AddDbContext<SHSDPDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("SHSDPDB")!, 
                o => o.UseParameterizedCollectionMode(ParameterTranslationMode.Constant));
            options.UseLazyLoadingProxies();
#if DEBUG
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
#endif
        });
        #endregion
        
        #region CORS
        // Allow CORS
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(pb =>
            {
                pb.AllowAnyOrigin()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        #endregion

        WebApplication app = builder.Build();

        app.UseCors();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}