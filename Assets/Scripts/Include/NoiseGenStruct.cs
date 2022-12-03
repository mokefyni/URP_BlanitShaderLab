using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PersonalInclude.NoiseGenStruct
{
    // Noise选项设置
    public enum NOISEOPTIONS
    {
        PerlinNoise = 0,
        ValueNoise = 1,
        WorleyNoise = 2,
        PerlinFBM=3,
        WorleyFBM=4,
        PerlinWorley=5,
        PerlinNoise3D=6,
        WorleyNoise3D=7,
        WorleyFBM3D=8,
        PerlinWorley3D=9
    }
    
    public enum NOISEOPTIONS3D
    {
        PerlinNoise3D,
        WorleyNoise3D,
        WorleyFBM3D,
        PerlinWorley3D
    }

    // Noise 宽度设置
    public enum NOISEWIDTH
    {
        W1 = 1,
        W2 = 2,
        W4 = 4,
        W8 = 8,
        W16 = 16,
        W32 = 32,
        W64 = 64,
        W128 = 128,
        W256 = 256,
        W512 = 512,
        W1024 = 1024,
    }
    
    // Noise 高度设置
    public enum NOISEHEIGHT
    {
        H1 = 1,
        H2 = 2,
        H4 = 4,
        H8 = 8,
        H16 = 16,
        H32 = 32,
        H64 = 64,
        H128 = 128,
        H256 = 256,
        H512 = 512,
        H1024 = 1024,
    }
    
    public enum NOISEDEPTH
    {
        D1 = 1,
        D2 = 2,
        D4 = 4,
        D8 = 8,
        D16 = 16,
        D32 = 32,
        D64 = 64,
        D128 = 128,
        D256 = 256,
        D512 = 512,
        D1024 = 1024,
    }
}
