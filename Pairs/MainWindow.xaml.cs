using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace Pairs
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<BitmapImage> AvatarImages = new ObservableCollection<BitmapImage>
        {
                new BitmapImage(new Uri(@"/img/drax.jpg", UriKind.Relative)),
                new BitmapImage(new Uri(@"/img/hulk.png", UriKind.Relative)),
                new BitmapImage(new Uri(@"/img/daredevil.jpg", UriKind.Relative)),
                new BitmapImage(new Uri(@"/img/goose.jpg", UriKind.Relative)),
                new BitmapImage(new Uri(@"/img/gamora.jpg", UriKind.Relative)),
                new BitmapImage(new Uri(@"/img/nebula.jpg", UriKind.Relative)),
                new BitmapImage(new Uri(@"/img/taskmaster.jpg", UriKind.Relative)),
                new BitmapImage(new Uri(@"/img/ultron.jpg", UriKind.Relative))
        };
        ObservableCollection<Player> playerList = new ObservableCollection<Player>();

        public void UpdatePlayerList(ObservableCollection<Player> updatedList)
        {
            playerList = updatedList;
            listView.ItemsSource = playerList;
        }

        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists("UserData.xml"))
            {
                CreateUserXML();
            }
            LoadXml loadXml = new LoadXml();
            playerList = loadXml.LoadUsersFromXml(@"UserData.xml");
            deleteUserButton.IsEnabled = false;
            playButton.IsEnabled = false;
            listView.ItemsSource = playerList;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listView.SelectedItem != null)
            {
                deleteUserButton.IsEnabled = true;
                playButton.IsEnabled = true;
                Player player = (Player)listView.SelectedItem;
                imageUser.Source = AvatarImages[player.ImageIndex];
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) // Add User
        {
            this.Hide();
            NewUser newWindow = new NewUser(playerList, AvatarImages);
            newWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) // Close Button
        {
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) // Previous Button
        {
            int selectedIndex = listView.SelectedIndex;
            if(selectedIndex > 0)
            {
                listView.SelectedIndex = selectedIndex - 1;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) // Next Button
        {
            int selectedIndex = listView.SelectedIndex;
            if (selectedIndex < listView.Items.Count - 1)
            {
                listView.SelectedIndex = selectedIndex + 1;
            }
        }

        public void CreateUserXML()
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement root = xmlDoc.CreateElement("Players");
            xmlDoc.AppendChild(root);
            xmlDoc.Save(@"UserData.xml");
        }

        private void deleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(@"UserData.xml");
            XmlNode root = xmlDoc.DocumentElement;

            string usernameToDelete = ((Player)listView.SelectedItem).Name;
            XmlNode item = root.SelectSingleNode("//Player[username='" + usernameToDelete + "']");
            item.ParentNode.RemoveChild(item);
            xmlDoc.Save(@"UserData.xml");
            this.UpdatePlayerList(playerList);
            LoadXml loadXml = new LoadXml();
            playerList = loadXml.LoadUsersFromXml(@"UserData.xml");
            listView.ItemsSource = playerList;
        }

        private void playButton_Click(object sender, RoutedEventArgs e) // Play Button
        {
            this.Hide();
            Player player = (Player)listView.SelectedItem;
            PlayWindow playWindow = new PlayWindow(player);
            playWindow.Show();
        }
    }
}
