using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Collision;
using FarseerPhysics.Factories;

namespace Jeep_Racer
{
    public class Main : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public static KeyboardState ks = Keyboard.GetState(), lastks = Keyboard.GetState();
        public static MouseState ms = Mouse.GetState(), lastms = Mouse.GetState();

        public static int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        public static int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

        public static bool isPaused = false;
        public static bool inGame = true;

        public static PhysicsObject[] objects = new PhysicsObject[500];

        /// <summary>
        /// 0:brick  1:slate  2:aluminium 3:concrete  4:diamondplate  5:dirt  6:grass  7:ice  8:slate  9:wood
        /// </summary>
        public static Texture2D[] matTextures = new Texture2D[10];
        /// <summary>
        /// 0:car  1:wheel  2:aluminium 3:concrete  4:diamondplate  5:dirt  6:grass  7:ice  8:slate  9:wood
        /// </summary>
        public static Texture2D[] sprTextures = new Texture2D[4];
        public static Texture2D[] cursorTextures = new Texture2D[2];
        public static Texture2D colorTexture;
        public static Texture2D db_dot;
        public static Texture2D db_square;

        public static Vehicle _car;

        public static SpriteFont[] font = new SpriteFont[1];

        public static World physicsWorld;

        public static Vector2 camPos = Vector2.Zero;

        public static Random randomNum = new Random(System.Environment.TickCount);

        public static bool inEditorMode = true;
        public static bool editMakingLine = false;
        public static Vector2 editLineP1 = Vector2.Zero;
        public static PhysicsObject editGround;
        public static Vertices editVerts = new Vertices();

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;
            graphics.IsFullScreen = false;
        }

        protected override void Initialize()
        {
            for (int i = 0; i < objects.Length; i++) { objects[i] = new PhysicsObject(); objects[i].active = false; }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            for (int i = 0; i < font.Length; i++) font[i] = Content.Load<SpriteFont>("image/font/font_" + i);
            for (int i = 0; i < sprTextures.Length; i++) sprTextures[i] = Content.Load<Texture2D>("image/spr/spr_" + i);
            for (int i = 0; i < cursorTextures.Length; i++) cursorTextures[i] = Content.Load<Texture2D>("image/ui/cur_" + i);
            for (int i = 0; i < matTextures.Length; i++) matTextures[i] = Content.Load<Texture2D>("image/mat/mat_" + i);
            db_dot = Content.Load<Texture2D>("image/db/db_dot");
            db_square = Content.Load<Texture2D>("image/db/db_square");
            colorTexture = Content.Load<Texture2D>("image/color");

            physicsWorld = new World(new Vector2(0, 1));

            editGround = PhysicsObject.createEmpty();
            editGround.shape = 2;
            editGround.texture = colorTexture;

            _car = Vehicle.createJeep(Vector2.Zero, 1f, 1.5f, 15.0f, 1f, 5f, 2.5f);
            _car.body[0].body.LocalCenter = ConvertUnits.ToSimUnits(new Vector2(370, 85));
        }

        protected override void UnloadContent()
        {
            
        }

        protected override void Update(GameTime gameTime)
        {
            ks = Keyboard.GetState();
            ms = Mouse.GetState();

            handleInput(gameTime, GraphicsDevice);
            if (!inEditorMode)
            {
                if (inGame)
                {
                    if (!isPaused)
                    {
                        camPos = Vector2.Lerp(camPos, (_car.body[0].dposition + new Vector2(_car.body[0].dwidth / 2, _car.body[0].dheight / 2)) - new Vector2(screenWidth / 2, screenHeight / 2), 0.1f);
                        PhysicsObject.UpdateAll();
                        physicsWorld.Step(1f / 30f);
                    }
                }
            }

            lastks = ks;
            lastms = ms;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.SkyBlue);

            spriteBatch.Begin();
            foreach (PhysicsObject obj in objects)
            {
                if (obj.active)
                {
                    Vector2 pos = obj.dposition - camPos;
                    if (obj.texture != null)
                    {
                        spriteBatch.Draw(obj.texture, pos, null, Color.White, obj.body.Rotation, obj.origin, 1f, SpriteEffects.None, 0f);
                    }
                    if (obj.shape == 2)
                    {
                        Vertices verts = new Vertices();
                        List<Fixture> fList = obj.body.FixtureList;
                        for (int i = 0; i < fList.Count; ++i)
                        {
                            Texture2D tex = matTextures[obj.matType];
                            if (obj.texture != null) tex = obj.texture;

                            EdgeShape shape = (EdgeShape)fList[i].Shape;
                            Vector2 v1 = ConvertUnits.ToDisplayUnits(shape.Vertex1) - camPos;
                            Vector2 v2 = ConvertUnits.ToDisplayUnits(shape.Vertex2) - camPos;
                            float a = (float) Math.Atan2(v2.Y - v1.Y, v2.X - v1.X);
                            int d = (int) Vector2.Distance(v1, v2);
                            spriteBatch.Draw(tex, new Rectangle((int)v1.X, (int)v1.Y, d, 10), null, Color.White, a, new Vector2(0, 0), SpriteEffects.None, 0f);
                        }
                    }
                }
            }
            string[] dbtxt = Debug.getText();
            spriteBatch.DrawString(font[0], dbtxt[0], new Vector2(0, 0), Color.Red);
            spriteBatch.DrawString(font[0], dbtxt[1], new Vector2(screenWidth - 800, 0), Color.Black);
            spriteBatch.DrawString(font[0], dbtxt[2], new Vector2(0, screenHeight - 200), Color.Black);
            if (ms.LeftButton == ButtonState.Pressed)
                spriteBatch.Draw(cursorTextures[1], new Vector2(ms.X, ms.Y), Color.White);
            else
                spriteBatch.Draw(cursorTextures[0], new Vector2(ms.X, ms.Y), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private static void resetCar()
        {
            _car.body[0].position = Vector2.Zero;
            _car.body[0].body.Rotation = 0f;
            _car.body[0].body.AngularVelocity = 0f;
            _car.body[0].body.LinearVelocity = Vector2.Zero;

            _car.wheels[0].position = _car.lJoints[0].LocalAnchorA;
            _car.wheels[0].body.Rotation = 0f;
            _car.wheels[0].body.AngularVelocity = 0f;
            _car.wheels[0].body.LinearVelocity = Vector2.Zero;

            _car.wheels[1].position = _car.lJoints[1].LocalAnchorA;
            _car.wheels[1].body.Rotation = 0f;
            _car.wheels[1].body.AngularVelocity = 0f;
            _car.wheels[1].body.LinearVelocity = Vector2.Zero;
        }

        private static void handleInput(GameTime gameTime, GraphicsDevice device)
        {
            if (ks.IsKeyDown(Keys.E) && lastks.IsKeyUp(Keys.E))
            {
                inEditorMode = !inEditorMode;
                if (inEditorMode)
                {
                    camPos = Vector2.Zero;
                    resetCar();
                }
            }
            if (!inEditorMode)
            {
                if (inGame)
                {
                    if (ks.IsKeyDown(Keys.Space) && lastks.IsKeyUp(Keys.Space))
                    {
                        isPaused = !isPaused;
                    }
                    #region debug
                    if (ks.IsKeyDown(Keys.T) && lastks.IsKeyUp(Keys.T))
                    {
                        Debug.print("Attempting to attach objects");
                        for (int i = 0; i < objects.Length; i++)
                        {
                            if (objects[i].active)
                            {
                                Vector2 m = ConvertUnits.ToSimUnits(new Vector2(ms.X, ms.Y));
                                if (!objects[i].attached)
                                {
                                    Debug.AttachObject(i);
                                    Debug.print("Attached object " + i);
                                }
                            }
                        }
                    }
                    #endregion
                    if (ks.IsKeyDown(Keys.Y) && lastks.IsKeyUp(Keys.Y))
                    {
                        Debug.print("De attaching all objects");
                        for (int i = 0; i < Debug.attachedObjects.Length; i++)
                        {
                            Debug.DeattachObject(i);
                            Debug.print("Deattached object " + i);
                        }
                    }
                    if (!isPaused)
                    {
                        if (ms.LeftButton == ButtonState.Pressed && lastms.LeftButton == ButtonState.Released)
                        {
                            PhysicsObject obj = PhysicsObject.createRectangle(1, 1);
                            obj.dposition = new Vector2(ms.X, ms.Y) + camPos;
                            obj.texture = TextureCreator.TextureFromShape(obj.body.FixtureList[0].Shape, 6, Color.Orange, 1.5f, device);
                            obj.matType = 6;
                        }
                        if (ms.RightButton == ButtonState.Pressed)
                        {

                        }
                        if (ks.IsKeyDown(Keys.R))
                        {
                            _car.body[0].body.AngularVelocity = 1f;
                        }
                        if (ks.IsKeyDown(Keys.A))
                        {
                            if (_car.lJoints[1].MotorSpeed > -_car.maxSpeed)
                                _car.lJoints[1].MotorSpeed -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (_car.lJoints[0].MotorSpeed > -_car.maxSpeed)
                                _car.lJoints[0].MotorSpeed -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else if (ks.IsKeyDown(Keys.D))
                        {
                            if (_car.lJoints[1].MotorSpeed < _car.maxSpeed)
                                _car.lJoints[1].MotorSpeed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                            if (_car.lJoints[0].MotorSpeed < _car.maxSpeed)
                                _car.lJoints[0].MotorSpeed += (float)gameTime.ElapsedGameTime.TotalSeconds;
                        }
                        else
                        {
                            _car.lJoints[1].MotorSpeed = MathHelper.Lerp(_car.lJoints[1].MotorSpeed, 0.0f, 0.01f);
                            _car.lJoints[0].MotorSpeed = MathHelper.Lerp(_car.lJoints[1].MotorSpeed, 0.0f, 0.01f);
                        }
                        if (ks.IsKeyDown(Keys.X))
                        {
                            _car.lJoints[1].MotorSpeed = 0.0f;
                            _car.lJoints[0].MotorSpeed = 0.0f;
                        }
                        int inc = 2;
                        if (ks.IsKeyDown(Keys.LeftShift)) inc = 5;
                        if (ks.IsKeyDown(Keys.Right))
                        {
                            camPos.X += inc;
                        }
                        else if (ks.IsKeyDown(Keys.Left))
                        {
                            camPos.X -= inc;
                        }
                        if (ks.IsKeyDown(Keys.Up))
                        {
                            camPos.Y -= inc;
                        }
                        else if (ks.IsKeyDown(Keys.Down))
                        {
                            camPos.Y += inc;
                        }
                        PhysicsObject.UpdateAll();
                    }
                }
            }
            else  //      EDITOR MODE      //
            {
                int inc = 5;
                if (ks.IsKeyDown(Keys.LeftShift)) inc = 10;
                if (ks.IsKeyDown(Keys.D))
                {
                    camPos.X += inc;
                }
                else if (ks.IsKeyDown(Keys.A))
                {
                    camPos.X -= inc;
                }
                if (ks.IsKeyDown(Keys.W))
                {
                    camPos.Y -= inc;
                }
                else if (ks.IsKeyDown(Keys.S))
                {
                    camPos.Y += inc;
                }

                if (ms.LeftButton == ButtonState.Pressed && lastms.LeftButton == ButtonState.Released)
                {
                    editVerts.Add(ConvertUnits.ToSimUnits(new Vector2(ms.X, ms.Y) + camPos));
                    editGround.body = new Body(physicsWorld);
                    if (editVerts.Count > 1)
                    {
                        for (int i = 0; i < editVerts.Count; i++)
                        {
                            try
                            {
                                FixtureFactory.AttachEdge(editVerts[i], editVerts[i + 1], editGround.body);
                            }
                            catch { }
                        }
                    }
                }

                if (ms.RightButton == ButtonState.Pressed && lastms.RightButton == ButtonState.Released)
                {
                    if (editVerts.Count > 0)
                    {
                        editVerts.Remove(editVerts[editVerts.Count - 1]);
                    }
                    editGround.body = new Body(physicsWorld);
                    if (editVerts.Count > 1)
                    {
                        for (int i = 0; i < editVerts.Count; i++)
                        {
                            try
                            {
                                FixtureFactory.AttachEdge(editVerts[i], editVerts[i + 1], editGround.body);
                            }
                            catch { }
                        }
                    }
                }
            }
        }
    }
}
