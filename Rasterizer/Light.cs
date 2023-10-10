using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public class Light : GameObject
    {
        public Vector3 lightColor;
        public Vector3 lightPos;
        public float attenuation;


        public Light(Vector3 color, float _attenuation, Vector3 position = new Vector3()) : base(null)
        {
            lightColor = color;
            lightPos = position;
            attenuation = _attenuation;
        }

    }

    public class DirLight : Light
    {
        public Vector3 direction;
        public DirLight(Vector3 color, Vector3 direction, float _attenuation) : base(color, _attenuation)
        {
            this.direction = direction;
        }
    }

    public class SpotLight : Light
    {
        public Vector3 direction;
        public float angle; //In radians
        public SpotLight(Vector3 color, Vector3 direction, float angle, float _attenuation) : base(color, _attenuation)
        {
            this.direction = direction;
            this.angle = angle;
        }
    }
}
