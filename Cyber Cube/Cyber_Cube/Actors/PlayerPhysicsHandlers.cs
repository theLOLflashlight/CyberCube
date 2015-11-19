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
    public partial class Player
    {

        private bool Torso_OnCollision( Fixture fixtureA, Fixture fixtureB, Contact contact )
        {
            if ( contact.IsTouching && !fixtureB.IsSensor
                 && contact.Manifold.Type == ManifoldType.FaceA
                 && contact.Manifold.PointCount >= 1 )
            {
                Vector2 groundNormal = contact.Manifold.LocalNormal.Rotate( fixtureB.Body.Rotation );
                float groundNormalAngle = (float) Math.Atan2( groundNormal.Y, groundNormal.X ) + MathHelper.PiOver2;

                if ( MathTools.AnglesWithinRange( Rotation, groundNormalAngle, MathHelper.PiOver4 ) )
                    Rotation = groundNormalAngle;

                return true;
            }
            return contact.IsTouching;
        }

        private bool Torso_OnDoorCollision( Fixture fixtureA, Fixture fixtureB, Contact contact )
        {
            if ( fixtureB.UserData is Door
                && MathTools.AnglesWithinRange( Rotation, fixtureB.Body.Rotation, 0.01f ) )
            {
                this.Screen.NextLevel( ((Door) fixtureB.UserData).UserData );
            }

            return contact.IsTouching;
        }

        private bool Torso_OnHazardCollision( Fixture fixtureA, Fixture fixtureB, Contact contact )
        {
            if ( fixtureB.UserData is Hazard )
                KillPlayer();

            return contact.IsTouching;
        }

        private int mNumFootContacts;

        public override bool FreeFall
        {
            get {
                return mNumFootContacts == 0;
            }
        }

        private bool BeginFeetContact( Contact contact )
        {
            if ( contact.FixtureA == mFeet || contact.FixtureB == mFeet )
                ++mNumFootContacts;
            return true;
        }

        private void EndFeetContact( Contact contact )
        {
            if ( contact.FixtureA == mFeet || contact.FixtureB == mFeet )
                --mNumFootContacts;
        }

        private void Feet_OnSeparation( Fixture fixtureA, Fixture fixtureB )
        {
            if ( fixtureB.Body.BodyType != BodyType.Dynamic
                 && fixtureB.UserData is Convex
                 && !IsJumping )
            {
                ApplyRelativeLinearImpulse( Vector2.UnitY * Velocity.Length() * 2 );
            }
        }

        private bool Feet_OnCollision( Fixture fixtureA, Fixture fixtureB, Contact contact )
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
