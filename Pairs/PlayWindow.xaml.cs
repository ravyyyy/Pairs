using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Pairs
{
    /// <summary>
    /// Interaction logic for PlayWindow.xaml
    /// </summary>
    public partial class PlayWindow : Window
    {
        private Player player;
        private int counter, rows, columns, level, timer;
        private List<Player> gamesSaved = new List<Player>();
        private List<BitmapImage> doubledImages;
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private ObservableCollection<BitmapImage> images = new ObservableCollection<BitmapImage>
        {
            new BitmapImage(new Uri(@"/img/drax.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/hulk.png", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/daredevil.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/goose.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/gamora.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/nebula.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/taskmaster.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/ultron.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/agent_coulson.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/baron_zemo.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/black_widow.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/captain_america.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/dr_strange.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/hawkeye.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/ironheart.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/m_baku.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/moon_knight.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/spider_man.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/vision.jpg", UriKind.Relative)),
            new BitmapImage(new Uri(@"/img/wolverine.jpg", UriKind.Relative))
        };

        public void Shuffle(ObservableCollection<BitmapImage> images)
        {
            Random random = new Random();
            int n = images.Count;
            while(n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                BitmapImage value = images[k];
                images[k] = images[n];
                images[n] = value;
            }
        }

        public void Shuffle2<T>(IList<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            while(n>1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public PlayWindow(Player player)
        {
            InitializeComponent();
            this.player = player;
            usernameTextBlock.Text = player.Name;
            usernameImage.Source = images[player.ImageIndex];
            gameTimer.Interval = TimeSpan.FromSeconds(1);
            gameTimer.Tick += GameTimer_Tick;
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            timer--;
            seconds.Text = $"{timer}";
            if(timer == 0)
            {
                gameTimer.Stop();
                foreach(var child in myGrid.Children)
                {
                    if(child is Button button)
                    {
                        button.IsEnabled = false;
                    }
                }
                MessageBox.Show("You lost!");
            }
        }

        private void CreateButtonMatrix(int n, int m)
        {
            counter = 0;
            Shuffle(images);
            ObservableCollection<BitmapImage> randomImages = new ObservableCollection<BitmapImage>(images.Take((n * m) / 2));
            doubledImages = randomImages.Concat(randomImages).ToList();
            Shuffle2(doubledImages);
            myGrid.Children.Clear();
            myGrid.RowDefinitions.Clear();
            myGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < n + m; i++)
            {
                myGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                myGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            int j = 0;

            for(int row = 0; row < n; row++)
            {
                for(int col = 0; col < m; col++)
                {
                    BitmapImage image = doubledImages[j++];
                    Button button = new Button();
                    Image buttonImage = new Image() { Source = image };
                    buttonImage.Visibility = Visibility.Collapsed;
                    button.Content = buttonImage;
                    button.Click += ButtonShowClick;
                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    myGrid.Children.Add(button);
                }
            }
        }

        private void ButtonShowClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Image image = button.Content as Image;
            button.IsEnabled = false;
            image.Visibility = Visibility.Visible;
            clickedButtons.Add(button);

            if (clickedButtons.Count == 2)
            {
                Button button1 = clickedButtons[0];
                Button button2 = clickedButtons[1];
                Image firstImage = clickedButtons[0].Content as Image;
                BitmapImage firstBitmapImage = firstImage.Source as BitmapImage;
                Image secondImage = clickedButtons[1].Content as Image;
                BitmapImage secondBitmapImage = secondImage.Source as BitmapImage;
                clickedButtons[0].IsEnabled = false;
                clickedButtons[1].IsEnabled = false;
                if (firstBitmapImage.UriSource == secondBitmapImage.UriSource)
                {
                    Task.Delay(750).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            button1.Visibility = Visibility.Collapsed;
                            button2.Visibility = Visibility.Collapsed;
                        });
                    });
                    counter += 2;
                }
                else
                {
                    Task.Delay(750).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            firstImage.Visibility = Visibility.Collapsed;
                            secondImage.Visibility = Visibility.Collapsed;
                        });
                    });
                }
                clickedButtons[0].IsEnabled = true;
                clickedButtons[1].IsEnabled = true;
                clickedButtons.Clear();
            }

            if (counter == doubledImages.Count)
            {
                if (level == 3)
                {
                    Task.Delay(500).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            XDocument xmlDoc = XDocument.Load("UserData.xml");
                            XElement currentPlayerElement = xmlDoc.Descendants("Player").Where(p => (string)p.Element("username") == player.Name).FirstOrDefault();
                            if (currentPlayerElement != null)
                            {
                                int wonGames = int.Parse(currentPlayerElement.Element("wonGames").Value);
                                wonGames++;
                                currentPlayerElement.Element("wonGames").Value = wonGames.ToString();
                                xmlDoc.Save("UserData.xml");
                            }
                            MessageBox.Show("Congratulations!", "", MessageBoxButton.OK);
                            gameTimer.Stop();
                        });
                    });
                }
                if (level == 2)
                {
                    Task.Delay(1250).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            CreateButtonMatrix(rows, columns);
                            levelTextBlock.Text = "Level 3";
                            level = 3;
                        });
                    });
                }
                if (level == 1)
                {
                    Task.Delay(1250).ContinueWith(_ =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            CreateButtonMatrix(rows, columns);
                            levelTextBlock.Text = "Level 2";
                            level = 2;
                        });
                    });
                }
            }
        }

        private List<Button> clickedButtons = new List<Button>();

        private void Exit_Click(object sender, RoutedEventArgs e) // Exit Click
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void About_Click(object sender, RoutedEventArgs e) // About Click
        {
            MessageBox.Show("Efrim Dragos-Alexandru\n10LF211\nInformatica", "About", MessageBoxButton.OK);
        }

        private void OnCustomCheck(object sender, RoutedEventArgs e) // Custom Click
        {
            StandardMenuItem.IsChecked = false;
            CustomMenuItem.IsChecked = true;
            CustomWindow customWindow = new CustomWindow();
            customWindow.DataEntered += CustomWindow_DataEntered;
            customWindow.Show();
        }

        private void CustomWindow_DataEntered(object sender, DataEnteredEvent e)
        {
            rows = e.rows;
            columns = e.columns;
        }

        private void OnStandardCheck(object sender, RoutedEventArgs e) // Standard Click
        {
            CustomMenuItem.IsChecked = false;
            StandardMenuItem.IsChecked = true;
            rows = 6;
            columns = 6;
        }

        private void StatisticsButton(object sender, RoutedEventArgs e) // Statistics Button
        {
            Statistics statisticsWindow = new Statistics(player);
            statisticsWindow.Show();
        }

        private void NewGameButton(object sender, RoutedEventArgs e) // New Game Button
        {
            timer = rows * columns * 3 * 3;
            gameTimer.Start();
            XDocument xmlDoc = XDocument.Load("UserData.xml");
            XElement currentPlayerElement = xmlDoc.Descendants("Player").Where(p => (string)p.Element("username") == player.Name).FirstOrDefault();
            if (currentPlayerElement != null)
            {
                int playedGames = int.Parse(currentPlayerElement.Element("playedGames").Value);
                playedGames++;
                currentPlayerElement.Element("playedGames").Value = playedGames.ToString();
                xmlDoc.Save("UserData.xml");
            }
            myGrid.Visibility = Visibility.Visible;
            levelTextBlock.Text = "Level 1";
            CreateButtonMatrix(rows, columns);
            level = 1;
            counter = 0;
            seconds.Text = timer.ToString();
        }

        private void SaveGameButton(object sender, RoutedEventArgs e) // Save Game Button
        {

        }

        private void OpenGameButton(object sender, RoutedEventArgs e) // Open Game Button
        {

        }
    }
}
