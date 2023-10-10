using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template
{
	class MyApplication
	{
		// member variables
		public Surface screen;                  // background surface for printing etc.
		Mesh floor, charmandolphin, kart, backWheels, leftFrontwheel, rightFrontwheel, clouds, environment;
        Mesh[] track = new Mesh[9];  // a mesh to draw using OpenGL
		const float PI = 3.1415926535f;         // PI
		float a = 0;                            // teapot rotation angle
		Stopwatch timer;                        // timer for measuring frame duration
		public Shader shader;                          // shader to use for rendering
		Shader postproc;                        // shader to use for post processing
        public Texture wood, charm, kartTexture, wheelTexture, cloudsTexture, environmentTexture;
        Texture[] trackTextures = new Texture[9];                           // texture to use for rendering
		RenderTarget target;                    // intermediate render target
		ScreenQuad quad;                        // screen filling quad for post processing
        SceneGraph scene;
        Node camera;
        Matrix4 view = Matrix4.CreatePerspectiveFieldOfView(1.2f, 1.3f, .1f, 1000);
        bool useRenderTarget = true;
        int lastWheelValue;
        float theta = PI /30;
        OpenTK.Input.KeyboardState prevKeyState;

        // initialize
        public void Init()
		{
            camera = new Node(null, Matrix4.CreateFromAxisAngle(new Vector3(1, 0, 0), -PI / 2).Inverted(), Matrix4.CreateTranslation(0, -50, 0), Matrix4.Identity);
            lastWheelValue = OpenTK.Input.Mouse.GetState().ScrollWheelValue;
            shader = new Shader("../../shaders/vs.glsl", "../../shaders/fs.glsl");
            wood = new Texture("../../assets/wood.jpg");
            charm = new Texture("../../assets/Charmandolphin_body.png");
            kartTexture = new Texture("../../assets/Kart.png");
            wheelTexture = new Texture("../../assets/Tire.png");
            cloudsTexture = new Texture("../../assets/Clouds.png");
            environmentTexture = new Texture("../../assets/Environment.png");

            for (int i = 0; i < trackTextures.Length; i++)
            {
                trackTextures[i] = new Texture("../../assets/trackpart" + (i+1) + "texture.png");
            }

            scene = new SceneGraph(camera.LocalTransform, view, shader);
            // load teapot
            kart = new Mesh("../../assets/Standard Kart.obj", new Material(5, new Vector3(1.68f, 1.8f, 1.81f ), kartTexture));
            charmandolphin = new Mesh("../../assets/Pokemon.obj", new Material(int.MaxValue, new Vector3(0f, 1.39f, 2.55f), charm));
            floor = new Mesh( "../../assets/floor.obj", new Material(20, new Vector3(0.8f, 0.2f, 0.5f), wood));
            backWheels = new Mesh("../../assets/BackWheels.obj", new Material(int.MaxValue, new Vector3(0f, 0f, 0f), wheelTexture));
            rightFrontwheel = new Mesh("../../assets/rightFrontWheel.obj", new Material(int.MaxValue, new Vector3(0.0f, 0.0f, 0.0f), wheelTexture));
            leftFrontwheel = new Mesh("../../assets/leftFrontWheel.obj", new Material(int.MaxValue, new Vector3(0.0f, 0.0f, 0.0f), wheelTexture));
            environment = new Mesh("../../assets/Environment.obj", new Material(int.MaxValue, new Vector3(0, 0, 0), environmentTexture));
            clouds = new Mesh("../../assets/Clouds.obj", new Material(int.MaxValue, new Vector3(0, 0, 0), cloudsTexture));


            scene.world = new Node(floor, Matrix4.Identity, Matrix4.Identity, Matrix4.Identity);
            Node KartNode = new Node(kart, Matrix4.Identity, Matrix4.Identity, Matrix4.Identity);
            KartNode.scaleMatrix = Matrix4.CreateScale(0.1f);
            KartNode.translateMatrix = Matrix4.CreateTranslation(0, 10, 0);
            KartNode.rotateMatrix = Matrix4.CreateRotationY(PI);
            scene.world.AddChild(KartNode, nameof(KartNode));

            scene.world.children[0].AddChild(new Node(charmandolphin, Matrix4.Identity, Matrix4.Identity, Matrix4.Identity),"charmandolphin");
            scene.world.children[0].AddChild(new Node(backWheels, Matrix4.Identity, Matrix4.CreateTranslation(0,0,-28), Matrix4.Identity),"backWheels");
            scene.world.children[0].AddChild(new Node(rightFrontwheel, Matrix4.Identity, Matrix4.CreateTranslation(0, 0, 19), Matrix4.Identity),"rightFrontwheel");
            scene.world.children[0].AddChild(new Node(leftFrontwheel, Matrix4.Identity, Matrix4.CreateTranslation(0,0,19), Matrix4.Identity), "leftFrontwheel");
            //scene.world.children[0].AddChild(new Node(new SpotLight(new Vector3(500, 500, 500), new Vector3(0, 10, 0), 10, 200f), Matrix4.CreateRotationZ(PI/4), Matrix4.CreateTranslation(0, 20, 0), Matrix4.Identity), "RightSpotlight");

            //scene.world.children[0].AddChild(new Node(new SpotLight(new Vector3(50, 50, 50), new Vector3(1, 0, 0), 45), Matrix4.CreateRotationX(PI/2), Matrix4.CreateTranslation(-30,0,50), Matrix4.Identity), "LeftSpotlight");
            //scene.world.children[0].AddChild(new Node(new SpotLight(new Vector3(50, 50, 50), new Vector3(1, 0, 0), 45), Matrix4.Identity, Matrix4.Identity, Matrix4.Identity), "RightSpotlight");




            for (int i = 0; i < track.Length; i++)
            {
                track[i] = new Mesh("../../assets/trackpart" + (i + 1) + ".obj", new Material(50, new Vector3(1, 1, 1), trackTextures[i]));
                scene.world.children.Add(new Node(track[i], Matrix4.Identity, Matrix4.Identity, Matrix4.Identity));
            }
            scene.world.AddChild(new Node(environment, Matrix4.Identity, Matrix4.Identity, Matrix4.Identity), "environment");
            scene.world.AddChild(new Node(clouds, Matrix4.Identity, Matrix4.Identity, Matrix4.Identity), "clouds");



            scene.world.AddChild(new Node(new Light(new Vector3(60, 60, 45), 20, new Vector3(1, 10, 2)), Matrix4.Identity, Matrix4.CreateTranslation(4, 0, 0), Matrix4.Identity), "light1");
            scene.world.AddChild(new Node(new Light(new Vector3(80, 40, 60), 20, new Vector3(5, 3, 0)), Matrix4.Identity, Matrix4.CreateTranslation(50, 10, 0), Matrix4.Identity), "light2");
            scene.world.AddChild(new Node(new Light(new Vector3(40, 60, 10), 20, new Vector3(0, 0, 5)), Matrix4.Identity, Matrix4.Identity, Matrix4.Identity), "light3");
            scene.world.AddChild(new Node(new Light(new Vector3(3, 1, 2), 20, new Vector3(2, 0, 2)), Matrix4.Identity, Matrix4.Identity, Matrix4.Identity), "light4");
            scene.world.AddChild(new Node(new DirLight(new Vector3(0.1f, 0.1f, 0.1f), new Vector3(0, 1f, 0), 2f), Matrix4.Identity, Matrix4.Identity, Matrix4.Identity), "sun");

            // initialize stopwatch
            timer = new Stopwatch();
			timer.Reset();
			timer.Start();
			// create shaders
			postproc = new Shader( "../../shaders/vs_post.glsl", "../../shaders/fs_post.glsl" );
			// load a texture
			// create the render target
			target = new RenderTarget( screen.width, screen.height );
			quad = new ScreenQuad();

            
            int ambientID = GL.GetUniformLocation(shader.programID, "ambientColor");
            GL.UseProgram(shader.programID);
            GL.Uniform3(ambientID, 0.5f, 0.5f, 0.5f);
        }
        
        public void HandleInput(OpenTK.Input.KeyboardState keyboardState, OpenTK.Input.MouseState mouseState)
        {
            Node Backwheels, leftFrontWheel, rightFrontWheel;

            if (keyboardState.IsKeyDown(OpenTK.Input.Key.Q))
            {
                camera.scaleMatrix = Matrix4.CreateRotationY(-theta) * camera.scaleMatrix;
                scene.world.children[0].rotateMatrix = scene.world.children[0].rotateMatrix * Matrix4.CreateRotationY(theta);
                GameValues.gameObjects.TryGetValue("leftFrontwheel", out leftFrontWheel);
                leftFrontWheel.rotateMatrix =  Matrix4.CreateRotationY(theta);
                GameValues.gameObjects.TryGetValue("rightFrontwheel", out rightFrontWheel);
                rightFrontWheel.rotateMatrix = Matrix4.CreateRotationY(theta);
            }
            else if (keyboardState.IsKeyDown(OpenTK.Input.Key.E))
            {
                camera.scaleMatrix = Matrix4.CreateRotationY(theta) * camera.scaleMatrix;
                scene.world.children[0].rotateMatrix = scene.world.children[0].rotateMatrix * Matrix4.CreateRotationY(-theta);
                
                GameValues.gameObjects.TryGetValue("leftFrontwheel", out leftFrontWheel);
                leftFrontWheel.rotateMatrix = Matrix4.CreateRotationY(-theta);
                GameValues.gameObjects.TryGetValue("rightFrontwheel", out rightFrontWheel);
                rightFrontWheel.rotateMatrix = Matrix4.CreateRotationY(-theta);
            }
            else
            {
                GameValues.gameObjects.TryGetValue("backWheels", out Backwheels);
                GameValues.gameObjects.TryGetValue("rightFrontwheel", out rightFrontWheel);
                GameValues.gameObjects.TryGetValue("leftFrontwheel", out leftFrontWheel);
                if(prevKeyState.IsKeyDown(OpenTK.Input.Key.E) || prevKeyState.IsKeyDown(OpenTK.Input.Key.Q))
                {
                    leftFrontWheel.rotateMatrix = Matrix4.Identity;
                    rightFrontWheel.rotateMatrix = Matrix4.Identity;
                }

                if (keyboardState.IsKeyDown(OpenTK.Input.Key.W))
                {
                    leftFrontWheel.rotateMatrix = leftFrontWheel.rotateMatrix * Matrix4.CreateRotationX(theta);
                    rightFrontWheel.rotateMatrix = rightFrontWheel.rotateMatrix * Matrix4.CreateRotationX(theta);
                }
                else if (keyboardState.IsKeyDown(OpenTK.Input.Key.S))
                {
                    scene.world.children[0].translateMatrix = scene.world.children[0].translateMatrix * (Matrix4.CreateTranslation(0, 0, -1) * scene.world.children[0].rotateMatrix).ClearRotation();
                }
                else if (keyboardState.IsKeyDown(OpenTK.Input.Key.A))
                {
                    scene.world.children[0].translateMatrix = scene.world.children[0].translateMatrix * (Matrix4.CreateTranslation(1, 0, 0) * scene.world.children[0].rotateMatrix).ClearRotation();
                }
                else if (keyboardState.IsKeyDown(OpenTK.Input.Key.D))
                {
                    scene.world.children[0].translateMatrix = scene.world.children[0].translateMatrix * (Matrix4.CreateTranslation(-1, 0, 0) * scene.world.children[0].rotateMatrix).ClearRotation();
                }
            }

            if (keyboardState.IsKeyDown(OpenTK.Input.Key.W))
            {
                scene.world.children[0].translateMatrix = scene.world.children[0].translateMatrix * (Matrix4.CreateTranslation(0, 0, 1) * scene.world.children[0].rotateMatrix).ClearRotation();
                GameValues.gameObjects.TryGetValue("backWheels", out Backwheels);
                Backwheels.rotateMatrix = Backwheels.rotateMatrix * Matrix4.CreateRotationX(theta);
            }
            else if (keyboardState.IsKeyDown(OpenTK.Input.Key.S))
            {
                scene.world.children[0].translateMatrix = scene.world.children[0].translateMatrix * (Matrix4.CreateTranslation(0, 0, -1) * scene.world.children[0].rotateMatrix).ClearRotation();
            }
            else if (keyboardState.IsKeyDown(OpenTK.Input.Key.A))
            {
                scene.world.children[0].translateMatrix = scene.world.children[0].translateMatrix * (Matrix4.CreateTranslation(1, 0, 0) * scene.world.children[0].rotateMatrix).ClearRotation();
            }
            else if (keyboardState.IsKeyDown(OpenTK.Input.Key.D))
            {
                scene.world.children[0].translateMatrix = scene.world.children[0].translateMatrix * (Matrix4.CreateTranslation(-1, 0, 0) * scene.world.children[0].rotateMatrix).ClearRotation();
            }

            if (mouseState.ScrollWheelValue == lastWheelValue)
            {
            }
            else if(mouseState.ScrollWheelValue < lastWheelValue)
            {
                lastWheelValue = mouseState.ScrollWheelValue;
                camera.translateMatrix = Matrix4.CreateTranslation(0, -1, 0) * camera.translateMatrix;
            }
            else
            {
                lastWheelValue = mouseState.ScrollWheelValue;
                camera.translateMatrix = Matrix4.CreateTranslation(0, 1, 0) * camera.translateMatrix;
            }
            if (keyboardState.IsKeyDown(OpenTK.Input.Key.G))
            {
                Node light; 
                GameValues.gameObjects.TryGetValue("RightSpotlight", out light);
                light.rotateMatrix = light.rotateMatrix * Matrix4.CreateRotationY(theta);
                Console.WriteLine(light.rotateMatrix);
                Console.WriteLine();
            }
            if (keyboardState.IsKeyDown(OpenTK.Input.Key.H))
            {
                Node light;
                GameValues.gameObjects.TryGetValue("RightSpotlight", out light);
                light.rotateMatrix = light.rotateMatrix * Matrix4.CreateRotationZ(theta);
                Console.WriteLine(light.rotateMatrix);
                Console.WriteLine();
            }
            camera.translateMatrix = Matrix4.CreateTranslation(-scene.world.children[0].translateMatrix.M41, camera.translateMatrix.M42, -scene.world.children[0].translateMatrix.M43);
            prevKeyState = keyboardState;
        }

        // tick for background surface
        public void Tick()
		{
			screen.Clear( 0 );
		}

		// tick for OpenGL rendering code
		public void RenderGL()
		{
			// measure frame duration
			float frameDuration = timer.ElapsedMilliseconds;
			timer.Reset();
			timer.Start();

			// prepare matrix for vertex shader
			//Matrix4 Tpot = Matrix4.CreateScale( 0.5f ) * Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), a );
			//Matrix4 Tfloor = Matrix4.CreateScale( 4.0f ) * Matrix4.CreateFromAxisAngle( new Vector3( 0, 1, 0 ), a );
            //Matrix4 toWorld = Tpot;
            //scene.world.children[0].toWorld = toWorld;
            //scene.world.localTransform = Tfloor;
            //scene.world.children[0].localTransform = Tpot;

            // update rotation
            //a += 0.001f * frameDuration;
			//if( a > 2 * PI ) a -= 2 * PI;

			if( useRenderTarget )
			{
                // enable render target
                // render scene to render target
                scene.setUpLighting();
                scene.Render(scene.world, Matrix4.Identity, camera, view);

				// render quad
			}
			else
			{
                scene.setUpLighting();

                // render scene directly to the screen
                scene.Render(scene.world, Matrix4.Identity, camera, view);
            }
		}
	}
}