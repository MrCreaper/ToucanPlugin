using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using HarmonyLib;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace ToucanPlugin
{
    public class Tcp
    {
        public void Start()
        {
            while (true)
                if (IsConnected())
                    SendQueue();
                else
                    Main();
        }
        public static Socket S { get; set; } = null;
        static List<String> messageQueue = new List<string>();
        static public Stopwatch topicUpdateTimer;
        //Removed Static
        public void Main()
        {
            if (IsConnected()) return;
            topicUpdateTimer = Stopwatch.StartNew();
            topicUpdateTimer.Start();
            try
            {
                // Define those variables to be evaluated in the next for loop and
                // then used to connect to the server. These variables are defined
                // outside the for loop to make them accessible there after.
                IPEndPoint hostEndPoint;
                IPAddress hostAddress = null;
                int conPort = 173; //Get it?

                // Get DNS host information.
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
                        // Connection failed, try next IPaddress.
                        Log.Error("Connection Failed");
                        S = null;
                        continue;
                        //return;
                    }
                    else
                    {
                        Log.Info("Connected To Toucan Server.");
                        while (S.Connected)
                        //while (S != null)
                        {
                            try
                            {
                                if (!S.Connected) return;
                                byte[] bytes = new byte[2000]; //256
                                if (bytes == null) return;
                                int i = S.Receive(bytes);
                                MessageResponder mr = new MessageResponder();
                                mr.Respond(Encoding.UTF8.GetString(bytes));
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
                Log.Error("Connecting failed");
            }
        }
        public static bool IsConnected()
        {
            if (S == null)
            {
                return false;
            }

            try
            {
                return !((S.Poll(1000, SelectMode.SelectRead) && (S.Available == 0)) || !S.Connected);
            }
            catch (ObjectDisposedException e)
            {
                Log.Warn("TCP client was unexpectedly closed.");
                Log.Debug(e.ToString());
                return false;
            }
        }
        private void SendShit(String data)
        {
            if (data == null) return;
            try
            {
                if (S.Connected || S != null)
                {
                    // Process the data sent by the client.
                    byte[] niceData = Encoding.ASCII.GetBytes(data);
                    S.Send(niceData);
                    Log.Debug($"Sent: {data}");
                }
                else
                {
                    Log.Debug($"Not Connected?");
                }
            }
            catch (Exception e)
            {
                Log.Debug($"Failed to send data: {e}");
            }
        }
        public void Send(String data)
        {
            Log.Info($"I should send: {data}");
            messageQueue.Add(data);
        }
        public void SendQueue()
        {
            Log.Info("Sending data...");
            if (topicUpdateTimer.ElapsedMilliseconds >= 10000)
            {
                topicUpdateTimer.Reset();
                topicUpdateTimer.Start();
            }
            String EpicData = null;
            for (int i = 0; i < messageQueue.Count; i++)
                if (messageQueue[i + 1] != null)
                    EpicData += $"{messageQueue[i]} ||| ";
                else
                    EpicData += messageQueue[i];
            SendShit(EpicData);
            Log.Info("Data sent!");
        }
    }

    public class ToucanPlugin : Plugin<Config>
    {
        readonly Tcp Tcp = new Tcp();
        private static readonly Lazy<ToucanPlugin> LazyInstance = new Lazy<ToucanPlugin>(() => new ToucanPlugin());
        public static ToucanPlugin Instance => LazyInstance.Value;

        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        private Handlers.Player player;
        private Handlers.Server server;

        private int _patchcounter;

        public Harmony Harmony { get; private set; }

        private ToucanPlugin()
        {
        }
        public override void OnEnabled()
        {
            base.OnEnabled();
            Log.Info($"Enabling Toucan Plugin V{Instance.Version.Major}.{Instance.Version.Minor}.{Instance.Version.Revision}");
            RegisterEvents();
            Patch();

            Task.Factory.StartNew(() => Tcp.Start());
        }
        public override void OnDisabled()
        {
            base.OnDisabled();
            UnRegisterEvents();
            Unpatch();
        }

        private void Patch()
        {
            try
            {
                Harmony = new Harmony($"ToucanPlugin.{++_patchcounter}");

                var lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;

                Harmony.PatchAll();

                Harmony.DEBUG = lastDebugStatus;

                Log.Debug($"patches applied succesfully", Loader.ShouldDebugBeShown);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        private void Unpatch()
        {
            Harmony.UnpatchAll();

            Log.Debug($"Patches have been undone!", Loader.ShouldDebugBeShown);
        }

        public void RegisterEvents()
        {
            player = new Handlers.Player();
            server = new Handlers.Server();

            Server.WaitingForPlayers += server.OnWaitingForPlayers;
            Server.RoundStarted += server.OnRoundStarted;
            Server.RestartingRound += server.OnRestartingRound;
            Server.RoundEnded += server.OnRoundEnded;
            Server.RespawningTeam += server.OnRespawningTeam;

            Player.Left += player.OnLeft;
            Player.Joined += player.OnJoin;
            Player.Escaping += player.OnEscape;
            Player.Died += player.OnDead;
            //Player.Spawning += player.OnSpawned;
            Player.Banned += player.OnBanned;
            Player.Kicked += player.OnKicked;
            Player.MedicalItemUsed += player.OnMedicalItemUsed;
            Player.ThrowingGrenade += player.OnThrowingGrenade;
        }

        public void UnRegisterEvents()
        {
            Server.WaitingForPlayers += server.OnWaitingForPlayers;
            Server.RoundStarted += server.OnRoundStarted;
            Server.RestartingRound += server.OnRestartingRound;
            Server.RoundEnded += server.OnRoundEnded;
            Server.RespawningTeam += server.OnRespawningTeam;

            Player.Left += player.OnLeft;
            Player.Joined += player.OnJoin;
            Player.Escaping += player.OnEscape;
            Player.Died += player.OnDead;
            //Player.Spawning += player.OnSpawned;
            Player.Banned += player.OnBanned;
            Player.Kicked += player.OnKicked;
            Player.MedicalItemUsed += player.OnMedicalItemUsed;
            Player.ThrowingGrenade += player.OnThrowingGrenade;
            player = null;
            server = null;
        }
    }
}