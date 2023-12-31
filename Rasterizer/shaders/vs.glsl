﻿#version 330
 
// shader input
in vec2 vUV;				// vertex uv coordinate
in vec3 vNormal;			// untransformed vertex normal
in vec3 vPosition;			// untransformed vertex position

// shader output
out vec4 normal;			// transformed vertex normal
out vec2 uv;	
out vec3 worldPos;
uniform mat4 transform;
uniform mat4 camera;
 
// vertex shader
void main()
{
	// transform vertex using supplied matrix
	gl_Position = camera * transform * vec4(vPosition, 1.0);

	// forward normal and uv coordinate; will be interpolated over triangle
	normal = mat4(transpose(inverse(transform))) * vec4( vNormal, 0.0f );
	uv = vUV;

	worldPos = (transform * vec4( vPosition, 1.0 )).xyz;
}