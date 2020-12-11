using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RouteDev.Data
{
    public abstract class Cargo : INotifyPropertyChanged
    {
        private protected const byte StorageX = 17;
        private protected const byte StorageY = 4;

        private protected int _products = 0;
        private protected int _chemistry = 0;
        private protected int _drinks = 0;

        public virtual int Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }
        public virtual int Chemistry
        {
            get => _chemistry;
            set
            {
                _chemistry = value;
                OnPropertyChanged(nameof(Chemistry));
            }
        }
        public virtual int Drinks
        {
            get => _drinks;
            set
            {
                _drinks = value;
                OnPropertyChanged(nameof(Drinks));
            }
        }


        public int Sum => Products + Chemistry + Drinks;

        public bool AnyEmpty() => Products == 0 || Chemistry == 0 || Drinks == 0;
        public bool AllEmpty() => Products == 0 && Chemistry == 0 && Drinks == 0;
        public bool AnyNeed() => Products != 0 || Chemistry != 0 || Drinks != 0;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}