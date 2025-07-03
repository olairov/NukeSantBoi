using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public PlatformSettings mobileSettings;
    public PlatformSettings pcSettings;

    private PlatformSettings currentSettings;

    void Awake()
    {
        if (Application.isMobilePlatform)
            currentSettings = mobileSettings;
        else
            currentSettings = pcSettings;

        ApplySettings(currentSettings);
    }

    void ApplySettings(PlatformSettings settings)
    {
        Application.targetFrameRate = settings.targetFramerate;
    }

    public PlatformSettings GetCurrentSettings()
    {
        return currentSettings;
    }
}