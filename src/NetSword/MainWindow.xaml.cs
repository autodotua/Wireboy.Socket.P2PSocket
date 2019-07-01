using FzLib.Control.Dialog;
using FzLib.Control.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : ExtendedWindow
    {
        public MainWindow()
        {
            InitializeComponent();


            P2PSocket.Core.Utils.ConsoleUtils.NewLine += (s, e) =>
              {
                  Dispatcher.Invoke(() =>
                  {
                      Logs.Add(e.Content);
                  });
              };
        }

        public ObservableCollection<string> Logs { get; } = new ObservableCollection<string>();

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frm.Navigate(new Uri(nameof(PageClient) + ".xaml",UriKind.Relative));
        }
    }
}
