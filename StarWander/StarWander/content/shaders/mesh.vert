#version 400
#feature(Camera)
#feature(ModelMatrix)

// In / Out
in vec3 in_vPos;

// Entry point
void main()
{
	gl_Position =  projection(view(model(vec4(in_vPos, 1))));
}