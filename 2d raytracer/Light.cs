using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Template
{
    class Light
    {
        public Vector2 lightPos;
        public Vector3 color;

        public Light(Vector3 _color, Vector2 _pos)
        {
            lightPos = _pos;
            color = _color;
        }
        public virtual Vector2 normalizedDirectionToLight(Vector2 origin)
        {
            Vector2 normalizedDirection = lightPos - origin;
            normalizedDirection = normalizedDirection.Normalized();
            return normalizedDirection;
        }
        public virtual float distanceToLight(Vector2 origin)
        {
            Vector2 difference = lightPos - origin;
            return difference.Length;
        }

        public virtual float lightAttenuation(Ray r)
        {
            return (float)(1 / (r.t * r.t * 4 * Math.PI));
        }
    }
}
