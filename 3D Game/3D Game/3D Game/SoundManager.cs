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
        public SoundEffect smashAttackSound;
        public SoundEffect smashHitSound;
        public SoundEffect shockSound;
        public SoundEffect jumpSound;
        public SoundEffect doubleJumpSound;
        public SoundEffect shieldSound;
        public SoundEffect rollSound;
        public SoundEffect selectSound;
        public SoundEffect backSelectSound;
        public SoundEffect respawnSound;
        public SoundEffect deathCrySound;
        public SoundEffect deathExplosionSound;
        public Song menuMusic;
        public Song battleMusic;
        public Song themeMusic;

        public float volume = .6f;

        // Constructor
        public SoundManager()
        {
            SoundEffect.MasterVolume = volume;
        }

        // Load Content
        public void LoadContent(ContentManager Content)
        {
            attackSound = Content.Load<SoundEffect>(@"audio\attack");
            smashAttackSound = Content.Load<SoundEffect>(@"audio\smashattack");
            smashHitSound = Content.Load<SoundEffect>(@"audio\smashhit");
            shockSound = Content.Load<SoundEffect>(@"audio\shock");
            jumpSound = Content.Load<SoundEffect>(@"audio\jump");
            doubleJumpSound = Content.Load<SoundEffect>(@"audio\doublejump");
            shieldSound = Content.Load<SoundEffect>(@"audio\shield");
            //rollSound = Content.Load<SoundEffect>(@"audio\roll");
            selectSound = Content.Load <SoundEffect>(@"audio\select");
            backSelectSound = Content.Load<SoundEffect>(@"audio\menuback");
            respawnSound = Content.Load<SoundEffect>(@"audio\ballopen");
            deathCrySound = Content.Load<SoundEffect>(@"audio\death");
            deathExplosionSound = Content.Load<SoundEffect>(@"audio\homerun");
            menuMusic = Content.Load<Song>(@"audio\menumusic");
            battleMusic = Content.Load<Song>(@"audio\battlemusic");
            themeMusic = Content.Load<Song>(@"audio\theme");
        }

        public void increaseVolume()
        {
            volume += .2f;
            if (volume > 1)
                volume = 1;
            SoundEffect.MasterVolume = volume;
        }

        public void decreaseVolume()
        {
            volume -= .2f;
            if (volume < 0)
                volume = 0;
            SoundEffect.MasterVolume = volume;
        }
    }
}
