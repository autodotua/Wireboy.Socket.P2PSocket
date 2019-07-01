﻿using P2PSocket.Client.Utils;
using P2PSocket.Core.Commands;
using P2PSocket.Core.Models;
using P2PSocket.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace P2PSocket.Client
{
    public class CoreModule
    {
        private P2PClient P2PClient = new P2PClient();
        public CoreModule()
        {
        }
        public void Start(string serverAddress,int serverPort,string clientName,int[] allowPorts, PortMapItem[] portMaps)
        {
            Global.ServerAddress = serverAddress;
            Global.ServerPort = serverPort;
            Global.ClientName = clientName;
            if (allowPorts != null)
            {
                foreach (var port in allowPorts)
                {
                    Global.AllowPort.Add(port);
                }
            }
            if (portMaps != null)
            {
                foreach (var portMap in portMaps)
                {
                    if (!Global.PortMapList.Any(t => t.LocalPort == portMap.LocalPort))
                    {
                        Global.PortMapList.Add(portMap);
                    }
                }
            }
            InitGlobal();
            P2PClient.StartServer();
        }
        public void Start()
        {
            ConsoleUtils.Write($"P2PClient - > 程序版本:{Global.SoftVerSion}");
            ConsoleUtils.Write($"P2PClient - > 通讯协议:{Global.DataVerSion}");
            //读取配置文件
            if (ConfigUtils.IsExistConfig())
            {
                //初始化全局变量
                InitGlobal();
                //加载配置文件
                ConfigUtils.LoadFromFile();
                //启动服务
                P2PClient.StartServer();
                //todo:控制台显示
            }
            else
            {
                ConsoleUtils.Write($"启动失败，配置文件不存在");
            }
        }
        /// <summary>
        ///     初始化全局变量
        /// </summary>
        public void InitGlobal()
        {
            InitCommandList();
        }

        /// <summary>
        ///     初始化命令
        /// </summary>
        public void InitCommandList()
        {
            Type[] commandList = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(P2PCommand).IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();
            foreach (Type type in commandList)
            {
                IEnumerable<Attribute> attributes = type.GetCustomAttributes();
                if (!attributes.Any(t => t is CommandFlag))
                {
                    continue;
                }
                CommandFlag flag = attributes.First(t => t is CommandFlag) as CommandFlag;
                if (!Global.CommandDict.ContainsKey(flag.CommandType))
                {
                    Global.CommandDict.Add(flag.CommandType, type);
                }
            }
        }
    }
}
