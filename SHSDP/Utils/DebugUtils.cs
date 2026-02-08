namespace SHSDP.Utils;

public static class DebugUtils
{
    private const bool ENABLE_DEBUG = true;

    private static bool IsDebugConfig
    {
        get
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }

    /// <summary>
    /// The debug mode is enabled if the project is using the Debug solution 
    /// configuration and the ENABLE_DEBUG constant (in the DebugUtils class) is true.
    /// </summary>
    public static bool IsDebugMode => IsDebugConfig && ENABLE_DEBUG;
}