using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace UkaUart0.MVVM.Model.DataModels.UI
{
    public static class BrushesToBeUsed
    {
        public static readonly Color CHANNEL_OPEN = Colors.LightGreen;
        public static readonly Color CHANNEL_CLOSE = Colors.DarkRed;
        public static readonly Color NOCOMMUNICATION = Color.FromArgb( 0xFF,0x3C,0x34,0x34 );
        public static readonly Color FINE = Color.FromArgb(0xFF,0x56,0xFF,0x00);
        public static readonly Color ERROR = Colors.Red;
        public static readonly double OPACITY_ACTIVE = 1;
        public static readonly double OPACITY_PASSIVE = 0.3;
        public static readonly Dictionary<int,Color[]> ERROR_STATUS = new Dictionary<int, Color []>();

        /// <summary>
        /// Index   Meaning on UI
        /// 
        ///   0     ShortCircuit LED
        ///   1     OverCurrent  LED
        ///   2     VoltageError LED
        /// </summary>
        private static readonly Color [] errorStatus0   = { FINE,FINE,FINE }; 
        private static readonly Color [] errorStatus1   = { ERROR,FINE,FINE }; 
        private static readonly Color [] errorStatus2   = { FINE,ERROR,FINE }; 
        private static readonly Color [] errorStatus3   = { ERROR,ERROR,FINE }; 
        private static readonly Color [] errorStatus4   = { FINE,FINE,ERROR }; 
        private static readonly Color [] errorStatus5   = { ERROR,FINE,ERROR }; 
        private static readonly Color [] errorStatus6   = { FINE,ERROR,ERROR }; 
        private static readonly Color [] errorStatus7   = { ERROR,ERROR,ERROR }; 
        private static readonly Color [] errorStatus128 = { NOCOMMUNICATION,NOCOMMUNICATION,NOCOMMUNICATION }; 
        static BrushesToBeUsed ()
        { 
            /* 0- Ok
             * 1- shortCircuit 
             * 2- overCurrent 
             * 3- shortCircuit + overCurrent
             * 4- voltageError
             * 5- shortCircuit + voltageError 
             * 6- overCurrent + voltageError
             * 7- shortCircuit + overCurrent + voltageError
             * 128- No communication 
             */
            ERROR_STATUS.Add(  0, errorStatus0);
            ERROR_STATUS.Add(  1, errorStatus1);
            ERROR_STATUS.Add(  2, errorStatus2);
            ERROR_STATUS.Add(  3, errorStatus3);
            ERROR_STATUS.Add(  4, errorStatus4);
            ERROR_STATUS.Add(  5, errorStatus5);
            ERROR_STATUS.Add(  6, errorStatus6);
            ERROR_STATUS.Add(  7, errorStatus7);
            ERROR_STATUS.Add(128, errorStatus128);
        }

    }
}
