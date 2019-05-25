#version 400
#feature(ModelColor)
#feature(DiffuseTexture)

// In / Out
in 	vec2 varying_vTexCoord;
out vec4 out_fColor;

// Entry point
void main()
{
	out_fColor = modelColor(diffuse(varying_vTexCoord));
}