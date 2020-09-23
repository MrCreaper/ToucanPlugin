using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                        new QuietPlace().Quietplace();
                        break;
                }
            }
        }
    }
}
