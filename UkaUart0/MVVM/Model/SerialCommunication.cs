using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UkaUart0.MVVM.Model
{
    class SerialCommunication : INotifyPropertyChanged
    {
        StringComparer stringComparer;
        Thread readThread;

        public static bool _continue;
        public static SerialPort _serialPort;


        public string SelectedComPort { get; set; }
        public int SelectedBaudRate { get; set; }
        public Parity SelectedParity { get; set; }
        public int SelectedDataBits { get; set; }
        public StopBits SelectedStopBits { get; set; }
        public Handshake SelectedHandShake { get; set; }
        public int SelectedReadTimeOut { get; set; }
        public int SelectedWriteTimeOut { get; set; }
        public DataHandler DedicatedDataHandler { get; set; }


        public SerialCommunication ()
        {
            stringComparer = StringComparer.OrdinalIgnoreCase;
            readThread = new Thread( Read );
            DedicatedDataHandler = new DataHandler();
            DedicatedDataHandler.PropertyChanged += new PropertyChangedEventHandler( DataHandler_PropertyChanged );

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();

            // Allow the user to set the appropriate properties.
            _serialPort.PortName = "COM8";
            _serialPort.BaudRate = 115200;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;

            // Set the read/write timeouts
            _serialPort.ReadTimeout = -1;
            _serialPort.WriteTimeout = -1;

            _serialPort.DataReceived += new SerialDataReceivedEventHandler( DataReceivedHandler );
            
            
            
            
            
            
            _serialPort.Open();
            _continue = true;
            readThread.Start();

            readThread.Join();
        }

        private void DataHandler_PropertyChanged ( object? sender, PropertyChangedEventArgs e )
        {
            throw new NotImplementedException();
        }

        private void DataReceivedHandler ( object sender, SerialDataReceivedEventArgs e )
        {
            throw new NotImplementedException();
        }

        public static void Read ()
        {
            while ( _continue )
            {
                try
                {
                    string message = _serialPort.ReadLine();
                    Console.WriteLine( message );
                }
                catch ( TimeoutException ) { }
            }
        }

        public static void Write ()
        {
        }

        public static void StartCommunication ()
        {

        }

        public static void StopCommunication ()
        {
            _serialPort.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
