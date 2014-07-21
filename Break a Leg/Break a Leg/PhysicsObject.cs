using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using FarseerPhysics.Common.Decomposition;
using FarseerPhysics.Common.PolygonManipulation;

namespace Jeep_Racer
{
    public class PhysicsObject
    {
        public bool active;
        public bool attached;
        public byte shape;
        public byte matType;
        public Texture2D texture;
        public float width;
        public float height;
        public float radius;
        public int whoAmI = -1;
        public Vector2 origin;
        public Body body;
        public Vector2 position
        {
            get
            {
                return this.body.Position;
            }
            set
            {
                this.body.Position = value;
            }
        }
        public Vector2 dposition
        {
            get
            {
                return ConvertUnits.ToDisplayUnits(this.body.Position);
            }
            set
            {
                this.position = ConvertUnits.ToSimUnits(value);
            }
        }
        public float dwidth
        {
            get
            {
                return ConvertUnits.ToDisplayUnits(this.width);
            }
            set
            {
                this.width = ConvertUnits.ToSimUnits(value);
            }
        }
        public float dheight
        {
            get
            {
                return ConvertUnits.ToDisplayUnits(this.height);
            }
            set
            {
                this.height = ConvertUnits.ToSimUnits(value);
            }
        }
        public float dradius
        {
            get
            {
                return ConvertUnits.ToDisplayUnits(this.radius);
            }
            set
            {
                this.radius = ConvertUnits.ToSimUnits(value);
            }
        }
        public Vector2 velocity
        {
            get
            {
                return this.body.LinearVelocity;
            }
            set
            {
                this.body.LinearVelocity = value;
            }
        }

        public PhysicsObject()
        {

        }

        public void Delete()
        {
            this.active = false;
            if (!this.body.IsDisposed)
            {
                this.body.Dispose();
            }
        }

        public void Update()
        {

        }

        public static void UpdateAll()
        {
            foreach (PhysicsObject obj in Main.objects)
            {
                if (obj.active)
                {
                    obj.Update();
                }
            }
        }

        public static PhysicsObject createRectangle(float width, float height, float density = 1f)
        {
            PhysicsObject obj = new PhysicsObject();
            for (int i = 0; i < Main.objects.Length; i++)
            {
                if (Main.objects[i].active == false)
                {
                    Main.objects[i] = obj;
                    obj.whoAmI = i;
                    break;
                }
            }
            obj.shape = 0;
            obj.body = BodyFactory.CreateRectangle(Main.physicsWorld, width, height, density);
            obj.active = true;
            obj.width = width;
            obj.height = height;
            obj.origin = new Vector2(obj.dwidth/2, obj.dheight/2);
            obj.body.BodyType = BodyType.Dynamic;
            Debug.print("New rectangle with " + width + " width and " + height + " height successfully added");
            return obj;
        }

        public static PhysicsObject createFromTexture(Texture2D texture, float density = 1f)
        {
            int width = texture.Width;
            int height = texture.Height;
            uint[] data = new uint[texture.Width * texture.Height];
            texture.GetData<uint>(data);

            PhysicsObject obj = new PhysicsObject();
            for (int i = 0; i < Main.objects.Length; i++)
            {
                if (Main.objects[i].active == false)
                {
                    Main.objects[i] = obj;
                    obj.whoAmI = i;
                    break;
                }
            }
            Vertices verts = PolygonTools.CreatePolygon(data, width, false);
            Vector2 centroid = -verts.GetCentroid();
            obj.origin = Vector2.Zero;// centroid / 2;
            verts = SimplifyTools.ReduceByDistance(verts, 4f);
            List<Vertices> list = BayazitDecomposer.ConvexPartition(verts);
            Vector2 vScale = new Vector2(ConvertUnits.ToSimUnits(1));
            foreach (Vertices v in list)
            {
                v.Scale(ref vScale);
            }
            obj.body = BodyFactory.CreateCompoundPolygon(Main.physicsWorld, list, 1f);
            obj.shape = 3;
            obj.active = true;
            obj.dwidth = width;
            obj.dheight = height;
            obj.body.BodyType = BodyType.Dynamic;
            Debug.print("New texture body with " + verts.Count + " vertices successfully added");
            return obj;
        }

        public static PhysicsObject createCircle(float radius, float density = 1f)
        {
            PhysicsObject obj = new PhysicsObject();
            for (int i = 0; i < Main.objects.Length; i++)
            {
                if (Main.objects[i].active == false)
                {
                    Main.objects[i] = obj;
                    obj.whoAmI = i;
                    break;
                }
            }
            obj.shape = 1;
            obj.body = BodyFactory.CreateCircle(Main.physicsWorld, radius, density);
            obj.active = true;
            obj.radius = radius;
            obj.origin = new Vector2(obj.dradius, obj.dradius);
            obj.body.BodyType = BodyType.Dynamic;
            Debug.print("New circle with " + radius + " radius successfully added");
            return obj;
        }

        public static PhysicsObject createEdge(Vertices vertices)
        {
            PhysicsObject obj = new PhysicsObject();
            for (int i = 0; i < Main.objects.Length; i++)
            {
                if (Main.objects[i].active == false)
                {
                    Main.objects[i] = obj;
                    obj.whoAmI = i;
                    break;
                }
            }
            obj.body = new Body(Main.physicsWorld);
            for (int i = 0; i < vertices.Count; i++)
            {
                try
                {
                    FixtureFactory.AttachEdge(vertices[i], vertices[i + 1], obj.body);
                }
                catch { }
            }
            obj.shape = 2;
            obj.active = true;
            obj.body.BodyType = BodyType.Dynamic;
            Debug.print("New edge with " + vertices.Count + " vertices successfully added");
            return obj;
        }

        public static PhysicsObject createEmpty()
        {
            PhysicsObject obj = new PhysicsObject();
            for (int i = 0; i < Main.objects.Length; i++)
            {
                if (Main.objects[i].active == false)
                {
                    Main.objects[i] = obj;
                    obj.whoAmI = i;
                    break;
                }
            }
            obj.active = true;
            obj.body = BodyFactory.CreateBody(Main.physicsWorld);
            obj.body.BodyType = BodyType.Static;
            obj.matType = 1;
            Debug.print("New empty physics object successfully added");
            return obj;
        }
    }
}
