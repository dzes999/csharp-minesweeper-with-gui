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
using System.Windows.Threading;

namespace minesweeper
{
    public partial class Game : Page
    {
        DispatcherTimer dt = new DispatcherTimer();
        Label lb_flagCounter = new Label();
        Label lb_timer = new Label();
        Button[] tiles;
        Button bt_reset;
        Style? st_reset1;
        Style? st_reset2;
        Style? st_reset3;
        Style? st_reset4;
        Style? st_logo;
        Style? st_bt1;
        Style? st_bt2;
        Style? st_bomb;
        Style? st_flag;
        Style? st_exit;
        Style? st_options;

        // dxd = difficulty * difficulty \\
        int difficulty;
        int dxd;
        int bombCount = 1;

        bool firstclick = true;

        public Game()
        {
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //timer
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += Ticker;

            //styles for buttons
            st_options = FindResource("BT_Options") as Style;
            st_reset1 = FindResource("BT_Reset1") as Style;
            st_reset2 = FindResource("BT_Reset2") as Style;
            st_reset3 = FindResource("BT_Reset3") as Style;
            st_reset4 = FindResource("BT_Reset4") as Style;
            st_bomb = FindResource("BT_Bomb") as Style;
            st_flag = FindResource("BT_Flag") as Style;
            st_exit = FindResource("BT_Exit") as Style;
            st_logo = FindResource("Logo") as Style;
            st_bt1 = FindResource("BT_Looks") as Style;
            st_bt2 = FindResource("BT_Looks2") as Style;

            //game start
            CreateBoard();
        }

        private int increment = 0;

        private void Ticker(object sender, EventArgs e)
        {
            increment++;

            lb_timer.Content = increment.ToString();
        }
        public void CreateBoard()
        {
            //grid
            Grid board = new Grid();

            //difficulty
            difficulty = Convert.ToInt32(System.IO.File.ReadAllText("../../../TextFile1.txt").Split(" ")[1]);
            dxd = difficulty * difficulty;

            //buttons
            tiles = new Button[dxd];

            for (int i = 0; i < dxd; i++)
            {
                tiles[i] = new Button();
                tiles[i].MouseRightButtonDown += rightclick;
                tiles[i].Click += default_click;
                tiles[i].Name = "BT_0";
                tiles[i].Tag = i;
                tiles[i].Style = st_bt1;
            }

            //creating rows and columns
            for (int i = 0; i < difficulty; i++)
            {
                board.RowDefinitions.Add(new RowDefinition());
                board.ColumnDefinitions.Add(new ColumnDefinition());

                //setting buttons
                for (int j = 0; j < difficulty; j++)
                {
                    Grid.SetRow(tiles[j + (i * difficulty)], i);
                    Grid.SetColumn(tiles[j + (i * difficulty)], j);
                    board.Children.Add(tiles[j + (i * difficulty)]);
                }
            }

            checkboard();

            //bomb count for flag counter
            if (difficulty == 16)
            {
                bombCount = 2;
            }
            else if (difficulty == 24)
            {
                bombCount = 4;
            }

            int bombs = bombCount * difficulty;

            //board alignment
            board.HorizontalAlignment = HorizontalAlignment.Center;
            board.VerticalAlignment = VerticalAlignment.Center;

            //creating a stack panel to add a button
            StackPanel stackPanel1 = new StackPanel();
            StackPanel stackPanel2 = new StackPanel();
            StackPanel LogoStackPanel = new StackPanel();

            //logo
            Button bt_logo = new Button();
            bt_logo.Style = st_logo;

            stackPanel2.Orientation = Orientation.Horizontal;
            stackPanel2.HorizontalAlignment = HorizontalAlignment.Center;
            LogoStackPanel.Orientation = Orientation.Vertical;
            LogoStackPanel.Children.Add(bt_logo);
            stackPanel1.Children.Add(stackPanel2);
            stackPanel1.Children.Add(board);
            stackPanel1.Children.Add(LogoStackPanel);

            //developer button
            Button bt_options = new Button();
            bt_options.Height = 50;
            bt_options.Width = 50;
            bt_options.Click += developer_tool;
            bt_options.Style = st_options;

            //restart button
            bt_reset = new Button();
            bt_reset.Height = 60;
            bt_reset.Width = 60;
            bt_reset.Click += restart_button;
            bt_reset.Style = st_reset1;

            //exit button
            Button bt_exit = new Button();
            bt_exit.Height = 50;
            bt_exit.Width = 50;
            bt_exit.Click += exit_button;
            bt_exit.Style = st_exit;

            //label
            lb_timer.Height = 50;
            lb_timer.Width = 50;
            lb_timer.FontSize = 12;
            lb_timer.VerticalContentAlignment = VerticalAlignment.Center;
            lb_timer.HorizontalContentAlignment = HorizontalAlignment.Center;
            lb_timer.Foreground = new SolidColorBrush(Colors.White);
            lb_timer.Content = 0;

            //label
            lb_flagCounter.Height = 50;
            lb_flagCounter.Width = 50;
            lb_flagCounter.FontSize = 12;
            lb_flagCounter.VerticalContentAlignment = VerticalAlignment.Center;
            lb_flagCounter.HorizontalContentAlignment = HorizontalAlignment.Center;
            lb_flagCounter.Foreground = new SolidColorBrush(Colors.White);
            lb_flagCounter.Content = bombs;

            stackPanel2.Children.Add(lb_flagCounter);
            stackPanel2.Children.Add(bt_exit);
            stackPanel2.Children.Add(bt_reset);
            stackPanel2.Children.Add(bt_options);
            stackPanel2.Children.Add(lb_timer);

            //applying the stackpanel content
            this.Content = stackPanel1;
        }
        private void default_click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Style != st_flag)
            {
                if (!firstclick)
                {
                    if (((Button)sender).Tag != "boom")
                    {
                        floodfill(sender);
                        win();
                    }
                    else
                    {
                        bombClick(sender);
                    }
                }
                else
                {
                    MinesLogic();
                    floodfill(sender);
                    dt.Start();
                    firstclick = false;
                }
            }
        }
        private void floodfill(object sender)
        {
            int k = 0;
            int h = 0;
            for (int i = 0; i < dxd; i++)
            {
                if (tiles[i].Tag == ((Button)sender).Tag)
                {
                    k = i;
                }
            }

            List<Button> list = new List<Button>();
            list.Add(tiles[k]);

            while (list.Count() > 0)
            {
                
                for (int i = 0; i < dxd; i++)
                {
                    if (tiles[i] == list.Last())
                    {
                        h = i;
                    }
                }

                list.Remove(list.Last());

                if (tiles[h].IsEnabled && tiles[h].Tag != "boom" && tiles[h].Name.Split("_")[1] == "0")
                {
                    if (tiles[h].Style != st_flag)
                    {
                        tiles[h].IsEnabled = false;
                    }

                    //right
                    if (h % difficulty < difficulty - 1)
                    {
                        list.Add(tiles[h + 1]);
                    }
                    //left
                    if (h % difficulty > 0)
                    {
                        list.Add(tiles[h - 1]);
                    }
                    //down
                    if (h < dxd - difficulty)
                    {
                        list.Add(tiles[h + difficulty]);
                    }
                    //up
                    if (h > difficulty)
                    {
                        list.Add(tiles[h - difficulty]);
                    }
                }
                else
                {
                    tiles[h].IsEnabled = false;
                    if (tiles[h].Name.Split("_")[1] != "0")
                    {
                        tiles[h].Content = tiles[h].Name.Split("_")[1];
                    }
                }
            }
            firstclick = false;
        }
        public void MinesLogic()
        {
            //creating and setting mines

            int mines;

            for (int i = 0; i < bombCount * difficulty; i++)
            {
                Random rnd = new Random();
                mines = rnd.Next(1, dxd);

                if (tiles[mines].Tag == "boom")
                {
                    i--;
                    continue;
                }
                
                int help = 0;

                                        //proximity numeration\\

                //left
                if (mines % difficulty > 0)
                {
                    help = Convert.ToInt32(tiles[mines - 1].Name.Split("_")[1]) + 1;
                    /*help++;*/
                    tiles[mines - 1].Name = Convert.ToString("BT_" + help);
                }
                //right
                if (mines % difficulty < difficulty - 1)
                {
                    help = Convert.ToInt32(tiles[mines + 1].Name.Split("_")[1]) + 1;
                    tiles[mines + 1].Name = Convert.ToString("BT_" + help);
                }
                //down
                if (mines < ((dxd) - difficulty))
                {
                    help = Convert.ToInt32(tiles[mines + difficulty].Name.Split("_")[1]) + 1;
                    tiles[mines + difficulty].Name = Convert.ToString("BT_" + help);
                }
                //up
                if (mines >= difficulty)
                {
                    help = Convert.ToInt32(tiles[mines - difficulty].Name.Split("_")[1]) + 1;
                    tiles[mines - difficulty].Name = Convert.ToString("BT_" + help);
                }
                //bottom left
                if (mines < dxd - difficulty && mines % difficulty != 0)
                {
                    help = Convert.ToInt32(tiles[mines + (difficulty - 1)].Name.Split("_")[1]) + 1;
                    tiles[mines + (difficulty - 1)].Name = Convert.ToString("BT_" + help);
                }
                //upper right
                if (mines >= difficulty && mines % difficulty != difficulty - 1)
                {
                    help = Convert.ToInt32(tiles[mines - (difficulty - 1)].Name.Split("_")[1]) + 1;
                    tiles[mines - (difficulty - 1)].Name = Convert.ToString("BT_" + help);
                }
                //bottom right
                if (mines % difficulty != difficulty - 1 && mines < dxd - difficulty)
                {
                    help = Convert.ToInt32(tiles[mines + (difficulty + 1)].Name.Split("_")[1]) + 1;
                    tiles[mines + (difficulty + 1)].Name = Convert.ToString("BT_" + help);
                }
                //upper left
                if (mines > difficulty && mines % difficulty != 0)
                {
                    help = Convert.ToInt32(tiles[mines - (difficulty + 1)].Name.Split("_")[1]) + 1;
                    tiles[mines - (difficulty + 1)].Name = Convert.ToString("BT_" + help);
                }
                tiles[mines].Tag = "boom";
            }
        }
        private void zero(int x, string n)
        {
            if (n.Split("_")[1] != "0")
            {
                tiles[x].Content = n.Split("_")[1];
            }
        }
        private void disableAllButtons()
        {
            for (int i = 0; i < dxd; i++)
            {
                tiles[i].IsEnabled = false;
            }
        }
        private void bombClick(object sender)
        {
            showbombs();
            bt_reset.Style = st_reset4;
            MessageBox.Show("you lost");
            disableAllButtons();
        }
        private void win()
        {
            int counter = 0;
            int bombs = bombCount * difficulty;
            for (int i = 0; i < dxd; i++)
            {
                if (tiles[i].IsEnabled == false)
                {
                    counter++;
                }
                if (dxd - bombs <= counter)
                {
                    bt_reset.Style = st_reset3;
                    dt.Stop();
                    string time = lb_timer.Content.ToString();
                    MessageBox.Show("Congratulations!" + "\n Your time is: " + time);
                    disableAllButtons();
                }
            }
        }
        //flags not working
        private void rightclick(object sender, RoutedEventArgs e)
        {
            int x = Convert.ToInt32(lb_flagCounter.Content);
            if (((Button)sender).Style == st_flag)
            {
                if (((Button)sender).Content == " ")
                {
                    ((Button)sender).Style = st_bt2;
                }
                else
                {
                    ((Button)sender).Style = st_bt1;
                }
                x += 1;
            }
            else
            {
                if (x > 0)
                {
                    ((Button)sender).Style = st_flag;
                    x -= 1;
                }
            }
            lb_flagCounter.Content = x;
        }

        private void showbombs()
        {
            for (int i = 0; i < dxd; i++)
            {
                if (tiles[i].Tag == "boom")
                {
                    tiles[i].Style = st_bomb;
                }
            }
        }
        private void developer_tool(object sender, RoutedEventArgs e)
        {
            //developer tool
            for (int i = 0; i < dxd; i++)
            {

                if (tiles[i].Tag == "boom")
                {
                    tiles[i].Style = st_bomb;
                }
                else
                {
                    tiles[i].Content = tiles[i].Name.Split("_")[1];
                }
            }
        }
        private void exit_button(object sender, RoutedEventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            ((MainWindow)Application.Current.MainWindow).Content = mainMenu;
        }
        private void restart_button(object sender, RoutedEventArgs e)
        {
            Game game = new Game();
            ((MainWindow)Application.Current.MainWindow).Content = game;
        }
        private void checkboard()
        {
            //checkboard
            int counter = 0;
            bool flag = true;
        
            for (int i = 0; i < dxd; i++)
            {
                if (i % difficulty == 0 && i != 0)
                {
                    flag ^= true;
                    if (flag)
                    {
                        counter = 0;
                    }
                    else
                    {
                        counter = 1;
                    }
                }
                if (counter % 2 == 0)
                {
                    tiles[i].Style = st_bt2;
                    tiles[i].Content = " ";
                }
                counter++;
            }
        }
    }
}
