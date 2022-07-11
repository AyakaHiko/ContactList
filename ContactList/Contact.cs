using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ContactList
{
    internal class Contact : INotifyPropertyChanged, IEquatable<Contact>
    {
        private string _name=string.Empty;
        private string _number=string.Empty;
        private string _email=string.Empty;
        private string _address=string.Empty;
        private string _country=string.Empty;
        public string Initials
        {
            get
            {
                var fullName = Name.Split(" ".ToCharArray(),StringSplitOptions.RemoveEmptyEntries);
                return fullName.Length > 1 ? $"{fullName[0][0].ToString().ToUpper()} {fullName[1][0].ToString().ToUpper()}" : fullName[0][0].ToString().ToUpper();
            }
        }
        public string Email
        {
            get => _email;
            set => _setChanged(ref _email, value, nameof(Email));
        }

        public string Address
        {
            get => _address;
            set => _setChanged(ref _address, value, nameof(Address));
        }

        public string Country
        {
            get => _country;
            set => _setChanged(ref _country, value, nameof(Country));
        }
        public string Name
        {
            get => _name;
            set
            {
                _setChanged(ref _name, value, nameof(Name));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Initials)));
            }
        }

        public string Number
        {
            get => _number;
            set => _setChanged(ref _number, value, nameof(Number));
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
            return _name == other._name && _number == other._number && _email== other._email && _address == other._address && _country==other._country;
        }
    }

}
