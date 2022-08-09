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
        Button[] buttons;
        Style style;
        Style style2;
        Style bomb;
        Style bt_flag;
        Style smile;

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
            CreateBoard();
        }
        public void CreateBoard()
        {
            style = this.FindResource("BT_Looks") as Style;
            style2 = this.FindResource("BT_Looks2") as Style;
            bomb = this.FindResource("BT_Bomb") as Style;
            bt_flag = this.FindResource("BT_Flag") as Style;
            smile = this.FindResource("BT_Smile") as Style;
            //difficulty
            difficulty = Convert.ToInt32(System.IO.File.ReadAllText(@"C:\Users\grzes\Desktop\all\programs\minesweeper - final edition\minesweeper\TextFile1.txt").Split(" ")[1]);
            dxd = difficulty * difficulty;
            //buttons array
            buttons = new Button[dxd];

            //grid
            Grid board = new Grid();

            //buttons
            for (int i = 0; i < dxd; i++)
            {
                buttons[i] = new Button();
                buttons[i].MouseRightButtonDown += rightclick;
                buttons[i].Click += default_click;
                buttons[i].Name = "BT_0";
                buttons[i].Tag = i;
                buttons[i].Style = style;
            }

            //creating rows and columns
            for (int i = 0; i < difficulty; i++)
            {
                board.RowDefinitions.Add(new RowDefinition());
                board.ColumnDefinitions.Add(new ColumnDefinition());

                //setting buttons
                for (int j = 0; j < difficulty; j++)
                {
                    Grid.SetRow(buttons[j + (i * difficulty)], i);
                    Grid.SetColumn(buttons[j + (i * difficulty)], j);
                    board.Children.Add(buttons[j + (i * difficulty)]);
                }
            }

            checkboard();

            //board alignment
            board.HorizontalAlignment = HorizontalAlignment.Center;
            board.VerticalAlignment = VerticalAlignment.Center;

            //creating a stack panel to add a button
            StackPanel stackPanel = new StackPanel();
            StackPanel stackPanel2 = new StackPanel();
            stackPanel2.Orientation = Orientation.Horizontal;
            stackPanel2.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.Children.Add(stackPanel2);
            stackPanel.Children.Add(board);

            //developer button
            Button devBT = new Button();
            devBT.Height = 60;
            devBT.Width = 60;
            devBT.Content = "dev tool";
            devBT.Click += developer_tool;

            //restart button
            Button restart = new Button();
            restart.Height = 60;
            restart.Width = 60;
            restart.Content = "exit";
            restart.Click += restart_button;
            restart.Style = smile;

            //exit button
            Button exitBT = new Button();
            exitBT.Height = 60;
            exitBT.Width = 60;
            exitBT.Content = "exit";
            exitBT.Click += exit_button;

            //label
            Label timer = new Label();
            timer.Height = 60;
            timer.Width = 60;
            timer.FontSize = 12;
            timer.VerticalContentAlignment = VerticalAlignment.Center;
            timer.HorizontalContentAlignment = HorizontalAlignment.Center;
            timer.Content = "timer";

            //label
            Label timer2 = new Label();
            timer2.Height = 60;
            timer2.Width = 60;
            timer2.FontSize = 12;
            timer2.VerticalContentAlignment = VerticalAlignment.Center;
            timer2.HorizontalContentAlignment = HorizontalAlignment.Center;
            timer2.Content = "10";

            stackPanel2.Children.Add(exitBT);
            stackPanel2.Children.Add(timer2);
            stackPanel2.Children.Add(restart);
            stackPanel2.Children.Add(timer);
            stackPanel2.Children.Add(devBT);

            //applying the stackpanel content
            this.Content = stackPanel;
        }
        private void default_click(object sender, RoutedEventArgs e)
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
                firstclick = false;
            }
        }
        private void floodfill(object sender)
        {
            int k = 0;
            int h = 0;
            for (int i = 0; i < dxd; i++)
            {
                if (buttons[i].Tag == ((Button)sender).Tag)
                {
                    k = i;
                }
            }

            List<Button> list = new List<Button>();
            list.Add(buttons[k]);

            while (list.Count() > 0)
            {
                
                for (int i = 0; i < dxd; i++)
                {
                    if (buttons[i] == list.Last())
                    {
                        h = i;
                    }
                }

                list.Remove(list.Last());

                if (buttons[h].IsEnabled && buttons[h].Tag != "boom" && buttons[h].Name.Split("_")[1] == "0")
                {
                    buttons[h].IsEnabled = false;
                    //right
                    if (h % difficulty < difficulty - 1)
                    {
                        list.Add(buttons[h + 1]);
                    }
                    //left
                    if (h % difficulty > 0)
                    {
                        list.Add(buttons[h - 1]);
                    }
                    //down
                    if (h < dxd - difficulty)
                    {
                        list.Add(buttons[h + difficulty]);
                    }
                    //up
                    if (h > difficulty)
                    {
                        list.Add(buttons[h - difficulty]);
                    }
                }
                else
                {
                    buttons[h].IsEnabled = false;
                    if (buttons[h].Name.Split("_")[1] != "0")
                    {
                        buttons[h].Content = buttons[h].Name.Split("_")[1];
                    }
                }
            }
            firstclick = false;
        }

        /*private void ffExtension(int x)
        {
            Button[] ext = {
            //upper left
            buttons[x - (difficulty + 1)],
            //upper right
            buttons[x - (difficulty - 1)],
            //bottom left
            buttons[x + (difficulty - 1)],
            //bottom right
            buttons[x + (difficulty + 1)]};

            for (int i = 0; i < ext.Length -1; i++)
            {
                ext[i].IsEnabled = false;
                ext[i].Content = ext[i].Name.Split("_")[1];
            }
        }*/
        public void MinesLogic()
        {
            //creating and setting mines

            int mines;

            if (difficulty == 16)
            {
                bombCount = 2;
            }
            else if (difficulty == 24)
            {
                bombCount = 4;
            }

            for (int i = 0; i < bombCount * difficulty; i++)
            {
                Random rnd = new Random();
                mines = rnd.Next(1, dxd);

                if (buttons[mines].Tag == "boom")
                {
                    i--;
                    continue;
                }
                
                int help = 0;

                                        //proximity numeration\\

                //left
                if (mines % difficulty > 0)
                {
                    help = Convert.ToInt32(buttons[mines - 1].Name.Split("_")[1]) + 1;
                    /*help++;*/
                    buttons[mines - 1].Name = Convert.ToString("BT_" + help);
                }
                //right
                if (mines % difficulty < difficulty - 1)
                {
                    help = Convert.ToInt32(buttons[mines + 1].Name.Split("_")[1]) + 1;
                    buttons[mines + 1].Name = Convert.ToString("BT_" + help);
                }
                //down
                if (mines < ((dxd) - difficulty))
                {
                    help = Convert.ToInt32(buttons[mines + difficulty].Name.Split("_")[1]) + 1;
                    buttons[mines + difficulty].Name = Convert.ToString("BT_" + help);
                }
                //up
                if (mines >= difficulty)
                {
                    help = Convert.ToInt32(buttons[mines - difficulty].Name.Split("_")[1]) + 1;
                    buttons[mines - difficulty].Name = Convert.ToString("BT_" + help);
                }
                //bottom left
                if (mines < dxd - difficulty && mines % difficulty != 0)
                {
                    help = Convert.ToInt32(buttons[mines + (difficulty - 1)].Name.Split("_")[1]) + 1;
                    buttons[mines + (difficulty - 1)].Name = Convert.ToString("BT_" + help);
                }
                //upper right
                if (mines >= difficulty && mines % difficulty != difficulty - 1)
                {
                    help = Convert.ToInt32(buttons[mines - (difficulty - 1)].Name.Split("_")[1]) + 1;
                    buttons[mines - (difficulty - 1)].Name = Convert.ToString("BT_" + help);
                }
                //bottom right
                if (mines % difficulty != difficulty - 1 && mines < dxd - difficulty)
                {
                    help = Convert.ToInt32(buttons[mines + (difficulty + 1)].Name.Split("_")[1]) + 1;
                    buttons[mines + (difficulty + 1)].Name = Convert.ToString("BT_" + help);
                }
                //upper left
                if (mines > difficulty && mines % difficulty != 0)
                {
                    help = Convert.ToInt32(buttons[mines - (difficulty + 1)].Name.Split("_")[1]) + 1;
                    buttons[mines - (difficulty + 1)].Name = Convert.ToString("BT_" + help);
                }
                buttons[mines].Tag = "boom";
            }
        }
        private void zero(int x, string n)
        {
            if (n.Split("_")[1] != "0")
            {
                buttons[x].Content = n.Split("_")[1];
            }
        }
        private void disableAllButtons()
        {
            for (int i = 0; i < dxd; i++)
            {
                buttons[i].IsEnabled = false;
            }
        }
        private void bombClick(object sender)
        {
            showbombs();
            MessageBox.Show("you lost");
            disableAllButtons();
        }
        private void win()
        {
            int counter = 0;
            int bombs = bombCount * difficulty;
            for (int i = 0; i < dxd; i++)
            {
                if (buttons[i].IsEnabled == false)
                {
                    counter++;
                }
                if (dxd - bombs <= counter)
                {
                    MessageBox.Show("you won");
                    disableAllButtons();
                }
            }
        }
        //flags not working
        private void rightclick(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Style == bt_flag)
            {
                if (((Button)sender).Content == " ")
                {
                    ((Button)sender).Style = style2;
                }
                else
                {
                    ((Button)sender).Style = style;
                }
            }
            else
            {
                ((Button)sender).Style = bt_flag;
            }
        }

        private void showbombs()
        {
            for (int i = 0; i < dxd; i++)
            {
                if (buttons[i].Tag == "boom")
                {
                    buttons[i].Style = bomb;
                }
            }
        }
        private void developer_tool(object sender, RoutedEventArgs e)
        {
            //developer tool
            for (int i = 0; i < dxd; i++)
            {

                if (buttons[i].Tag == "boom")
                {
                    buttons[i].Style = bomb;
                }
                else
                {
                    buttons[i].Content = buttons[i].Name.Split("_")[1];
                }
                /*if (buttons[i].Name.Split("_")[1] == "0" && buttons[i].Tag != "boom")
                {
                    buttons[i].Content = 0;
                }
                else
                {
                    buttons[i].Content = "";
                }*/
            }
        }
        private void exit_button(object sender, RoutedEventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            ((MainWindow)Application.Current.MainWindow).Content = mainMenu;
        }
        private void restart_button(object sender, RoutedEventArgs e)
        {
            /*int counter = 0;
            bool flag = true;
            for (int i = 0; i < dxd; i++)
            {
                buttons[i].IsEnabled = true;
                buttons[i].Name = "BT_0";
                buttons[i].Tag = i;
                buttons[i].Content = "";
                buttons[i].Style = style;
                firstclick = true;
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
                    buttons[i].Style = style2;
                }
                counter++;
            }
            checkboard();*/
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
                    buttons[i].Style = style2;
                    buttons[i].Content = " ";
                }
                counter++;
            }
        }
    }
}