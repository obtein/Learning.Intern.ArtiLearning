using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UkaUart0.MVVM.Model.CommunicationModels
{
    public static class CommunicationConstants
    {
        /// <summary>
        /// BaudRateList   |    ParityList   |    StopBitList     
        /// 0 - 110        |    0 - None     |    0 - None        
        /// 1 - 300        |    1 - Odd      |    1 - One         
        /// 2 - 600        |    2 - Even     |    2 - OnePointFive
        /// 3 - 1200       |    3 - Mark     |    3 - Two         
        /// 4 - 2400       |    4 - Space    |                    
        /// 5 - 4800       |                                       
        /// 6 - 9600       |             HandShakeList                          
        /// 7 - 14400      |             0 - None                           
        /// 8 - 19200      |             1 - XOnxOff                          
        /// 9 - 38400      |             2 - RequestToSend                         
        ///10 - 57600      |             3 - RequestToSendXOnXOff                         
        ///11 - 115200     |                                       
        ///12 - 128000     |                                       
        ///13 - 256000     |                                       
        /// </summary>
        public static readonly Int32[] BaudRateList = { 110, 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        public static readonly Parity [] ParityList = { Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space };
        public static readonly StopBits [] StopBitList = { StopBits.None, StopBits.One, StopBits.OnePointFive, StopBits.Two };
        public static readonly Handshake [] HandShakeList = { Handshake.None, Handshake.XOnXOff, Handshake.RequestToSend, Handshake.RequestToSendXOnXOff };
    }
}
