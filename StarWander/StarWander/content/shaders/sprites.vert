#version 400
#feature(Camera)
#feature(TextureSource)
#feature(ModelMatrix)

uniform vec2 uniform_origin;

// In / Out
out vec2 varying_vTexCoord;

// Constants
const vec2 positions[] = vec2[](
		vec2(0, 0), vec2(1, 0), vec2(0, -1), vec2(1, -1)
	);
const vec2 texCoords[] = vec2[](
		vec2(0, 0), vec2(1, 0), vec2(0, 1), vec2(1, 1)
	);

// Entry point
void main()
{
	// Select a info based on the vertex ID
	varying_vTexCoord = textureSourceCoords(texCoords[gl_VertexID]);

	// Screen-space position is just the vertex position
	gl_Position =  projection(view(model(vec4(positions[gl_VertexID] - vec2(uniform_origin.x, -uniform_origin.y), 0, 1))));
}