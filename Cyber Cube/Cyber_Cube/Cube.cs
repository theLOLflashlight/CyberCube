using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using CyberCube.Physics;
using FarseerPhysics.Dynamics;
using CyberCube.Screens;
using FarseerPhysics.Common;
using CyberCube.Levels;
using System.IO;
using System.Xml.Serialization;
using CyberCube.Tools;
using CyberCube.Actors;

namespace CyberCube
{
    /// <summary>
    /// Represents an entire level. Contains the 6 faces of the cube.
    /// </summary>
    public abstract partial class Cube : DrawableCubeScreenGameComponent
    {
        internal static void LoadContent( ContentManager content )
        {
            sBG = content.Load<Texture2D>( @"Textures\Background" );
            //sSkybox = content.Load<TextureCube>( @"Textures\cubeFaceBackground" );
            Face.LoadContent( content );

            sSkySphereEffect = content.Load<Effect>( "SkySphere" );
            TextureCube SkyboxTexture = content.Load<TextureCube>( "gridSkyBox1024" );
            sSkySphere = content.Load<Model>( "SphereHighPoly" );

            // Set the parameters of the effect
            sSkySphereEffect.Parameters[ "SkyboxTexture" ].SetValue(
                SkyboxTexture );
            // Set the Skysphere Effect to each part of the Skysphere model
            foreach ( ModelMesh mesh in sSkySphere.Meshes )
            {
                foreach ( ModelMeshPart part in mesh.MeshParts )
                {
                    part.Effect = sSkySphereEffect;
                }
            }
        }

        private static Effect sSkySphereEffect;
        private static Model sSkySphere;

        private static Texture2D sBG;
        //private static TextureCube sSkybox;

        public readonly BoundingBox BoundingBox = new BoundingBox( -Vector3.One, Vector3.One );

        protected Face mFrontFace;
        protected Face mBackFace;
        protected Face mTopFace;
        protected Face mBottomFace;
        protected Face mLeftFace;
        protected Face mRightFace;

        public string Name
        {
            get;
            private set;
        }

        public CubeEffect Effect
        {
            get; protected set;
        }

        public Vector3 Position
        {
            get; set;
        } = Vector3.Zero;
        public Vector3 Rotation
        {
            get; set;
        } = Vector3.Zero;
        public Vector3 Scale
        {
            get; set;
        } = Vector3.One;

        public CubePosition StartPosition = new CubePosition( Vector3.UnitZ, 0 );

        public List<CubePosition> EnemyPositions = new List<CubePosition>();

        public string NextLevel
        {
            get; set;
        }

        public float CameraDistance
        {
            get; protected set;
        }

        public Face CurrentFace
        {
            get; protected set;
        }

        public Direction UpDir
        {
            get; protected set;
        }

        public Vector3 ComputeUpVector()
        {
            return VectorUtils.RoundVector(
                       CurrentFace.UpVec.Rotate(
                           CurrentFace.Normal,
                           -UpDir.Angle ) );
        }

        /*
        Picture the unfolded cube:
            T
            F
          L X R
            B
        Where:
            T = top
            F = front
            L = left
            X = bottom
            R = right
            B = back
        The adjacent faces are labeled with respect to this diagram, noting the orientation of each 
        letter (face) when folded into a cube.
        */

        public Cube( CubeGame game, CubeScreen screen )
            : base( game, screen )
        {
            Screen = screen;

            mFrontFace = NewFace( CubeFaceType.Front, Vector3.UnitZ, Vector3.UnitY, Direction.Up );
            mBackFace = NewFace( CubeFaceType.Back, -Vector3.UnitZ, -Vector3.UnitY, Direction.Down );
            mTopFace = NewFace( CubeFaceType.Top, Vector3.UnitY, -Vector3.UnitZ, Direction.Up );
            mBottomFace = NewFace( CubeFaceType.Bottom, -Vector3.UnitY, Vector3.UnitZ, Direction.Up );
            mLeftFace = NewFace( CubeFaceType.Left, -Vector3.UnitX, Vector3.UnitZ, Direction.Right );
            mRightFace = NewFace( CubeFaceType.Right, Vector3.UnitX, Vector3.UnitZ, Direction.Left );

            CurrentFace = mFrontFace;
            UpDir = CompassDirection.North;
        }

        protected abstract Face NewFace( CubeFaceType type, Vector3 normal, Vector3 up, Direction rotation );

        protected void ConnectFaces()
        {
            mFrontFace.NorthFace = mTopFace;
            mFrontFace.EastFace = mRightFace;
            mFrontFace.SouthFace = mBottomFace;
            mFrontFace.WestFace = mLeftFace;
			mFrontFace.OppositeFace = mBackFace;

            mBackFace.NorthFace = mBottomFace;
            mBackFace.EastFace = mRightFace;
            mBackFace.SouthFace = mTopFace;
            mBackFace.WestFace = mLeftFace;
			mBackFace.OppositeFace = mFrontFace;

            mTopFace.NorthFace = mBackFace;
            mTopFace.EastFace = mRightFace;
            mTopFace.SouthFace = mFrontFace;
            mTopFace.WestFace = mLeftFace;
			mTopFace.OppositeFace = mBottomFace;

            mBottomFace.NorthFace = mFrontFace;
            mBottomFace.EastFace = mRightFace;
            mBottomFace.SouthFace = mBackFace;
            mBottomFace.WestFace = mLeftFace;
			mBottomFace.OppositeFace = mTopFace;

            mLeftFace.NorthFace = mFrontFace;
            mLeftFace.EastFace = mBottomFace;
            mLeftFace.SouthFace = mBackFace;
            mLeftFace.WestFace = mTopFace;
			mLeftFace.OppositeFace = mRightFace;

            mRightFace.NorthFace = mFrontFace;
            mRightFace.EastFace = mTopFace;
            mRightFace.SouthFace = mBackFace;
            mRightFace.WestFace = mBottomFace;
			mRightFace.OppositeFace = mLeftFace;
        }

        public class CubeFile
        {
            public CubePosition StartPosition;
            public List<CubePosition> EnemyPositions;
            public string NextLevel;

            public string FrontFace;
            public string BackFace;
            public string TopFace;
            public string BottomFace;
            public string LeftFace;
            public string RightFace;

            #region Serialization
            public static void Serialize( CubeFile cube, string filename )
            {
                using ( StreamWriter writer = new StreamWriter( $@"..\..\..\{filename}" ) )
                    new XmlSerializer( typeof( CubeFile ) ).Serialize( writer, cube );
            }

            public static CubeFile Deserialize( string filename )
            {
                using ( StreamReader reader = new StreamReader( TitleContainer.OpenStream( filename ) ) )
                    return (CubeFile) new XmlSerializer( typeof( CubeFile ) ).Deserialize( reader );
            }

            private static World StringToWorld( string str )
            {
                using ( MemoryStream stream = new MemoryStream( Encoding.UTF8.GetBytes( str ) ) )
                    return WorldSerializer.Deserialize( stream );
            }

            private static string WorldToString( World world )
            {
                using ( MemoryStream stream = new MemoryStream() )
                {
                    WorldSerializer.Serialize( world, stream );
                    stream.Seek( 0, SeekOrigin.Begin );
                    using ( StreamReader reader = new StreamReader( stream ) )
                        return reader.ReadToEnd();
                }
            }
            #endregion

            public World this[ CubeFaceType faceType ]
            {
                get {
                    switch ( faceType )
                    {
                    case CubeFaceType.Front:
                        return StringToWorld( FrontFace );
                    case CubeFaceType.Back:
                        return StringToWorld( BackFace );
                    case CubeFaceType.Top:
                        return StringToWorld( TopFace );
                    case CubeFaceType.Bottom:
                        return StringToWorld( BottomFace );
                    case CubeFaceType.Left:
                        return StringToWorld( LeftFace );
                    case CubeFaceType.Right:
                        return StringToWorld( RightFace );
                    default:
                        throw new ArgumentException();
                    }
                }
                set {
                    switch ( faceType )
                    {
                    case CubeFaceType.Front:
                        FrontFace = WorldToString( value );
                        break;
                    case CubeFaceType.Back:
                        BackFace = WorldToString( value );
                        break;
                    case CubeFaceType.Top:
                        TopFace = WorldToString( value );
                        break;
                    case CubeFaceType.Bottom:
                        BottomFace = WorldToString( value );
                        break;
                    case CubeFaceType.Left:
                        LeftFace = WorldToString( value );
                        break;
                    case CubeFaceType.Right:
                        RightFace = WorldToString( value );
                        break;
                    default:
                        throw new ArgumentException();
                    }
                }
            }
        }

        internal void Save( string name )
        {
            CubeFile file = new CubeFile();
            file.StartPosition = StartPosition;
            file.EnemyPositions = EnemyPositions;
            file.NextLevel = NextLevel;

            foreach ( Face face in Faces )
                file[ face.Type ] = face.World;

            CubeFile.Serialize( file, $@"GameLevels\{name}.ccf" );
        }

        internal void Load( string name )
        {
            this.Name = name;

            CubeFile file = CubeFile.Deserialize( $@"GameLevels\{name}.ccf" );
            StartPosition = file.StartPosition;
            EnemyPositions = file.EnemyPositions;
            if (EnemyPositions == null)
            {
                EnemyPositions = new List<CubePosition>();
            }
            NextLevel = file.NextLevel;

            foreach ( Face face in Faces )
                face.World = file[ face.Type ];
        }

        public const float NEAR_PLANE = 1f;
        public const float FAR_PLANE = 10f;

        public override void Initialize()
        {
            base.Initialize();

            ConnectFaces();

            Screen.Camera.Fov = MathHelper.PiOver4;
            Screen.Camera.AspectRatio = GraphicsDevice.Viewport.AspectRatio;
            Screen.Camera.NearPlaneDistance = NEAR_PLANE;
            Screen.Camera.FarPlaneDistance = FAR_PLANE;

            Screen.Camera.Position = CameraDistance * CurrentFace.Normal;
            Screen.Camera.Target = Position;
            Screen.Camera.UpVector = ComputeUpVector();

            //Effect = new BasicEffect( GraphicsDevice );
            //Effect.AmbientLightColor = new Vector3( 1.0f, 1.0f, 1.0f );
            //Effect.DirectionalLight0.Enabled = true;
            //Effect.DirectionalLight0.DiffuseColor = Vector3.One;
            //Effect.DirectionalLight0.Direction = Vector3.Normalize( Vector3.One );
            //Effect.LightingEnabled = true;

            Effect = new CubeEffect( Game.Content );
            Effect.LightColor = Color.White.ToVector4();
            Effect.LightDirection = Vector3.Normalize( -Vector3.One );
            Effect.AmbientColor = Color.Gray.ToVector4();

            Effect.World = Matrix.Identity;
            Effect.View = Screen.Camera.View;
            Effect.Projection = Screen.Camera.Projection;

            foreach ( Face face in Faces )
                face.Initialize();
        }

        public override void Update( GameTime gameTime )
        {
            base.Update( gameTime );

            foreach ( Face face in Faces )
                face.Update( gameTime );
        }


        public override void Draw( GameTime gameTime )
        {
            Matrix S = Matrix.CreateScale( Scale );
            Matrix R = Matrix.CreateFromYawPitchRoll( Rotation.Y, Rotation.X, Rotation.Z );
            Matrix T = Matrix.CreateTranslation( Position );
            Effect.World = S * R * T;

            Vector3 cameraPos = Screen.Camera.Position;
            Vector3 cameraTgt = Screen.Camera.Target;
            Effect.LightDirection = Vector3.Normalize( cameraTgt - cameraPos );

            Effect.View = Screen.Camera.View;
            Effect.Projection = Screen.Camera.Projection;

            //Screen.Camera.Apply( Effect );

            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            foreach ( Face face in Faces )
                face.Render2D( gameTime );

            //mSpriteBatch.Begin();
            //mSpriteBatch.Draw(mBG, Vector2.Zero, Color.White);
            //mSpriteBatch.End();

            //Effect.SkyboxTexture = sSkybox;

            RenderSkybox();

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            GraphicsDevice.RasterizerState = rs;

            foreach ( Face face in Faces )
                face.Render3D( Effect );

            //CurrentFace.Render3D( Effect );
        }

        private void RenderSkybox()
        {
            // Set the View and Projection matrix for the effect
            sSkySphereEffect.Parameters[ "ViewMatrix" ].SetValue(
                Effect.View );
            sSkySphereEffect.Parameters[ "ProjectionMatrix" ].SetValue(
                Effect.Projection );
            // Draw the sphere model that the effect projects onto
            foreach ( ModelMesh mesh in sSkySphere.Meshes )
            {
                mesh.Draw();
            }
        }

    }
}
