using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;

namespace Jeep_Racer
{
    class Level
    {
        public static string levelDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + "\\My Games\\JeepGame\\levels\\";
        public static List<PhysicsObject> levelBody = new List<PhysicsObject>();

        public static void loadLevel(string path)
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            BinaryReader reader = new BinaryReader(stream);
            Vertices vert = new Vertices();

            int amount = reader.ReadInt32();
            for (int i = 0; i < amount; i++)
            {
                float X = reader.ReadSingle();
                float Y = reader.ReadSingle();
                vert.Add(new Vector2(X, Y));
            }

            reader.Close();
            stream.Close();
            Main.physicsWorld.Clear();
            //for (int i = 0; i < Main.physicsWorld.BodyList.Count; i++)
            //{
            //    Main.physicsWorld.RemoveBody(Main.physicsWorld.BodyList[i]);
            //}
            PhysicsObject obj = PhysicsObject.createEdge(vert);
            obj.body.BodyType = BodyType.Static;
            obj.matType = 1;
            obj.body.BodyType = BodyType.Static;
            levelBody.Add(obj);
        }

        public static void saveLevel(string path, Vertices vert)
        {
            Directory.CreateDirectory(Level.levelDirectory);
            FileStream stream = new FileStream(path, FileMode.Create);
            BinaryWriter writer = new BinaryWriter(stream);

            int amount = vert.Count;
            writer.Write(amount);
            for (int i = 0; i < amount; i++)
            {
                writer.Write(vert[i].X);
                writer.Write(vert[i].Y);
            }

            writer.Close();
            stream.Close();

            PhysicsObject obj = PhysicsObject.createEdge(vert);
            obj.body.BodyType = BodyType.Static;
            obj.matType = 1;
            levelBody.Add(obj);
        }
    }
}
