#version 400
#feature(ModelColor)

// In / Out
out vec4 out_fColor;

// Entry point
void main()
{
	out_fColor = modelColor();
}