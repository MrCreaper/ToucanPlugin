using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using HarmonyLib;
using System;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using System.Diagnostics;
using System.Linq;
using MEC;
using System.Collections.Generic;

namespace ToucanPlugin
{
    public class ToucanPlugin : Plugin<Config>
    {
        readonly Tcp Tcp = new Tcp();
        private static readonly Lazy<ToucanPlugin> LazyInstance = new Lazy<ToucanPlugin>(() => new ToucanPlugin());
        public static ToucanPlugin Instance => LazyInstance.Value;

        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        private Handlers.Player player;
        private Handlers.Server server;

        private MessageResponder mr;
        private Gamemodes.AmongUs gm0;
        private Gamemodes.ZombieInfection gm1;

        public string VersionStr = $"{ToucanPlugin.Instance.Version.Major}.{ToucanPlugin.Instance.Version.Minor}.{ToucanPlugin.Instance.Version.Revision}";
        private int _patchcounter;

        public Harmony Harmony { get; private set; }

        private ToucanPlugin()
        {
        }
        public override void OnEnabled()
        {
            base.OnEnabled();
            Log.Info($"Hes right, Toucan Plugin (V{VersionStr}) is enabled");
            RegisterEvents();
            Patch();
            Tcp.topicUpdateTimer = Stopwatch.StartNew();
            Tcp.topicUpdateTimer.Start();
            Tcp.Start();
            //ToucanPlugin.Instance.Config.PlayerCountMentions.ForEach(r => server.LastPlayerCountMentions.Add(r.PlayerCount, false));
            AutoUpdates();
        }
        public override void OnDisabled()
        {
            base.OnDisabled();
            UnRegisterEvents();
            Unpatch();
            Tcp.Disconnect();
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

            mr = new MessageResponder();
            gm0 = new Gamemodes.AmongUs();
            gm1 = new Gamemodes.ZombieInfection();

            Tcp.ConnectedEvent += mr.Connected;
            Tcp.RecivedMessageEvent += mr.Respond;

            Server.WaitingForPlayers += server.OnWaitingForPlayers;
            Server.RoundStarted += server.OnRoundStarted;
            Server.RestartingRound += server.OnRestartingRound;
            Server.RoundEnded += server.OnRoundEnded;
            Server.RespawningTeam += server.OnRespawningTeam;
            Server.SendingRemoteAdminCommand += server.OnSendingRemoteAdminCommand;

            Player.PreAuthenticating += player.OnPreAuthenticating;
            Player.Joined += player.OnJoin;
            Player.Left += player.OnLeft;
            Player.Escaping += player.OnEscape;
            Player.Died += player.OnDied;
            Player.Spawning += player.OnSpawned;
            Player.InteractingDoor += player.OnInteractingDoor;
            Player.Banned += player.OnBanned;
            Player.Kicked += player.OnKicked;
            Player.MedicalItemUsed += player.OnMedicalItemUsed;
            Player.ThrowingGrenade += player.OnThrowingGrenade;
            Player.EnteringFemurBreaker += player.OnEnteringFemurBreaker;
            Player.Hurting += player.OnHurting;
            //Player.Shot += player.OnShot;
            Player.ChangingRole += player.OnChangingRole;
            Player.TriggeringTesla += player.OnTriggeringTesla;

            Player.Hurting += gm0.OnHurting;
            Player.ChangingRole += gm1.OnChangingRole;
        }
        public void UnRegisterEvents()
        {
            player = null;
            server = null;

            mr = null;
            gm0 = null;
            gm1 = null;

            Tcp.ConnectedEvent -= mr.Connected;
            Tcp.RecivedMessageEvent -= mr.Respond;

            Server.WaitingForPlayers -= server.OnWaitingForPlayers;
            Server.RoundStarted -= server.OnRoundStarted;
            Server.RestartingRound -= server.OnRestartingRound;
            Server.RoundEnded -= server.OnRoundEnded;
            Server.RespawningTeam -= server.OnRespawningTeam;
            Server.SendingRemoteAdminCommand -= server.OnSendingRemoteAdminCommand;

            Player.PreAuthenticating -= player.OnPreAuthenticating;
            Player.Joined -= player.OnJoin;
            Player.Left -= player.OnLeft;
            Player.Escaping -= player.OnEscape;
            Player.Died -= player.OnDied;
            Player.Spawning -= player.OnSpawned;
            Player.InteractingDoor -= player.OnInteractingDoor;
            Player.Banned -= player.OnBanned;
            Player.Kicked -= player.OnKicked;
            Player.MedicalItemUsed -= player.OnMedicalItemUsed;
            Player.ThrowingGrenade -= player.OnThrowingGrenade;
            Player.EnteringFemurBreaker -= player.OnEnteringFemurBreaker;
            Player.Hurting -= player.OnHurting;
            //Player.Shot -= player.OnShot;
            Player.ChangingRole -= player.OnChangingRole;
            Player.TriggeringTesla -= player.OnTriggeringTesla;

            Player.Hurting -= gm0.OnHurting;
            Player.ChangingRole -= gm1.OnChangingRole;
        }
        private IEnumerator<float> AutoUpdates()
        {
            AutoUpdater.CheckReleases();
            while (true)
            {
                yield return Timing.WaitForSeconds(7200);

                AutoUpdater.RunUpdater(10000);
            }
        }
    }
}