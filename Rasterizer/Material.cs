using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace Template
{
    public class Material
    {
        Vector3 specularColor;
        int glossiness;
        public Texture texture;

        public Material(int _glossiness, Vector3 _specularColor, Texture _texture)
        {
            glossiness = _glossiness;
            specularColor = _specularColor;
            texture = _texture;
        }

        public void setupShader(Shader shader)
        {
            int matSpecID = GL.GetUniformLocation(shader.programID, "matSpecColor");
            GL.UseProgram(shader.programID);
            GL.Uniform3(matSpecID, specularColor);
            int glossID = GL.GetUniformLocation(shader.programID, "glossiness");
            GL.UseProgram(shader.programID);
            GL.Uniform1(glossID, glossiness);
        }
    }
}
