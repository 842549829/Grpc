using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace Discovery
{
    public class DiscoveryOptions
    {
        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 版本标签
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        public Dictionary<string, string> Meta { get; set; }

        /// <summary>
        /// 服务器连接地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 等待超时时间
        /// </summary>
        public TimeSpan? WaitTime { get; set; }

        /// <summary>
        /// 健康检查间隔时间(单位秒)
        /// </summary>
        public int HealthCheckSeconds { get; set; }

        /// <summary>
        /// 检查处于临界状态的值超过此配置值，然后它的关联服务(及其所有关联检查)将自动注销。(单位秒) {默认30秒}
        /// </summary>
        public int CriticalDeregisterSeconds { get; set; } = 30;

        /// <summary>
        /// 数据中心名称
        /// </summary>
        public string DataCenter { get; set; }

        /// <summary>
        /// 服务监听地址，可以是ip地址也可以是ip网段，例如192.160.100.1或者192.168.100.1/24
        /// </summary>
        public string ListenAddress { get; set; }

        /// <summary>
        /// 服务监听端口
        /// </summary>
        public int ListenPort { get; set; }

        /// <summary>
        /// 根据配置的监听地址获取实际的地址
        /// </summary>
        /// <returns></returns>
        public string GetHostAddress()
        {
            if (!string.IsNullOrWhiteSpace(ListenAddress))
            {
                var separator = ListenAddress.IndexOf("/");
                if (separator > 0)
                {
                    var baseAddress = ListenAddress.Substring(0, separator);
                    if (int.TryParse(ListenAddress.Substring(separator + 1), out int maskBits))
                    {
                        return GetHostAddress(baseAddress, maskBits);
                    }

                    return baseAddress;
                }
            }

            return ListenAddress;
        }

        private string GetHostAddress(string baseAddress, int maskBits)
        {
            if (IPAddress.TryParse(baseAddress, out var ip))
            {
                var subAddress = GetSubAddressBytes(ip, maskBits);
                var addressList = NetworkInterface.GetAllNetworkInterfaces()
                    .Select(p => p.GetIPProperties())
                    .SelectMany(p => p.UnicastAddresses)
                    .Where(p => p.Address.AddressFamily == ip.AddressFamily)
                    .Select(p => p.Address);

                foreach (var _ip in addressList)
                {
                    var _subAddress = GetSubAddressBytes(_ip, maskBits);
                    if (AddressEquals(subAddress, _subAddress))
                    {
                        return _ip.ToString();
                    }
                }
            }

            return baseAddress;
        }

        private byte[] GetSubAddressBytes(IPAddress ip, int maskBits)
        {
            var addressBytes = ip.GetAddressBytes();
            int mod = (int)Math.Floor(maskBits / 8m),
               remainder = maskBits % 8;
            if (remainder > 0)
            {
                addressBytes[mod] &= (byte)(0xFF << (8 - remainder));
                mod += 1;
            }

            for (; mod < addressBytes.Length; mod++)
            {
                addressBytes[mod] = 0x00;
            }

            return addressBytes;
        }

        private bool AddressEquals(byte[] subAddress1, byte[] subAddress2)
        {
            if (subAddress1.Length != subAddress2.Length)
                return false;

            for (var i = 0; i < subAddress1.Length; i++)
            {
                if (subAddress1[i] != subAddress2[i])
                    return false;
            }

            return true;
        }
    }
}