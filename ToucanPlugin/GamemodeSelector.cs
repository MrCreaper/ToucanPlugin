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
    }
    public class GamemodeChances
    {
        public int QuietPlace { get; set; }
        public int PeanutInfection { get; set; }
        public int AmongUs { get; set; }
        public int CandyRush { get; set; }
    }
    public class GamemodeSelector
    {
        public void GameSelector()
        {
            if (AcGame.NextGamemode != 0)
                switch (AcGame.NextGamemode)
                {
                    case GamemodeType.QuitePlace:
                        new QuietPlace().QuietPlacee();
                        break;
                    case GamemodeType.PeanutInfection:
                        new RealPeanutInfection().RealPeanutInfectione();
                        break;
                    case GamemodeType.AmongUs:
                        new AmongUs().Amongus();
                        break;
                    /*case GamemodeType.CandyRush:
                        new CandyRush().Candyrush();
                        break;*/
                }
        }
    }
    public class GamemodeStuff
    {
        public void ClearCache()
        {
            AcGame.RoundGamemode = 0;
            AmongUs.DeathCords.Clear();
            AmongUs.Imposters.Clear();
            /*CandyRush.BommerList.Clear();
            CandyRush.DefuserList.Clear();*/
        }
        public string ConvertToNice(GamemodeType Type)
        {
            switch (Type) {
                default:
                    return null;
                case GamemodeType.None:
                    return null;
                case GamemodeType.AmongUs:
                    return "Among Us";
                case GamemodeType.CandyRush:
                    return "Candy Rush";
                case GamemodeType.PeanutInfection:
                    return "Real Peanut Infection Hours";
                case GamemodeType.QuitePlace:
                    return "QuietPlace";
            }
        }
    }
}
