using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NetSword
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            FzLib.Program.Runtime.UnhandledException.RegistAll();

            IsStartup = e.Args.Length > 0 && e.Args[0] == "startup";
            
        }
        public static bool IsStartup { get; private set; }
    }
}
