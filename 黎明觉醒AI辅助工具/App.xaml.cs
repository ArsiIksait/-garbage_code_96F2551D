using NekoLib.ADB;
using System.Windows;
using ViewModel;

namespace 黎明觉醒AI辅助工具
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // 运行命令
            MainWindowViewModel.IsRuning = false;
            ADB.Disconnect(ADB.driveIPAddress, out _);
        }
    }
}
