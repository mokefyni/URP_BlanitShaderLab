using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static PersonalInclude.MathLib.Math;

namespace PersonalInclude.NoiseGenLib
{
    public class Noise
    {
        public static float ValueNoise(Vector2 coord, float scale)
        {
            Vector2 i = Floor(coord);
            Vector2 f = Frac(coord);
            
            // 4 corners of a rectangle surrounding our point
            float tl = Rand21(Mod(i, scale));
            float tr = Rand21(Mod(i + new Vector2(1.0f, 0.0f), scale));
            float bl = Rand21(Mod(i + new Vector2(0.0f, 1.0f), scale));
            float br = Rand21(Mod(i + new Vector2(1.0f, 1.0f), scale));

            Vector2 cubic = f * f * (new Vector2(3.0f, 3.0f) - 2.0f * f);

            float topmix = Lerp(tl, tr, cubic.x);
            float botmix = Lerp(bl, br, cubic.x);
            float finalNoise = Lerp(topmix, botmix, cubic.y);
            
            return finalNoise;
        }
        
        // Gradient Noise by Inigo Quilez - iq/2013
        // https://www.shadertoy.com/view/XdXGW8
        public static float PerlinNoise(Vector2 coord, float scale)
        {
            Vector2 i = Floor(coord);
            Vector2 f = Frac(coord);
            
            // 获取每个格子的顶点 -> 随机向量
            Vector2 a = Rand22_OneToOne(Mod(i, scale));
            Vector2 b = Rand22_OneToOne(Mod(i + new Vector2(1.0f, 0.0f), scale));
            Vector2 c = Rand22_OneToOne(Mod(i + new Vector2(0.0f, 1.0f), scale));
            Vector2 d = Rand22_OneToOne(Mod(i + new Vector2(1.0f, 1.0f), scale));

            // 平滑插值
            Vector2 cubic = f * f * (new Vector2(3.0f, 3.0f) - 2.0f * f);
            
            // interpolation
            float finalNoise = Lerp(Lerp(Vector2.Dot(a, f - new Vector2(0.0f, 0.0f)),
                                        Vector2.Dot(b, f - new Vector2(1.0f, 0.0f)), cubic.x),
                                    Lerp(Vector2.Dot(c, f - new Vector2(0.0f, 1.0f)),
                                        Vector2.Dot(d, f - new Vector2(1.0f, 1.0f)), cubic.x), cubic.y);
            finalNoise = finalNoise * 0.5f + 0.5f;
            return finalNoise;
        }

        public static float WorleyNoise(Vector2 coord, float scale)
        {
            Vector2 i = Floor(coord);
            Vector2 f = Frac(coord);

            float min_dist = 1000.0f;
            // going through the current tile and the tiles surrounding it
            for (float x = -1.0f; x <= 1.0; x++)
            {
                for (float y = -1.0f; y <= 1.0; y++)
                {

                    // generate a random point in each tile,
                    // but also account for whether it's a farther, neighbouring tile
                    Vector2 node = Rand22(Mod(i + new Vector2(x, y), scale)) + new Vector2(x, y);

                    // check for distance to the point in that tile
                    // decide whether it's the minimum
                    Vector2 d = f - node;
                    float dist = Mathf.Sqrt(Vector2.Dot(d, d));
                    min_dist = Mathf.Min(min_dist, dist);
                }
            }
            return min_dist;
        }

        public static float PerlinFBM(Vector2 coord, float scale)
        {
            int OCTAVES = 4;

            float normalize_factor = 0.0f;
            float value = 0.0f;
            float multiScale = 0.5f;

            for (int i = 0; i < OCTAVES; i++)
            {
                value += PerlinNoise(coord, scale) * multiScale;
                normalize_factor += multiScale;
                // 此处coord和scale需要配合，同时缩放
                coord *= 2.0f;
                scale *= 2.0f;
                multiScale *= 0.5f;
            }
            return value / normalize_factor;
        }

        public static float WorleyFBM(Vector2 coord, float scale)
        {
            int OCTAVES = 3;
            
            float normalize_factor = 0.0f;
            float value = 0.0f;
            float multiScale = 0.5f;

            for (int i = 0; i < OCTAVES; i++)
            {
                value += WorleyNoise(coord, scale) * multiScale;
                normalize_factor += multiScale;
                // 此处coord和scale需要配合，同时缩放
                coord *= 2.0f;
                scale *= 2.0f;
                multiScale *= 0.5f;
            }
            return value / normalize_factor;
        }
        
        // method refer: Nubis: Authoring Real-Time Volumetric Cloudscapes with the Decima Engine - Guerrilla Games.
        // https://www.guerrilla-games.com/read/nubis-authoring-real-time-volumetric-cloudscapes-with-the-decima-engine.
        // code refer: https://www.shadertoy.com/view/3dVXDc
        public static float PerlinWorley(Vector2 coord, float scale)
        {
            float perlin = PerlinNoise(coord, scale);
            float worley_fbm = WorleyFBM(coord, scale);
            float finalNoise = Remap(perlin, 0.0f, 1.0f, 1.0f-worley_fbm, 1.0f);
            return Saturate(finalNoise);
        }
        
        public static float PerlinNoise3D(Vector3 coord, float scale)
        {
            Vector3 i = Floor(coord);
            Vector3 f = Frac(coord);

            // 获取每个格子的顶点 -> 随机向量
            Vector3 a_b = Rand33_OneToOne(Mod(i, scale));
            Vector3 b_b = Rand33_OneToOne(Mod(i + new Vector3(1.0f, 0.0f, 0.0f), scale));
            Vector3 c_b = Rand33_OneToOne(Mod(i + new Vector3(0.0f, 1.0f, 0.0f), scale));
            Vector3 d_b = Rand33_OneToOne(Mod(i + new Vector3(1.0f, 1.0f, 0.0f), scale));
            
            Vector3 a_t = Rand33_OneToOne(Mod(i + new Vector3(0.0f, 0.0f, 1.0f), scale));
            Vector3 b_t = Rand33_OneToOne(Mod(i + new Vector3(1.0f, 0.0f, 1.0f), scale));
            Vector3 c_t = Rand33_OneToOne(Mod(i + new Vector3(0.0f, 1.0f, 1.0f), scale));
            Vector3 d_t = Rand33_OneToOne(Mod(i + new Vector3(1.0f, 1.0f, 1.0f), scale));
            
            // 平滑插值
            Vector3 f2 = Vector3.Scale(f, f);
            Vector3 cubic = Vector3.Scale(f2, new Vector3(3.0f, 3.0f, 3.0f) - 2.0f * f);

            // interpolation
            float lerp_bottom = Lerp(Lerp(Vector3.Dot(a_b, f - new Vector3(0.0f, 0.0f, 0.0f)),
                    Vector3.Dot(b_b, f - new Vector3(1.0f, 0.0f, 0.0f)), cubic.x),
                Lerp(Vector3.Dot(c_b, f - new Vector3(0.0f, 1.0f, 0.0f)),
                    Vector3.Dot(d_b, f - new Vector3(1.0f, 1.0f, 0.0f)), cubic.x), cubic.y);
            float lerp_top = Lerp(Lerp(Vector3.Dot(a_t, f - new Vector3(0.0f, 0.0f, 1.0f)),
                    Vector3.Dot(b_t, f - new Vector3(1.0f, 0.0f, 1.0f)), cubic.x),
                Lerp(Vector3.Dot(c_t, f - new Vector3(0.0f, 1.0f, 1.0f)),
                    Vector3.Dot(d_t, f - new Vector3(1.0f, 1.0f, 1.0f)), cubic.x), cubic.y);
            
            float finalNoise = Lerp(lerp_bottom, lerp_top, cubic.z);
            finalNoise = finalNoise * 0.5f + 0.5f;
            return finalNoise;
        }
        
        public static float WorleyNoise3D(Vector3 coord, float scale)
        {
            Vector3 i = Floor(coord);
            Vector3 f = Frac(coord);

            float min_dist = 1000.0f;
            // going through the current tile and the tiles surrounding it
            for (float x = -1.0f; x <= 1.0; x++)
            {
                for (float y = -1.0f; y <= 1.0; y++)
                {
                    for (float z = -1.0f; z <= 1.0; z++)
                    {
                        // generate a random point in each tile,
                        // but also account for whether it's a farther, neighbouring tile
                        // Vector2 node = Rand2(Mod(i + new Vector2(x, y), scale)) + new Vector2(x, y);
                        Vector3 node = Rand33(Mod(i + new Vector3(x, y, z), scale)) + new Vector3(x, y, z);

                        // check for distance to the point in that tile
                        // decide whether it's the minimum
                        Vector3 d = f - node;
                        float dist = Mathf.Sqrt(Vector3.Dot(d, d));
                        min_dist = Mathf.Min(min_dist, dist);
                    }
                }
            }
            return min_dist;
        }

        public static float WorleyFBM3D(Vector3 coord, float scale)
        {
            int OCTAVES = 3;
            
            float normalize_factor = 0.0f;
            float value = 0.0f;
            float multiScale = 0.5f;

            for (int i = 0; i < OCTAVES; i++)
            {
                value += WorleyNoise3D(coord, scale) * multiScale;
                normalize_factor += multiScale;
                // 此处coord和scale需要配合，同时缩放
                coord *= 2.0f;
                scale *= 2.0f;
                multiScale *= 0.5f;
            }
            return value / normalize_factor;
        }

        public static float PerlinWorley3D(Vector3 coord, float scale)
        {
            float perlin = PerlinNoise3D(coord, scale);
            float worley_fbm = WorleyFBM3D(coord, scale);
            float finalNoise = Remap(perlin, 0.0f, 1.0f, 1.0f-worley_fbm, 1.0f);
            return Saturate(finalNoise);
        }
        
    }
    
}
