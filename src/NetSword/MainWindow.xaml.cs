using FzLib.Control.Dialog;
using FzLib.Control.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
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

namespace NetSword
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ExtendedWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            if (FzLib.Program.Startup.IsStartupFolderShortcutExist() == FzLib.IO.Shortcut.ShortcutStatus.Exist)
            {
                btnStartup.Content = "取消开机自启";
            }
            else
            {
                btnStartup.Content = "设置开机自启";
            }

            P2PSocket.Core.Utils.ConsoleUtils.NewLine += (s, e) =>
              {
                  Dispatcher.Invoke(() =>
                  {
                      Logs.Add(new LogInfo(e.Content));
                  });
              };
        }

        public ObservableCollection<LogInfo> Logs { get; } = new ObservableCollection<LogInfo>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn.Content.Equals("作为服务端"))
            {
                frm.Navigate(new Uri(nameof(PageServer) + ".xaml", UriKind.Relative));
                btn.Content = "作为客户端";
                Config.Instance.LastMode = 1;
            }
            else
            {

                frm.Navigate(new Uri(nameof(PageClient) + ".xaml", UriKind.Relative));
                btn.Content = "作为服务端";
                Config.Instance.LastMode = 2;
            }
            Config.Instance.Save();

        }

        public void ServerStarted()
        {
            btnSwitch.IsEnabled = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FzLib.Program.Information.Restart();
        }

        private void ExtendedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Config.Instance.LastMode == 1)
            {
                frm.Navigate(new Uri(nameof(PageServer) + ".xaml", UriKind.Relative));
                btnSwitch.Content = "作为客户端";
            }
            else
            {
                frm.Navigate(new Uri(nameof(PageClient) + ".xaml", UriKind.Relative));
                btnSwitch.Content = "作为服务端";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (FzLib.Program.Startup.IsStartupFolderShortcutExist() == FzLib.IO.Shortcut.ShortcutStatus.Exist)
            {
                FzLib.Program.Startup.CreateStartupFolderShortcut("startup", true);
                btnStartup.Content = "取消开机自启";
            }
            else
            {
                FzLib.Program.Startup.DeleteStartupFolderShortcut();

                btnStartup.Content = "设置开机自启";
            }
        }
    }
}
