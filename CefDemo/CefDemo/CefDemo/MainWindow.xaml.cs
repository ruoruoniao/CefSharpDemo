using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using MSHTML;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace CefDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WebBrowser.CSharpEventsBinding(new JsEvent(), false);
            Thread th = new Thread(() => {
                while(!(WebBrowser.IsChromeLoaded && WebBrowser.IsDocumentLoaded))
                {
                    Thread.Sleep(15);
                }
                //WebBrowser.InvokeScript("Init", new object[] { "a" });
            });
            th.IsBackground = true;
            th.Start();
            Uri uri = new Uri("D:\\Project Code\\CefDemo\\CefDemo\\CefDemo\\Test.html");
            WebBrowser.Navigate(uri);
            

        }
        private void Button_Click(object sender, FrameLoadEndEventArgs e)
        {
            HTMLDocument a = WebBrowser.Document;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < 20; i++)
            {
                WebBrowser.InvokeScript("Init", new object[] { i.ToString() });
            }
        }
    }
    public class JsEvent
    {
        public JsEvent()
        {
        }

        public bool Init(string a)
        {
            return true;
        }
    }
}
