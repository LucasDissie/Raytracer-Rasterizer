using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class AreaLight : Light
    {
        public float radius;
        Random rng = new Random();
        Vector2 posInLight;

        public AreaLight(Vector3 _color, Vector2 _pos, float _radius) : base(_color, _pos)
        {
            radius = _radius;
        }

        public override Vector2 normalizedDirectionToLight(Vector2 origin)
        {
            posInLight = RandomVector();
            Vector2 normalizedDirection = posInLight - origin;
            normalizedDirection = normalizedDirection.Normalized();
            return normalizedDirection;
        }

        public override float distanceToLight(Vector2 origin)
        {
            
            Vector2 difference = posInLight - origin;
            return difference.Length;
        }
        Vector2 RandomVector()
        {
            float r = radius * (float)rng.NextDouble();
            float angle = 2 * (float)(Math.PI * rng.NextDouble());
            Vector2 randVector = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            return lightPos + (r * randVector);
        }

        public override float lightAttenuation(Ray r)
        {
            if((r.Origin - lightPos).Length < radius)
            {
                return 1;
            }
            return (float)(1 / (r.t * r.t * 4* Math.PI));
        }
    }
}
