using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModbusLib
{
    public interface IProtocol
    {
        bool Connect();
        Task<bool> ConnectAsync();
    }
}
