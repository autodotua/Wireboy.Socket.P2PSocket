namespace NetSword
{

        public class PortInfo {
            private int port;
            public int Port
            {
                get => port;
                set
                {
                    if(port<0 && port> 65535)
                    {
                        return;
                    }
                    port = value;
                }
            }
        }
    }
  