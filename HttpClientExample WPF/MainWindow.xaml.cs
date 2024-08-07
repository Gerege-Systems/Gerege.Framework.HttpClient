﻿using System;
using System.Windows;
using System.Windows.Controls;

using Gerege.Framework.Logger;
using Gerege.Framework.HttpClient;

using SharedExample;
using System.Text.Json.Serialization;

/////// date: 2022.01.29 //////////
///// author: Narankhuu ///////////
//// contact: codesaur@gmail.com //

namespace HttpClientExample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    readonly SampleClient Client;
    readonly DatabaseLogger Logger;

    public MainWindow()
    {
        InitializeComponent();

        Logger = new ConsoleLogger();

        var pipeline = new BusyHandler()
        {
            InnerHandler = new LoggingHandler(Logger)
            {
                InnerHandler = new RetryHandler()
                {
                    // Бодит серверлүү хандах бол энэ удирдлага ашиглаарай
                    //InnerHandler = new System.Net.Http.HttpClientHandler()

                    // Туршилтын зорилгоор хуурамч сервер хандалтын удирдлага ашиглаж байна
                    InnerHandler = new MockServerHandler()
                }
            }
        };
        Client = new(pipeline);
    }

    public struct Welcome
    {
        [JsonPropertyName("title")]
        [JsonRequired]
        public string Title { get; set; }
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        string res = "";
        try
        {
            Welcome t = Client.Request<Welcome>(new { get = "title" });
            res = t.Title;
        }
        catch (Exception ex)
        {
            Logger.Error("button", "Welcome-ийг авах үед алдаа гарлаа", ex);
            res = ex.Message;
        }
        finally
        {
            var button = (Button)sender;
            button.Content = res;
        }
    }
}
