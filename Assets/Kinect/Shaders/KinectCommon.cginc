static const int DepthWidth = 512;
static const int DepthHeight = 424;

static const float MillimetersToMetersScale = 1.0 / 1000.0;

static const float2 DepthWidthHeight = float2(DepthWidth, DepthHeight);
static const float2 DepthHalfWidthHeight = DepthWidthHeight / 2.0;

static const float SensorHorizontalFOVDegrees = 70.6;
static const float XYSpread = tan(radians(SensorHorizontalFOVDegrees) * 0.5) / (DepthWidth * 0.5); 

static const float MinDepthMM = 500.0;
static const float MaxDepthMM = 8000.0;

static const float cx = 254.878;
static const float cy = 205.395;

static const float fx = 365.456;
static const float fy = 365.456;

// vertex offsets for building a quad from a depth pixel
static const float4 quadOffsets[4] = 
{
	float4( 1.0, 0.0, 0, 0),			
	float4( 0.0, 0.0, 0, 0),
	float4( 1.0, 1.0, 0, 0),
	float4( 0.0, 1.0, 0, 0),
};

// texture lookup offsets for loading current and nearby depth pixels
static const int3 textureOffsets4Samples[4] =
{
	int3(1, 0, 0),
	int3(0, 0, 0),
	int3(1, 1, 0),
	int3(0, 1, 0),
};				

struct EMPTY_INPUT
{
};

struct POSCOLOR_INPUT
{
	float4	pos		: POSITION;
	float4  color   : COLOR;
};

float DepthFromPacked4444(float4 packedDepth)
{
	// convert from [0,1] to [0,15]
	packedDepth *= 15.01f;
	
	// truncate to an int
	int4 rounded = (int4)packedDepth;				
	
	return rounded.w * 4096 + rounded.x * 256 + rounded.y * 16 + rounded.z;				
}

EMPTY_INPUT VS_Empty()
{
	return (EMPTY_INPUT)0;
}

float4 FS_Passthrough(POSCOLOR_INPUT input) : COLOR
{
	return input.color;
}