﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Created by Team Rocket: Hieu Do, Jason Santos, Ramzi Bandak, Saif Sabar, Steven Tran
// San Diego State University
// CS 583 - 3D Game Development | Professor Stewart

namespace _3D_Game
{
    public class Bullet : BasicModel
    {
        Matrix rotation = Matrix.Identity;
        Matrix translation = Matrix.Identity;
        Vector3 direction;
        Player1 myPlayer;
        float mySpeed;
        InteractionMediator.attackType myType;

        public Bullet(Model m)
            : base(m)
        {
        }

        public Bullet(Model m, Vector3 position, Vector3 Direction, 
                    Player1 owner, InteractionMediator.attackType type)
            : base(m)
        {
            world = Matrix.CreateTranslation(position);      
            myPlayer = owner;
            myType = type;
            if (type == InteractionMediator.attackType.BULLET)
            {
                tint = Color.Blue;
                mySpeed = .9f;
            }
            else if (type == InteractionMediator.attackType.SMASHBULLET)
            {
                tint = Color.Red;
                mySpeed = .5f;
            }
            direction = Direction * mySpeed;
        }

        public override void Update()
        {
            //Move model
            translation *= Matrix.CreateTranslation(direction);
        }

        public override Matrix GetWorld()
        {
            return world * translation;
        }

        public Player1 getOwner()
        {
            return myPlayer;
        }

        public InteractionMediator.attackType getType()
        {
            return myType;
        }

        public float getSpeed()
        {
            return mySpeed;
        }
    }
}
