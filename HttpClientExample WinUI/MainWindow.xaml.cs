using System;
using System.Text.Json.Serialization;

using Microsoft.UI.Xaml;

using Gerege.Framework.Logger;
using Gerege.Framework.HttpClient;

using SharedExample;

namespace HttpClientExample;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    readonly SampleClient Client;
    readonly DatabaseLogger Logger;

    public MainWindow()
    {
        this.InitializeComponent();

        Logger = new ConsoleLogger();

        var pipeline = new BusyHandler()
        {
            InnerHandler = new LoggingHandler(Logger)
            {
                InnerHandler = new RetryHandler()
                {
                    // Бодит серверлүү хандах бол энэ удирдлага ашиглаарай
                    //InnerHandler = new System.Net.Http.HttpClientHandler()

                    // Туршилтын зорилгоор хуурмаг сервер хандалтын удирдлага ашиглаж байна
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

    private void MyButton_Click(object sender, RoutedEventArgs e)
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
            MyButton.Content = res;
        }
    }
}
