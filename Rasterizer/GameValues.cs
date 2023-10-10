using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public static class GameValues
    {
        public static Dictionary<string, Node> gameObjects = new Dictionary<string, Node>();
        public static List<Light> lights = new List<Light>();
        public static List<SpotLight> spotLights = new List<SpotLight>();
        public static List<DirLight> dirLights = new List<DirLight>();
    }
}
