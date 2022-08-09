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
using System.IO;
namespace minesweeper
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(@"C:/Users/grzes/Desktop/all\programs/minesweeper - final edition/minesweeper/TextFile1.txt", "difficulty: 8");
            MainMenu mainMenu = new MainMenu();
            ((MainWindow)Application.Current.MainWindow).Content = mainMenu;
        }
    }
}
