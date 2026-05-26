using StarfallStudio.Config;
using StarfallStudio.Game.GPose;
using StarfallStudio.UI.Windows;
using System;

namespace StarfallStudio.Services;

public class WelcomeService : IDisposable
{
    private readonly GPoseService _gPoseService;

    public WelcomeService(ConfigurationService configService, MainWindow mainWindow, UpdateWindow updateWindow, GPoseService gPoseService)
    {
        _gPoseService = gPoseService;

        // Mark popup as seen without showing it - changelog auto-popup disabled
        if(configService.Configuration.PopupKey != Configuration.CurrentPopupKey)
        {
            configService.Configuration.PopupKey = Configuration.CurrentPopupKey;
        }

        if(configService.Configuration.Interface.OpenStarfallStudioBehavior == OpenStarfallStudioBehavior.OnPluginStartup)
        {
            mainWindow.IsOpen = true;
        }

        #region Configuration Reestablishment

        // Version 1
        if(configService.Configuration.Version == 1)
        {
            StarfallStudio.Log.Error($"Library sources need to be re-established! oldVerion:{configService.Configuration.Version} newVerion:{Configuration.CurrentVersion}");

            configService.Configuration.Library.Files.Clear();

            configService.Configuration.Library.ReEstablishDefaultPaths();

            configService.Configuration.Version = Configuration.CurrentVersion;

            StarfallStudio.Log.Warning($"Library sources have been re-established!");
        }

        if(configService.Configuration.Version <= 3)
        {
            configService.Configuration.Version = Configuration.CurrentVersion;
        }

        configService.ApplyChange();

        #endregion

        _gPoseService.OnGPoseStateChange += GPoseService_OnGPoseStateChange;
    }

    private void GPoseService_OnGPoseStateChange(bool newState)
    {
        if(newState)
        {

        }
    }

    public void Dispose()
    {
        _gPoseService.OnGPoseStateChange -= GPoseService_OnGPoseStateChange;
        GC.SuppressFinalize(this);
    }
}
