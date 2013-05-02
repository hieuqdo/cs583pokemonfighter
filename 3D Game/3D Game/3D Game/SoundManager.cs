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
        public SoundEffect jumpSound;
        public Song menuMusic;
        public Song battleMusic;

        // Constructor
        public SoundManager()
        {
            playerShootSound = null;
            jumpSound = null;
            menuMusic = null; 
            battleMusic = null;
        }

        // Load Content
        public void LoadContent(ContentManager Content)
        {
            //playerShootSound = Content.Load<SoundEffect>("playershoot");
            jumpSound = Content.Load<SoundEffect>(@"audio\jump");
            menuMusic = Content.Load<Song>(@"audio\menumusic");
            battleMusic = Content.Load<Song>(@"audio\battlemusic");
            
        }
    }
}
