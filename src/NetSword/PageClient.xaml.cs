using P2PSocket.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
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
    /// PageServer.xaml 的交互逻辑
    /// </summary>
    public partial class PageClient : Page
    {
        public PageClient()
        {
            InitializeComponent();
        }

        public ClientConfig Config => ClientConfig.Instance;
        //public ClientConfig ClientConfig => ClientConfig.Instance;

        private void StartBtnClick(object sender, RoutedEventArgs e)
        {
           IsEnabled = false;
            (Window.GetWindow(this) as MainWindow).ServerStarted();
            P2PSocket.Client.CoreModule core = new P2PSocket.Client.CoreModule();
            foreach (var map in Config.Maps)
            {
                if (map.RemoteAddress.Count(p => p == '.') >= 3)
                {
                    map.MapType = PortMapType.ip;
                }
                else
                {
                    map.MapType = PortMapType.servername;
                }
            }
            core.Start(Config.ServerIP, Config.ServerPort, Config.ClientName, Config.AllowPorts.Select(p => p.Port).ToArray(), Config.Maps.ToArray());
            Config.Save();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Tag.Equals("0"))
            {
                Config.Maps.Add(new PortMapItem());
            }
            else
            {
                Config.AllowPorts.Add(new PortInfo());
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(App.IsStartup)
            {
                StartBtnClick(null, null);
            }
        }
    }
    //public class PortMapTypeConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //       if(((PortMapType)value)==PortMapType.ip)
    //        {
    //            return true;
    //        }
    //        return      false;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if((bool)value)
    //        {
    //            return PortMapType.ip;
    //        }
    //        return PortMapType.servername;
    //    }
    //}
}
