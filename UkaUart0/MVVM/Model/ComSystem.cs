using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UkaUart0.MVVM.Model
{
    class ComSystem : INotifyPropertyChanged
    {
        public int CardIndexUpdated { get; set; }
        public int ChannelIndexUpdated { get; set; }

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

        public SerialCommunication SerialCommunicationModel { get; set; }
        public EthernetCommunication EthernetCommunicationModel { get; set; }
        public DataHandler DataHandlerModel { get; set; }

        /// <summary>
        ///  TO-DO : Fill the neccessary details for communication and data handlermodel
        /// </summary>
        /// <param name="selectedCommunication"></param>
        ///             0 : Serial
        ///             1 : Ethernet
        public ComSystem (int selectedCommunication)
        {
            if ( selectedCommunication == 0 )
                SerialCommunicationModel = new SerialCommunication();
            if ( selectedCommunication == 1 )
                EthernetCommunicationModel = new EthernetCommunication();
            DataHandlerModel = new DataHandler();
            CardList = new ObservableCollection<Card>();
            PopulateCards(3);
        }

        /// <summary>
        /// First enterance, it creates the sytem based on first received data
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
                CardList.Add( card );
            }
            //PopulateCards();
        }
        /// <summary>
        /// helper function for CreateSystem
        /// </summary>
        private void PopulateCards (int cardCount)
        {
            for (int i = 0; i < cardCount; i++ )
            {
                Card card = new Card(i+1,8);
                card.PropertyChanged += Card_PropertyChanged;
                CardList.Add( card);
            }
        }

        private void Card_PropertyChanged ( object? sender, PropertyChangedEventArgs e )
        {
            OnPropertyChanged( e.PropertyName );
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

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged ( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}
