﻿using System;
using System.Collections.Generic;
using GLFW;

namespace SharpEngine
{
    class Program {
        static float Lerp(float from, float to, float t) {
            return from + (to - from) * t;
        }

        static float GetRandomFloat(Random random, float min = 0, float max = 1) {
            return Lerp(min, max, (float)random.Next() / int.MaxValue);
        }
        
        static void FillSceneWithTriangles(Scene scene, Material material) {
            var random = new Random();
            for (var i = 0; i < 10; i++) {
                var triangle = new Triangle(new Vertex[] {
                    new Vertex(new Vector(-.1f, 0f), Color.Red),
                    new Vertex(new Vector(.1f, 0f), Color.Green),
                    new Vertex(new Vector(0f, .133f), Color.Blue)
                }, material);
                triangle.Rotate(GetRandomFloat(random));
                triangle.Transform.Move(new Vector(GetRandomFloat(random, -1, 1), GetRandomFloat(random, -1, 1)));
                scene.Add(triangle);
            }
        }
        
        static void Main(string[] args) {
            
            var window = new Window();
            var material = new Material("shaders/world-position-color.vert", "shaders/vertex-color.frag");
            var scene = new Scene();
            window.Load(scene);

            //FillSceneWithTriangles(scene, material);
           // var newTriangle = new Rectangle( material);
           var shape = new Triangle(material);
           shape.Transform.CurrentScale = new Vector(0.5f, 1f, 1f);
           scene.Add(shape);
            
            var ground = new Rectangle(material);
            ground.Transform.CurrentScale = new Vector(20f, 1f, 1f);
            ground.Transform.Position = new Vector(0f, -1f);
            scene.Add(ground);
            
            // engine rendering loop
         
            const int fixedStepNumberPerSecond = 30;
            const float fixedDeltaTime = 1.0f / fixedStepNumberPerSecond;
            const float movementSpeed = 0.5f;
            double previousFixedStep = 0.0;
           // Glfw.Time
           while (window.IsOpen())
           {
               while (Glfw.Time > previousFixedStep + fixedDeltaTime)
               {

                   var walkDirection = new Vector();
                   previousFixedStep = Glfw.Time;
                   // Update Triangles

                   if (window.GetKey(Keys.W))
                   {
                       walkDirection += shape.Transform.Forward; 
                   }
                   if (window.GetKey(Keys.S))
                   {
                       walkDirection += Vector.Backward;
                   }
                   if (window.GetKey(Keys.A))
                   {
                       walkDirection += Vector.Left;
                   }
                   if (window.GetKey(Keys.D))
                   {
                       walkDirection += Vector.Right;
                   }
                   
                   if (window.GetKey(Keys.Q))
                   {
                       var rotation = shape.Transform.Rotation;
                       rotation.z += MathF.PI * fixedDeltaTime;
                       shape.Transform.Rotation = rotation;
                   }
                   if (window.GetKey(Keys.E))
                   {
                       var rotation = shape.Transform.Rotation;
                       rotation.z -= MathF.PI * fixedDeltaTime;
                       shape.Transform.Rotation = rotation;
                   }

                   walkDirection = walkDirection.Normalize();
                   shape.Transform.Position += walkDirection * movementSpeed * fixedDeltaTime;
               }
               window.Render();
           }
        }
    }
}
