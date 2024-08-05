using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace UkaUart0.MVVM.Model
{
    class Channel
    {
        public bool IsOpen { get; set; }
        public int ErrorCode { get; set; }
        public double ChannelCurrent { get; set; }
    }
}
