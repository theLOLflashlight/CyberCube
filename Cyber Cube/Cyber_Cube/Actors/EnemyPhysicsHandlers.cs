using CyberCube.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using CyberCube.Screens;
using CyberCube.Levels;
using CyberCube.Tools;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Collision;
using CyberCube.Physics;
using FarseerPhysics.Common;

namespace CyberCube.Actors
{
    public partial class Enemy
    {
        private bool OnCollision( Fixture fixtureA, Fixture fixtureB, Contact contact )
        {
            if ( !fixtureB.IsSensor && fixtureB.UserData is Flat )
            {
                var diff = MathHelper.WrapAngle( UpDir.Angle - Rotation );
                Rotation = Rotation + diff;
            }
            return contact.IsTouching;
        }
    }
}
