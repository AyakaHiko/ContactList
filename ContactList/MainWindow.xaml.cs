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
            foreach (var box in EditPanel.Children.OfType<TextBox>())
            {
                if (string.IsNullOrEmpty(box.Text)) return;
            }
            Contact contact = _newContact();
            if (!_contacts.Contains(contact))
                _contacts.Add(contact);
            ContactListbox.UnselectAll();
        }

        private Contact _newContact()
        {
            Contact contact = new Contact();
            contact.Name = ContactName.Text;
            contact.PhoneNum = ContactNumber.Text;
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

            XmlNode root = xmlDoc.DocumentElement;

            foreach (var contact in _contacts)
            {
                root?.AppendChild(_createNode(contact));
            }
            xmlDoc.Save(path);

        }

        private void MenuOpen_OnClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Xml Doc|*.xml";
            if (fileDialog.ShowDialog() != true) return;

            _contacts.Clear();
            _createFile(fileDialog.FileName);
            foreach (XmlNode contact in xmlDoc.GetElementsByTagName("Contact"))
            {
                if (!contact.HasChildNodes) continue;
                Contact newContact = new Contact();
                XmlNode item = contact.FirstChild;
                do
                {
                    switch (item.Name)
                    {
                        case "Name":
                            newContact.Name = item.InnerText; break;
                        case "Number":
                            newContact.PhoneNum = item.InnerText; break;
                    }
                    item = item.NextSibling;
                } while (item != null);
                _contacts.Add(newContact);
            }
        }

        private XmlDocument xmlDoc = new XmlDocument();
        private XmlNode _createNode(Contact contact)
        {
            XmlNode root = xmlDoc.CreateElement("Contact");
            XmlNode name = xmlDoc.CreateElement("Name");
            XmlNode num = xmlDoc.CreateElement("Number");
            name.InnerText = contact.Name;
            root.AppendChild(name);
            num.InnerText = contact.PhoneNum;
            root.AppendChild(num);
            return root;
        }

        private void _createFile(string path)
        {
            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path.Substring(0, path.Length - System.IO.Path.GetFileName(path).Length));
                xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", "yes"));
                xmlDoc.AppendChild(xmlDoc.CreateElement("Contacts"));
                xmlDoc.Save(path);
            }
            else
            {
                xmlDoc.Load(path);
            }
        }
        private void _delete()
        {
            XmlNode root = xmlDoc.DocumentElement;

            if (root != null)
            {
                root.RemoveAll();
            }
        }

    }
}
