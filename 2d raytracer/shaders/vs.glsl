#version 330
in vec3 vPosition;
in  vec3 vColor;
out vec4 color;
uniform mat4 M;
void main()
{
	gl_Position = M* vec4(vPosition.x, vPosition.y, -vPosition.z , 1.0);
	if(vColor.z > 0)
	{
		color = vec4( vColor.z + 0.3 , vColor.z + 0.3, vColor.z + 0.3, 1.0);
	}
	else
	{
		color = vec4( 0 , 0, 0, 1.0);
	}
}