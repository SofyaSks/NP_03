using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace chatClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ChatModel chatModel = new();

        private TcpClient server;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = chatModel;
            server = new TcpClient("192.168.1.5", 2024);
            Recieve();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(chatModel.UserName))
            {
                MessageBox.Show("Введите имя пользователя, чтобы общаться: ");
                return;
            }
            if(string.IsNullOrEmpty(chatModel.Message))
            {
                MessageBox.Show("Введите сообщение: ");
                return;
            }

            string concat = $"{chatModel.UserName}: {chatModel.Message}";
            byte[] bytes = Encoding.UTF8.GetBytes(concat);
            await server.GetStream().WriteAsync(bytes);

            chatModel.Message = "";
        }

        private async void Recieve()
        {
            byte[] buffer = new byte[1024];
            while (true)
            {
                int read = await server.GetStream().ReadAsync(buffer);
                if (cancel.Token.IsCancellationRequested)
                    break;
                string message = Encoding.UTF8.GetString(buffer, 0, read);
                chatModel.Chat = chatModel.Chat
                    .Concat(new[] { message })
                    .ToList();
                listBox.SelectedIndex = chatModel.Chat.Count - 1;
            }
        }

        private CancellationTokenSource cancel = new CancellationTokenSource();
        private void Window_Closed(object sender, EventArgs e) => cancel.Cancel();
    }
}
