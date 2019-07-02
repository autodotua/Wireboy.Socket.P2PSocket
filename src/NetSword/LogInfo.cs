using System;

namespace NetSword
{
    public class LogInfo
    {
        public LogInfo(string content)
        {
            Time = DateTime.Now;
            Content = content;
        }

        public DateTime Time { get; }
        public string Content { get; }

        public override string ToString()
        {
            return Time.ToString("[yyyy-MM-dd  HH:mm:ss]   ") + Content;
        }
    }
}
