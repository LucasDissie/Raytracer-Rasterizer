using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public class GameObject
    {
        Texture texture;
        public Material material;
        public virtual void Render(Shader shader, Matrix4 transform, Node camera, Matrix4 view, Texture texture)
        {

        }


        public GameObject(Material _material)
        {
            material = _material;
        }
    }
}
