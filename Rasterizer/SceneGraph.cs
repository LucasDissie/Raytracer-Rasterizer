using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    class SceneGraph
    {
        Shader shader;
        public Node world;

        public SceneGraph(Matrix4 _camera, Matrix4 _perspective, Shader _shader)
        {
            shader = _shader;
        }


        /// <summary>
        /// Render(camera, SceneGraph.modelView);
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="nextmesh"></param>
        public void Render(Node node, Matrix4 concatedMatrix, Node camera, Matrix4 view)
        {
            concatedMatrix = node.LocalTransform * concatedMatrix;
            node.WorldMatrix = concatedMatrix;
            if(node.connectedObject.material != null)
            {
                node.connectedObject.material.setupShader(shader);
                node.connectedObject.Render(shader, concatedMatrix, camera, view, node.connectedObject.material.texture);
            }
            else
            {
                node.setupLight();
            }
            foreach (Node n in node.children)
            {
                Render(n, concatedMatrix, camera, view);
            }
        }

        public void setUpLighting()
        {

            int AmountOfPointLightsID = GL.GetUniformLocation(shader.programID, "AmountOfPointLights");
            GL.Uniform1(AmountOfPointLightsID, GameValues.lights.Count);
            int AmountOfSpotlightsID = GL.GetUniformLocation(shader.programID, "AmountOfSpotlights");
            GL.Uniform1(AmountOfSpotlightsID, GameValues.spotLights.Count);
            int AmountOfDirectionalLightsID = GL.GetUniformLocation(shader.programID, "AmountOfDirectionalLights");
            GL.Uniform1(AmountOfDirectionalLightsID, GameValues.dirLights.Count);

            for (int i = 0; i < GameValues.lights.Count; i++)
            {
                int positionID = GL.GetUniformLocation(shader.programID, "pointLights[" + i + "].position");
                GL.Uniform3(positionID, GameValues.lights[i].lightPos);
                int attenuationID = GL.GetUniformLocation(shader.programID, "pointLights[" + i + "].attenuation");
                GL.Uniform1(attenuationID, GameValues.lights[i].attenuation);
                int colorID = GL.GetUniformLocation(shader.programID, "pointLights[" + i + "].color");
                GL.Uniform3(colorID, GameValues.lights[i].lightColor);
            }
            for (int i = 0; i < GameValues.spotLights.Count; i++)
            {
                int directionID = GL.GetUniformLocation(shader.programID, "spotlights[" + i + "].direction");
                GL.Uniform3(directionID, GameValues.spotLights[i].direction);
                int positionID = GL.GetUniformLocation(shader.programID, "spotlights[" + i + "].position");
                GL.Uniform3(positionID, GameValues.spotLights[i].lightPos);
                int attenuationID = GL.GetUniformLocation(shader.programID, "spotlights[" + i + "].attenuation");
                GL.Uniform1(attenuationID, GameValues.spotLights[i].attenuation);
                int colorID = GL.GetUniformLocation(shader.programID, "spotlights[" + i + "].color");
                GL.Uniform3(colorID, GameValues.spotLights[i].lightColor);
                int angleID = GL.GetUniformLocation(shader.programID, "spotlights[" + i + "].angle");
                GL.Uniform1(colorID, GameValues.spotLights[i].angle);
            }
            for (int i = 0; i < GameValues.dirLights.Count; i++)
            {
                int directionID = GL.GetUniformLocation(shader.programID, "directionalLights[" + i + "].direction");
                GL.Uniform3(directionID, GameValues.dirLights[i].direction);
                int attenuationID = GL.GetUniformLocation(shader.programID, "directionalLights[" + i + "].attenuation");
                GL.Uniform1(attenuationID, GameValues.dirLights[i].attenuation);
                int colorID = GL.GetUniformLocation(shader.programID, "directionalLights[" + i + "].color");
                GL.Uniform3(colorID, GameValues.dirLights[i].lightColor);
            }
        }
    }
}
