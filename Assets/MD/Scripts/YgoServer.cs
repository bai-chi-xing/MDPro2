using System.Runtime.InteropServices;
using System.Threading;

internal static unsafe class Dll
{
    [DllImport("ygoserver", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern int start_server([MarshalAs(UnmanagedType.LPStr)] string args);
    [DllImport("ygoserver", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
    public static extern void stop_server();
}

public class YgoServer
{
    public static Thread serverThread;
    public static void StartServer(string args)
    {
        if (serverThread != null)
            StopServer();

        serverThread = new Thread(() =>
        {
            Dll.start_server(args);
        });
        serverThread.Start();
    }
    public static void StopServer()
    {
        Dll.stop_server();
        serverThread?.Abort();
    }
}
