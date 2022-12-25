#ifndef CUSTOM_MATH_COLOR_INCLUDED
#define CUSTOM_MATH_COLOR_INCLUDED


float GetLuminance(float3 color)
{
    return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
}

float3 Desaturate(float3 color, float Desaturation)
{
    float lum = GetLuminance(color);
    return lerp(color, float3(lum, lum, lum), Desaturation);
}

#endif