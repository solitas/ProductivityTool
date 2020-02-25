using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ModbusLib
{
    public sealed class ModbusTcpMaster
    {
        private bool _isValid = false;
        private bool _isConnected = false;

        private ushort _transactionID = 0;
        private ushort _timeout = 3000;

        private TcpClient _client;

        private ModbusTcpMaster(string ip, int port)
        {
            if (IPAddress.TryParse(ip, out var ipAddress))
            {
                _isValid = false;
                return;
            }

            var endPoint = new IPEndPoint(ipAddress, port);

        }
        public static ModbusTcpMaster Open(string host, int port = 502, int timeout = 1000)
        {
            var modbusTcpMaster = new ModbusTcpMaster(host, port);

            if (IPAddress.TryParse(host, out var ipAddress))
            {
                return null;
            }
            var endPoint = new IPEndPoint(ipAddress, port);

            modbusTcpMaster._client = new TcpClient
            {
                ReceiveTimeout = timeout,
                SendTimeout = timeout
            };

            try
            {
                //client.Connect(endPoint);   
            }
            catch
            {

            }
            return modbusTcpMaster;
        }


        private static void Open(string ip, string port)
        {

        }
    }
}
