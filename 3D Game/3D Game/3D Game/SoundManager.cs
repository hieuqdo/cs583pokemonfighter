﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

// Created by Team Rocket: Hieu Do, Jason Santos, Ramzi Bandak, Saif Sabar, Steven Tran
// San Diego State University
// CS 583 - 3D Game Development | Professor Stewart

namespace _3D_Game
{
    public class SoundManager
    {
        //Attacks
        public SoundEffect attackSound;
        public SoundEffect smashAttackSound;
        public SoundEffect smashHitSound;
        public SoundEffect shockSound;
        public SoundEffect shockHitSound;
        public SoundEffect shockSmashHitSound;
        public SoundEffect bulletSound;
        public SoundEffect smashBulletSound;

        //Other
        public SoundEffect jumpSound, doubleJumpSound;
        public SoundEffect shieldSound, shieldBreakSound, pikaBreakSound;
        public SoundEffect rollSound;
        public SoundEffect selectSound, backSelectSound;
        public SoundEffect respawnSound;
        public SoundEffect deathCrySound, deathExplosionSound;
        public Song menuMusic, battleMusic, themeMusic, danceMusic;

        public float volume = .2f;

        // Constructor
        public SoundManager()
        {
            SoundEffect.MasterVolume = volume;
        }

        // Load Content
        public void LoadContent(ContentManager Content)
        {
            //Attacks
            attackSound = Content.Load<SoundEffect>(@"audio\attack");
            smashAttackSound = Content.Load<SoundEffect>(@"audio\smashattack");
            smashHitSound = Content.Load<SoundEffect>(@"audio\smashhit");
            shockSound = Content.Load<SoundEffect>(@"audio\shock");
            shockHitSound = Content.Load<SoundEffect>(@"audio\shockhit");
            shockSmashHitSound = Content.Load<SoundEffect>(@"audio\shocksmashhit");
            bulletSound = Content.Load<SoundEffect>(@"audio\bulletplus");
            smashBulletSound = Content.Load<SoundEffect>(@"audio\smashbullet");

            //Other
            jumpSound = Content.Load<SoundEffect>(@"audio\jump");
            doubleJumpSound = Content.Load<SoundEffect>(@"audio\doublejump");
            shieldSound = Content.Load<SoundEffect>(@"audio\shield");
            shieldBreakSound = Content.Load<SoundEffect>(@"audio\shieldbreak");
            pikaBreakSound = Content.Load<SoundEffect>(@"audio\pikabreak");
            selectSound = Content.Load <SoundEffect>(@"audio\select");
            backSelectSound = Content.Load<SoundEffect>(@"audio\menuback");
            respawnSound = Content.Load<SoundEffect>(@"audio\ballopen");
            deathCrySound = Content.Load<SoundEffect>(@"audio\deathCry");
            deathExplosionSound = Content.Load<SoundEffect>(@"audio\homerun");
            menuMusic = Content.Load<Song>(@"audio\menumusic");
            battleMusic = Content.Load<Song>(@"audio\battleMusic");
            themeMusic = Content.Load<Song>(@"audio\theme");
            danceMusic = Content.Load<Song>(@"audio\Gangnam Style");
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
