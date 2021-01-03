using System;
using System.Collections.Generic;
using ToucanPlugin.Gamemodes;

namespace ToucanPlugin
{
    public enum GamemodeType
    {
        None = 0,
        QuitePlace = 1,
        PeanutInfection = 2,
        AmongUs = 3,
        //CandyRush = 4,
        Scp682 = 5,
        LivingNerd = 6,
        SpoopyGhosts = 7,
        ZombieInfection = 8,
    }
    public class GamemodeLogic
    {
        public static bool GamemodesPaused { get; set; } = false;
        public static GamemodeType NextGamemode { get; set; } = GamemodeType.None;
        public static GamemodeType RoundGamemode { get; set; } = GamemodeType.None;
        public void GamemodeStarter()
        {
            if (RoundGamemode != GamemodeType.None)
                switch (RoundGamemode)
                {
                    case GamemodeType.QuitePlace:
                        new QuietPlace().Setup();
                        break;
                    case GamemodeType.PeanutInfection:
                        new RealPeanutInfection().Setup();
                        break;
                    case GamemodeType.AmongUs:
                        new AmongUs().Setup();
                        break;
                    /*case GamemodeType.CandyRush:
                        new CandyRush().Setup();
                        break;*/
                    case GamemodeType.Scp682:
                        new Scp682().Setup();
                        break;
                    case GamemodeType.LivingNerd:
                        new LivingNerd().Setup();
                        break;
                    case GamemodeType.SpoopyGhosts:
                        new SpoopyGhosts().Setup();
                        break;
                    case GamemodeType.ZombieInfection:
                        new ZombieInfection().Setup();
                        break;
                }
        }
        public void ClearCache()
        {
            RoundGamemode = GamemodeType.None;
            AmongUs.DeathCords.Clear();
            AmongUs.Imposters.Clear();
            /*CandyRush.BommerList.Clear();
            CandyRush.DefuserList.Clear();*/
            SpoopyGhosts.InvisScpList.Clear();
        }
        public string ConvertToNice(GamemodeType Type)
        {
            switch (Type)
            {
                default:
                    return "None";
                case GamemodeType.None:
                    return "None";
                case GamemodeType.AmongUs:
                    return "Among Us";
                /*case GamemodeType.CandyRush:
                    return "Candy Rush";*/
                case GamemodeType.PeanutInfection:
                    return "Peanut Infection";
                case GamemodeType.QuitePlace:
                    return "Quiet Place";
                case GamemodeType.Scp682:
                    return "SCP-682 Breakout";
                case GamemodeType.LivingNerd:
                    return "Living Nerd";
                case GamemodeType.SpoopyGhosts:
                    return "Spoopy Ghosts";
                case GamemodeType.ZombieInfection:
                    return "Zombie Infection";
            }
        }
    }
}
