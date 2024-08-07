using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UkaUart0.MVVM.Model
{
    class DataHandler : INotifyPropertyChanged
    {


        /// <summary>
        /// Decodes the received msg into meaningfull parts
        /// </summary>
        /// <returns></returns>
        private void SplitDataIntoMeanings (byte [] input)
        {
            Stream current = new MemoryStream(input);
            byte stx = 0x02;
            byte etx = 0x03;
            byte msgID;
            byte msgCode;
            byte dataLengthMSB;
            byte dataLengthLSB;
            int dataLength;
            byte [] data;
            byte checkSumMSB;
            byte checkSumLSB;
            byte [] checkSum;
            if ( (byte)current.ReadByte() == stx )
            {
                msgID = (byte)current.ReadByte();
                msgCode = (byte)current.ReadByte();
                dataLengthMSB = (byte)current.ReadByte();
                dataLengthLSB = (byte)current.ReadByte();
                dataLength = CalculateDataLength( dataLengthMSB, dataLengthLSB );
                data = new byte [dataLength];
                for ( int i = 0; i < dataLength; i++ )
                {
                    data [i] = (byte)current.ReadByte();
                }
                WhatToDoWithData( msgCode, data );
                checkSumMSB = (byte)current.ReadByte();
                checkSumLSB = (byte)current.ReadByte();
                if ( (byte)current.ReadByte() == etx )
                {
                    SerialDataReceived = CreateMessage( msgID, msgCode, data, checkSumMSB, checkSumLSB );
                }
                else
                {
                    Trace.WriteLine( "Etx not received" );
                    current.Flush();
                }
            }
            else
            {
                Trace.WriteLine( "Stx not received" );
                current.Flush();
            }
        }

        /// <summary>
        /// 1 - 0xC9 is for Channel information response
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
        /// <param name="msgCode"></param>
        /// <param name="dataToBeChecked"></param>
        /// <returns></returns>
        private async Task WhatToDoWithData ( byte msgCode, byte [] dataToBeChecked )
        {
            if ( msgCode == 0xC9 ) // Channel info response 6Byte each cards channel will be sent as 2 bytes
            {
                await Task.Run( () => Card1ChannelCount = CalculateDataLength( dataToBeChecked [0], dataToBeChecked [1] ) );
                await Task.Run( () => Card2ChannelCount = CalculateDataLength( dataToBeChecked [2], dataToBeChecked [3] ) );
                await Task.Run( () => Card3ChannelCount = CalculateDataLength( dataToBeChecked [4], dataToBeChecked [5] ) );
            }
            else if ( msgCode == 0xCA ) // Analog info response for each card first 4 byte is card voltage second 4 byte channel1 current third 4 byte channel2 current and so on
            {
                // Card 1
                Card1Voltage = Calculate4byteInput( dataToBeChecked [0], dataToBeChecked [1], dataToBeChecked [2], dataToBeChecked [3] );
                Card1Channel1Current = Calculate4byteInput( dataToBeChecked [4], dataToBeChecked [5], dataToBeChecked [6], dataToBeChecked [7] );
                Card1Channel2Current = Calculate4byteInput( dataToBeChecked [8], dataToBeChecked [9], dataToBeChecked [10], dataToBeChecked [11] );
                Card1Channel3Current = Calculate4byteInput( dataToBeChecked [12], dataToBeChecked [13], dataToBeChecked [14], dataToBeChecked [15] );
                Card1Channel4Current = Calculate4byteInput( dataToBeChecked [16], dataToBeChecked [17], dataToBeChecked [18], dataToBeChecked [19] );
                Card1Channel5Current = Calculate4byteInput( dataToBeChecked [20], dataToBeChecked [21], dataToBeChecked [22], dataToBeChecked [23] );
                Card1Channel6Current = Calculate4byteInput( dataToBeChecked [24], dataToBeChecked [25], dataToBeChecked [26], dataToBeChecked [27] );
                Card1Channel7Current = Calculate4byteInput( dataToBeChecked [28], dataToBeChecked [29], dataToBeChecked [30], dataToBeChecked [31] );
                Card1Channel8Current = Calculate4byteInput( dataToBeChecked [32], dataToBeChecked [33], dataToBeChecked [34], dataToBeChecked [35] );
                // Card 2
                Card2Voltage = Calculate4byteInput( dataToBeChecked [36], dataToBeChecked [37], dataToBeChecked [38], dataToBeChecked [39] );
                Card2Channel1Current = Calculate4byteInput( dataToBeChecked [40], dataToBeChecked [41], dataToBeChecked [42], dataToBeChecked [43] );
                Card2Channel2Current = Calculate4byteInput( dataToBeChecked [44], dataToBeChecked [45], dataToBeChecked [46], dataToBeChecked [47] );
                Card2Channel3Current = Calculate4byteInput( dataToBeChecked [48], dataToBeChecked [49], dataToBeChecked [50], dataToBeChecked [51] );
                Card2Channel4Current = Calculate4byteInput( dataToBeChecked [52], dataToBeChecked [53], dataToBeChecked [54], dataToBeChecked [55] );
                Card2Channel5Current = Calculate4byteInput( dataToBeChecked [56], dataToBeChecked [57], dataToBeChecked [58], dataToBeChecked [59] );
                Card2Channel6Current = Calculate4byteInput( dataToBeChecked [60], dataToBeChecked [61], dataToBeChecked [62], dataToBeChecked [63] );
                Card2Channel7Current = Calculate4byteInput( dataToBeChecked [64], dataToBeChecked [65], dataToBeChecked [66], dataToBeChecked [67] );
                Card2Channel8Current = Calculate4byteInput( dataToBeChecked [68], dataToBeChecked [69], dataToBeChecked [70], dataToBeChecked [71] );
                // Card 3
                Card3Voltage = Calculate4byteInput( dataToBeChecked [72], dataToBeChecked [73], dataToBeChecked [74], dataToBeChecked [75] );
                Card3Channel1Current = Calculate4byteInput( dataToBeChecked [76], dataToBeChecked [77], dataToBeChecked [78], dataToBeChecked [79] );
                Card3Channel2Current = Calculate4byteInput( dataToBeChecked [80], dataToBeChecked [81], dataToBeChecked [82], dataToBeChecked [83] );
                Card3Channel3Current = Calculate4byteInput( dataToBeChecked [84], dataToBeChecked [85], dataToBeChecked [86], dataToBeChecked [87] );
                Card3Channel4Current = Calculate4byteInput( dataToBeChecked [88], dataToBeChecked [89], dataToBeChecked [90], dataToBeChecked [91] );
                Card3Channel5Current = Calculate4byteInput( dataToBeChecked [92], dataToBeChecked [93], dataToBeChecked [94], dataToBeChecked [95] );
                Card3Channel6Current = Calculate4byteInput( dataToBeChecked [96], dataToBeChecked [97], dataToBeChecked [98], dataToBeChecked [99] );
                Card3Channel7Current = Calculate4byteInput( dataToBeChecked [100], dataToBeChecked [101], dataToBeChecked [102], dataToBeChecked [103] );
                Card3Channel8Current = Calculate4byteInput( dataToBeChecked [104], dataToBeChecked [105], dataToBeChecked [106], dataToBeChecked [107] );
            }
            else if ( msgCode == 0xCB ) // Error inspection response for each card  bit0(short circuit),bit1 (over current), bit2(voltage error), bit3-7 reserved => byte0-15 card1, 16-31 card2, 31-47 card3 
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
                Card1Channel1IsActive = dataToBeChecked [0] != 128 && dataToBeChecked [1] != 128;
                Card1Channel1IsOk = CalculateDataLength( dataToBeChecked [0], dataToBeChecked [1] );
                Card1Channel2IsActive = dataToBeChecked [2] != 128 && dataToBeChecked [3] != 128;
                Card1Channel2IsOk = CalculateDataLength( dataToBeChecked [2], dataToBeChecked [3] );
                Card1Channel3IsActive = dataToBeChecked [4] != 128 && dataToBeChecked [5] != 128;
                Card1Channel3IsOk = CalculateDataLength( dataToBeChecked [4], dataToBeChecked [5] );
                Card1Channel4IsActive = dataToBeChecked [6] != 128 && dataToBeChecked [7] != 128;
                Card1Channel4IsOk = CalculateDataLength( dataToBeChecked [6], dataToBeChecked [7] );
                Card1Channel5IsActive = dataToBeChecked [8] != 128 && dataToBeChecked [9] != 128;
                Card1Channel5IsOk = CalculateDataLength( dataToBeChecked [8], dataToBeChecked [9] );
                Card1Channel6IsActive = dataToBeChecked [10] != 128 && dataToBeChecked [11] != 128;
                Card1Channel6IsOk = CalculateDataLength( dataToBeChecked [10], dataToBeChecked [11] );
                Card1Channel7IsActive = dataToBeChecked [12] != 128 && dataToBeChecked [13] != 128;
                Card1Channel7IsOk = CalculateDataLength( dataToBeChecked [12], dataToBeChecked [13] );
                Card1Channel8IsActive = dataToBeChecked [14] != 128 && dataToBeChecked [15] != 128;
                Card1Channel8IsOk = CalculateDataLength( dataToBeChecked [14], dataToBeChecked [15] );
                Card2Channel1IsActive = dataToBeChecked [16] != 128 && dataToBeChecked [17] != 128;
                Card2Channel1IsOk = CalculateDataLength( dataToBeChecked [16], dataToBeChecked [17] );
                Card2Channel2IsActive = dataToBeChecked [18] != 128 && dataToBeChecked [19] != 128;
                Card2Channel2IsOk = CalculateDataLength( dataToBeChecked [18], dataToBeChecked [19] );
                Card2Channel3IsActive = dataToBeChecked [20] != 128 && dataToBeChecked [21] != 128;
                Card2Channel3IsOk = CalculateDataLength( dataToBeChecked [20], dataToBeChecked [21] );
                Card2Channel4IsActive = dataToBeChecked [22] != 128 && dataToBeChecked [23] != 128;
                Card2Channel4IsOk = CalculateDataLength( dataToBeChecked [22], dataToBeChecked [23] );
                Card2Channel5IsActive = dataToBeChecked [24] != 128 && dataToBeChecked [25] != 128;
                Card2Channel5IsOk = CalculateDataLength( dataToBeChecked [24], dataToBeChecked [25] );
                Card2Channel6IsActive = dataToBeChecked [26] != 128 && dataToBeChecked [27] != 128;
                Card2Channel6IsOk = CalculateDataLength( dataToBeChecked [26], dataToBeChecked [27] );
                Card2Channel7IsActive = dataToBeChecked [28] != 128 && dataToBeChecked [29] != 128;
                Card2Channel7IsOk = CalculateDataLength( dataToBeChecked [28], dataToBeChecked [29] );
                Card2Channel8IsActive = dataToBeChecked [30] != 128 && dataToBeChecked [31] != 128;
                Card2Channel8IsOk = CalculateDataLength( dataToBeChecked [30], dataToBeChecked [31] );
                Card3Channel1IsActive = dataToBeChecked [32] != 128 && dataToBeChecked [33] != 128;
                Card3Channel1IsOk = CalculateDataLength( dataToBeChecked [32], dataToBeChecked [33] );
                Card3Channel2IsActive = dataToBeChecked [34] != 128 && dataToBeChecked [35] != 128;
                Card3Channel2IsOk = CalculateDataLength( dataToBeChecked [34], dataToBeChecked [35] );
                Card3Channel3IsActive = dataToBeChecked [36] != 128 && dataToBeChecked [37] != 128;
                Card3Channel3IsOk = CalculateDataLength( dataToBeChecked [36], dataToBeChecked [37] );
                Card3Channel4IsActive = dataToBeChecked [38] != 128 && dataToBeChecked [39] != 128;
                Card3Channel4IsOk = CalculateDataLength( dataToBeChecked [38], dataToBeChecked [39] );
                Card3Channel5IsActive = dataToBeChecked [40] != 128 && dataToBeChecked [41] != 128;
                Card3Channel5IsOk = CalculateDataLength( dataToBeChecked [40], dataToBeChecked [41] );
                Card3Channel6IsActive = dataToBeChecked [42] != 128 && dataToBeChecked [43] != 128;
                Card3Channel6IsOk = CalculateDataLength( dataToBeChecked [42], dataToBeChecked [43] );
                Card3Channel7IsActive = dataToBeChecked [44] != 128 && dataToBeChecked [45] != 128;
                Card3Channel7IsOk = CalculateDataLength( dataToBeChecked [44], dataToBeChecked [45] );
                Card3Channel8IsActive = dataToBeChecked [46] != 128 && dataToBeChecked [47] != 128;
                Card3Channel8IsOk = CalculateDataLength( dataToBeChecked [46], dataToBeChecked [47] );
            }
            else if ( msgCode == 0xCE ) // Card 1 is open/closed 2Byte lsb 1open 0 closed
            {
                Card1IsActive = dataToBeChecked [1] == 1;
            }
            else if ( msgCode == 0xCF ) // Card 2 is open/closed 2Byte lsb 1open 0 closed
            {
                Card2IsActive = dataToBeChecked [1] == 1;
            }
            else if ( msgCode == 0xD0 ) // Card 3 is open/closed 2Byte lsb 1open 0 closed
            {
                Card3IsActive = dataToBeChecked [1] == 1;
            }
            else
            {
                SerialDataReceived = "UNEXPECTED RESPONSE";
            }

        }

        #region
        private string CreateMessage ( byte id, byte msgCode, byte [] dataReceived, byte checkSumMsb, byte checkSumLsb )
        {
            string result = "";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append( ByteToString( id ) );
            stringBuilder.Append( ByteToString( msgCode ) );
            for ( int i = 0; i < dataReceived.Length; i++ )
            {
                stringBuilder.Append( ByteToString( dataReceived [i] ) );
            }
            stringBuilder.Append( ByteToString( checkSumMsb ) );
            stringBuilder.Append( ByteToString( checkSumLsb ) );
            result = stringBuilder.ToString();
            return result;
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
        /// Calculates the checkSum of given array of bytes 
        /// </summary>
        /// <param name="checkSumToBeCalculated"></param>
        /// <returns> [0] as msb [1] as lsb </returns>
        private byte [] CalculateCheckSum ( byte [] checkSumToBeCalculated )
        {
            byte tempResult = 0x00;
            foreach ( byte x in checkSumToBeCalculated )
            {
                tempResult ^= x;
            }
            return SplitByteToTwo( tempResult );
        }
        /// <summary>
        /// Takes Card index and channel index as input then calculates the byte array to be sent
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <param name="channelIndex"></param>
        /// <returns> return checksum as byte[] = { msb[0], lsb[1]} </returns>
        private byte [] CalculateOpenCommandToBeSent ( int cardIndex, int channelIndex )
        {
            byte toBegin = 0b00000001;
            byte [] result = new byte [12]; // to be returned
            byte [] checkSumCalc = new byte [9]; // to calculate checksum
            byte [] checkSumTemp = new byte [2]; // to send checksum Method
                                                 // Generic properties
            byte stx = STX;
            byte etx = ETX;
            byte id = ID;
            byte msgCode = 0x00;
            // Data part
            byte dataLengthMSB = 0x30;
            byte dataLengthLSB = 0x34;
            byte dataByte1MSB = 0xFF;
            byte dataByte1LSB = 0x00;
            byte dataByte0MSB = 0xFF;
            byte dataByte0LSB = 0x00;
            // Check sum
            byte checkSumMSB;
            byte checkSumLSB;

            if ( cardIndex == 0 )
            {
                msgCode = CARD1_OPEN_CHANNEL;
            }
            else if ( cardIndex == 1 )
            {
                msgCode = CARD2_OPEN_CHANNEL;
            }
            else if ( cardIndex == 2 )
            {
                msgCode = CARD3_OPEN_CHANNEL;
            }

            if ( channelIndex == 1 )
            {
                byte temp1 = toBegin;
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 2 )
            {
                byte temp1 = (byte)( toBegin << 1 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 3 )
            {
                byte temp1 = (byte)( toBegin << 2 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 4 )
            {
                byte temp1 = (byte)( toBegin << 3 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 5 )
            {
                byte temp1 = (byte)( toBegin << 4 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 6 )
            {
                byte temp1 = (byte)( toBegin << 5 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 7 )
            {
                byte temp1 = (byte)( toBegin << 6 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 8 )
            {
                byte temp1 = (byte)( toBegin << 7 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }

            checkSumCalc [0] = stx;
            checkSumCalc [1] = id;
            checkSumCalc [2] = msgCode;
            checkSumCalc [3] = dataLengthMSB;
            checkSumCalc [4] = dataLengthLSB;
            checkSumCalc [5] = dataByte1MSB;
            checkSumCalc [6] = dataByte1LSB;
            checkSumCalc [7] = dataByte0MSB;
            checkSumCalc [8] = dataByte0LSB;
            checkSumTemp = CalculateCheckSum( checkSumCalc );
            checkSumMSB = checkSumTemp [0];
            checkSumLSB = checkSumTemp [1];

            result [0] = stx;
            result [1] = id;
            result [2] = msgCode;
            result [3] = dataLengthMSB;
            result [4] = dataLengthLSB;
            result [5] = dataByte1MSB;
            result [6] = dataByte1LSB;
            result [7] = dataByte0MSB;
            result [8] = dataByte0LSB;
            result [9] = checkSumMSB;
            result [10] = checkSumLSB;
            result [11] = etx;
            return result;
        }
        /// <summary>
        /// Takes Card index and channel index as input then calculates the byte array to be sent
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <param name="channelIndex"></param>
        /// <returns> return checksum as byte[] = { msb[0], lsb[1]} </returns>
        private byte [] CalculateCloseCommandToBeSent ( int cardIndex, int channelIndex )
        {
            byte toBegin = 0b00000001;
            byte [] result = new byte [12]; // to be returned
            byte [] checkSumCalc = new byte [9]; // to calculate checksum
            byte [] checkSumTemp = new byte [2]; // to send checksum Method
                                                 // Generic properties
            byte stx = STX;
            byte etx = ETX;
            byte id = ID;
            byte msgCode = 0x00;
            // Data part
            byte dataLengthMSB = 0x30;
            byte dataLengthLSB = 0x34;
            byte dataByte1MSB = 0xFF;
            byte dataByte1LSB = 0x00;
            byte dataByte0MSB = 0xFF;
            byte dataByte0LSB = 0x00;
            // Check sum
            byte checkSumMSB;
            byte checkSumLSB;

            if ( cardIndex == 0 )
            {
                msgCode = CARD1_CLOSE_CHANNEL;
            }
            else if ( cardIndex == 1 )
            {
                msgCode = CARD2_CLOSE_CHANNEL;
            }
            else if ( cardIndex == 2 )
            {
                msgCode = CARD3_CLOSE_CHANNEL;
            }

            if ( channelIndex == 1 )
            {
                byte temp1 = toBegin;
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 2 )
            {
                byte temp1 = (byte)( toBegin << 1 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 3 )
            {
                byte temp1 = (byte)( toBegin << 2 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 4 )
            {
                byte temp1 = (byte)( toBegin << 3 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 5 )
            {
                byte temp1 = (byte)( toBegin << 4 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 6 )
            {
                byte temp1 = (byte)( toBegin << 5 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 7 )
            {
                byte temp1 = (byte)( toBegin << 6 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }
            else if ( channelIndex == 8 )
            {
                byte temp1 = (byte)( toBegin << 7 );
                byte temp0 = (byte)( ~temp1 );
                byte [] split1 = SplitByteToTwo( temp1 );
                byte [] split0 = SplitByteToTwo( temp0 );
                dataByte1MSB = split1 [0];
                dataByte1LSB = split1 [1];
                dataByte0MSB = split0 [0];
                dataByte0LSB = split0 [1];
            }

            checkSumCalc [0] = stx;
            checkSumCalc [1] = id;
            checkSumCalc [2] = msgCode;
            checkSumCalc [3] = dataLengthMSB;
            checkSumCalc [4] = dataLengthLSB;
            checkSumCalc [5] = dataByte1MSB;
            checkSumCalc [6] = dataByte1LSB;
            checkSumCalc [7] = dataByte0MSB;
            checkSumCalc [8] = dataByte0LSB;
            checkSumTemp = CalculateCheckSum( checkSumCalc );
            checkSumMSB = checkSumTemp [0];
            checkSumLSB = checkSumTemp [1];

            result [0] = stx;
            result [1] = id;
            result [2] = msgCode;
            result [3] = dataLengthMSB;
            result [4] = dataLengthLSB;
            result [5] = dataByte1MSB;
            result [6] = dataByte1LSB;
            result [7] = dataByte0MSB;
            result [8] = dataByte0LSB;
            result [9] = checkSumMSB;
            result [10] = checkSumLSB;
            result [11] = etx;
            return result;
        }
        #endregion



        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged ( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}
