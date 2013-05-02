using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace _3D_Game
{
    public class SoundManager
    {
        public SoundEffect playerShootSound;
        public SoundEffect explodeSound;
        public Song menuMusic;
        public Song bgMusic;

        // Constructor
        public SoundManager()
        {
            playerShootSound = null;
            explodeSound = null;
            menuMusic = null; 
            bgMusic = null;
        }

        // Load Content
        public void LoadContent(ContentManager Content)
        {
            //playerShootSound = Content.Load<SoundEffect>("playershoot");
            //explodeSound = Content.Load<SoundEffect>("explode");
            menuMusic = Content.Load<Song>(@"audio\menumusic");
            //bgMusic = Content.Load<Song>("theme");
            
        }
    }
}
