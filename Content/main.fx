
#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

float4x4 World;
float4x4 View;
float4x4 Projection;

Texture TextureAtlas;
sampler Sampler0 = sampler_state {
	Texture = (TextureAtlas);
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float2 AnimationCoordinate : TEXCOORD1;
	float2 Value : TEXCOORD2;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float2 TextureCoordinate : TEXCOORD0;
	float2 AnimationCoordinate : TEXCOORD1;
	float2 Value : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
	VertexShaderOutput output;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.TextureCoordinate = input.TextureCoordinate;
	output.AnimationCoordinate = input.AnimationCoordinate;
	output.Value = input.Value;

	return output;
}


float4 PixelShaderFunction(VertexShaderOutput input) : COLOR
{
	float4 color = tex2D(Sampler0, input.TextureCoordinate);
	float4 result = float4(color.rgb * input.Value.x, 1);

	if (!(input.AnimationCoordinate.x <= 0 && input.AnimationCoordinate.x >= -1.0f))
	{
		// COMBINE WITH ADDITIONAL TEXTURE!

		float4 animation = tex2D(Sampler0, input.AnimationCoordinate);
		return float4(animation.r + result.r, animation.g + result.g, animation.b + result.b, 1.0f) - float4(1, 1, 1, 0);

	}

	return result;
}

technique main
{
	pass Pass0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderFunction();
	}
}