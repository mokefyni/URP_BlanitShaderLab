#ifndef CUSTOM_MATH_RANDOM_INCLUDED
#define CUSTOM_MATH_RANDOM_INCLUDED

// generate random number with range [0.0, 1.0]


float Rand21(float2 p)
{
    // prevents randomness decreasing from coordinates too large
    p = p % 10000.0;
    // returns "Random" float between 0 and 1
    return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
}

float2 Rand22(float2 p)
{
    // prevents randomness decreasing from coordinates too large
    p = p % 10000.0;
    // returns "random" vec2 with x and y between 0 and 1
    return frac((float2(sin(dot(p, float2(127.1, 311.7))), sin(dot(p, float2(269.5, 183.3))))) * 43758.5453);
}



float3 Rand33(float3 p)
{
    float3 q = float3(
        dot(p, float3(127.1, 311.7, 74.7)),
        dot(p, float3(269.5, 183.3, 246.1)),
        dot(p, float3(113.5, 271.9, 124.6))
    );

    return frac(sin(q) * 43758.5453);
}



#endif