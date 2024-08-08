using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UkaUart0.MVVM.Model.DataModels
{
    public enum EnumCommunicationParameters
    {
        Stx = 2, // 0x02 start text
        Etx = 3, // 0x03 end text
        Id = 57, // 0x39 device id
        ChInspection = 201, // OxC9
        AnalogInspection = 202, // 0xCA
        CardErrorInspection = 203, // 0xCB
        WarMode = 204, // 0xCC
        TempInspection = 205, // 0xCD
        OpenCloseCard1 = 206, // 0xCE
        OpenCloseCard2 = 207, // 0xCF
        OpenCloseCard3 = 208, // 0xD0
        OpenChannelCard1 = 212, // 0xD4
        OpenChannelCard2 = 213, // 0xD5
        OpenChannelCard3 = 214, // 0xD6
        CloseChannelCard1 = 215, // 0xD7
        CloseChannelCard2 = 216, // 0xD8
        CloseChannelCard3 = 217 // 0xD9
    }
}
