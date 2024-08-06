using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UkaUart0.MVVM.Model
{
    class ComSystem : INotifyPropertyChanged
    {
        private List<Card>? cardList;
        public List<Card>? CardList 
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
            CardList = new List<Card>();
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
                Card card = new Card();
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
            PopulateCards();
        }
        /// <summary>
        /// helper function for CreateSystem
        /// </summary>
        private void PopulateCards ()
        {
            foreach ( Card c in CardList )
            {
                for ( int i = 0; i < c.ChannelCount; i++ )
                {
                    c.ChannelList.Add(new Channel());
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged ( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}
