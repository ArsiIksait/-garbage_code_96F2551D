using NekoLib.ADB;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ViewModel
{
    class MessageBox : Window
    {
        public MessageBox(string title, string text)        //构造函数
        {
            InitializeComponent(title, text);
        }
        private void InitializeComponent(string title, string text)  //控件属性初始化
        {
            //设置窗体参数
            //Width = 200;
            //Height = 300;
            //Left = Top = 100;
            Title = title;

            var label = new Label()
            {
                Margin = new Thickness(20, 10, 20, 10),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Top,
                Content = text
            };

            AddChild(label);
        }
    }
    class DeviceScreen : Window
    {
        private string _title = string.Empty;
        private static Image _image = new(); 
        Dispatcher dispatcher = Application.Current.Dispatcher;

        public DeviceScreen()        //构造函数
        {
            InitializeComponent();
            UpdataScreen();
        }
        private void InitializeComponent()  //控件属性初始化
        {
            //设置窗体参数
            Width = 340;
            Height = 740;
            //Left = Top = 100;
            var devices = ADB.Devices();

            var linkType = "USB";

            if (devices.Count > 0 && !string.IsNullOrEmpty(devices[0].IPAddress))
            {
                linkType = devices[0].IPAddress;
                _title = $"{linkType} {devices[0].Model}";
            }
            else
            {
                _title = "未连接";
            }

            Title = _title;
            AddChild(_image);
            Mouse.OverrideCursor = Cursors.Cross;
        }
        private void UpdataScreen()
        {
            Action? t = null;
            t = async () =>
            {
                while (MainWindowViewModel.IsRuning)
                {
                    var image = ADB.Screencap();

                    await dispatcher.InvokeAsync(() =>
                    {
                        #error 线程不安全
                        _image.Source = image;
                        _image.Height = image.Height;
                        _image.Width = image.Width;
                    });
                }
            };

            Task.Run(t);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (MainWindowViewModel.IsRuning)
            {
                e.Cancel = true;
            }
        }
    }
}
