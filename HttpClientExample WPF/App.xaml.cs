﻿using System.Windows;
using System.Net.Http;

/////// date: 2022.01.29 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace HttpClientExample;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
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
