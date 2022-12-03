using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PersonalInclude.MathLib
{
    public struct Math
    {
        public static float Mod(float x, float a)
        {
            return x % a;
        }
        public static Vector2 Mod(Vector2 p, float a)
        {
            Vector2 result = Vector2.zero;
            if (p.x < 0.0f)
            {
                result.x = p.x % a + a;
            }
            else{result.x = p.x % a;}

            if (p.y < 0.0f)
            {
                result.y = p.y % a + a;
            }
            else{result.y = p.y % a;}
            
            return result;
        }
        
        public static Vector3 Mod(Vector3 p, float a)
        {
            Vector3 result = Vector3.zero;
            if (p.x < 0.0f)
            {
                result.x = p.x % a + a;
            }
            else{result.x = p.x % a;}

            if (p.y < 0.0f)
            {
                result.y = p.y % a + a;
            }
            else{result.y = p.y % a;}
            
            if (p.z < 0.0f)
            {
                result.z = p.z % a + a;
            }
            else{result.z = p.z % a;}
            
            return result;
        }
        
        
        
        public static float Frac(float x)
        {
            return x - Mathf.Floor(x);
        }
        public static Vector2 Frac(Vector2 p)
        {
            return new Vector2(
                p.x - Mathf.Floor(p.x), 
                p.y - Mathf.Floor(p.y));
        }

        public static Vector3 Frac(Vector3 p)
        {
            return new Vector3(
                p.x - Mathf.Floor(p.x),
                p.y - Mathf.Floor(p.y),
                p.z - Mathf.Floor(p.z));
        }
        
        
        public static Vector2 Floor(Vector2 p)
        {
            return new Vector2(
                Mathf.Floor(p.x), 
                Mathf.Floor(p.y));
        }
        
        public static Vector3 Floor(Vector3 p)
        {
            return new Vector3(
                Mathf.Floor(p.x),
                Mathf.Floor(p.y),
                Mathf.Floor(p.z));
        }
        
        
        public static Vector2 Sin(Vector2 p)
        {
            return new Vector2(
                Mathf.Sin(p.x), 
                Mathf.Sin(p.y));
        }
        
        public static Vector3 Sin(Vector3 p)
        {
            return new Vector3(
                Mathf.Sin(p.x), 
                Mathf.Sin(p.y), 
                Mathf.Sin(p.z));
        }
        
        public static float Lerp(float x,float y,float level)
        {
            return x * (1 - level) + y * level;
        }
        
        
        // ----------------------------- Random -----------------------------
        /// <summary>
        /// 输入2维
        /// 生成1维随机数，返回值范围 [0.0f, 1.0f]
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static float Rand21(Vector2 coord)
        {
            // prevents randomness decreasing from coordinates too large
            coord = Mod(coord, 10000.0f);
            // returns "Random" float between 0 and 1
            return Frac(Mathf.Sin(Vector2.Dot(coord, new Vector2(12.9898f, 78.233f))) * 43758.5453f);
        }
        
        /// <summary>
        /// 生成2维随机数，返回值范围 [0.0f, 1.0f]
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static Vector2 Rand22(Vector2 coord)
        {
            // prevents randomness decreasing from coordinates too large
            coord = Mod(coord, 10000.0f);
            // returns "random" vec2 with x and y between 0 and 1
            return Frac((new Vector2(Mathf.Sin(Vector2.Dot(coord, new Vector2(127.1f, 311.7f))), Mathf.Sin(Vector2.Dot(coord, new Vector2(269.5f, 183.3f))))) * 43758.5453f);
        }
        /// <summary>
        /// 生成2维随机数，返回值范围 [-1.0f, 1.0f]
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static Vector2 Rand22_OneToOne(Vector2 coord)
        {
            coord = new Vector2(Vector2.Dot(coord,new Vector2(127.1f,311.7f)),
                Vector2.Dot(coord, new Vector2(269.5f,183.3f)) );
            return new Vector2(-1.0f, -1.0f) + new Vector2(2.0f, 2.0f) * 
                Frac(new Vector2(Mathf.Sin(coord.x), Mathf.Sin(coord.y))
                     * new Vector2(43758.5453123f, 43758.5453123f)); // ;
        }
        
        /// <summary>
        /// 生成3维随机数，返回值范围 [0.0f, 1.0f]
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static Vector3 Rand33(Vector3 coord)
        {
            Vector3 q = new Vector3(
                Vector3.Dot(coord, new Vector3(127.1f, 311.7f, 74.7f)),
                Vector3.Dot(coord, new Vector3(269.5f, 183.3f, 246.1f)),
                Vector3.Dot(coord, new Vector3(113.5f, 271.9f, 124.6f))
            );

            return Frac(Sin(q) * 43758.5453f);
        }
        /// <summary>
        /// 生成3维随机数，返回值范围 [-1.0f, 1.0f]
        /// </summary>
        /// <param name="coord"></param>
        /// <returns></returns>
        public static Vector3 Rand33_OneToOne(Vector3 coord)
        {
            Vector3 q = new Vector3(
                Vector3.Dot(coord, new Vector3(127.1f, 311.7f, 74.7f)),
                Vector3.Dot(coord, new Vector3(269.5f, 183.3f, 246.1f)),
                Vector3.Dot(coord, new Vector3(113.5f, 271.9f, 124.6f))
            );

            return Frac(Sin(q) * 43758.5453f) * 2.0f - Vector3.one;
        }
        
        
        public static float Remap(float x, float t1, float t2, float s1, float s2)
        {
            float y = (x - t1) / (t2 - t1) * (s2 - s1) + s1;
            return y;
        }

        public static float Saturate(float x)
        {
            float min = 0.0f;
            float max = 1.0f;
            return Mathf.Max(Mathf.Min(x, max), min);
        }
    }
}
