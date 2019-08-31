#version 400
#feature(ModelColor)

// In / Out
in vec3 varying_vNorm;
out vec4 out_fColor;

// Entry point
void main()
{
	out_fColor = modelColor();
}