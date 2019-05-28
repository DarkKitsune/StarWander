#version 400
#feature(Camera)
#feature(ModelMatrix)

// In / Out
in vec3 in_vPos;
in vec3 in_vNorm;
out vec3 varying_vNorm;

// Entry point
void main()
{
	varying_vNorm = in_vNorm;
	gl_Position =  projection(view(model(vec4(in_vPos, 1))));
}