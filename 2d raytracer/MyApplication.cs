namespace Template
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Collections.Generic;
    using OpenTK;
    using OpenTK.Graphics.OpenGL;
    using System.Threading;

    class MyApplication
	{
        List<Light> lights = new List<Light>();
        List<Primitive> primitives = new List<Primitive>();
        int finishedthreadCount = 0;
        Stopwatch watch = new Stopwatch();
        Surface texture = new Surface("../../assets/wood.png");
        Vector3[,] textureColor = new Vector3[640, 400];


        // member variables
        public Surface screen;

        int threads = 4; 
        float antiAliasingGrade = 8;
        // initialize
        public void Init()
		{
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    textureColor[x, y] = ToVectorColor(texture.pixels[x + y * texture.width]);
                }
            }
            watch.Start();
            lights.Add(new Light(new Vector3(2f, 0.4f, 0.4f), new Vector2(0, 0)));
            lights.Add(new Light(new Vector3(0.4f, 0.4f, 2), new Vector2(-0.5f, 0.5f)));
            lights.Add(new Light(new Vector3(2f, 0.4f, 0.4f), new Vector2(-0.75f, -0.5f)));
            lights.Add(new AreaLight(new Vector3(3f, 4f, 1f), new Vector2(0.75f, -0.75f), 0.2f));

            primitives.Add(new Circle(0.05f, new Vector2(0, 0.3f)));
            primitives.Add(new Circle(0.1f, new Vector2(0.8f, 0.8f)));
            primitives.Add(new Circle(0.05f, new Vector2(-0.3f, 0.4f)));
            primitives.Add(new Circle(0.1f, new Vector2(0.5f, -0.5f)));


            Thread[] pixelThreads = new Thread[threads];
            for (int i = 0; i < threads; i++)
            {
                int tmp = i;
                pixelThreads[tmp] = new Thread(() => Raytracer(tmp));
                pixelThreads[tmp].Start();
            }
        }

        void Raytracer(int beginValue)
        {
            for (int x = beginValue; x < screen.width; x += threads)
            {
                for (int y = 0; y < screen.height; y++)
                {
                    Vector3 averagePixelcolor = Vector3.Zero;
                    for (float i = (-0.5f + 1 / (antiAliasingGrade + 1)); i < 0.5f; i += 1 / (antiAliasingGrade + 1))
                    {
                        for (float j = (-0.5f + 1 / (antiAliasingGrade + 1)); j < 0.5f; j += 1 / (antiAliasingGrade + 1))
                        {
                            int location = x + y * screen.width;
                            Vector3 pixelColor = Vector3.Zero;
                            screen.pixels[location] = 0;
                            foreach (Light l in lights)
                            {
                                Ray ray = new Ray();
                                ray.Origin = pixelPosition(invTX(x + i), invTY(y + j));
                                ray.Direction = l.normalizedDirectionToLight(ray.Origin);
                                ray.t = l.distanceToLight(ray.Origin);
                                bool occluded = false;
                                foreach (Primitive p in primitives)
                                {
                                    if (p.Intersects(ray))
                                    {
                                        occluded = true;
                                    }
                                }
                                if (!occluded)
                                {
                                    pixelColor += l.color * l.lightAttenuation(ray);
                                }
                            }
                            averagePixelcolor += pixelColor;
                        }
                    }
                    averagePixelcolor /= (antiAliasingGrade * antiAliasingGrade);

                    averagePixelcolor *= textureColor[x, y];
                    screen.Plot(x, y, ToRGB32(averagePixelcolor));
                }
            }
            finishedthreadCount++;
            if (finishedthreadCount == threads)
            {
                watch.Stop();
                Console.WriteLine(watch.ElapsedMilliseconds);
            }
        }

        
        int ToRGB32(Vector3 color)
        {
            int red = (int)(Clamp(0,1,color.X) * 255);
            int green = (int)(Clamp(0, 1, color.Y) * 255);
            int blue = (int)(Clamp(0, 1, color.Z) * 255);

            return (red << 16) + (green << 8) + blue;
        }

        Vector3 ToVectorColor(int color)
        {
            float B = -(float)(color % 256) / 255;
            float G = -(float)(((color - B) / 256) % 256) / 255;
            float R = -(float)(((color - B) / (256 * 256)) - G / 256) / 255;
            return new Vector3(R, G, B);
        }

        float Clamp(float min, float max, float input)
        {
            if(input > max)
            {
                return max;
            }
            else if(input < min)
            {
                return min;
            }
            else
            {
                return input;
            }
        }


        Vector2 pixelPosition(float x, float y)
        {
            return new Vector2(x, y);
        }

        float invTX(float x)
        {
            x -= screen.width/2;
            float newX = (float)x / 200;
            return newX;

        }

        float invTY(float y)
        {
            y -= screen.height / 2;
            float newY = (float)y / -200;
            return newY;
        }
    }

}