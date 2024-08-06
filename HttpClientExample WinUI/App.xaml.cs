using System.Net.Http;

using Microsoft.UI.Xaml;

namespace HttpClientExample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched normally by the end user.  Other entry points
    /// will be used such as when the application is launched to open a specific file.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        m_window = new MainWindow();
        m_window.Activate();
    }

    private Window m_window;

    // App Busy төлөв
    public bool Busy { get; set; } = false;
}

#pragma warning disable IDE0060
#pragma warning disable CS8603
public static class AppExtension
{
    public static App App(this DelegatingHandler a)
    {
        return Application.Current as App;
    }
}
#pragma warning restore IDE0060
#pragma warning restore CS8603
