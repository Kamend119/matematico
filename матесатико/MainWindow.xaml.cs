using System.Windows;

namespace матесатико
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.Show();
            this.Close(); // Закрываем текущее окно после открытия игрового окна
        }
    }
}
