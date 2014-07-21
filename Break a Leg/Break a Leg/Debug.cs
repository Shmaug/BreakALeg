using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Collision.Shapes;

namespace Jeep_Racer
{
    public class Debug
    {
        public static string[] constantText = new string[10];
        public static string[] debugText = new string[10];
        public static int[] attachedObjects = new int[10] { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

        public static void print(string output)
        {
            debugText[0] = debugText[1];
            debugText[1] = debugText[2];
            debugText[2] = debugText[3];
            debugText[3] = debugText[4];
            debugText[4] = debugText[5];
            debugText[5] = debugText[6];
            debugText[6] = debugText[7];
            debugText[7] = debugText[8];
            debugText[8] = debugText[9];
            debugText[9] = output;
        }

        public static void DeattachObject(int index)
        {
            int i = attachedObjects[index];
            if (i >= 0 && i < Main.objects.Length)
            {
                if (Main.objects[i].attached)
                    Main.objects[i].attached = false;
            }
            attachedObjects[index] = -1;
        }

        public static void AttachObject(int obj)
        {
            DeattachObject(0);
            attachedObjects[0] = attachedObjects[1];
            attachedObjects[1] = attachedObjects[2];
            attachedObjects[2] = attachedObjects[3];
            attachedObjects[3] = attachedObjects[4];
            attachedObjects[4] = attachedObjects[5];
            attachedObjects[5] = attachedObjects[6];
            attachedObjects[6] = attachedObjects[7];
            attachedObjects[7] = attachedObjects[8];
            attachedObjects[8] = attachedObjects[9];
            attachedObjects[9] = obj;
            Main.objects[obj].attached = true;
        }

        public static string[] getText()
        {
            string final = "";
            string final2 = "";
            string final3 = "";
            for (int i = 0; i < debugText.Length; i++)
            {
                if (debugText[i] != null && debugText[i] != "")
                {
                    final = final + debugText[i] + "\n";
                }
                if (constantText[i] != null && constantText[i] != "")
                {
                    final2 = final2 + constantText[i] + "\n";
                }
                if (attachedObjects[i] != -1)
                {
                    if (!Main.objects[attachedObjects[i]].active)
                    {
                        Main.objects[attachedObjects[i]].attached = false;
                        attachedObjects[i] = -1;
                    }
                    else
                    {
                        int index = attachedObjects[i];
                        Shape shape = Main.objects[index].body.FixtureList[0].Shape;
                        PolygonShape polyshape = (PolygonShape)shape;
                        Transform xf;
                        Main.objects[index].body.GetTransform(out xf);
                        Vector2[] vert = new Vector2[100];
                        string verts = "";
                        for (int j = 0; j < polyshape.Vertices.Count; ++j)
                        {
                            vert[j] = MathUtils.Multiply(ref xf, polyshape.Vertices[j]);
                            verts = verts + "v" + j + ": " + vert[j] + "  ";
                        }
                        final3 = final3 + "Object " + index +
                            Main.objects[index].position + "  " + verts + "\n";
                    }
                }
            }

            return new string[] {final, final2, final3};
        }
    }
}
