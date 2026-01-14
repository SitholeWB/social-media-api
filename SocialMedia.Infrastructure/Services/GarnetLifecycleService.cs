using Garnet;

namespace SocialMedia.Infrastructure;

public class GarnetLifecycleService : IHostedService, IDisposable
{
    private GarnetServer? _server;
    public int Port { get; private set; }
    private bool _running = false;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_running)
        {
            return Task.CompletedTask;
        }
        Port = GetFreeTcpPort();
        // Define settings as an array of strings
        string[] args = [
            "--bind", "127.0.0.1",
            "--port", $"{Port}",
            "--checkpointdir", "./GarnetData",
            "--aof",
            "--recover"
        ];

        _server = new GarnetServer(args);
        _server.Start();
        _running = true;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // This is called automatically when the app is shutting down
        _server?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _server?.Dispose();
        GC.SuppressFinalize(this);
    }

    public static int GetFreeTcpPort()
    {
        using var socket = new System.Net.Sockets.Socket(
            System.Net.Sockets.AddressFamily.InterNetwork,
            System.Net.Sockets.SocketType.Stream,
            System.Net.Sockets.ProtocolType.Tcp);

        // Binding to port 0 tells the OS to pick a free one
        socket.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Loopback, 0));
        return ((System.Net.IPEndPoint)socket.LocalEndPoint!).Port;
    }
}