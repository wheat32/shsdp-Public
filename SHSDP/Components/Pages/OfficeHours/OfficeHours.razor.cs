using System.Text.Json;
using Microsoft.AspNetCore.Components;
using SHSDP.Code.Bases;

namespace SHSDP.Components.Pages.OfficeHours;

public partial class OfficeHours : PageBase
{
    private record OfficeHoursConfig(
        String DayOfWeek, 
        String StartTime, 
        String EndTime, 
        String TermStartDate, 
        String TermEndDate,
        bool ExcludeFirstWeek,
        bool ExcludeLastWeek,
        String[] DaysOff,
        String Link
    );

    private OfficeHoursConfig? Config { get; set; }
    private String? ErrorMessage { get; set; }
    private bool IsWithinSchedule { get; set; } = true;
    private bool IsDayOff { get; set; } = false;
    private bool IsFirstWeekExcluded { get; set; }
    private bool IsLastWeekExcluded { get; set; }
    private String? FormattedTermStartDate { get; set; }
    private String? FormattedTermEndDate { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        LoadConfig();
        CheckSchedule();
    }

    private void LoadConfig()
    {
        try
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            String baseDir = AppContext.BaseDirectory;
            String configPath = Path.Combine(baseDir, "Components/Pages/OfficeHours/office-hours.config.json");
            
            if (File.Exists(configPath) == false)
            {
                ErrorMessage = "Configuration file not found.";
                Console.Error.WriteLine($"[ERROR] Config file not found: {configPath}");
                return;
            }

            String json = File.ReadAllText(configPath);
            Config = JsonSerializer.Deserialize<OfficeHoursConfig>(json, jsonOptions);

            if (Config == null)
            {
                ErrorMessage = "Failed to load configuration.";
                Console.Error.WriteLine($"[ERROR] Failed to deserialize config: {configPath}");
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error loading configuration: {ex.Message}";
            Console.Error.WriteLine($"[ERROR] Exception reading office hours config: {ex.Message}");
        }
    }

    private void CheckSchedule()
    {
        if (Config == null)
        {
            IsWithinSchedule = false;
            return;
        }

        DateTime today = DateTime.Now.Date;

        try
        {
            DateTime termStart = DateTime.Parse(Config.TermStartDate);
            DateTime termEnd = DateTime.Parse(Config.TermEndDate);

            // Format dates for display as MM/DD/YYYY
            FormattedTermStartDate = termStart.ToString("MM/dd/yyyy");
            FormattedTermEndDate = termEnd.ToString("MM/dd/yyyy");

            // Check if we're within the term
            if (today < termStart || today > termEnd)
            {
                IsWithinSchedule = false;
                return;
            }

            // Check if we're in the first week (excluded)
            if (Config.ExcludeFirstWeek)
            {
                DateTime firstWeekEnd = termStart.AddDays(7);
                if (today >= termStart && today < firstWeekEnd)
                {
                    IsFirstWeekExcluded = true;
                    IsWithinSchedule = false;
                    return;
                }
            }

            // Check if we're in the last week (excluded)
            if (Config.ExcludeLastWeek)
            {
                DateTime lastWeekStart = termEnd.AddDays(-7);
                if (today > lastWeekStart && today <= termEnd)
                {
                    IsLastWeekExcluded = true;
                    IsWithinSchedule = false;
                    return;
                }
            }

            // Check if today is a day off
            foreach (String dayOff in Config.DaysOff)
            {
                if (DateTime.TryParse(dayOff, out DateTime dayOffDate) && dayOffDate == today)
                {
                    IsDayOff = true;
                    IsWithinSchedule = false;
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"[ERROR] Exception parsing dates: {ex.Message}");
            IsWithinSchedule = false;
        }
    }
}