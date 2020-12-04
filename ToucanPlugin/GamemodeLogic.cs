using System;
using ToucanPlugin.Commands;
using ToucanPlugin.Gamemodes;

namespace ToucanPlugin
{
    public enum GamemodeType
    {
        None = 0,
        QuitePlace = 1,
        PeanutInfection = 2,
        AmongUs = 3,
        CandyRush = 4,
        Scp682 = 5,
        LivingNerd = 6,
        SpoopyGhosts = 7,
    }
    public class GamemodeChances
    {
        public int QuietPlace { get; set; }
        public int PeanutInfection { get; set; }
        public int AmongUs { get; set; }
        public int CandyRush { get; set; }
        public int Scp682 { get; set; }
        public int LivingNerd { get; set; }
        public int SpoopyGhosts { get; set; }
    }
    public class GamemodeLogic
    {
        public static bool GamemodesPaused { get; set; } = false;
        public static GamemodeType NextGamemode { get; set; } = 0;
        public static GamemodeType RoundGamemode { get; set; } = 0;
        public void GameSelector()
        {
            if (GamemodeLogic.NextGamemode != 0)
                switch (GamemodeLogic.NextGamemode)
                {
                    case GamemodeType.QuitePlace:
                        new QuietPlace().QuietPlace_();
                        break;
                    case GamemodeType.PeanutInfection:
                        new RealPeanutInfection().RealPeanutInfection_();
                        break;
                    case GamemodeType.AmongUs:
                        new AmongUs().AmongUs_();
                        break;
                    /*case GamemodeType.CandyRush:
                        new CandyRush().Candyrush();
                        break;*/
                    case GamemodeType.Scp682:
                        new Scp682().Scp682_();
                        break;
                    case GamemodeType.LivingNerd:
                        new LivingNerd().LivingNerd_();
                        break;
                    case GamemodeType.SpoopyGhosts:
                        new SpoopyGhosts().SpoopyGhosts_();
                        break;
                }
        }
        public void ClearCache()
        {
            GamemodeLogic.RoundGamemode = 0;
            AmongUs.DeathCords.Clear();
            AmongUs.Imposters.Clear();
            /*CandyRush.BommerList.Clear();
            CandyRush.DefuserList.Clear();*/
            SpoopyGhosts.InvisScpList.Clear();
        }
        public string ConvertToNice(GamemodeType Type)
        {
            switch (Type) {
                default:
                    return "None";
                case GamemodeType.None:
                    return "None";
                case GamemodeType.AmongUs:
                    return "Among Us";
                case GamemodeType.CandyRush:
                    return "Candy Rush";
                case GamemodeType.PeanutInfection:
                    return "Real Peanut Infection";
                case GamemodeType.QuitePlace:
                    return "QuietPlace";
                case GamemodeType.Scp682:
                    return "SCP-682 Breakout";
                case GamemodeType.LivingNerd:
                    return "Living Nerd";
                case GamemodeType.SpoopyGhosts:
                    return "Spoopy Ghosts";
            }
        }
    }
}
