using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Template
{
    class Ray
    {
        public Vector2 Origin;
        public Vector2 Direction;
        public float t;

        public void Intersects(Primitive p)
        {
            p.Intersects(this);
        }
    }
}
