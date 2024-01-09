using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    internal class Program
    {
        static async Task Main() => await new Program().Run();

        private IList<TcpClient> clients = new List<TcpClient>();  // всегда lock (clients)

        private async Task Run()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("192.168.1.5"), 2024);
            listener.Start();

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                lock (clients)
                    clients.Add(client);
                ListenToClient(client);
            }
        }

        private async void ListenToClient(TcpClient from)
        {
            byte[] buffer = new byte[1024];

            while (true)
            {
                int read;
                try
                {
                    // получить один сегмент
                    read = await from.GetStream().ReadAsync(buffer, 0, buffer.Length);
                }
                catch (Exception)
                {
                    break;
                }

                // делаем копию коллекции клиентов
                IReadOnlyList<TcpClient> copy;
                lock (clients)
                    copy = clients.ToList();

                // ретранслируем всем клиентам
                foreach (TcpClient to in copy)
                    await to.GetStream().WriteAsync(buffer, 0, read);
            }

            from.Dispose();
            lock (clients)
                clients.Remove(from);
        }
    }
}