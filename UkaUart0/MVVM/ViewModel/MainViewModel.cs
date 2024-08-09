﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using UkaUart0.MVVM.Model;
using UkaUart0.MVVM.Model.DataModels.UI;

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

        /// <summary>
        /// activity collections hold isopen status for cards and channels 0=card 1= ch1...
        /// </summary>
        private ObservableCollection<double> card1Activity;
        public ObservableCollection<double> Card1Activity
        {
            get
            {
                return card1Activity;
            }
            set
            {
                if ( card1Activity != value )
                {
                    card1Activity = value;
                    OnPropertyChanged( nameof( Card1Activity ) );
                }
            }
        }
        // card 2
        private ObservableCollection<double> card2Activity;
        public ObservableCollection<double> Card2Activity
        {
            get
            {
                return card2Activity;
            }
            set
            {
                if ( card2Activity != value )
                {
                    card2Activity = value;
                    OnPropertyChanged( nameof( Card2Activity ) );
                }
            }
        }
        // card3
        private ObservableCollection<double> card3Activity;
        public ObservableCollection<double> Card3Activity
        {
            get
            {
                return card3Activity;
            }
            set
            {
                if ( card3Activity != value )
                {
                    card3Activity = value;
                    OnPropertyChanged( nameof( Card3Activity ) );
                }
            }
        }

        /// <summary>
        /// Error status collections keep channel error leds conditions Total 24 element for each
        /// 
        /// Index 
        /// 0,1,2    => ch1 =>  0(shorCircuit),  1(OverCurrent),  2(VoltageError)
        /// 3,4,5    => ch2 =>  3(shorCircuit),  4(OverCurrent),  5(VoltageError)
        /// 6,7,8    => ch3 =>  6(shorCircuit),  7(OverCurrent),  8(VoltageError)
        /// 9,10,11  => ch4 =>  9(shorCircuit), 10(OverCurrent), 11(VoltageError)
        /// 12,13,14 => ch5 => 12(shorCircuit), 13(OverCurrent), 14(VoltageError)
        /// 15,16,17 => ch6 => 15(shorCircuit), 16(OverCurrent), 17(VoltageError)
        /// 18,19,20 => ch7 => 18(shorCircuit), 19(OverCurrent), 20(VoltageError)
        /// 21,22,23 => ch8 => 21(shorCircuit), 22(OverCurrent), 23(VoltageError)
        /// </summary>
        private ObservableCollection<Color> card1ChannelErrorStatus;
        public ObservableCollection<Color> Card1ChannelErrorStatus
        {
            get
            {
                return card1ChannelErrorStatus;
            }
            set
            {
                if ( card1ChannelErrorStatus != value)
                {
                    card1ChannelErrorStatus = value;
                    OnPropertyChanged(nameof(Card1ChannelErrorStatus));
                }
            }
        }

        private ObservableCollection<Color> card2ChannelErrorStatus;
        public ObservableCollection<Color> Card2ChannelErrorStatus
        {
            get
            {
                return card2ChannelErrorStatus;
            }
            set
            {
                if ( card2ChannelErrorStatus != value )
                {
                    card2ChannelErrorStatus = value;
                    OnPropertyChanged( nameof( Card2ChannelErrorStatus ) );
                }
            }
        }

        private ObservableCollection<Color> card3ChannelErrorStatus;
        public ObservableCollection<Color> Card3ChannelErrorStatus
        {
            get
            {
                return card3ChannelErrorStatus;
            }
            set
            {
                if ( card3ChannelErrorStatus != value )
                {
                    card3ChannelErrorStatus = value;
                    OnPropertyChanged( nameof( Card3ChannelErrorStatus ) );
                }
            }
        }


        private ObservableCollection<Brush> card1CommunicationStatus;
        public ObservableCollection<Brush> Card1CommunicationStatus
        {
            get
            {
                return card1CommunicationStatus;
            }
            set
            {
                if ( card1CommunicationStatus != value)
                {
                    card1CommunicationStatus = value;
                    OnPropertyChanged( nameof( Card1CommunicationStatus ) );
                }
            }
        }

        private ObservableCollection<Brush> card2CommunicationStatus;
        public ObservableCollection<Brush> Card2CommunicationStatus
        {
            get
            {
                return card2CommunicationStatus;
            }
            set
            {
                if ( card2CommunicationStatus != value )
                {
                    card2CommunicationStatus = value;
                    OnPropertyChanged( nameof( Card2CommunicationStatus ) );
                }
            }
        }

        private ObservableCollection<Brush> card3CommunicationStatus;
        public ObservableCollection<Brush> Card3CommunicationStatus
        {
            get
            {
                return card3CommunicationStatus;
            }
            set
            {
                if ( card3CommunicationStatus != value )
                {
                    card1CommunicationStatus = value;
                    OnPropertyChanged( nameof( Card1CommunicationStatus ) );
                }
            }
        }

        private double card1Voltage;
        public double Card1Voltage
        {
            get
            {
                return card1Voltage;
            }
            set
            {
                if ( card1Voltage != value )
                {
                    card1Voltage = value;
                    OnPropertyChanged( nameof( Card1Voltage ) );
                }
            }
        }

        private double card2Voltage;
        public double Card2Voltage
        {
            get
            {
                return card2Voltage;
            }
            set
            {
                if ( card2Voltage != value )
                {
                    card2Voltage = value;
                    OnPropertyChanged( nameof( Card2Voltage ) );
                }
            }
        }

        private double card3Voltage;
        public double Card3Voltage
        {
            get
            {
                return card3Voltage;
            }
            set
            {
                if ( card3Voltage != value )
                {
                    card3Voltage = value;
                    OnPropertyChanged( nameof( Card3Voltage ) );
                }
            }
        }

        private double card1Temp;
        public double Card1Temp
        {
            get
            {
                return card1Temp;
            }
            set
            {
                if ( card1Temp != value)
                {
                    card1Temp = value;
                    OnPropertyChanged(nameof(Card1Temp));
                }
            }
        }

        private double card2Temp;
        public double Card2Temp
        {
            get
            {
                return card2Temp;
            }
            set
            {
                if ( card2Temp != value )
                {
                    card2Temp = value;
                    OnPropertyChanged( nameof( Card2Temp ) );
                }
            }
        }

        private double card3Temp;
        public double Card3Temp
        {
            get
            {
                return card3Temp;
            }
            set
            {
                if ( card3Temp != value )
                {
                    card3Temp = value;
                    OnPropertyChanged( nameof( Card3Temp ) );
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

            Card1Activity = new ObservableCollection<double>();
            Card2Activity = new ObservableCollection<double>();
            Card3Activity = new ObservableCollection<double>();

            Card1ChannelErrorStatus = new ObservableCollection<Color>();
            Card2ChannelErrorStatus = new ObservableCollection<Color>();
            Card3ChannelErrorStatus = new ObservableCollection<Color>();
        }

        /// <summary>
        /// Initilize UI for 1st time opening
        /// </summary>
        private void InitUI ()
        {
            for ( int i = 0; i < 9; i++ )
            {
                card1Visibility.Add( Visibility.Visible );
                Card1Activity.Add(BrushesToBeUsed.OPACITY_PASSIVE);

                Card1ChannelErrorStatus.Add(BrushesToBeUsed.NOCOMMUNICATION);
                Card1ChannelErrorStatus.Add(BrushesToBeUsed.NOCOMMUNICATION);
                Card1ChannelErrorStatus.Add(BrushesToBeUsed.NOCOMMUNICATION);
            }
            for ( int i = 0; i < 9; i++ )
            {
                card2Visibility.Add( Visibility.Visible );
                Card2Activity.Add( BrushesToBeUsed.OPACITY_PASSIVE );

                Card2ChannelErrorStatus.Add( BrushesToBeUsed.NOCOMMUNICATION );
                Card2ChannelErrorStatus.Add( BrushesToBeUsed.NOCOMMUNICATION );
                Card2ChannelErrorStatus.Add( BrushesToBeUsed.NOCOMMUNICATION );
            }
            for ( int i = 0; i < 9; i++ )
            {
                card3Visibility.Add( Visibility.Visible );
                Card3Activity.Add( BrushesToBeUsed.OPACITY_PASSIVE );

                Card3ChannelErrorStatus.Add( BrushesToBeUsed.NOCOMMUNICATION );
                Card3ChannelErrorStatus.Add( BrushesToBeUsed.NOCOMMUNICATION );
                Card3ChannelErrorStatus.Add( BrushesToBeUsed.NOCOMMUNICATION );
            }
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

        private string FindPropertyName (string input)
        {
            string result = "unknown";
            int tempIndex = input.IndexOf(" ");
            result = input.Substring( 0, tempIndex );
            return result;
        }

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
            int [] indexes = FindChannelDetails( e.PropertyName );
            string whatToDo = FindPropertyName( e.PropertyName );
            UpdateDetails( whatToDo, indexes [0], indexes [1] );
        }

        /// <summary>
        /// Triggered when ComSystem Fires Property change event
        /// </summary>
        /// <param name="cardIndex"></param>
        /// <param name="chIndex"></param>
        private void UpdateDetails ( string whatToDo, int cardIndex, int chIndex )
        {
            switch (whatToDo)
            {
                case "IsOpen":
                    if ( chIndex == 9 ) // Means we only need to change card visibility
                    {
                        if ( cardIndex == 1 ) // Card 1
                        {
                            switch ( ComSystem.DataHandlerModel.CardList [0].IsOpen )
                            {
                                case true:
                                    Card1Activity [0] = BrushesToBeUsed.OPACITY_ACTIVE;
                                    break;
                                case false:
                                    Card1Activity [0] = BrushesToBeUsed.OPACITY_PASSIVE;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if ( cardIndex == 2 ) // Card 2
                        {
                            switch ( ComSystem.DataHandlerModel.CardList [1].IsOpen )
                            {
                                case true:
                                    Card2Activity [0] = BrushesToBeUsed.OPACITY_ACTIVE;
                                    break;
                                case false:
                                    Card2Activity [0] = BrushesToBeUsed.OPACITY_PASSIVE;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if ( cardIndex == 3 ) // Card 3
                        {
                            switch ( ComSystem.DataHandlerModel.CardList [2].IsOpen )
                            {
                                case true:
                                    Card3Activity [0] = BrushesToBeUsed.OPACITY_ACTIVE;
                                    break;
                                case false:
                                    Card3Activity [0] = BrushesToBeUsed.OPACITY_PASSIVE;
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
                            switch ( ComSystem.DataHandlerModel.CardList [0].ChannelList [chIndex - 1].IsOpen )
                            {
                                case true:
                                    Card1Activity [chIndex] = BrushesToBeUsed.OPACITY_ACTIVE;
                                    break;
                                case false:
                                    Card1Activity [chIndex] = BrushesToBeUsed.OPACITY_PASSIVE;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if ( cardIndex == 2 ) // Card 2
                        {
                            switch ( ComSystem.DataHandlerModel.CardList [1].ChannelList [chIndex - 1].IsOpen )
                            {
                                case true:
                                    Card2Activity [chIndex] = BrushesToBeUsed.OPACITY_ACTIVE;
                                    break;
                                case false:
                                    Card2Activity [chIndex] = BrushesToBeUsed.OPACITY_PASSIVE;
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if ( cardIndex == 3 ) // Card 3
                        {
                            switch ( ComSystem.DataHandlerModel.CardList [2].ChannelList [chIndex - 1].IsOpen )
                            {
                                case true:
                                    Card3Activity [chIndex] = BrushesToBeUsed.OPACITY_ACTIVE;
                                    break;
                                case false:
                                    Card3Activity [chIndex] = BrushesToBeUsed.OPACITY_PASSIVE;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    break;
                case "CardVoltage":
                    if ( cardIndex == 1 )
                    {
                        Card1Voltage = ComSystem.DataHandlerModel.CardList [0].CardVoltage;
                    }
                    else if ( cardIndex == 2 )
                    {
                        Card2Voltage = ComSystem.DataHandlerModel.CardList [1].CardVoltage;
                    }
                    else if ( cardIndex == 3 )
                    {
                        Card3Voltage = ComSystem.DataHandlerModel.CardList [2].CardVoltage;
                    }
                    break;
                case "CardTemp":
                    if ( cardIndex == 1 )
                    {
                        Card1Temp = ComSystem.DataHandlerModel.CardList [0].CardTemp;
                    }
                    else if ( cardIndex == 2 )
                    {
                        Card2Temp = ComSystem.DataHandlerModel.CardList [1].CardTemp;
                    }
                    else if ( cardIndex == 3 )
                    {
                        Card3Temp = ComSystem.DataHandlerModel.CardList [2].CardTemp;
                    }
                    break;
                case "ErrorCode":
                    HandleErrorCode(cardIndex, chIndex);
                    break;
                default:
                    break;
            }
        }

        private void HandleErrorCode (int cardIndex, int chIndex)
        {
            var errorCode = 0;
            byte shortCircuit = 0b00000001; // 1
            byte overCurrent = 0b00000010; // 2
            byte voltageError = 0b00000100; // 4
            /* 1- shortCircuit 
             * 2- overCurrent 
             * 3- shortCircuit + overCurrent
             * 4- voltageError
             * 5- shortCircuit + voltageError 
             * 6- overCurrent + voltageError
             * 7- shortCircuit + overCurrent + voltageError
             * 128- No communication 
             */
            errorCode = ComSystem.DataHandlerModel.CardList [cardIndex - 1].ChannelList [chIndex - 1].ErrorCode;
            Color [] temp = BrushesToBeUsed.ERROR_STATUS [errorCode];
            if ( cardIndex == 1 )
            {
                Card1ChannelErrorStatus [(chIndex*3) -3] = temp [0];
                Card1ChannelErrorStatus [(chIndex*3) -2] = temp [1];
                Card1ChannelErrorStatus [(chIndex*3) -1] = temp [2];
            }
            else if ( cardIndex == 2 )
            {
                Card2ChannelErrorStatus [( chIndex * 3 ) - 3] = temp [0];
                Card2ChannelErrorStatus [( chIndex * 3 ) - 2] = temp [1];
                Card2ChannelErrorStatus [( chIndex * 3 ) - 1] = temp [2];
            }
            else if ( cardIndex == 3 )
            {
                Card3ChannelErrorStatus [( chIndex * 3 ) - 3] = temp [0];
                Card3ChannelErrorStatus [( chIndex * 3 ) - 2] = temp [1];
                Card3ChannelErrorStatus [( chIndex * 3 ) - 1] = temp [2];
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
