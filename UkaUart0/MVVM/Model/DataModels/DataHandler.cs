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

        public DataHandler ()
        {
            CardList = new ObservableCollection<Card>();
            PopulateCards( 3 );
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
