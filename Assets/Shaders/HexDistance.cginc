#ifndef HEX_DISTANCE_INCLUDED
#define HEX_DISTANCE_INCLUDED

void HexDistance_float(float2 p, float size, out float outDistance)
{
    p = abs(p * 2 - 1);
    float2 norm = normalize(float2(1, 1.73));
    float c = dot(p, norm);
    c = max(c, p.x);
    outDistance = (c - 1) * size;
}

#endif