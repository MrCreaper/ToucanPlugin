using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ToucanPlugin
{
    public delegate void TcpConDel();
    public delegate void TcpMsgDel(string msg);

    public delegate void IdleModeUpdate(bool newState);
    public class Tcp
    {
        public static event TcpConDel ConnectedEvent;
        public static event TcpMsgDel RecivedMessageEvent;

        public static event IdleModeUpdate IdleModeUpdateEvent;

        readonly int conPort = 173;
        private static Socket S { get; set; } = null;
        readonly static List<String> messageQueue = new List<string>();
        private static Stopwatch topicUpdateTimer;
        private static Stopwatch chillTimer = new Stopwatch();
        public static bool auth = false;
        private static bool connected = false;
        public static bool connecting = false;
        private static bool STOP = false;
        public int MaxMessageLenght = 3000;
        const int AuthTimeout = 10000;
        const int ChillTimeout = 30000;
        static int DissconnectCounter = 0;
        private void Main()
        {
            if (connecting || IsConnected() || auth || connected) return;
            connecting = true;
            try
            {
                // Define those variables to be evaluated in the next for loop and
                // then used to connect to the server. These variables are defined
                // outside the for loop to make them accessible there after.
                IPEndPoint hostEndPoint;
                IPAddress hostAddress = null;

                // Get DNS host information.
                if (ToucanPlugin.Instance.Config.ToucanServerIP == "") { Log.Warn($"No Toucan Server ip set!"); return; };
                IPHostEntry hostInfo = Dns.GetHostEntry(ToucanPlugin.Instance.Config.ToucanServerIP);
                // Get the DNS IP addresses associated with the host.
                IPAddress[] IPaddresses = hostInfo.AddressList;

                // Evaluate the socket and receiving host IPAddress and IPEndPoint.
                for (int index = 0; index < IPaddresses.Length; index++)
                {
                    hostAddress = IPaddresses[index];
                    hostEndPoint = new IPEndPoint(hostAddress, conPort);

                    // Creates the Socket to Send data over a Tcp connection.
                    S = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Connect to the host using its IPEndPoint.
                    S.Connect(hostEndPoint);

                    if (!IsConnected())
                    {
                        connecting = false;
                        // Connection failed, try next IPaddress.
                        Log.Error("Connection Failed");
                        S = null;
                        auth = false;
                        continue;
                        //return;
                    }
                    else
                    {
                        connected = true;
                        Task.Factory.StartNew(() =>
                        {
                            Thread.Sleep(10000);
                            connecting = false;
                        });
                        if (!auth)
                        {
                            Log.Debug("Authenticating...", ToucanPlugin.Instance.Config.Debug);
                            Stopwatch authTimer = new Stopwatch();
                            authTimer.Start();
                            Task.Factory.StartNew(() =>
                            {
                                while (true)
                                {
                                    if (authTimer.ElapsedMilliseconds > AuthTimeout && !auth && !IsConnected())
                                    {
                                        DissconnectCounter++;
                                        Log.Debug($"Authenication Timed out [{DissconnectCounter}]. FUCK", ToucanPlugin.Instance.Config.Debug);
                                        if (DissconnectCounter >= 5)
                                        {
                                            chillTimer.Start();
                                            Log.Debug("Starting chill", ToucanPlugin.Instance.Config.Debug);
                                            Disconnect("Chill man");
                                        }
                                        if (DissconnectCounter >= 10)
                                        {
                                            STOP = true;
                                            Log.Error("Shit has gone really wrong.");
                                        }
                                        return;
                                    }
                                }
                            });
                        }
                        while (S.Connected)
                        {
                            try
                            {
                                byte[] bytes = new byte[MaxMessageLenght];
                                int i = S.Receive(bytes);
                                string msg = Encoding.UTF8.GetString(bytes);
                                if (!auth && msg.StartsWith("auth")) // Uncomment when you figure out "On Disscounnected"
                                {
                                    auth = true;
                                    connected = true;
                                    Log.Debug("Authanticated", ToucanPlugin.Instance.Config.Debug);
                                    ConnectedEvent();
                                    Log.Info("Connected to Toucan Server");
                                }
                                else RecivedMessageEvent(msg);
                                Thread.Sleep(30000);
                            }
                            catch (Exception e)
                            {
                                Log.Error($"Message Responder Failed/Reciving messages Failed, {e}");
                            }
                        }
                    }
                }
            }
            catch
            {
                Log.Error("Connecting failed.");
                connecting = false;
            }
        }
        public void Disconnect(string Reason = "")
        {
            auth = false;
            connected = false;
            if (IsConnected())
                S.Disconnect(false);
            Log.Debug($"Disconnected, {Reason}", ToucanPlugin.Instance.Config.Debug);
        }
        public static bool IsConnected()
        {
            if (S == null)
                return false;

            try
            {
                return !((S.Poll(1000, SelectMode.SelectRead) && (S.Available == 0)) || !S.Connected);
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
        private bool IsPortAvailable(int port)
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
                if (tcpi.LocalEndPoint.Port == port)
                    return false;
            return true;
        }
        private bool SendShit(string data)
        {
            if (data == null) return false;
            try
            {
                if (S.Connected || S != null)
                {
                    // Process the data sent by the client.
                    byte[] niceData = Encoding.ASCII.GetBytes(data);
                    S.Send(niceData);
                    Log.Debug($"Sent: {data}", ToucanPlugin.Instance.Config.Debug);
                    return true;
                }
                else
                {
                    Log.Debug($"Not Connected?", ToucanPlugin.Instance.Config.Debug);
                    return false;
                }
            }
            catch (Exception e)
            {
                Log.Debug($"Failed to send data: {e}", ToucanPlugin.Instance.Config.Debug);
                return false;
            }
        }
        public void Send(string data) =>
            messageQueue.Add(data);
        public void SendLog(string log) =>
            Send($"log [{DateTime.Now}] {log}");

        private void SendQueue()
        {
            if (!IsConnected() || !auth) return;
            if (topicUpdateTimer.ElapsedMilliseconds >= 10000)
            {
                topicUpdateTimer.Reset();
                topicUpdateTimer.Start();
            }
            for (int i = 0; i < messageQueue.Count; i++)
            {
                if (SendShit(messageQueue[i]))
                {
                    messageQueue.RemoveAt(i);
                    i--;
                    Thread.Sleep(100);
                }
            }
            if (messageQueue.Count != 0)
                Log.Error("Could not send all messages.");
        }
        string lastDebug = "";
        public void Start()
        {
            Log.Debug("Starting Tcp...");
            topicUpdateTimer.Restart();
            Task.Factory.StartNew(() =>
            {
                IdleModeUpdateEvent(IdleMode.IdleModeActive);
                Thread.Sleep(7000);
                while (!IdleMode.IdleModeActive)
                {
                    try
                    {
                        if (ToucanPlugin.Instance.Config.Debug)
                        {
                            string fuckingDebugMsg = $"\nauth: {auth}\nconnected: {IsConnected()}\nconnected 2: {connected}\nconnecting: {connecting}\ndissconnect count: {DissconnectCounter}\nchilling: {chillTimer.IsRunning} [ {chillTimer.ElapsedMilliseconds} ]\nSTOP: {STOP}";
                            if (lastDebug != fuckingDebugMsg)
                            {
                                lastDebug = fuckingDebugMsg;
                                Log.Debug(fuckingDebugMsg);
                            }
                        }
                        if (IsConnected() && auth)
                            SendQueue();
                        else if (!IsConnected() && !chillTimer.IsRunning && !STOP)
                            Task.Factory.StartNew(() => Main());

                        if (chillTimer.ElapsedMilliseconds > ChillTimeout)
                            chillTimer.Reset();
                        Thread.Sleep(7000);
                    }
                    catch (Exception e)
                    {
                        Log.Error($"Could not connect: {e}");
                    }
                }
                // If i fucking uncomment this it all fucking breaks
                //Log.Debug($"Idle mode updated", ToucanPlugin.Instance.Config.Debug);
                IdleModeUpdateEvent(IdleMode.IdleModeActive);
            });
        }
    }
}