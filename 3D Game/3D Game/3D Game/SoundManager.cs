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
        public SoundEffect attackSound;
        public SoundEffect jumpSound;
        public SoundEffect doubleJumpSound;
        public SoundEffect shieldSound;
        public SoundEffect rollSound;
        public SoundEffect selectSound;
        public Song menuMusic;
        public Song battleMusic;

        // Constructor
        public SoundManager()
        {
            attackSound = null;
            jumpSound = null;
            doubleJumpSound = null;
            menuMusic = null; 
            battleMusic = null;
        }

        // Load Content
        public void LoadContent(ContentManager Content)
        {
            //attackSound = Content.Load<SoundEffect>("attack");
            jumpSound = Content.Load<SoundEffect>(@"audio\jump");
            doubleJumpSound = Content.Load<SoundEffect>(@"audio\doublejump");
            //shieldSound = Content.Load<SoundEffect>(@"audio\shield");
            //rollSound = Content.Load<SoundEffect>(@"audio\roll");
            selectSound = Content.Load <SoundEffect>(@"audio\select");
            menuMusic = Content.Load<Song>(@"audio\menumusic");
            battleMusic = Content.Load<Song>(@"audio\battlemusic");
            
        }
    }
}
