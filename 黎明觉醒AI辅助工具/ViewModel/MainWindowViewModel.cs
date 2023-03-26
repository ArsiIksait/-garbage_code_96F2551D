using NekoLib.ADB;
using QRCoder;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ViewModel
{
    public class DelegateCommand : ICommand
    {
        Action action;
        public DelegateCommand(Action action)
        {
            this.action = action;
        }
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            action();
        }
    }

    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private bool _isExpanded = true;
        public static bool IsRuning = false;
        private int _linkModel = 0;
        private double _width = 125;
        private string _ipAddressEndPort = "192.168.0.100:23333";
        private string _pairingCode = "086935";
        private BitmapImage _pairingQRCode = new();
        private string _linkStatus = "未连接";
        private string _runingStatus = "未运行";
        private const string secretKey = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890$";
        private Visibility _homePageVisibility = Visibility.Collapsed;
        private Visibility _devicePageVisibility = Visibility.Visible;
        private Visibility _consolePageVisibility = Visibility.Collapsed;
        private Visibility _settingsPageVisibility = Visibility.Collapsed;
        private Visibility _aboutPageVisibility = Visibility.Collapsed;
        private Visibility _paringBoxVisibility = Visibility.Collapsed;
        private Visibility _paringQRCodeBoxVisibility = Visibility.Collapsed;
        private Visibility _paringButtonVisibility = Visibility.Collapsed;
        private Visibility _startButtonVisibility = Visibility.Visible;
        private Visibility _stopButtonVisibility = Visibility.Collapsed;
        private Visibility _ipAddressInputBoxVisibility = Visibility.Visible;
        public event PropertyChangedEventHandler? PropertyChanged;
        private DeviceScreen? deviceScreen = null;
        #region 自动折叠侧边栏
        /*
        MainWindowViewModel()
        {
            Task autoMenuExpand = new(async () =>
            {
                while (true)
                {
                    if (_isExpanded)
                    {
                        await Task.Delay(10000);
                        if (_isExpanded)
                            MenuExpandSwitch();
                    }
                    await Task.Delay(1000);
                }
            });
            autoMenuExpand.Start();
        }
        */
        #endregion
        public Visibility HomePageVisibility
        {
            get
            {
                return _homePageVisibility;
            }
            set
            {
                _homePageVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(HomePageVisibility)));
            }
        }
        public Visibility DevicePageVisibility
        {
            get
            {
                return _devicePageVisibility;
            }
            set
            {
                _devicePageVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(DevicePageVisibility)));
            }
        }
        public Visibility ConsolePageVisibility
        {
            get
            {
                return _consolePageVisibility;
            }
            set
            {
                _consolePageVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(ConsolePageVisibility)));
            }
        }
        public Visibility SettingsPageVisibility
        {
            get
            {
                return _settingsPageVisibility;
            }
            set
            {
                _settingsPageVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(SettingsPageVisibility)));
            }
        }
        public Visibility AboutPageVisibility
        {
            get
            {
                return _aboutPageVisibility;
            }
            set
            {
                _aboutPageVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(AboutPageVisibility)));
            }
        }
        public Visibility ParingBoxVisibility
        {
            get
            {
                return _paringBoxVisibility;
            }
            set
            {
                _paringBoxVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(ParingBoxVisibility)));
            }
        }
        public Visibility ParingQRCodeBoxVisibility
        {
            get
            {
                return _paringQRCodeBoxVisibility;
            }
            set
            {
                _paringQRCodeBoxVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(ParingQRCodeBoxVisibility)));
            }
        }
        public Visibility ParingButtonVisibility
        {
            get
            {
                return _paringButtonVisibility;
            }
            set
            {
                _paringButtonVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(ParingButtonVisibility)));
            }
        }
        public Visibility StartButtonVisibility
        {
            get
            {
                return _startButtonVisibility;
            }
            set
            {
                _startButtonVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(StartButtonVisibility)));
            }
        }
        public Visibility StopButtonVisibility
        {
            get
            {
                return _stopButtonVisibility;
            }
            set
            {
                _stopButtonVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(StopButtonVisibility)));
            }
        }
        public Visibility IPAddressInputBoxVisibility
        {
            get
            {
                return _ipAddressInputBoxVisibility;
            }
            set
            {
                _ipAddressInputBoxVisibility = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(IPAddressInputBoxVisibility)));
            }
        }

        public double SidebarWidth
        {
            get
            {
                return _width;
            }
            set
            {
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(SidebarWidth)));
                _width = value;
            }
        }
        public int LinkModel
        {
            get
            {
                return _linkModel;
            }
            set
            {
                _linkModel = value;
                switch (_linkModel)
                {
                    case 0:
                        IPAddressInputBoxVisibility = Visibility.Visible;
                        ParingBoxVisibility = Visibility.Collapsed;
                        ParingQRCodeBoxVisibility = Visibility.Collapsed;
                        ParingButtonVisibility = Visibility.Collapsed;
                        StartButtonVisibility = Visibility.Visible;
                        break;
                    case 1:
                        IPAddressInputBoxVisibility = Visibility.Visible;
                        ParingBoxVisibility = Visibility.Visible;
                        ParingQRCodeBoxVisibility = Visibility.Collapsed;
                        ParingButtonVisibility = Visibility.Visible;
                        StartButtonVisibility = Visibility.Collapsed;
                        break;
                    case 2:
                        IPAddressInputBoxVisibility = Visibility.Collapsed;
                        ParingBoxVisibility = Visibility.Collapsed;
                        ParingQRCodeBoxVisibility = Visibility.Collapsed;
                        ParingButtonVisibility = Visibility.Collapsed;
                        StartButtonVisibility = Visibility.Visible;
                        break;
                }
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(LinkModel)));
            }
        }
        public string IPAddressEndPort
        {
            get
            {
                return _ipAddressEndPort;
            }
            set
            {
                _ipAddressEndPort = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(IPAddressEndPort)));
            }
        }
        public string PairingCode
        {
            get
            {
                return _pairingCode;
            }
            set
            {
                _pairingCode = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(PairingCode)));
            }
        }
        public BitmapImage PairingQRCode
        {
            get
            {
                return _pairingQRCode;
            }
            set
            {
                _pairingQRCode = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(PairingQRCode)));
            }
        }
        public string LinkStatus
        {
            get
            {
                return _linkStatus;
            }
            set
            {
                _linkStatus = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(LinkStatus)));
            }
        }
        public string RuningStatus
        {
            get
            {
                return _runingStatus;
            }
            set
            {
                _runingStatus = value;
                PropertyChanged!(this, new PropertyChangedEventArgs(nameof(RuningStatus)));
            }
        }
        public ICommand MenuExpand => new DelegateCommand(() =>
        {
            MenuExpandSwitch();
        });
        public ICommand HomePage => new DelegateCommand(() =>
        {
            HomePageVisibility = Visibility.Visible;
            DevicePageVisibility = Visibility.Collapsed;
            ConsolePageVisibility = Visibility.Collapsed;
            SettingsPageVisibility = Visibility.Collapsed;
            AboutPageVisibility = Visibility.Collapsed;
        });
        public ICommand DevicePage => new DelegateCommand(() =>
        {
            HomePageVisibility = Visibility.Collapsed;
            DevicePageVisibility = Visibility.Visible;
            ConsolePageVisibility = Visibility.Collapsed;
            SettingsPageVisibility = Visibility.Collapsed;
            AboutPageVisibility = Visibility.Collapsed;
        });
        public ICommand ConsolePage => new DelegateCommand(() =>
        {
            HomePageVisibility = Visibility.Collapsed;
            DevicePageVisibility = Visibility.Collapsed;
            ConsolePageVisibility = Visibility.Visible;
            SettingsPageVisibility = Visibility.Collapsed;
            AboutPageVisibility = Visibility.Collapsed;
        });
        public ICommand SettingsPage => new DelegateCommand(() =>
        {
            HomePageVisibility = Visibility.Collapsed;
            DevicePageVisibility = Visibility.Collapsed;
            ConsolePageVisibility = Visibility.Collapsed;
            SettingsPageVisibility = Visibility.Visible;
            AboutPageVisibility = Visibility.Collapsed;
        });
        public ICommand AboutPage => new DelegateCommand(() =>
        {
            HomePageVisibility = Visibility.Collapsed;
            DevicePageVisibility = Visibility.Collapsed;
            ConsolePageVisibility = Visibility.Collapsed;
            SettingsPageVisibility = Visibility.Collapsed;
            AboutPageVisibility = Visibility.Visible;
        });
        public ICommand JoinTelegram => new DelegateCommand(() =>
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://t.me/+EjnhOJ6jzkhmY2Fl",
                UseShellExecute = true
            });
        });
        public ICommand UsePairingCode => new DelegateCommand(() =>
        {
            IPAddressInputBoxVisibility = Visibility.Visible;
            ParingQRCodeBoxVisibility = Visibility.Collapsed;
            ParingBoxVisibility = Visibility.Visible;
        });
        public ICommand UsePairingQRCode => new DelegateCommand(() =>
        {
            IPAddressInputBoxVisibility = Visibility.Collapsed;
            ParingBoxVisibility = Visibility.Collapsed;
            ParingQRCodeBoxVisibility = Visibility.Visible;
            PairingQRCode = GeneratePairingQRCode();
        });
        public ICommand PairingDrive => new DelegateCommand(() =>
        {
            if (ADB.Pairing(IPAddressEndPort, PairingCode, out var error))
            {
                new MessageBox("提示", "WLAN ABD调试配对成功！").Show();
            }
            else
            {
                new MessageBox("错误", error).Show();
            }
        });
        public ICommand StartRuning => new DelegateCommand(() =>
        {
            bool hasError = false;

            StartButtonVisibility = Visibility.Collapsed;
            StopButtonVisibility = Visibility.Visible;
            RuningStatus = "正在连接";

            if (LinkStatus == "未连接")
            {
                // 0是无线调试模式，1是无线调试配对模式，2是USB调试模式
                switch (LinkModel)
                {
                    case 0:
                        #warning 由于ADB.Execute()方法修改后，导致了无法判断是否连接 error 为 string.Empty
                        if (ADB.Connect(IPAddressEndPort, out var error1))
                        {
                            LinkStatus = "已连接到无线调试";
                        }
                        else
                        {
                            hasError = true;
                            new MessageBox("错误", error1).Show();
                        }
                        break;
                    case 2:
                        if (ADB.Usb(out var error2))
                        {
                            LinkStatus = "已连接到USB调试";
                        }
                        else
                        {
                            hasError = true;
                            new MessageBox("错误", error2).Show();
                        }
                        break;
                }

                if (!hasError)
                {
                    RuningStatus = "正在运行";
                    IsRuning = true;
                    deviceScreen = new();
                    deviceScreen?.Show();
                }
                else
                {
                    StartButtonVisibility = Visibility.Visible;
                    StopButtonVisibility = Visibility.Collapsed;
                    LinkStatus = "未连接";
                    RuningStatus = "未运行";
                }
            }
        });
        public ICommand StopRuning => new DelegateCommand(() =>
        {
            Mouse.OverrideCursor = null;

            if (LinkModel == 0)
            {
                if (ADB.Disconnect(ADB.driveIPAddress, out var error2))
                {
                    LinkStatus = "未连接";
                }
                else
                {
                    new MessageBox("错误", error2).Show();
                }
            }

            StartButtonVisibility = Visibility.Visible;
            StopButtonVisibility = Visibility.Collapsed;
            RuningStatus = "未运行";
            IsRuning = false;
            deviceScreen?.Close();
        });
        private void MenuExpandSwitch()
        {
            Action? t = null;
            t = async () =>
            {
                if (_isExpanded)
                {
                    while (SidebarWidth > 57)
                    {
                        SidebarWidth -= 10;
                        await Task.Delay(10);
                    }
                    _isExpanded = false;
                }
                else
                {
                    while (SidebarWidth < 125)
                    {
                        SidebarWidth += 10;
                        await Task.Delay(10);
                    }
                    _isExpanded = true;
                }
            };
            Task.Run(t);
        }
        private string GenerateKey()
        {
            var tmpKey = string.Empty;
            for (int i = 0; i < 8; i++)
            {
                tmpKey += secretKey[new Random().Next(0, secretKey.Length)];
            }
            return tmpKey;
        }
        private BitmapImage GeneratePairingQRCode()
        {
            // 定义要生成二维码的字符串
            var qrCodeContent = $"WIFI:T:ADB;S:studio-{GenerateKey()};P:{GenerateKey()};;";

            // 创建 QRCodeGenerator 实例
            QRCodeGenerator qrGenerator = new();

            // 生成 QRCodeData 实例
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodeContent, QRCodeGenerator.ECCLevel.Q);

            // 创建 QRCode 实例
            QRCode qrCode = new(qrCodeData);

            // 将 QRCode 转换为 Bitmap
            Bitmap qrBitmap = qrCode.GetGraphic(20);

            // 将 Bitmap 转换为 BitmapImage
            BitmapImage qrImage = new();
            using (MemoryStream stream = new())
            {
                qrBitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                qrImage.BeginInit();
                qrImage.CacheOption = BitmapCacheOption.OnLoad;
                qrImage.StreamSource = stream;
                qrImage.EndInit();
            }

            return qrImage;
        }
    }
}