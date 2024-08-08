using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace UkaUart0.MVVM.Model.DataModels
{
    partial class Channel : INotifyPropertyChanged
    {
        public int CardIndex { get; set; }
        public int ChannelIndex { get; set; }
        
        private bool isOpen;
        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                if ( isOpen != value )
                {
                    isOpen = value;
                    OnPropertyChanged(nameof(IsOpen));
                }
            }
        }

        private int errorCode;
        public int ErrorCode
        {
            get
            {
                return errorCode;
            }
            set
            {
                if ( errorCode != value)
                {
                    errorCode = value;
                    OnPropertyChanged(nameof(ErrorCode));
                }
            }
        }

        private double current;
        public double Current
        {
            get
            {
                return current;
            }
            set
            {
                if ( current != value )
                {
                    current = value;
                    OnPropertyChanged(nameof(Current));
                }
            }
        }

        public Channel (int cardIndex, int chIndex)
        {
            this.CardIndex = cardIndex;
            this.ChannelIndex = chIndex;
            isOpen = false;
            ErrorCode = 0;
            current = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged ( string propertyName )
        {
            PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName + " " + CardIndex + " " + ChannelIndex ) );
        }

    }
}
