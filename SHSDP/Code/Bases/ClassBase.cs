using System.Text.Json;
using Microsoft.AspNetCore.Components;
using SHSDP.Shared;
using SHSDP.Shared.Models.API;

namespace SHSDP.Code.Bases;

public abstract class ClassBase : PageBase
{
    protected record ClassConfig(
        Course Course, 
        CourseSyllabus Syllabus, 
        List<AssignmentRuntime> AssignmentRuntime
    );

    private static Dictionary<String, ClassConfig> ConfigCache = [];
    private static DateTime nextCacheRefresh = DateTime.MinValue;

    protected ClassConfig? Config { get; private set; }
    protected bool IsDisabled => Config?.Course.CourseDisabled == true;

    [Inject] 
    protected NavigationManager Nav { get; set; } = null!;
    [Inject]
    private IHttpClientFactory? HttpClientFactory { get; set; }

    protected override async Task OnInitializedAsync()
    {
        if(DateTime.UtcNow >= nextCacheRefresh || ConfigCache.Count == 0)
        {
            await RefreshPages();
            nextCacheRefresh = DateTime.UtcNow.AddMinutes(10);
        }
    }

    private async Task RefreshPages()
    {
        HttpClient client = HttpClientFactory!.CreateClient();
        ApiArbitrator apiArbitrator = new ApiArbitrator(client);
        GetAllCourseConfigurationsResponse resp = await apiArbitrator.GetFromApiAsync<GetAllCourseConfigurationsResponse>($"{Program.API_BASE_URL}v1/Course/GetAllCourseConfigurations", Constants.AUTH_TOKEN);
        ConfigCache = resp.Configurations.ToDictionary(
            c => c.Course.CourseCode, 
            c => new ClassConfig(c.Course, c.Syllabus, c.AssignmentRuntime), 
            StringComparer.OrdinalIgnoreCase
        );
    }
    
    protected async Task LoadConfig(String courseCode)
    {
        if (ConfigCache.TryGetValue(courseCode, out ClassConfig? config))
        {
            Config = config;
        }
        else
        {
            HttpClient client = HttpClientFactory!.CreateClient();
            ApiArbitrator apiArbitrator = new ApiArbitrator(client);
            GetCourseConfigurationRequest req = new(courseCode);
            GetCourseConfigurationResponse? resp = await apiArbitrator.GetFromApiAsync<GetCourseConfigurationResponse?>($"{Program.API_BASE_URL}v1/Course/GetCourseConfiguration", Constants.AUTH_TOKEN, req);
            Config = resp == null ? null : new ClassConfig(resp.Course, resp.Syllabus, resp.AssignmentRuntime);

            if (ConfigCache.ContainsKey(courseCode) == false)
            {
                ConfigCache.Add(courseCode, Config!);
            }
        }
    }

    protected void RedirectIfDisabled()
    {
        if (IsDisabled)
        {
            Nav.NavigateTo("/Classes", forceLoad: true);
        }
    }
}