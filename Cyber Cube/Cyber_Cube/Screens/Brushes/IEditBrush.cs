using CyberCube.Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyberCube.Screens
{
    public interface IEditBrush
    {
        bool Started { get; }
        void Start( EditableCube.Face face, Vector2 mousePos, GameTime gameTime );
        void End( EditableCube.Face face, Vector2? mousePos, GameTime gameTime );
        void Cancel();
        void Update( EditableCube.Face face, Vector2 mousePos, GameTime gameTime );
        void Draw( EditableCube.Face face, SpriteBatch spriteBatch, GameTime gameTime );
    }
}
