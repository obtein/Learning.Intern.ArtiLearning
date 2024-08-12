using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UkaUart0.MVVM.Model.DataModels;

namespace UkaUart0.MVVM.Model.CommunicationModels
{
    class SerialCommunication : INotifyPropertyChanged
    {
        Thread readThread;

        public static SerialPort _serialPort;

        private Queue<byte> currentDataReceived;
        public Queue<byte> CurrentDataReceived
        {
            get
            {
                return currentDataReceived;
            }
            set
            {
                if ( currentDataReceived != value )
                {
                    currentDataReceived = value;
                    OnPropertyChanged(nameof(CurrentDataReceived));
                }
            }
        }

        public SerialCommunication (string ctorPortName, int ctorBaudRate, Parity ctorParity,
                                    int ctorDataBits, StopBits ctorStopBits, Handshake ctorHandshake,
                                    int ctorReadTimeout, int ctorWriteTimeout)
        {
            CurrentDataReceived =  new Queue<byte>();
            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();
            // Allow the user to set the appropriate properties.
            _serialPort.PortName = ctorPortName;
            _serialPort.BaudRate = ctorBaudRate;
            _serialPort.Parity = ctorParity;
            _serialPort.DataBits = ctorDataBits;
            _serialPort.StopBits = ctorStopBits;
            _serialPort.Handshake = ctorHandshake;
            // Set the read/write timeouts
            _serialPort.ReadTimeout = ctorReadTimeout;
            _serialPort.WriteTimeout = ctorWriteTimeout;

            _serialPort.DataReceived += new SerialDataReceivedEventHandler( DataReceivedHandler );
        }

        private void DataReceivedHandler ( object sender, SerialDataReceivedEventArgs e )
        {
            Trace.WriteLine("SerialCommunication : DataReceivedHandler ");
            readThread = new Thread( Read );
            readThread.Start();
            //readThread.Join();
        }

        private void Read ()
        {
            var flag = true;
            var data = new Queue<byte>();
            var readStream = _serialPort.BaseStream;
            while ( flag )
            {
                try
                {
                    byte message = ((byte)readStream.ReadByte());
                    if(message != (byte)EnumCommunicationParameters.Stx)
                        data.Enqueue( message );
                    if ( message == (byte)EnumCommunicationParameters.Etx )
                        flag = false;
                }
                catch ( TimeoutException ) 
                {
                    Console.WriteLine( "SerialCommunication : DataReceivedHandler *********TIMEOUT" );
                }
            }
            CurrentDataReceived.Clear();
            CurrentDataReceived = data;
        }

        public void StartCommunication ()
        {
            if(!_serialPort.IsOpen)
                _serialPort.Open();
        }

        public void StopCommunication ()
        {
            if(_serialPort.IsOpen)
                _serialPort.Close();
        }


        #region PredefinedCommands

        public bool SendChannelInspection () // start of the conversation
        {
            if ( _serialPort.IsOpen )
            {
                var writeStream = _serialPort.BaseStream;
                if ( writeStream.CanWrite )
                    writeStream.Write( CommunicationPackages.ChannelInspection);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Sends Analog inspection
        /// </summary>
        /// <returns></returns>
        public bool SendAnalogInspection ()
        {
            if ( _serialPort.IsOpen )
            {
                var writeStream = _serialPort.BaseStream;
                if ( writeStream.CanWrite )
                    writeStream.Write( CommunicationPackages.AnalogInspection );
                return true;
            }
            return false;
        }
        /// <summary>
        /// Sends card error inspection
        /// </summary>
        public bool SendCardErrorInspection ()
        {
            if ( _serialPort.IsOpen )
            {
                var writeStream = _serialPort.BaseStream;
                if ( writeStream.CanWrite )
                    _serialPort.BaseStream.Write( CommunicationPackages.CardErrorInspection );
                return true;
            }
            return false;
        }
        /// <summary>
        /// Sends inspection open/close CARD1
        /// </summary>
        public bool SendCard1OpenOrClosed ()
        {
            if ( _serialPort.IsOpen )
            {
                var writeStream = _serialPort.BaseStream;
                if ( writeStream.CanWrite )
                    writeStream.Write( CommunicationPackages.Card1OpenOrCloseInspection );
                return true;
            }
            return false;
        }
        /// <summary>
        /// Sends inspection open/close CARD2
        /// </summary>
        public bool SendCard2OpenOrClosed ()
        {
            if ( _serialPort.IsOpen )
            {
                var writeStream = _serialPort.BaseStream;
                if ( writeStream.CanWrite )
                    writeStream.Write( CommunicationPackages.Card2OpenOrCloseInspection );
                return true;
            }
            return false;
        }
        /// <summary>
        /// Sends inspection open/close CARD3
        /// </summary>
        public bool SendCard3OpenOrClosed ()
        {
            if ( _serialPort.IsOpen )
            {
                var writeStream = _serialPort.BaseStream;
                if ( writeStream.CanWrite )
                    writeStream.Write( CommunicationPackages.Card3OpenOrCloseInspection );
                return true;
            }
            return false;
        }
        /// <summary>
        /// Sends open request for specifed cards channel
        /// </summary>
        public void SendChannelOpenRequest ( byte [] dataToBeSent )
        {
            var writeStream = _serialPort.BaseStream;
            if ( writeStream.CanWrite )
                writeStream.Write( dataToBeSent );
        }
        /// <summary>
        /// Sends close request for specifed cards channel
        /// </summary>
        public void SendChannelCloseRequest ( byte [] dataToBeSent )
        {
            var writeStream = _serialPort.BaseStream;
            if ( writeStream.CanWrite )
                writeStream.Write( dataToBeSent );
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged ( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}
