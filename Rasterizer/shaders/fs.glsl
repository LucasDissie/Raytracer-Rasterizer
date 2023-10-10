#version 330
 

 struct pointLight{
	vec3 position;
	float attenuation;
	vec3 color;
};

struct directionalLight{
	vec3 direction;
	float attenuation;
	vec3 color;
};
 
struct spotlight {
	vec3 direction;
	vec3 position;
	float attenuation;
	vec3 color;
	float angle;
};
// shader input

in vec2 uv;						// interpolated texture coordinates
in vec4 normal;					// interpolated normal
in vec3 worldPos;

uniform sampler2D pixels;		// texture sampler
uniform pointLight pointLights[20];
uniform directionalLight directionalLights[20];
uniform spotlight spotlights[40];
uniform int AmountOfPointLights;
uniform int AmountOfDirectionalLights;
uniform int AmountOfSpotlights;
uniform vec3 viewPos;
vec3 viewVector;
vec3 normalVector;
vec3 textureColor;




void PhongShading(in vec3 lightDirection, in vec3 normal, in float strength, in vec3 lightColor, inout vec3 color);

// shader output
layout(location = 0) out vec4 outputColor;
layout(location = 1) out vec4 brightColor;

// fragment shader
void main()
{

	viewVector = normalize(worldPos -vec3(-viewPos.x, viewPos.y, -viewPos.z));
	viewVector.y *= -1;
    textureColor = texture( pixels, vec2( uv.x, -uv.y) ).xyz;

	normalVector = normalize(normal.xyz);
	vec3 color = textureColor * 0.15;


	for(int i = 0; i < AmountOfDirectionalLights; ++i) {
			PhongShading(normalize(directionalLights[i].direction), normalVector, directionalLights[i].attenuation, directionalLights[i].color, color);
	}

	for(int i = 0; i < AmountOfPointLights; ++i) {
			vec3 lightDirection = pointLights[i].position - worldPos;
			float attenuation = pointLights[i].attenuation / (length(lightDirection) * length(lightDirection));
			lightDirection = normalize(lightDirection);
			PhongShading(lightDirection, normalVector, attenuation, pointLights[i].color, color);
	}

	for(int i = 0; i < AmountOfSpotlights; ++i) {
		vec3 lightDirection = spotlights[i].position - worldPos;
		if(dot(normalize(lightDirection), -spotlights[i].direction) > spotlights[i].angle) {
			PhongShading(normalize(lightDirection), normalVector, spotlights[i].attenuation / (length(lightDirection) * length(lightDirection)), spotlights[i].color, color);
		}
	}
	outputColor = vec4(color, 1);

}

void PhongShading(in vec3 lightDirection, in vec3 normal, in float strength, in vec3 lightColor, inout vec3 color) {
	float diffuse = max(dot(normalVector, lightDirection), 0);
	vec3 reflectDirection = reflect(viewVector, normalVector);
	float specular = 0;
	if(dot(lightDirection, reflectDirection) > 0){
		specular = pow(max(dot(lightDirection, reflectDirection), 0), 32);
	}
	color += (textureColor * (diffuse + specular) * lightColor) * strength;
}
