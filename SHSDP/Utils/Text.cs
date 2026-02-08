using System.Text.Json;

namespace SHSDP.Utils;

public class Text
{
    private static String? _versionString = null;
    public static String VersionString
    {
        get
        {
            if (_versionString == null)
            {
                // Read all text from the file
                String jsonString = System.IO.File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "version.json"));

                // Deserialize JSON into a dictionary
                var versionData = JsonSerializer.Deserialize<Dictionary<String, JsonElement>>(jsonString);

                int majorVersion = versionData!["MajorVersion"].GetInt32();
                int minorVersion = versionData["MinorVersion"].GetInt32();
                int patchNumber = versionData["PatchNumber"].GetInt32();
                int buildNumber = versionData["BuildNumber"].GetInt32();
                String lastCommitId = DebugUtils.IsDebugMode ? $" ({versionData["LastCommitID"].GetString()!})" : String.Empty;

                _versionString = $"v{majorVersion}.{minorVersion}.{patchNumber}:{buildNumber}{lastCommitId}";
            }
            return _versionString;
        }
    }
}