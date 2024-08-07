using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
            ComSystem = new ComSystem(0);
            ComSystem.PropertyChanged += new PropertyChangedEventHandler (ComSystem_PropertyChanged);

            InitVariables();
            InitUI();
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

        /*
         * To DO : 
         * 
            Console.WriteLine( "Available Ports:" );
            foreach ( string s in SerialPort.GetPortNames() )
            {
                Console.WriteLine( "   {0}", s );
            }
         */

        #region UpdateUIFromModelNotifications
        /// <summary>
        /// To update data properly splits propertychanged string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private int [] FindChannelDetails ( string input )
        {
            int [] result = new int [2];
            int tempIndex = input.IndexOf( " " );
            string tempCardIndex = input.Substring( tempIndex, 2 );
            string tempChannelIndex = input.Substring( tempIndex + 2, 2 );
            result [0] = Convert.ToInt32( tempCardIndex );
            result [1] = Convert.ToInt32( tempChannelIndex );
            return result;
        }

        /// <summary>
        /// Works with UpdateDetails
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComSystem_PropertyChanged ( object? sender, PropertyChangedEventArgs e )
        {
            int [] whatToDo = FindChannelDetails( e.PropertyName );
            UpdateDetails( whatToDo [0], whatToDo [1] );
        }

        /// <summary>
        /// Triggered when ComSystem Fires Property change event
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <param name="chIndex"></param>
        private void UpdateDetails ( int cardIndex, int chIndex )
        {
            if ( chIndex == 9 ) // Means we only need to change card visibility
            {
                if ( cardIndex == 1 ) // Card 1
                {
                    switch ( ComSystem.CardList [0].IsOpen )
                    {
                        case true:
                            Card1Visibility [0] = Visibility.Visible;
                            break;
                        case false:
                            Card1Visibility [0] = Visibility.Hidden;
                            break;
                        default:
                            break;
                    }
                }
                else if ( cardIndex == 2 ) // Card 2
                {
                    switch ( ComSystem.CardList [1].IsOpen )
                    {
                        case true:
                            Card2Visibility [0] = Visibility.Visible;
                            break;
                        case false:
                            Card2Visibility [0] = Visibility.Hidden;
                            break;
                        default:
                            break;
                    }
                }
                else if ( cardIndex == 3 ) // Card 3
                {
                    switch ( ComSystem.CardList [2].IsOpen )
                    {
                        case true:
                            Card3Visibility [0] = Visibility.Visible;
                            break;
                        case false:
                            Card3Visibility [0] = Visibility.Hidden;
                            break;
                        default:
                            break;
                    }
                }
            }
            else // Means channel property changed
            {
                if ( cardIndex == 1 ) // Card 1
                {
                    switch ( ComSystem.CardList [0].ChannelList [chIndex - 1].IsOpen )
                    {
                        case true:
                            Card1Visibility [chIndex] = Visibility.Visible;
                            break;
                        case false:
                            Card1Visibility [chIndex] = Visibility.Hidden;
                            break;
                        default:
                            break;
                    }
                }
                else if ( cardIndex == 2 ) // Card 2
                {
                    switch ( ComSystem.CardList [1].ChannelList [chIndex - 1].IsOpen )
                    {
                        case true:
                            Card2Visibility [chIndex] = Visibility.Visible;
                            break;
                        case false:
                            Card2Visibility [chIndex] = Visibility.Hidden;
                            break;
                        default:
                            break;
                    }
                }
                else if ( cardIndex == 3 ) // Card 3
                {
                    switch ( ComSystem.CardList [2].ChannelList [chIndex - 1].IsOpen )
                    {
                        case true:
                            Card3Visibility [chIndex] = Visibility.Visible;
                            break;
                        case false:
                            Card3Visibility [chIndex] = Visibility.Hidden;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged ( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
        }
    }
}
