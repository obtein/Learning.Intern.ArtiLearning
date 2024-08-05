using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UkaUart0.MVVM.Model
{
    class Card
    {
        public double CardVoltage { get; set; }
        public double CardTemp { get; set; }
        public bool IsOpen { get; set; }
        public int ChannelCount { get; set; }
        public List<Channel> ChannelList { get; set; }
    }
}
