using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UkaUart0.MVVM.Model.CommunicationModels;
using UkaUart0.MVVM.Model.DataModels;

namespace UkaUart0.MVVM.Model
{
    class ComSystem : INotifyPropertyChanged
    {
        public SerialCommunication SerialCommunicationModel { get; set; }
        public EthernetCommunication EthernetCommunicationModel { get; set; }
        public DataHandler DataHandlerModel { get; set; }

        /// <summary>
        ///  TO-DO : Fill the neccessary details for communication and data handlermodel
        /// </summary>
        /// <param name="selectedCommunication"></param>
        ///             0 : Serial
        ///             1 : Ethernet
        public ComSystem (int selectedCommunication, string comSystemCtorPortName,
                          int comSystemCtorBaudRate, Parity comSystemCtorParity,
                          int comSystemCtorDataBits, StopBits comSystemCtorStopBits,
                          Handshake comSystemCtorHandShake,
                          int comSystemCtorReadTimeOut,
                          int comSystemCtorWriteTimeOut
                          )
        {
            if ( selectedCommunication == 0 )
            {
                SerialCommunicationModel = new SerialCommunication( ctorPortName: comSystemCtorPortName,
                                                                   ctorBaudRate: comSystemCtorBaudRate,
                                                                   ctorParity: comSystemCtorParity,
                                                                   ctorDataBits: comSystemCtorDataBits,
                                                                   ctorStopBits: comSystemCtorStopBits,
                                                                   ctorHandshake: comSystemCtorHandShake,
                                                                   ctorReadTimeout: comSystemCtorReadTimeOut,
                                                                   ctorWriteTimeout: comSystemCtorWriteTimeOut );
                SerialCommunicationModel.PropertyChanged += SerialCommunicationModel_PropertyChanged;
                SerialCommunicationModel.StartCommunication();
                SerialCommunicationModel.SendChannelInspection();// Starts the communication
            }
            if ( selectedCommunication == 1 )
            {
                EthernetCommunicationModel = new EthernetCommunication();
            }
            DataHandlerModel = new DataHandler();
            DataHandlerModel.PropertyChanged += DataHandlerModel_PropertyChanged;
            DataHandlerModel.AnswerReady += DataHandlerModel_AnswerReady;
        }

        /// <summary>
        /// First enterance, it creates the sytem based on first received data
        /// 
        ///  TODO: Ilk başta cevap dönene kadar arayüze basma daha sonrasında ilk cevabı alınca serial com
        ///  event ateşle bunu çağırsın(bool ile kontrol olabilir) (Datahandlerdaki 2li event gibi bişey)
        /// 
        /// </summary>
        /// <param name="cardCount"></param>
        /// <param name="card1ChannelCount"></param>
        /// <param name="card2ChannelCount"></param>
        /// <param name="card3ChannelCount"></param>
        private void CreateSystem ( int cardCount, int card1ChannelCount, int card2ChannelCount, int card3ChannelCount )
        {
            for ( int i = 0; i < cardCount; i++ )
            {
                Card card = new Card(i+1,8);
                card.ChannelList = new List<Channel>();

                card.CardTemp = 0;
                card.CardVoltage = 0;
                card.IsOpen = true;
                if ( i == 0 )
                {
                    card.ChannelCount = card1ChannelCount;
                }
                else if ( i == 1 )
                {
                    card.ChannelCount = card2ChannelCount;
                }
                else if ( i == 2 )
                {
                    card.ChannelCount = card3ChannelCount;
                }
                //CardList.Add( card );
            }
            //PopulateCards();
        }
        
        
        private int [] FindChannelDetails(string input)
        {
            int [] result = new int [2];
            int tempIndex = input.IndexOf (" ");
            string tempCardIndex = input.Substring(tempIndex,2);
            string tempChannelIndex = input.Substring(tempIndex+2,2);
            result [0] = Convert.ToInt32( tempCardIndex );
            result [1] = Convert.ToInt32( tempChannelIndex );
            return result;
        }

        private void SerialCommunicationModel_PropertyChanged ( object? sender, PropertyChangedEventArgs e )
        {
            Queue<byte> temp = new Queue<byte>();
            temp = SerialCommunicationModel.CurrentDataReceived;
            int dataLength = temp.Count;
            byte [] dataToBeChecked = new byte [dataLength];
            for (int i = 0; i < dataLength; i++ )
            {
                dataToBeChecked [i] = temp.Dequeue();
            }
            DataHandlerModel.ReadData ( dataToBeChecked );
        }

        private void DataHandlerModel_AnswerReady ( object? sender, PropertyChangedEventArgs e )
        {
            // TODO : Gelen cevaba göre aksiyon almasını sağla
        }

        private void DataHandlerModel_PropertyChanged ( object? sender, PropertyChangedEventArgs e )
        {
            // TODO: burası direkt ui güncelleyecek
            Trace.WriteLine($"ComSystem DataHandler_PropertyChanged : {e.PropertyName}");
            OnPropertyChanged( e.PropertyName );
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged ( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}
