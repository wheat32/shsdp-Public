using System.Text.Json;
using Microsoft.AspNetCore.Components;
using SHSDP.Code;
using SHSDP.Shared;
using SHSDP.Shared.Models.API;

namespace SHSDP.Components.Pages.Classes;

public partial class Index : ComponentBase
{
    [Inject]
    private IHttpClientFactory? HttpClientFactory { get; set; }
    
    private record PageInfo(String DisplayName, String Url);

    private static List<PageInfo> ClassPages { get; set; } = [];
    private static DateTime nextRefresh = DateTime.MinValue;

    protected override async Task OnInitializedAsync()
    {
        if(DateTime.UtcNow >= nextRefresh || ClassPages.Count == 0)
        {
            await RefreshPages();
            nextRefresh = DateTime.UtcNow.AddMinutes(10);
        }
    }

    private async Task RefreshPages()
    {
        HttpClient client = HttpClientFactory!.CreateClient();
        ApiArbitrator apiArbitrator = new ApiArbitrator(client);
        GetCoursesResponse resp = await apiArbitrator.GetFromApiAsync<GetCoursesResponse>($"{Program.API_BASE_URL}v1/Course/GetCourses", Constants.AUTH_TOKEN);
        ClassPages = resp.Courses.Select(c => new PageInfo($"{c.CourseCode}: {c.CourseName}", $"/Classes/{c.CourseCode}")).ToList();
    }
}