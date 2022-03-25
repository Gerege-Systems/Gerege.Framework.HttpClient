using System;
using System.Windows;
using System.Windows.Controls;

using Newtonsoft.Json;

using Gerege.Framework.Logger;
using Gerege.Framework.HttpClient;

namespace HttpClientExample
{
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
            Client = new SampleClient(pipeline);
        }

        public struct Welcome
        {
            public static int GeregeMessage() => 3;

            [JsonProperty("title", Required = Required.Always)]
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
}
