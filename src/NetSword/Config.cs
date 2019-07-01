using P2PSocket.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSword
{
    public class ServeConfig : FzLib.DataStorage.Serialization.JsonSerializationBase
    {
        private static ServeConfig instance;
        public static ServeConfig Instance
        {
            get
            {
                if(instance==null)
                {
                    instance = TryOpenOrCreate<ServeConfig>("ServerConfig.json");
                }
                return instance;
            }
        }

        public int Port { get; set; } = 8009;


#if DEBUG

        public override void Save()
        {

        }
#endif
    }
    public class ClientConfig : FzLib.DataStorage.Serialization.JsonSerializationBase
    {
        private static ClientConfig instance;
        public static ClientConfig Instance
        {
            get
            {
                if(instance==null)
                {
                    instance = TryOpenOrCreate<ClientConfig>("ClientConfig.json");
                }
                return instance;
            }
        }
        //public string ServerIP { get; set; } = "192.168.2.234";//127.0.0.1";
        //public string ServerIP { get; set; } = "47.101.216.232";
        public string ServerIP { get; set; } = "127.0.0.1";
        //public int ServerPort { get; set; } = 8008;
        public int ServerPort { get; set; } = 8009;
        public string ClientName { get; set; } = "（客户机名字）";
        public ObservableCollection<PortMapItem> Maps { get; } = new ObservableCollection<PortMapItem>();
        public ObservableCollection<PortInfo> AllowPorts { get; } = new ObservableCollection<PortInfo>();


#if DEBUG

        public override void Save()
        {

        }
#endif
    }
}
