#version 400
#feature(ModelColor)

uniform int Seed;
uniform float CloudThickness;
uniform float Radius;

// In / Out
in vec3 varying_vPos;
in vec3 varying_vNorm;
in vec3 varying_vDirectionFromCam;
out vec4 out_fColor;

// Calculate light intensity
vec4 lightIntensity(vec3 norm)
{
	return vec4(vec3((1 + dot(norm, vec3(0, 0, 1))) / 2), 1);
}

// Noise
const float NOISE_WORLDSCALE = 1.0;
const float NOISE_SCALEMULTIPLIER = 2;
const int NOISE_LEVELS = 4;

float noise_applyCurve(float n)
{
    float fromHalf = (n - 0.5f) * 2f;
    float curved = pow(abs(fromHalf), 1f / sqrt(sqrt(float(NOISE_LEVELS)))) * sign(fromHalf);
    return 0.5f + curved / 2f;
}

uint noise_nextUInt(int seedAdd)
{
    int seed = (214013 * (Seed ^ seedAdd)) + 2531011;
    return uint(seed);
}

float noise_nextFloat(int seedAdd)
{
    return float(noise_nextUInt(seedAdd)) / 4294967295.0;
}

float noise_sampleAtInt(int X, int Y, int Z)
{
    return noise_nextFloat(X * 214013 ^ Y * 514013 ^ Z * 852937);
}

float noise_sampleAtIntIteration(int iteration, int X, int Y, int Z)
{
    return noise_nextFloat(X * 214013 ^ Y * 514013 ^ Z * 852937 ^ iteration * 2531011);
}

ivec3 noise_positionMin(vec3 position)
{
	return ivec3(floor(position));
}

ivec3 noise_positionMax(vec3 position)
{
	return ivec3(ceil(position));
}

vec3 noise_positionRatio(vec3 position)
{
	return position - floor(position);
}

float noise_iterationSample(vec3 position, int iteration)
{
	ivec3 min = noise_positionMin(position);
	ivec3 max = noise_positionMax(position);
	vec3 ratio = noise_positionRatio(position);

    float up = mix(
            noise_sampleAtIntIteration(iteration, min.x, min.y, min.z),
            noise_sampleAtIntIteration(iteration, max.x, min.y, min.z),
            ratio.x
        );
    float down = mix(
            noise_sampleAtIntIteration(iteration, min.x, max.y, min.z),
            noise_sampleAtIntIteration(iteration, max.x, max.y, min.z),
            ratio.x
        );
    float bottom = mix(up, down, ratio.y);

    up = mix(
            noise_sampleAtIntIteration(iteration, min.x, min.y, max.z),
            noise_sampleAtIntIteration(iteration, max.x, min.y, max.z),
            ratio.x
        );
    down = mix(
            noise_sampleAtIntIteration(iteration, min.x, max.y, max.z),
            noise_sampleAtIntIteration(iteration, max.x, max.y, max.z),
            ratio.x
        );
    float top = mix(up, down, ratio.y);
    float samp = mix(bottom, top, ratio.z);
    return samp;
}

float noise_sample(vec3 position)
{
	float scale = NOISE_WORLDSCALE;
    float samp = 0;
    for (int i = 0; i < NOISE_LEVELS; i++)
    {
        samp += noise_iterationSample(position / scale, i) / float(NOISE_LEVELS);
        scale *= NOISE_SCALEMULTIPLIER;
    }
    return noise_applyCurve(samp);
}

// Threshold at which the "dark" (opposite of the planet base color) color is used
const float DARK_THRESHOLD = 0.6;

// Entry point
void main()
{
	// Compute base color
	vec4 color = modelColor();

	// Mix dark spots
	vec4 dark = vec4(1 - color.r, 1 - color.g, 1 - color.b, 1);
	float darkSample = noise_sample((-varying_vPos / Radius) * 50.0);
	color = mix(color, dark, darkSample > DARK_THRESHOLD);

	// Mix lighting
	vec4 intensity = lightIntensity(varying_vNorm);
	color = color * intensity;

	// Mix rim lighting
	float rimLighting = min(1, 1 + dot(varying_vDirectionFromCam, varying_vNorm));
	rimLighting = rimLighting * rimLighting;
	color = mix(color, intensity, rimLighting);

/*
	// Mix clouds
	float cloud = noise_sample(varying_vPos);
	cloud = max(0, cloud - (1 - CloudThickness)) / max(0.0001, CloudThickness);
	color = mix(color, vec4(1, 1, 1, 1), cloud);*/

	out_fColor = color;
}