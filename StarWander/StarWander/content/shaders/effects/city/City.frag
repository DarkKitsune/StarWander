#version 400
#feature(ModelColor)

// In / Out
in vec3 varying_vPos;
in vec3 varying_vNorm;
in vec3 varying_vFromModelToEye;
out vec4 out_fColor;

uniform float uniform_time;

const float BUILDING_SIZE = 35.0;
const float MIN_BUILDING_HEIGHT = 40.0;
const float MAX_BUILDING_HEIGHT = 90.0;
const vec2 WINDOW_SIZE = vec2(0.9, 1.1);
const vec2 SPACE_BETWEEN_WINDOWS = vec2(1.5);
const vec4 BUILDING_COLOR = vec4(0.5, 0.5, 0.5, 1.0);
const vec4 WINDOW_COLOR = vec4(0.9, 0.9, 0.7, 1.0);
const float STREET_SIZE = 40.0;
const int BLOCK_BUILDINGS = 2;
const float SPACE_BETWEEN_BUILDINGS = 3.0;
const int ITERATIONS = 300;
const vec3 REVERSE_LIGHT_DIRECTION = normalize(vec3(1, 2, 4));

const float BUILDING_SPACING = BUILDING_SIZE + SPACE_BETWEEN_BUILDINGS;
const float BLOCK_SIZE = BUILDING_SIZE * float(BLOCK_BUILDINGS) + (SPACE_BETWEEN_BUILDINGS * float(BLOCK_BUILDINGS - 1));
const float BLOCK_SPACING = BLOCK_SIZE + STREET_SIZE;
const vec2 WINDOW_SPACING = WINDOW_SIZE + SPACE_BETWEEN_WINDOWS;

struct blockInfo {
	vec2 blockNum;
	vec2 blockCorner;
	vec2 positionInBlock;
	bool insideBlock;
};

struct buildingInfo {
	vec2 buildingNum;
	vec2 buildingCorner;
	vec2 positionInBuilding;
	bool insideBuilding;
	float buildingHeight;
};

struct positionInfo {
	blockInfo block;
	buildingInfo building;
};

float randFloat(vec3 pos, float minVal, float maxVal)
{
	vec3 multiplied = pos * vec3(1, 1000, 10000);
	multiplied.y *= cos(multiplied.z);
	multiplied.z *= cos(multiplied.x);
	multiplied.x *= cos(multiplied.y);
	return minVal + abs(mod(multiplied.x + multiplied.y + multiplied.z, maxVal - minVal));
}

vec3 getBuildingNormal(vec3 position, buildingInfo building)
{
	float upRatio = position.z / building.buildingHeight;
	float sliceRadius = upRatio * BUILDING_SIZE * 0.5;
	vec2 fromCenter = building.positionInBuilding - BUILDING_SIZE / 2.0;
	bool isTop = abs(fromCenter.x) < sliceRadius && abs(fromCenter.y) < sliceRadius;
	vec3 sideNormal = mix(
			vec3(-1.0, 0.0, 0.0),
			vec3(1.0, 0.0, 0.0),
			float(fromCenter.x > 0)
		);
	sideNormal = mix(sideNormal, vec3(0.0, -1.0, 0.0), float(fromCenter.y < -abs(fromCenter.x)));
	sideNormal = mix(sideNormal, vec3(0.0, 1.0, 0.0), float(fromCenter.y > abs(fromCenter.x)));
	return mix(sideNormal, vec3(0.0, 0.0, 1.0), float(isTop));
}

blockInfo getBlockInfo(vec3 position)
{
	vec2 blockNum = floor(position.xy / BLOCK_SPACING);
	vec2 blockCorner = blockNum * BLOCK_SPACING;
	vec2 positionInBlock = position.xy - blockCorner.xy;
	bool insideBlock = positionInBlock.x < BLOCK_SIZE && positionInBlock.y < BLOCK_SIZE;
	return blockInfo (
		blockNum,
		blockCorner,
		positionInBlock,
		insideBlock
	);
}

buildingInfo getBuildingInfo(vec3 position, blockInfo block)
{
	vec2 buildingNum = vec2(floor(block.positionInBlock.xy / BUILDING_SPACING));
	vec2 buildingCorner = buildingNum * BUILDING_SPACING;
	vec2 positionInBuilding = vec2(block.positionInBlock.xy - buildingCorner.xy);
	float buildingHeight = randFloat(vec3(block.blockCorner + buildingCorner, 0.0), MIN_BUILDING_HEIGHT, MAX_BUILDING_HEIGHT)
		+ cos((block.blockCorner.x + buildingCorner.x + block.blockCorner.y + buildingCorner.y) * 0.1 + uniform_time) * MIN_BUILDING_HEIGHT * 0.5;
	bool insideBuilding = block.insideBlock
		&& position.z < buildingHeight
		&& positionInBuilding.x < BUILDING_SIZE
		&& positionInBuilding.y < BUILDING_SIZE;
	return buildingInfo (
		buildingNum,
		buildingCorner,
		positionInBuilding,
		insideBuilding,
		buildingHeight
	);
}

positionInfo getPositionInfo(vec3 position)
{
	blockInfo block = getBlockInfo(position);
	buildingInfo building = getBuildingInfo(position, block);
	return positionInfo (
		block = block,
		building = building
	);
}

bool getInsideWindow(vec3 position, buildingInfo building)
{
	const float windowCountH = floor(float(BUILDING_SIZE) / WINDOW_SPACING.x);
	const float windowCountV = floor(float(BUILDING_SIZE) / WINDOW_SPACING.y);
	const float windowStartH = (BUILDING_SIZE - windowCountH * WINDOW_SPACING.x - SPACE_BETWEEN_WINDOWS.x) / 2.0;
	const float windowStartV = (BUILDING_SIZE - windowCountV * WINDOW_SPACING.y - SPACE_BETWEEN_WINDOWS.y) / 2.0;
	float maxPosH = max(building.positionInBuilding.x, building.positionInBuilding.y);
	float windowBaseH = floor((maxPosH - windowStartH) / WINDOW_SPACING.x) * WINDOW_SPACING.x + windowStartH;
	float windowBaseV = floor((position.z - windowStartV) / WINDOW_SPACING.y) * WINDOW_SPACING.y + windowStartV;
	return (maxPosH - windowBaseH) < WINDOW_SIZE.x && (position.z - windowBaseV) < WINDOW_SIZE.y;
}

vec4 lightIntensity(vec3 norm)
{
	return vec4(vec3((1 + dot(norm, REVERSE_LIGHT_DIRECTION)) / 2.0), 1.0);
}

vec4 traceColor()
{
	vec3 traceVector = varying_vFromModelToEye / float(ITERATIONS);
	float furthestTraceFromOrigin = 0.0;

	for (int i = 0; i < ITERATIONS; i++)
	{
		float distance = float(i);
		distance = pow(distance / float(ITERATIONS), 2) * float(ITERATIONS);
		vec3 tracePos = varying_vPos + traceVector * distance;
		positionInfo traceInfo = getPositionInfo(tracePos);
		furthestTraceFromOrigin = mix(furthestTraceFromOrigin, max(furthestTraceFromOrigin, distance), traceInfo.building.insideBuilding);
	}

	vec3 finalPos = varying_vPos + furthestTraceFromOrigin * traceVector;
	positionInfo finalInfo = getPositionInfo(finalPos);
	vec3 buildingFaceNormal = getBuildingNormal(finalPos, finalInfo.building);

	vec4 color = vec4(0.0, 0.0, 0.0, 0.0);
	color = mix(color, BUILDING_COLOR, float(finalInfo.building.insideBuilding));
	//color = mix(color, WINDOW_COLOR, float(getInsideWindow(finalPos, finalInfo.building)));
	color = color * lightIntensity(buildingFaceNormal);

	return color;
}

// Entry point
void main()
{
	out_fColor = modelColor() * traceColor();
}