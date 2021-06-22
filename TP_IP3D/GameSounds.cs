using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace TP_IP3D
{
    public static class GameSounds
    {
        static SoundEffect cannonBallThrown;
        static SoundEffect cannonBallHitTank;
        
        public static void CannonBallThrown() { cannonBallThrown.Play(); }
        public static void CannonBallHitTank() { cannonBallHitTank.Play(); }

        public static void LoadAudio(Game1 game)
        {
            cannonBallThrown = game.Content.Load<SoundEffect>("cannonBallThrown");
            cannonBallHitTank = game.Content.Load<SoundEffect>("cannonBallHitTank");
        }
    }
}

