using System;
using System.Collections.Generic;
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

namespace minesweeper
{
    public partial class MainMenu : Page
    {
        public MainMenu()
        {
            InitializeComponent();
        }
        private void BT_Options_Click(object sender, RoutedEventArgs e)
        {
            Options options = new Options();
            ((MainWindow)Application.Current.MainWindow).Content = options;
        }

        private void BT_Start_Click(object sender, RoutedEventArgs e)
        {
            Game game = new Game();
            ((MainWindow)Application.Current.MainWindow).Content = game;
        }

        private void BT_Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
