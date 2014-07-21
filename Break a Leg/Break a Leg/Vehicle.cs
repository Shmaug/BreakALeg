using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;

namespace Jeep_Racer
{
    public class Vehicle
    {
        public List<PhysicsObject> wheels = new List<PhysicsObject>();
        public List<PhysicsObject> body = new List<PhysicsObject>();
        public List<LineJoint> lJoints = new List<LineJoint>();
        public float maxSpeed;

        public Vehicle()
        {

        }

        public static Vehicle createJeep(Vector2 pos, float damping = 0.5f, float freq = 2.5f, float torque = 1.0f, float friction = 2f, float bodyMassMultiplier = 1f, float wheelMassMultiplier = 1f)
        {
            pos = ConvertUnits.ToDisplayUnits(pos);
            PhysicsObject _car;
            PhysicsObject _fwheel;
            PhysicsObject _rwheel;
            LineJoint _fjoint;
            LineJoint _rjoint;

            // car body
            _car = PhysicsObject.createFromTexture(Main.sprTextures[2], Main.sprTextures[2].Width);
            _car.dposition = pos;
            _car.texture = Main.sprTextures[2];
            _car.body.Mass *= bodyMassMultiplier;

            // wheels
            _fwheel = PhysicsObject.createCircle(ConvertUnits.ToSimUnits(65));
            _fwheel.dposition = pos + new Vector2(459, 209);
            _fwheel.texture = Main.sprTextures[3];
            _fwheel.origin = TextureCreator.CalculateOrigin(_fwheel.body);
            _fwheel.body.Friction = friction;
            _fwheel.body.Mass *= wheelMassMultiplier;

            _rwheel = PhysicsObject.createCircle(ConvertUnits.ToSimUnits(65));
            _rwheel.dposition = pos + new Vector2(127, 209);
            _rwheel.texture = Main.sprTextures[3];
            _fwheel.origin = TextureCreator.CalculateOrigin(_fwheel.body);
            _rwheel.body.Friction = friction;
            _rwheel.body.Mass *= wheelMassMultiplier;

            // wheel joints
            Vector2 axis = new Vector2(0, 1f);
            _fjoint = new LineJoint(_car.body, _fwheel.body, _fwheel.position, axis);
            _fjoint.MotorEnabled = true;
            _fjoint.MotorSpeed = 0.0f;
            _fjoint.MaxMotorTorque = torque;
            _fjoint.Frequency = freq;
            _fjoint.DampingRatio = damping;
            Main.physicsWorld.AddJoint(_fjoint);

            _rjoint = new LineJoint(_car.body, _rwheel.body, _rwheel.position, axis);
            _rjoint.MotorEnabled = true;
            _rjoint.MotorSpeed = 0.0f;
            _rjoint.MaxMotorTorque = torque;
            _rjoint.Frequency = freq;
            _rjoint.DampingRatio = damping;
            Main.physicsWorld.AddJoint(_rjoint);

            Vehicle car = new Vehicle();
            car.body.Add(_car);
            car.wheels.Add(_fwheel);
            car.wheels.Add(_rwheel);
            car.lJoints.Add(_fjoint);
            car.lJoints.Add(_rjoint);
            car.maxSpeed = 10.0f;


            return car;
        }

        public static Vehicle createMinivan(Vector2 pos, float damping = 0.5f, float freq = 2.5f, float torque = 10.0f)
        {
            pos = ConvertUnits.ToDisplayUnits(pos);
            PhysicsObject _car;
            PhysicsObject _fwheel;
            PhysicsObject _rwheel;
            LineJoint _fjoint;
            LineJoint _rjoint;

            // car body
            _car = PhysicsObject.createFromTexture(Main.sprTextures[0], Main.sprTextures[0].Width);
            _car.dposition = pos;
            _car.texture = Main.sprTextures[0];

            // wheels
            _fwheel = PhysicsObject.createCircle(ConvertUnits.ToSimUnits(26));
            _fwheel.dposition = pos + new Vector2(293, 113);
            _fwheel.texture = Main.sprTextures[1];

            _rwheel = PhysicsObject.createCircle(ConvertUnits.ToSimUnits(26));
            _rwheel.dposition = pos + new Vector2(70, 113);
            _rwheel.texture = Main.sprTextures[1];
            _rwheel.body.Friction = 1f;
            // wheel joints
            Vector2 axis = new Vector2(0.5f, 1f);
            _fjoint = new LineJoint(_car.body, _fwheel.body, _fwheel.position, axis);
            _fjoint.MotorEnabled = false;
            _fjoint.MotorSpeed = 0.0f;
            _fjoint.MaxMotorTorque = torque;
            _fjoint.Frequency = freq;
            _fjoint.DampingRatio = damping;
            Main.physicsWorld.AddJoint(_fjoint);

            _rjoint = new LineJoint(_car.body, _rwheel.body, _rwheel.position, axis);
            _rjoint.MotorEnabled = true;
            _rjoint.MotorSpeed = 0.0f;
            _rjoint.MaxMotorTorque = torque;
            _rjoint.Frequency = freq;
            _rjoint.DampingRatio = damping;
            Main.physicsWorld.AddJoint(_rjoint);

            Vehicle car = new Vehicle();
            car.body.Add(_car);
            car.wheels.Add(_fwheel);
            car.wheels.Add(_rwheel);
            car.lJoints.Add(_fjoint);
            car.lJoints.Add(_rjoint);

            return car;
        }
    }
}
