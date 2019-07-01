﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using P2PSocket.Core.Commands;
using P2PSocket.Server.Utils;
using P2PSocket.Core.Models;
using P2PSocket.Core.Utils;

namespace P2PSocket.Server
{
    public class CoreModule
    {
        private P2PServer P2PServer = new P2PServer();
        public CoreModule()
        {

        }

        public void Start(int serverPort, PortMapItem[] portMaps)
        {
            Global.LocalPort = serverPort;
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
            P2PServer.StartServer();
        }
        public void Start()
        {
            ConsoleUtils.Write($"P2PServer - > 程序版本:{Global.SoftVerSion}");
            ConsoleUtils.Write($"P2PServer - > 通讯协议:{Global.DataVerSion}");
            //读取配置文件
            if (ConfigUtils.IsExistConfig())
            {
                //初始化全局变量
                InitGlobal();
                //加载配置文件
                ConfigUtils.LoadFromFile();
                //启动服务
                P2PServer.StartServer();
                //todo:控制台显示
            }
            else
            {
                ConsoleUtils.Write("启动失败，配置文件不存在.");
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
            //获取P2PCommand的子类
            Type[] commandList = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(P2PCommand).IsAssignableFrom(t) && !t.IsAbstract)
                .ToArray();
            foreach (Type type in commandList)
            {
                //去除没有CommandFlag的Attribute的类
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
