using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using SHSDP.Code.Bases;
using SHSDP.Models;

namespace SHSDP.Components.Pages;

public partial class GitHubReinvite : PageBase
{
    [Inject] public IHttpClientFactory HttpClientFactory { get; set; }
    [Inject] public NavigationManager Nav { get; set; }

    private readonly String GitHubClientId = Program.GITHUB_CLIENT_ID;

#if DEBUG
    private const String RedirectUri = "https://localhost:7200/github-callback";
#else
    private const String RedirectUri = "https://shsdp.dev/github-callback";
#endif

    private bool IsAuthenticated;
    private String GitHubLogin;
    private bool IsLoadingClassrooms = false;
    private List<GHClassroom> Classrooms = [];

    private int? _selectedClassroomId;

    private int? SelectedClassroomId
    {
        get => _selectedClassroomId;
        set
        {
            if (_selectedClassroomId != value)
            {
                _selectedClassroomId = value;
                _ = LoadAssignmentsAsync(value!.Value); // fire and forget
            }
        }
    }

    private List<GHAssignment> Assignments = [];
    private bool IsLoadingAssignments = false;
    private int? SelectedAssignmentId;

    private bool IsFixingRepo = false;
    private String ActionMessage;
    private bool WasFixSuccessful = false;
    
    protected override async Task OnInitializedAsync()
    {
        // Check for login from redirect
        Uri uri = Nav.ToAbsoluteUri(Nav.Uri);
        var query = QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("login", out var login))
        {
            GitHubLogin = login!;
            IsAuthenticated = true;
            
            // Fetch classrooms
            IsLoadingClassrooms = true;
            await LoadClassroomsAsync();
        }
    }
    
    private void SignInWithGitHub()
    {
        String authUrl =
            $"https://github.com/login/oauth/authorize" +
            $"?client_id={GitHubClientId}" +
            $"&redirect_uri={Uri.EscapeDataString(RedirectUri)}" +
            $"&scope=read:user" +
            $"&prompt=consent";

        Nav.NavigateTo(authUrl, forceLoad: true); // full page redirect
    }

    private void SwitchUser()
    {
        GitHubLogin = String.Empty;
        IsAuthenticated = false;
        Nav.NavigateTo("/GitHubReinvite", forceLoad: true);
    }

    private async Task LoadClassroomsAsync()
    {
        HttpClient http = HttpClientFactory.CreateClient();
        http.BaseAddress = new Uri("https://api.github.com/");
        http.DefaultRequestHeaders.UserAgent.ParseAdd("ClassroomReinviteTool/1.0");
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Program.GITHUB_PAT);

        int page = 1;
        int perPage = 100;

        while (true)
        {
            HttpResponseMessage resp = await http.GetAsync($"classrooms?per_page={perPage}&page={page}");
            resp.EnsureSuccessStatusCode();

            String json = await resp.Content.ReadAsStringAsync();
            JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array || root.GetArrayLength() == 0)
                break;

            foreach (JsonElement c in root.EnumerateArray())
            {
                bool archived = c.TryGetProperty("archived", out JsonElement archivedEl) && archivedEl.GetBoolean();
                if (archived)
                {
                    continue;
                }
                
                String name = c.GetProperty("name").GetString()!;
                int id = c.GetProperty("id").GetInt32();

                if (c.TryGetProperty("organization", out JsonElement orgEl) &&
                    orgEl.TryGetProperty("login", out JsonElement loginEl))
                {
                    if (String.Equals(loginEl.GetString(), Program.GITHUB_CLASSROOM_ORG,
                            StringComparison.OrdinalIgnoreCase) == false)
                        continue;
                }

                Classrooms.Add(new GHClassroom { Id = id, Name = name });
            }

            page++;
        }

        IsLoadingClassrooms = false;
        StateHasChanged();

#if DEBUG
        Console.WriteLine(String.Join(Environment.NewLine, Classrooms));
#endif
    }
    
    private async Task LoadAssignmentsAsync(int classroomId)
    {
        IsLoadingAssignments = true;
        Assignments.Clear();
        ActionMessage = String.Empty;
        SelectedAssignmentId = null;

        HttpClient http = HttpClientFactory.CreateClient();
        http.BaseAddress = new Uri("https://api.github.com/");
        http.DefaultRequestHeaders.UserAgent.ParseAdd("ClassroomReinviteTool/1.0");
        http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Program.GITHUB_PAT);

        HttpResponseMessage resp = await http.GetAsync($"classrooms/{classroomId}/assignments");
        resp.EnsureSuccessStatusCode();

        String json = await resp.Content.ReadAsStringAsync();
        JsonDocument doc = JsonDocument.Parse(json);

        foreach (JsonElement a in doc.RootElement.EnumerateArray())
        {
            int id = a.GetProperty("id").GetInt32();
            String title = a.GetProperty("title").GetString()!;

            Assignments.Add(new GHAssignment { Id = id, Title = title });
        }

        Assignments = Assignments.OrderBy(a => a.Title, StringComparer.OrdinalIgnoreCase).ToList();
        IsLoadingAssignments = false;

        StateHasChanged();
#if DEBUG
        Console.WriteLine(String.Join(Environment.NewLine, Assignments));
#endif
    }
    
    private async Task FixAccessAsync()
    {
        IsFixingRepo = true;
        WasFixSuccessful = false;
        ActionMessage = String.Empty;

        try
        {
            HttpClient http = HttpClientFactory.CreateClient();
            http.BaseAddress = new Uri("https://api.github.com/");
            http.DefaultRequestHeaders.UserAgent.ParseAdd("ClassroomReinviteTool/1.0");
            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            http.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Program.GITHUB_PAT);

            String repoUrl = $"assignments/{SelectedAssignmentId}/accepted_assignments";
            HttpResponseMessage resp = await http.GetAsync(repoUrl);
            resp.EnsureSuccessStatusCode();

            String json = await resp.Content.ReadAsStringAsync();
            JsonDocument doc = JsonDocument.Parse(json);

            JsonElement match = default;
            bool found = false;

            foreach (JsonElement accepted in doc.RootElement.EnumerateArray())
            {
                if (accepted.TryGetProperty("students", out JsonElement students) &&
                    students.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement student in students.EnumerateArray())
                    {
                        if (student.TryGetProperty("login", out JsonElement loginEl) &&
                            loginEl.GetString()?.Equals(GitHubLogin, StringComparison.OrdinalIgnoreCase) == true)
                        {
                            match = accepted;
                            found = true;
                            break;
                        }
                    }
                }
                
                if (found) break;
            }
            
            if (found == false)
            {
                ActionMessage = "No matching repository found for this assignment.";
                return;
            }

            String repoFullName = match.GetProperty("repository").GetProperty("full_name").GetString()!;
            
            // Remove pending invitation if it exists
            HttpResponseMessage invResp = await http.GetAsync($"repos/{repoFullName}/invitations");
            if (invResp.IsSuccessStatusCode)
            {
                String invJson = await invResp.Content.ReadAsStringAsync();
                JsonDocument invDoc = JsonDocument.Parse(invJson);
                foreach (JsonElement invite in invDoc.RootElement.EnumerateArray())
                {
                    if (invite.TryGetProperty("invitee", out JsonElement invitee) &&
                        invitee.GetProperty("login").GetString()
                            ?.Equals(GitHubLogin, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        int id = invite.GetProperty("id").GetInt32();
                        await http.DeleteAsync($"repos/{repoFullName}/invitations/{id}");
                        break;
                    }
                }
            }

            // Check if user is a collaborator
            HttpResponseMessage collabCheck = await http.GetAsync($"repos/{repoFullName}/collaborators/{GitHubLogin}");
            if (collabCheck.IsSuccessStatusCode)
            {
                await http.DeleteAsync($"repos/{repoFullName}/collaborators/{GitHubLogin}");
            }
            
            // Re-invite with write access
            var inviteContent = new StringContent(JsonSerializer.Serialize(new { permission = "push" }));
            inviteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            
            HttpResponseMessage putResp =
                await http.PutAsync($"repos/{repoFullName}/collaborators/{GitHubLogin}", inviteContent);
            if (putResp.IsSuccessStatusCode == false)
            {
                ActionMessage = "Failed to re-invite user. GitHub returned: " + putResp.StatusCode;
                return;
            }

            // At the end of successful re-invite
            String linkToRepo = $"https://github.com/{repoFullName}";
            ActionMessage = $"""
                             Success! You have been removed and re-invited to the repository.
                             <br />
                             GitHub does not always send an email or dashboard notification.
                             Please try visiting the repository directly: 
                             <br />
                             <a href="{linkToRepo}" target="_blank" rel="noopener noreferrer">{linkToRepo}</a>
                             """;
            WasFixSuccessful = true;
        }
        catch (Exception ex)
        {
            ActionMessage = "An unexpected error occurred: " + ex.Message;
        }
        finally
        {
            IsFixingRepo = false;
            StateHasChanged();
        }
    }
}