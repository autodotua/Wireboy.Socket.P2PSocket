using System;
using System.Collections.Generic;
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
    public partial class PageServer : Page
    {
        public PageServer()
        {
            InitializeComponent();
        }

        public ServeConfig Config => ServeConfig.Instance;
        //public ClientConfig ClientConfig => ClientConfig.Instance;

        private void StartBtnClick(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            (Window.GetWindow(this) as MainWindow).ServerStarted();

            P2PSocket.Server.CoreModule core = new P2PSocket.Server.CoreModule();
            core.Start(Config.Port,null);
            Config.Save();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (App.IsStartup)
            {
                StartBtnClick(null, null);
            }
        }
    }
}
