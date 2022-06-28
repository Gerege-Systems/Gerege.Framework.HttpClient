#pragma warning disable IDE0060
#pragma warning disable CS8603

namespace HttpClientExample;

/////// date: 2022.01.29 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

using System.Windows;
using System.Net.Http;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    // App Busy төлөв
    public bool Busy { get; set; } = false;
}

public static class AppExtension
{
    public static App App(this DelegatingHandler a)
    {
        return Application.Current as App;
    }
}
#pragma warning restore IDE0060
#pragma warning restore CS8603
