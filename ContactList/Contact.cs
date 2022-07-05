using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContactList
{
    internal class Contact : INotifyPropertyChanged, IEquatable<Contact>
    {
        private string _name;
        private string _phoneNum;
        public string Name
        {
            get => _name;
            set => _setChanged(ref _name, value, nameof(Name));
        }

        public string PhoneNum
        {
            get => _phoneNum;
            set => _setChanged(ref _phoneNum, value, nameof(PhoneNum));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void _setChanged<T>(ref T field, T value, string propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        

        public bool Equals(Contact other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _name == other._name && _phoneNum == other._phoneNum;
        }

    }
}
