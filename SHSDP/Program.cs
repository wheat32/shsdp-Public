using SHSDP.Components;

namespace SHSDP;

public class Program
{
    #region Globals
    public static String GITHUB_PAT { get; private set; }
    public static String GITHUB_CLASSROOM_ORG { get; private set; }
    public static String GITHUB_CLIENT_ID { get; private set; }
    public static String GITHUB_CLIENT_SECRET { get; private set; }
    public static String API_BASE_URL { get; private set; }
    #endregion
    
    public static void Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        builder.Services.AddHttpClient();
        
        builder.Services.AddControllers();
        
        builder.Services.AddScoped<SHSDP.Services.ProgramCompletionService>();
        
        SetupGlobals(builder);

        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() == false)
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapControllers(); 
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }

    private static void SetupGlobals(WebApplicationBuilder builder)
    {
        GITHUB_PAT = builder.Configuration["GitHubPAT"]!;
        if (String.IsNullOrWhiteSpace(GITHUB_PAT))
        {
            Console.Error.WriteLine("The GitHub PAT is not configured. Please set the 'GitHubPAT' environment variable.");
            return;
        }
        GITHUB_CLASSROOM_ORG = builder.Configuration["GitHubClassroomOrg"]!;
        if (String.IsNullOrWhiteSpace(GITHUB_CLASSROOM_ORG))
        {
            Console.Error.WriteLine("The GitHub Classroom organization is not configured. Please set the 'GitHubClassroomOrg' environment variable.");
            return;
        }
#if DEBUG
        GITHUB_CLIENT_ID = builder.Configuration["GitHubClientId_Local"]!;
#else
        GITHUB_CLIENT_ID = builder.Configuration["GitHubClientId_Prod"]!;
#endif
        if (String.IsNullOrWhiteSpace(GITHUB_CLIENT_ID))
        {
            Console.Error.WriteLine("The GitHub OAuth Client ID is not configured. Please set the 'GitHubClientId' environment variable.");
            return;
        }
#if DEBUG
        GITHUB_CLIENT_SECRET = builder.Configuration["GitHubClientSecret_Local"]!;
#else
        GITHUB_CLIENT_SECRET = builder.Configuration["GitHubClientSecret_Prod"]!;
#endif
        if (String.IsNullOrWhiteSpace(GITHUB_CLIENT_SECRET))
        {
            Console.Error.WriteLine("The GitHub OAuth Client Secret is not configured. Please set the 'GitHubClientSecret' environment variable.");
            return;
        }
        
#if DEBUG
        API_BASE_URL = builder.Configuration["API_URL_DEV"]!;
#else
        API_BASE_URL = builder.Configuration["API_URL_PROD"]!;
#endif
    }
}