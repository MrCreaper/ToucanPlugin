using ToucanPlugin.Commands;
using ToucanPlugin.Gamemodes;

namespace ToucanPlugin
{
    public class GamemodeSelector
    {
        public void GameSelector()
        {
            if (AcGame.NextGamemode != 0)
            {
                switch (AcGame.NextGamemode)
                {
                    case 1:
                        new QuietPlace().QuietPlacee();
                        break;
                    case 2:
                        new RealPeanutInfection().RealPeanutInfectione();
                        break;
                }
            }
        }
    }
}
