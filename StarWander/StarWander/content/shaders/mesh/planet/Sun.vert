#version 400
#feature(Camera)
#feature(ModelMatrix)

// In / Out
in vec3 in_vPos;
in vec3 in_vNorm;
in vec2 in_vTexCoord;
out vec3 varying_vPos;
out vec3 varying_vNorm;
out vec3 varying_vDirectionFromCam;

// Entry point
void main()
{
	// Vertex position
	vec4 modelPos = model(vec4(in_vPos, 1));
	gl_Position =  projection(view(modelPos));
	varying_vPos = modelPos.xyz;

	// Vertex normal
	mat3 normalMatrix = transpose(inverse(mat3(model())));
	varying_vNorm = normalize(normalMatrix * in_vNorm);

	// Direction from cam
	varying_vDirectionFromCam = normalize(modelPos.xyz - eye());
}