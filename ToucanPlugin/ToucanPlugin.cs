using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using HarmonyLib;
using System;
using Player = Exiled.Events.Handlers.Player;
using Server = Exiled.Events.Handlers.Server;
using System.Threading;
using System.Threading.Tasks;

namespace ToucanPlugin
{
    public class ToucanPlugin : Plugin<Config>
    {
        readonly Tcp Tcp = new Tcp();
        public static ToucanPlugin Singleton;
        public Harmony Instance;

        public override Version Version { get; } = Singleton.Version;
        public override Version RequiredExiledVersion { get; } = new Version(2, 2, 4, 0);

        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        private Handlers.Player player;
        private Handlers.Server server;

        private MessageResponder mr;
        private Gamemodes.AmongUs gm0;
        private Gamemodes.ZombieInfection gm1;

        private int _patchcounter;

        public Harmony Harmony { get; private set; }

        private ToucanPlugin()
        {
        }
        public override void OnEnabled()
        {
            Log.Debug("Mh. Lets see whats going on...", Singleton.Config.Debug);
            RegisterEvents();
            Patch();
            Tcp.Start();
            //ToucanPlugin.Singleton.Config.PlayerCountMentions.ForEach(r => server.LastPlayerCountMentions.Add(r.PlayerCount, false));
            Task.Factory.StartNew(() => AutoUpdate());
            base.OnEnabled();
        }
        public override void OnDisabled()
        {
            UnRegisterEvents();
            Unpatch();
            Tcp.Disconnect("Disabling plugin");
            base.OnDisabled();
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

            Tcp.IdleModeUpdateEvent += OnIdlemodeUpdate;

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

            Tcp.IdleModeUpdateEvent -= OnIdlemodeUpdate;

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
        public void OnIdlemodeUpdate(bool NewState)
        {
            // This is an awful idea, idle mode is never gonna end if debugs on..
            //Log.Debug($"New idlemode state: {NewState}", ToucanPlugin.Singleton.Config.Debug);
            if (!NewState && !Tcp.connecting)
                Tcp.Start();
        }
        private async void AutoUpdate()
        {
            /*Release[] Releases = AutoUpdater.GetReleases(AutoUpdater.REPOID).Result;
            string list = $"{Singleton.Name} releases:";
            Releases.ToList().ForEach(r => list += $"\n{r.TagName}");
            Log.Debug(list);*/
            if (Singleton.Config.Debug)
                if (AutoUpdater.UpToDate().Result)
                    Log.Debug("No updates");
                else Log.Debug("Update found");
            while (true)
            {
                Log.Debug("Checking for updates... again...", Singleton.Config.Debug);
                if (!AutoUpdater.UpToDate().Result)
                {
                    Log.Info("Update found");
                    await AutoUpdater.Update();
                }
                Thread.Sleep(3600000); //1h
            }
        }
    }
}