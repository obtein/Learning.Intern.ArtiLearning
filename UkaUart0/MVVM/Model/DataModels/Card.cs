using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UkaUart0.MVVM.Model.DataModels
{
    partial class Card : INotifyPropertyChanged
    {
        private bool checkBeforeNotify;
        public int CardIndex { get; set; }

        private int channelCount;
        public int ChannelCount
        {
            get
            {
                return channelCount;
            }
            set
            {
                if ( channelCount != value )
                {
                    channelCount = value;
                    OnPropertyChanged( nameof( ChannelCount ) );
                }
            }
        }

        private double cardVoltage;
        public double CardVoltage
        {
            get { return cardVoltage; }
            set 
            {
                if ( cardVoltage != value )
                {
                    cardVoltage = value;
                    OnPropertyChanged(nameof(CardVoltage));
                } 
            }
        }

        private double cardTemp;
        public double CardTemp
        {
            get { return cardTemp; }
            set
            {
                if ( cardTemp != value )
                {
                    cardTemp = value; 
                    OnPropertyChanged(nameof(CardTemp));
                }
            }
        }

        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                if ( isOpen != value )
                {
                    isOpen = value;
                    OnPropertyChanged( nameof( IsOpen ) );
                }
            }
        }

        private List<Channel> channelList;
        public List<Channel> ChannelList
        {
            get { return channelList; }
            set
            {
                if ( channelList != value )
                {
                    channelList = value;
                    OnPropertyChanged( nameof( ChannelList ) );
                }
            }
            
        }


        public Card (int cardIndex, int chCount)
        {
            checkBeforeNotify = false;
            CardIndex = cardIndex;
            ChannelCount = chCount;
            cardVoltage = 0;
            cardTemp = 0;
            isOpen = false;
            ChannelList = new List<Channel>();
            for ( int i = 0; i < ChannelCount; i++ )
            {
                Channel ch = new Channel(CardIndex,i+1);
                ch.PropertyChanged += new PropertyChangedEventHandler(Channel_PropertyChanged);
                ChannelList.Add(ch);
            }
        }

        private void Channel_PropertyChanged ( object? sender, PropertyChangedEventArgs e )
        {
            checkBeforeNotify = true;
            OnPropertyChanged( e.PropertyName );
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged ( string propertyName )
        {
            string temp = "";
            if ( checkBeforeNotify )
                temp = propertyName;
            else 
                temp = propertyName + " " + CardIndex + " " + 9;
            checkBeforeNotify = false;
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( temp ) );
        }

    }
}
