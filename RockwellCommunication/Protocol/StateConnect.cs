using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RockwellCommunication.Protocol
{
    /// <summary>
    /// Enumeration des différents état de connection Modbus
    /// </summary>
    public enum StateConnect
    {
        Connecting = 1,
        NoClientExist = 2,
        Connected = 3,
        Stopped = 4,
        Disconnected = 5,
    }
}
