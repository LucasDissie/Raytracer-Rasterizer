using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class Circle : Primitive
    {
        float r; 
        public override bool Intersects(Ray ray)
        {
            Vector2 c = this.pos - ray.Origin;
            if (c.Length < r)
            {
                return true;
            }
            float t = Vector2.Dot(c, ray.Direction);
            Vector2 q = c - (t * ray.Direction);
            float p = Vector2.Dot(q, q);
            if (p > (r * r))
            {
                return false;
            }
            t -= (float)Math.Sqrt(r * r - p);
            if ((t < ray.t) && t > 0)
            {
                return true;
            }
            return false;
        }

        public Circle(float _r, Vector2 _pos)
        {
            r = _r;
            pos = _pos;
        }

    }
}
