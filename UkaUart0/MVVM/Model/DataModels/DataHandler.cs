using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UkaUart0.MVVM.Model.DataModels
{
    class DataHandler : INotifyPropertyChanged
    {
        private ObservableCollection<Card> cardList;
        public ObservableCollection<Card> CardList
        {
            get => cardList;
            set
            {
                if ( cardList != value )
                {
                    cardList = value;
                    OnPropertyChanged( nameof( CardList ) );
                }
            }
        }

        private string answer;

        public string Answer
        {
            get
            {
                return answer;
            }
            set
            {
                if ( answer != value)
                {
                    answer = value;
                    OnAnswerReady(nameof(Answer));
                }
            }
        }


        public DataHandler ()
        {
            CardList = new ObservableCollection<Card>();
            PopulateCards( 3 );
        }

        /// <summary>
        /// Entrance point from SerialCommunication data received 
        /// Path : SerialCommunication => ComSystem => Here
        /// /// 1 - 0xC9 is for Channel information response
        ///     response is 0x02 0x39 0xC9 0x30 0x36 dataToBeChecked checkSum 0x03
        ///     dataToBeChecked consist of 6 bytes and each 2 byte represents 1 card
        ///     first two bytes are for card1
        ///     second two are for card2
        ///     third are for card3
        ///     those values show the channel amount on each card
        /// 2 - 0xCA is for Analog information response
        ///     response is 0x02 0x39 0xCA 0x36 0x43 dataToBeChecked checksum 0x03
        ///     dataToBeChecked consist of 108 bytes, 36 for each card
        ///     each part of the data i.e 0-35 card1, 36-71 card2, 72-107 card3 will be divided 
        ///     as first 4 bytes are for Card Voltage, second 4 are for channel1 current
        ///     third 4 are for channel2 current and so on
        /// 3 - 0xCB is for Card Error Inspection response
        ///     response is 0x02 0x39 0xCB 0x33 0x30 dataToBeChecked checkSum 0x03
        ///     dataToBeChecked consist of 48 bytes, 16 for each card
        ///     each part of the data i.e. 0-15 card1, 16-31 card2, 31-47 card3 will be divided
        ///     as bit0 represents ShortCircuit, bit1 represents OverCurrent, bi2 represents VoltageError
        ///     bit3-7 are reserved.
        ///     ******NOTE : if there is no communication data will be (128)!!!
        /// 4 - 0xCE is for Card1 open/closed response
        ///     response is 0x02 0x39 0xCE 0x30 0x32 dataToBeChecked checkSum 0x03
        ///     dataToBeChecked is 2 bytes and contain open/close information at its lsb
        /// 5 - 0xCF is for Card2 open/closed response
        ///     response is 0x02 0x39 0xCF 0x30 0x32 dataToBeChecked checkSum 0x03
        ///     dataToBeChecked is 2 bytes and contain open/close information at its lsb
        /// 6 - 0xD0 is for Card2 open/closed response
        ///     response is 0x02 0x39 0xD0 0x30 0x32 dataToBeChecked checkSum 0x03
        ///     dataToBeChecked is 2 bytes and contain open/close information at its lsb
        /// </summary>
        /// <param name="dataToRead"></param>
        public void ReadData ( byte [] dataToRead)
        {
            // 0
            if ( dataToRead [2] == (byte)0xC9 )
            {
                ChannelInspectionReceived( dataToRead );
            }
            //1
            else if ( dataToRead [2] == (byte)0xCA )
            {
                AnalogInspectionReceived( dataToRead );
            }
            //2
            else if ( dataToRead [2] == (byte)0xCB )
            {
                CardErrorInspectionReceived( dataToRead );
            }
            //3
            else if ( dataToRead [2] == (byte)0xCD )
            {
                CardTemperatureInspectionReceived( dataToRead );
            }
            //4
            else if ( dataToRead [2] == (byte)0xCE )
            {
                Card1OpenOrCloseReceived( dataToRead );
            }
            //5
            else if ( dataToRead [2] == (byte)0xCF )
            {
                Card2OpenOrCloseReceived( dataToRead );
            }
            //6
            else if ( dataToRead [2] == (byte)0xD0 )
            {
                Card3OpenOrCloseReceived( dataToRead );
            }
        }

        private void Card3OpenOrCloseReceived (byte [] dataToRead)
        {
            CardList [2].IsOpen = (dataToRead [1] == 1);
        }

        private void Card2OpenOrCloseReceived (byte [] dataToRead)
        {
            CardList [1].IsOpen = ( dataToRead [1] == 1 );
        }

        private void Card1OpenOrCloseReceived (byte [] dataToRead)
        {
            CardList [0].IsOpen = ( dataToRead [1] == 1 );
        }

        private void CardTemperatureInspectionReceived ( byte [] dataToRead)
        {
            // Work in progress
        }

        /// <summary>
        /// Error inspection response for each card  
        /// => bit0(short circuit),
        /// => bit1 (over current), 
        /// => bit2(voltage error), 
        /// => bit3-7 reserved 
        /// => byte0-15 card1, 16-31 card2, 31-47 card3
        /// </summary>
        /// <param name="dataToBeChecked"></param>
        private void CardErrorInspectionReceived ( byte [] dataToBeChecked)
        {
            byte shortCircuit = 0b00000001; // 1
            byte overCurrent = 0b00000010; // 2
            byte voltageError = 0b00000100; // 4
            /* 1 = shortCircuit 
             * 2- overCurrent 
             * 3- shortCircuit + overCurrent
             * 4- voltageError
             * 5- shortCircuit + voltageError 
             * 6- overCurrent + voltageError
             * 7- shortCircuit + overCurrent + voltageError
             * 128- No communication 
             */
            CardList [0].ChannelList [0].IsOpen    = dataToBeChecked [0] != 128 && dataToBeChecked [1] != 128;
            CardList [0].ChannelList [0].ErrorCode = CalculateDataLength( dataToBeChecked [0], dataToBeChecked [1] );
            CardList [0].ChannelList [1].IsOpen    = dataToBeChecked [2] != 128 && dataToBeChecked [3] != 128;
            CardList [0].ChannelList [1].ErrorCode = CalculateDataLength( dataToBeChecked [2], dataToBeChecked [3] );
            CardList [0].ChannelList [2].IsOpen    = dataToBeChecked [4] != 128 && dataToBeChecked [5] != 128;
            CardList [0].ChannelList [2].ErrorCode = CalculateDataLength( dataToBeChecked [4], dataToBeChecked [5] );
            CardList [0].ChannelList [3].IsOpen    = dataToBeChecked [6] != 128 && dataToBeChecked [7] != 128;
            CardList [0].ChannelList [3].ErrorCode = CalculateDataLength( dataToBeChecked [6], dataToBeChecked [7] );
            CardList [0].ChannelList [4].IsOpen    = dataToBeChecked [8] != 128 && dataToBeChecked [9] != 128;
            CardList [0].ChannelList [4].ErrorCode = CalculateDataLength( dataToBeChecked [8], dataToBeChecked [9] );
            CardList [0].ChannelList [5].IsOpen    = dataToBeChecked [10] != 128 && dataToBeChecked [11] != 128;
            CardList [0].ChannelList [5].ErrorCode = CalculateDataLength( dataToBeChecked [10], dataToBeChecked [11] );
            CardList [0].ChannelList [6].IsOpen    = dataToBeChecked [12] != 128 && dataToBeChecked [13] != 128;
            CardList [0].ChannelList [6].ErrorCode = CalculateDataLength( dataToBeChecked [12], dataToBeChecked [13] );
            CardList [0].ChannelList [7].IsOpen    = dataToBeChecked [14] != 128 && dataToBeChecked [15] != 128;
            CardList [0].ChannelList [7].ErrorCode = CalculateDataLength( dataToBeChecked [14], dataToBeChecked [15] );
            CardList [1].ChannelList [0].IsOpen    = dataToBeChecked [16] != 128 && dataToBeChecked [17] != 128;
            CardList [1].ChannelList [0].ErrorCode = CalculateDataLength( dataToBeChecked [16], dataToBeChecked [17] );
            CardList [1].ChannelList [1].IsOpen    = dataToBeChecked [18] != 128 && dataToBeChecked [19] != 128;
            CardList [1].ChannelList [1].ErrorCode = CalculateDataLength( dataToBeChecked [18], dataToBeChecked [19] );
            CardList [1].ChannelList [2].IsOpen    = dataToBeChecked [20] != 128 && dataToBeChecked [21] != 128;
            CardList [1].ChannelList [2].ErrorCode = CalculateDataLength( dataToBeChecked [20], dataToBeChecked [21] );
            CardList [1].ChannelList [3].IsOpen    = dataToBeChecked [22] != 128 && dataToBeChecked [23] != 128;
            CardList [1].ChannelList [3].ErrorCode = CalculateDataLength( dataToBeChecked [22], dataToBeChecked [23] );
            CardList [1].ChannelList [4].IsOpen    = dataToBeChecked [24] != 128 && dataToBeChecked [25] != 128;
            CardList [1].ChannelList [4].ErrorCode = CalculateDataLength( dataToBeChecked [24], dataToBeChecked [25] );
            CardList [1].ChannelList [5].IsOpen    = dataToBeChecked [26] != 128 && dataToBeChecked [27] != 128;
            CardList [1].ChannelList [5].ErrorCode = CalculateDataLength( dataToBeChecked [26], dataToBeChecked [27] );
            CardList [1].ChannelList [6].IsOpen    = dataToBeChecked [28] != 128 && dataToBeChecked [29] != 128;
            CardList [1].ChannelList [6].ErrorCode = CalculateDataLength( dataToBeChecked [28], dataToBeChecked [29] );
            CardList [1].ChannelList [7].IsOpen    = dataToBeChecked [30] != 128 && dataToBeChecked [31] != 128;
            CardList [1].ChannelList [7].ErrorCode = CalculateDataLength( dataToBeChecked [30], dataToBeChecked [31] );
            CardList [2].ChannelList [0].IsOpen    = dataToBeChecked [32] != 128 && dataToBeChecked [33] != 128;
            CardList [2].ChannelList [0].ErrorCode = CalculateDataLength( dataToBeChecked [32], dataToBeChecked [33] );
            CardList [2].ChannelList [1].IsOpen    = dataToBeChecked [34] != 128 && dataToBeChecked [35] != 128;
            CardList [2].ChannelList [1].ErrorCode = CalculateDataLength( dataToBeChecked [34], dataToBeChecked [35] );
            CardList [2].ChannelList [2].IsOpen    = dataToBeChecked [36] != 128 && dataToBeChecked [37] != 128;
            CardList [2].ChannelList [2].ErrorCode = CalculateDataLength( dataToBeChecked [36], dataToBeChecked [37] );
            CardList [2].ChannelList [3].IsOpen    = dataToBeChecked [38] != 128 && dataToBeChecked [39] != 128;
            CardList [2].ChannelList [3].ErrorCode = CalculateDataLength( dataToBeChecked [38], dataToBeChecked [39] );
            CardList [2].ChannelList [4].IsOpen    = dataToBeChecked [40] != 128 && dataToBeChecked [41] != 128;
            CardList [2].ChannelList [4].ErrorCode = CalculateDataLength( dataToBeChecked [40], dataToBeChecked [41] );
            CardList [2].ChannelList [5].IsOpen    = dataToBeChecked [42] != 128 && dataToBeChecked [43] != 128;
            CardList [2].ChannelList [5].ErrorCode = CalculateDataLength( dataToBeChecked [42], dataToBeChecked [43] );
            CardList [2].ChannelList [6].IsOpen    = dataToBeChecked [44] != 128 && dataToBeChecked [45] != 128;
            CardList [2].ChannelList [6].ErrorCode = CalculateDataLength( dataToBeChecked [44], dataToBeChecked [45] );
            CardList [2].ChannelList [7].IsOpen    = dataToBeChecked [46] != 128 && dataToBeChecked [47] != 128;
            CardList [2].ChannelList [7].ErrorCode = CalculateDataLength( dataToBeChecked [46], dataToBeChecked [47] );
        }
        /// <summary>
        /// Analog info response for each card first 4 byte is card voltage second 4 byte channel1 current third 4 byte channel2 current and so on
        /// </summary>
        /// <param name="dataToBeChecked"></param>
        private void AnalogInspectionReceived ( byte [] dataToBeChecked )
        {
            // Card 1
            CardList [0].CardVoltage = Calculate4byteInput( dataToBeChecked [0], dataToBeChecked [1], dataToBeChecked [2], dataToBeChecked [3] );
            CardList [0].ChannelList[0].Current = Calculate4byteInput( dataToBeChecked [4], dataToBeChecked [5], dataToBeChecked [6], dataToBeChecked [7] );
            CardList [0].ChannelList[1].Current = Calculate4byteInput( dataToBeChecked [8], dataToBeChecked [9], dataToBeChecked [10], dataToBeChecked [11] );
            CardList [0].ChannelList[2].Current = Calculate4byteInput( dataToBeChecked [12], dataToBeChecked [13], dataToBeChecked [14], dataToBeChecked [15] );
            CardList [0].ChannelList[3].Current = Calculate4byteInput( dataToBeChecked [16], dataToBeChecked [17], dataToBeChecked [18], dataToBeChecked [19] );
            CardList [0].ChannelList[4].Current = Calculate4byteInput( dataToBeChecked [20], dataToBeChecked [21], dataToBeChecked [22], dataToBeChecked [23] );
            CardList [0].ChannelList[5].Current = Calculate4byteInput( dataToBeChecked [24], dataToBeChecked [25], dataToBeChecked [26], dataToBeChecked [27] );
            CardList [0].ChannelList[6].Current = Calculate4byteInput( dataToBeChecked [28], dataToBeChecked [29], dataToBeChecked [30], dataToBeChecked [31] );
            CardList [0].ChannelList[7].Current = Calculate4byteInput( dataToBeChecked [32], dataToBeChecked [33], dataToBeChecked [34], dataToBeChecked [35] );
            // Card 2
            CardList [1].CardVoltage = Calculate4byteInput( dataToBeChecked [36], dataToBeChecked [37], dataToBeChecked [38], dataToBeChecked [39] );
            CardList [1].ChannelList[0].Current = Calculate4byteInput( dataToBeChecked [40], dataToBeChecked [41], dataToBeChecked [42], dataToBeChecked [43] );
            CardList [1].ChannelList[1].Current = Calculate4byteInput( dataToBeChecked [44], dataToBeChecked [45], dataToBeChecked [46], dataToBeChecked [47] );
            CardList [1].ChannelList[2].Current = Calculate4byteInput( dataToBeChecked [48], dataToBeChecked [49], dataToBeChecked [50], dataToBeChecked [51] );
            CardList [1].ChannelList[3].Current = Calculate4byteInput( dataToBeChecked [52], dataToBeChecked [53], dataToBeChecked [54], dataToBeChecked [55] );
            CardList [1].ChannelList[4].Current = Calculate4byteInput( dataToBeChecked [56], dataToBeChecked [57], dataToBeChecked [58], dataToBeChecked [59] );
            CardList [1].ChannelList[5].Current = Calculate4byteInput( dataToBeChecked [60], dataToBeChecked [61], dataToBeChecked [62], dataToBeChecked [63] );
            CardList [1].ChannelList[6].Current = Calculate4byteInput( dataToBeChecked [64], dataToBeChecked [65], dataToBeChecked [66], dataToBeChecked [67] );
            CardList [1].ChannelList[7].Current = Calculate4byteInput( dataToBeChecked [68], dataToBeChecked [69], dataToBeChecked [70], dataToBeChecked [71] );
            // Card 3
            CardList [2].CardVoltage = Calculate4byteInput( dataToBeChecked [72], dataToBeChecked [73], dataToBeChecked [74], dataToBeChecked [75] );
            CardList [2].ChannelList[0].Current = Calculate4byteInput( dataToBeChecked [76], dataToBeChecked [77], dataToBeChecked [78], dataToBeChecked [79] );
            CardList [2].ChannelList[1].Current = Calculate4byteInput( dataToBeChecked [80], dataToBeChecked [81], dataToBeChecked [82], dataToBeChecked [83] );
            CardList [2].ChannelList[2].Current = Calculate4byteInput( dataToBeChecked [84], dataToBeChecked [85], dataToBeChecked [86], dataToBeChecked [87] );
            CardList [2].ChannelList[3].Current = Calculate4byteInput( dataToBeChecked [88], dataToBeChecked [89], dataToBeChecked [90], dataToBeChecked [91] );
            CardList [2].ChannelList[4].Current = Calculate4byteInput( dataToBeChecked [92], dataToBeChecked [93], dataToBeChecked [94], dataToBeChecked [95] );
            CardList [2].ChannelList[5].Current = Calculate4byteInput( dataToBeChecked [96], dataToBeChecked [97], dataToBeChecked [98], dataToBeChecked [99] );
            CardList [2].ChannelList[6].Current = Calculate4byteInput( dataToBeChecked [100], dataToBeChecked [101], dataToBeChecked [102], dataToBeChecked [103] );
            CardList [2].ChannelList[7].Current = Calculate4byteInput( dataToBeChecked [104], dataToBeChecked [105], dataToBeChecked [106], dataToBeChecked [107] );
        }
        /// <summary>
        /// Channel info response 6Byte each cards channel will be sent as 2 bytes
        /// </summary>
        /// <param name="dataToBeChecked"></param>
        private void ChannelInspectionReceived (byte [] dataToBeChecked)
        {
            CardList [0].ChannelCount = CalculateDataLength( dataToBeChecked [0], dataToBeChecked [1] );
            CardList [1].ChannelCount = CalculateDataLength( dataToBeChecked [2], dataToBeChecked [3] );
            CardList [2].ChannelCount = CalculateDataLength( dataToBeChecked [4], dataToBeChecked [5] );
        }   
            
        /// <summary>
        /// helper function for CreateSystem
        /// </summary>
        private void PopulateCards ( int cardCount )
        {
            for ( int i = 0; i < cardCount; i++ )
            {
                Card card = new Card( i + 1, 8 );
                card.PropertyChanged += Card_PropertyChanged;
                CardList.Add( card );
            }
        }


        /// <summary>
        /// calculates the data length with msb and lsb
        /// </summary>
        /// <param name="lengthMSB"></param>
        /// <param name="lengthLSB"></param>
        /// <returns></returns>
        private int CalculateDataLength ( byte lengthMSB, byte lengthLSB )
        {
            int result = -1;
            // Calculates msb
            byte x = (byte)StringToInt( ByteToString( lengthMSB ) );
            if ( 48 <= x && x <= 57 )
            {
                result = ( x - 48 ) * 16;
            }
            else if ( 65 <= x && x <= 70 )
            {
                result = ( x - 55 ) * 16;
            }
            else
            {
                result = x;
            }
            // Calculates lsb
            byte x2 = (byte)StringToInt( ByteToString( lengthLSB ) );
            if ( 48 <= x2 && x2 <= 57 )
            {
                result += ( x2 - 48 );
            }
            else if ( 65 <= x2 && x2 <= 70 )
            {
                result += ( x2 - 55 );
            }
            else
            {
                result += x2;
            }
            return result;
        }
        /// <summary>
        /// Calculates 4 byte addition with the help of CalculateDataLength
        /// </summary>
        /// <param name="firstMSB"></param>
        /// <param name="firstLSB"></param>
        /// <param name="secondMSB"></param>
        /// <param name="secondLSB"></param>
        /// <returns></returns>
        private int Calculate4byteInput ( byte firstMSB, byte firstLSB, byte secondMSB, byte secondLSB )
        {
            return CalculateDataLength( firstMSB, firstLSB ) + CalculateDataLength( secondMSB, secondLSB );
        }
        /// <summary>
        /// takes byte as input and converts it to 0x00 format
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private string ByteToString ( byte x )
        {
            return x.ToString( "X2" );
        }
        /// <summary>
        /// Splits given byte to two bytes
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        byte [] SplitByteToTwo ( byte x )
        {
            return Encoding.ASCII.GetBytes( ByteToString( x ) );
        }
        /// <summary>
        /// takes byte string as input and converts it to integer value
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private int StringToInt ( string x )
        {
            return int.Parse( x, System.Globalization.NumberStyles.HexNumber );
        }

        private void Card_PropertyChanged ( object? sender, PropertyChangedEventArgs e )
        {
            OnPropertyChanged( e.PropertyName );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event PropertyChangedEventHandler? AnswerReady;

        private void OnPropertyChanged ( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }

        private void OnAnswerReady ( string answer )
        {
            AnswerReady?.Invoke( this, new PropertyChangedEventArgs( answer ) );
        }
    }
}
