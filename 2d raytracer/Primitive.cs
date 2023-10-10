using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class Primitive
    {
        protected Vector2 pos;
        public virtual bool Intersects(Ray ray)
        {
            return true;
        }

    }
}
