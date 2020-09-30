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
    }
    public class GamemodeSelector
    {
        public void GameSelector()
        {
            if (AcGame.NextGamemode != 0)
            {
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
                }
            }
        }
    }
}
