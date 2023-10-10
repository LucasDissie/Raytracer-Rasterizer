using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    public class Node
    {
        public List<Node> children = new List<Node>();
        public Matrix4 rotateMatrix, translateMatrix, scaleMatrix, orientationMatrix;
        Matrix4 localTransform;
        public Matrix4 WorldMatrix = Matrix4.Identity;

        public GameObject connectedObject;

        public Node(GameObject obj, Matrix4 _rotateMatrix, Matrix4 _translateMatrix, Matrix4 _scaleMatrix )
        {
            connectedObject = obj;
            rotateMatrix = _rotateMatrix;
            translateMatrix = _translateMatrix;
            scaleMatrix = _scaleMatrix;
        }

        public void setupLight()
        {
            if (connectedObject.GetType().Name == "Light")
            {
                Light l = (Light)connectedObject;
                l.lightPos = WorldMatrix.ExtractTranslation();
            }
            else if (connectedObject.GetType().Name == "SpotLight")
            {
                SpotLight l = (SpotLight)connectedObject;
                l.lightPos = WorldMatrix.ExtractTranslation();
                l.direction = rotateMatrix.ExtractRotation().Xyz;
            }
            else if (connectedObject.GetType().Name == "DirLight")
            {
                DirLight l = (DirLight)connectedObject;
                l.direction = rotateMatrix.ExtractRotation().Xyz;
            }
        }

        public void AddChild(Node child, string name)
        {
            children.Add(child);

            GameValues.gameObjects.Add(name, child);
            if (child.connectedObject.GetType().Name == "Light")
            {
                GameValues.lights.Add((Light)child.connectedObject);
            }
            else if (child.connectedObject.GetType().Name == "SpotLight")
            {
                GameValues.spotLights.Add((SpotLight)child.connectedObject);
            }
            else if (child.connectedObject.GetType().Name == "DirLight")
            {
                GameValues.dirLights.Add((DirLight)child.connectedObject);
            }
        }
        public Matrix4 LocalTransform
        {
            get
            {
                if (translateMatrix * rotateMatrix * scaleMatrix != localTransform)
                {
                    if( connectedObject == null)
                    {
                        localTransform = translateMatrix * scaleMatrix * rotateMatrix;
                        return localTransform;
                    }
                    localTransform = scaleMatrix * rotateMatrix * translateMatrix;
                    return localTransform;
                }
                else
                {
                    return localTransform;
                }
            }
            set
            {
                localTransform = value;
            }
        }

    }
}
