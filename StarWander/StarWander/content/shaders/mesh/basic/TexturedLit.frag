#version 400
#feature(ModelColor)
#feature(DiffuseTexture)

// In / Out
in vec3 varying_vNorm;
in vec2 varying_vTexCoord;
out vec4 out_fColor;

vec4 lightIntensity(vec3 norm)
{
	return vec4(vec3((1 + dot(norm, vec3(0, 0, 1))) / 2), 1);
}

// Entry point
void main()
{
	out_fColor = modelColor() * diffuse(varying_vTexCoord) * lightIntensity(varying_vNorm);
	if (out_fColor.a < 0.0001)
	{
		discard;
	}
}