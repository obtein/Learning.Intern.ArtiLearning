using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using UkaUart0.MVVM.Model;

namespace UkaUart0.MVVM.ViewModel
{
    class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// For updating ui
        /// 0 for cards and 1- channel 1, 2- channel 2...
        /// </summary>
        /// card 1
        private ObservableCollection<Visibility> card1Visibility;
        public ObservableCollection<Visibility> Card1Visibility
        {
            get
            {
                return card1Visibility;
            }
            set
            {
                if ( card1Visibility != value )
                {
                    card1Visibility = value;
                    OnPropertyChanged(nameof(Card1Visibility));
                }
            }
        }
        // card 2
        private ObservableCollection<Visibility> card2Visibility;
        public ObservableCollection<Visibility> Card2Visibility
        {
            get
            {
                return card2Visibility;
            }
            set
            {
                if ( card2Visibility != value )
                {
                    card2Visibility = value;
                    OnPropertyChanged( nameof( Card2Visibility ) );
                }
            }
        }
        // card3
        private ObservableCollection<Visibility> card3Visibility;
        public ObservableCollection<Visibility> Card3Visibility
        {
            get
            {
                return card3Visibility;
            }
            set
            {
                if ( card3Visibility != value )
                {
                    card3Visibility = value;
                    OnPropertyChanged( nameof( Card3Visibility ) );
                }
            }
        }

        ComSystem ComSystem { get; set; }

        public MainViewModel() 
        {
            //ComSystem.PropertyChanged += new PropertyChangedEventHandler (ComSystem_PropertyChanged);

            InitVariables();
            InitUI();
        }

        private void ComSystem_PropertyChanged ( object? sender, PropertyChangedEventArgs e )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initilize Variables for 1st time opening
        /// </summary>
        private void InitVariables ()
        {
            Card1Visibility = new ObservableCollection<Visibility>();
            Card2Visibility = new ObservableCollection<Visibility>();
            Card3Visibility = new ObservableCollection<Visibility>();
        }

        /// <summary>
        /// Initilize UI for 1st time opening
        /// </summary>
        private void InitUI ()
        {
            for ( int i = 0; i < 9; i++ )
                card1Visibility.Add(Visibility.Hidden);
            for ( int i = 0; i < 9; i++ )
                card2Visibility.Add( Visibility.Hidden );
            for ( int i = 0; i < 9; i++ )
                card3Visibility.Add( Visibility.Hidden );
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged ( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}
