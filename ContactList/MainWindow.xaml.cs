using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Xml;

using Microsoft.Win32;
using Path = System.Windows.Shapes.Path;

namespace ContactList
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<Contact> _contacts;

        public MainWindow()
        {
            _contacts = new ObservableCollection<Contact>();
            
            InitializeComponent();

            ContactListbox.ItemsSource = _contacts;
        }

        private void AddNew_OnClick(object sender, RoutedEventArgs e)
        {
            Contact contact = _newContact();
            if(contact == null) return;
            if (!_contacts.Contains(contact))
                _contacts.Add(contact);
            ContactListbox.UnselectAll();
        }

        private Contact _newContact()
        {
            Contact contact = new Contact();
            if (string.IsNullOrEmpty(ContactName.Text) || string.IsNullOrEmpty(ContactNumber.Text)) return null;
            contact.Name = ContactName.Text;
            contact.Number = ContactNumber.Text;
            contact.Email = ContactEmail.Text;
            contact.Address = ContactAddress.Text;
            contact.Country = ContactCountry.Text;
            return contact;
        }

        private void ContactNumber_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            Contact contact = _newContact();
            if (_contacts.Contains(contact)) return;
            if (ContactListbox.SelectedItem == null)
            {
                _contacts.Add(contact);
            }
            else
            {
                foreach (var textBox in EditPanel.Children.OfType<TextBox>())
                {
                    var b = textBox.GetBindingExpression(TextBox.TextProperty);
                    b.UpdateSource();
                }
                
            }
            ContactListbox.UnselectAll();

        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            if (ContactListbox.SelectedItem == null) return;
            _contacts.Remove((Contact)ContactListbox.SelectedItem);

        }

        private void MenuSave_OnClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "Xml Doc|*.xml";
            if (fileDialog.ShowDialog() != true) return;
            string path = fileDialog.FileName;
            _createFile(path);
            _delete();

            XmlNode root = _xmlDoc.DocumentElement;

            foreach (var contact in _contacts)
            {
                var c = _getXmlNode(contact);
                root?.AppendChild(c);
            }
            _xmlDoc.Save(path);

        }

        private XmlNode _getXmlNode(Contact contact)
        {
            XmlNode root = _xmlDoc.CreateElement("Contact");
            XmlNode name = _xmlDoc.CreateElement("Name");
            XmlNode num = _xmlDoc.CreateElement("Number");
            XmlNode email = _xmlDoc.CreateElement("Email");
            XmlNode address = _xmlDoc.CreateElement("Address");
            XmlNode country = _xmlDoc.CreateElement("Country");
            name.InnerText = contact.Name;
            num.InnerText = contact.Number;
            email.InnerText = contact.Email;
            address.InnerText = contact.Address;
            country.InnerText = contact.Country;
            root.AppendChild(name);
            root.AppendChild(num);
            root.AppendChild(email);
            root.AppendChild(address);
            root.AppendChild(country);
            return root;
        }


        private Contact _loadFromNode(XmlNode node)
        {
            node = node.FirstChild;
            if(node == null) return null;
            Contact contact = new Contact();
            do
            {
                switch (node.Name)
                {
                    case "Name":
                        contact.Name = node.InnerText; break;
                    case "Number":
                        contact.Number = node.InnerText; break;
                    case "Email":
                        contact.Email = node.InnerText; break;
                    case "Address":
                        contact.Address = node.InnerText; break;
                    case "Country":
                        contact.Country = node.InnerText; break;
                }
                node = node.NextSibling;
            } while (node != null);

            return contact;

        }
        private void MenuOpen_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Xml Doc|*.xml";
            if (fileDialog.ShowDialog() != true) return;
            _contacts.Clear();
            _createFile(fileDialog.FileName);
            foreach (XmlNode contact in _xmlDoc.GetElementsByTagName("Contact"))
            {
                if (!contact.HasChildNodes) continue;
               _contacts.Add(_loadFromNode(contact));
            }
        }

        private readonly XmlDocument _xmlDoc = new XmlDocument();

        private void _createFile(string path)
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path.Substring(0, path.Length - System.IO.Path.GetFileName(path).Length));
                _xmlDoc.AppendChild(_xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
                _xmlDoc.AppendChild(_xmlDoc.CreateElement("Contacts"));
                _xmlDoc.Save(path);
            }
            else
            {
                _xmlDoc.Load(path);
            }
        }
        private void _delete()
        {
            XmlNode root = _xmlDoc.DocumentElement;

            if (root != null)
            {
                root.RemoveAll();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ContactListbox.UnselectAll();
        }
    }
}
