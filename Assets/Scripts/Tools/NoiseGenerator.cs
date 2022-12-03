using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Packages.Rider.Editor.UnitTesting;
using PersonalInclude.NoiseGenStruct;
using PersonalInclude.NoiseGenLib;
using Unity.Mathematics;
using UnityEngine.UIElements;


public class NoiseGenerator : EditorWindow
{
    
    public NOISEOPTIONS noiseoption;
    public NOISEWIDTH noisewidthoption = NOISEWIDTH.W256;
    public NOISEHEIGHT noiseheightoption = NOISEHEIGHT.H256;
    public NOISEDEPTH noisedepthoption = NOISEDEPTH.D256;
    
    int channel = 0;                            // 通道选择
    int width = 256;                            // 宽度
    int height = 256;                           // 高度
    int depth = 256;                            // 深度
    int scale = 2;                              // 缩放: 整数便于tileable的实现
    float offsetX = 10.0f;                      // X位移
    float offsetY = 10.0f;                      // Y位移
    float offsetZ = 10.0f;                      // Y位移
    float noisePow = 1.0f;                      // 利用pow调整亮度
    float noiseContrast = 0.0f;                 // 对比度
    bool oneMinus = false;                      // 是否 1-noise
    string AssetsName = "Arts/GeneratorOutput"; // 默认保存路径
    Texture2D texture;                          // 纹理初始化
    Texture2D textureForShow;                    // 用于预览的纹理
    Texture3D texture3D;                          //纹理初始化
    Vector2 scrollPos;                          //界面DropDown
    int tempChannelMark = 0;
    int warningMark = 0;

    // 用于缓存上一帧的设置
    struct NoiseTextureSetting
    {
        public int channel;
        public int width;
        public int height;
        public int depth;
        public int scale;
        public NOISEOPTIONS noiseoption;
        public float offsetX;
        public float offsetY;
        public float offsetZ;
        public float noisePow;
        public float noiseContrast;
        public bool oneMinus;
    }
    NoiseTextureSetting noiseTextureSetting = new NoiseTextureSetting();
    NoiseTextureSetting[] noiseTextureSetting_channels = new NoiseTextureSetting[4];
    
    [MenuItem("Tools/Noise Generator")]
    static void Init()
    {
        EditorWindow window = EditorWindow.GetWindow<NoiseGenerator>();
        window.maxSize = new Vector2(560.0f, 700.0f);
        window.minSize = new Vector2(560.0f, 380.0f);
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical("frameBox", GUILayout.Width(270));
            EditorGUI.BeginDisabledGroup(channel == 4);
            noiseoption = (NOISEOPTIONS)EditorGUILayout.EnumPopup("噪音选项设置:", noiseoption);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("噪音宽 = ");
            noisewidthoption = (NOISEWIDTH)EditorGUILayout.EnumPopup(noisewidthoption);//噪音宽度设置
            width = (int)noisewidthoption;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("噪音高 = ");
            noiseheightoption =(NOISEHEIGHT)EditorGUILayout.EnumPopup(noiseheightoption);//噪音高度设置
            height = (int)noiseheightoption;
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("(3D)噪音深度 = ");
            noisedepthoption =(NOISEDEPTH)EditorGUILayout.EnumPopup(noisedepthoption);//噪音高度设置
            depth = (int)noisedepthoption;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("噪音缩放 = ");
            scale = EditorGUILayout.IntSlider(scale, 0, 100);//噪音缩放设置
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("噪音位移X = ");
            offsetX = EditorGUILayout.Slider(offsetX, 0.0f, 100.0f);//噪音位移设置
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("噪音位移Y = ");
            offsetY = EditorGUILayout.Slider(offsetY, 0.0f, 100.0f);//噪音位移设置
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("噪音位移Z = ");
            offsetZ = EditorGUILayout.Slider(offsetZ, 0.0f, 100.0f);//噪音位移设置
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("pow = ");
            noisePow = EditorGUILayout.Slider(noisePow, 0.0f, 10.0f);//噪音位移设置
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("contrast = ");
            noiseContrast = EditorGUILayout.Slider(noiseContrast, 0.0f, 0.95f);//噪音位移设置
            EditorGUILayout.EndHorizontal();

            oneMinus = EditorGUILayout.ToggleLeft("One Minus", oneMinus);
            EditorGUI.EndDisabledGroup();
            
            GUILayout.Space(10);
            
            string[] rgbStrings = new[] {"R", "G", "B", "A", "RGBA"};
            channel = GUILayout.Toolbar(channel, rgbStrings);
            
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("frameBox", GUILayout.Height(290), GUILayout.MaxWidth(270));
                // 纹理预览
                GUILayout.Label("纹理预览：");
                EditorGUI.DrawPreviewTexture(new Rect(285, 30, 256, 256), textureForShow,
                    null, ScaleMode.ScaleToFit);
            EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        
        // 文件设置
        var dirPath = "Assets/" + AssetsName + "/";
        GUILayout.Label("文件夹路径："+ dirPath);                             //当前保存文件夹名字
        GUILayout.BeginHorizontal();
        GUILayout.Label("设置输出文件夹：");
        AssetsName = EditorGUILayout.TextField(AssetsName);                   //文件夹名字设置
        GUILayout.EndHorizontal();
        
        if (!texture) texture = new Texture2D(width, height);
        // 纹理生成和通道切换
        if (NoiseCheck(channel,width,height,depth,scale,noiseoption,offsetX,offsetY,offsetZ,noisePow,noiseContrast,oneMinus))
        {
            // channel变换 优先读取
            if (ChannelCheck(channel))
            {
                Color[] tempCol = texture.GetPixels();
                switch (channel)
                {
                    // R
                    case 0:
                        for (int i = 0; i < tempCol.Length; i++)
                        {
                            tempCol[i] = new Color(tempCol[i].r,
                                                tempCol[i].r, 
                                                tempCol[i].r,
                                                tempCol[i].r);
                        }
                        noiseTextureSetting = noiseTextureSetting_channels[0];
                        break;
                    // G
                    case 1:
                        for (int i = 0; i < tempCol.Length; i++)
                        {
                            tempCol[i] = new Color(tempCol[i].g,
                                                    tempCol[i].g, 
                                                    tempCol[i].g,
                                                    tempCol[i].g);
                        }
                        noiseTextureSetting = noiseTextureSetting_channels[1];
                        break;
                    // B
                    case 2:
                        for (int i = 0; i < tempCol.Length; i++)
                        {
                            tempCol[i] = new Color(tempCol[i].b,
                                                tempCol[i].b, 
                                                tempCol[i].b,
                                                tempCol[i].b);
                        }
                        noiseTextureSetting = noiseTextureSetting_channels[2];
                        break;
                    // A
                    case 3:
                        for (int i = 0; i < tempCol.Length; i++)
                        {
                            tempCol[i] = new Color(tempCol[i].a,
                                tempCol[i].a, 
                                tempCol[i].a,
                                tempCol[i].a);
                        }
                        noiseTextureSetting = noiseTextureSetting_channels[3];
                        break;
                    // RGBA
                    default:
                        break;
                }
                textureForShow.SetPixels(tempCol);
                textureForShow.Apply();
                // 特殊处理：
                // 1. load：将读取出的Setting数据赋值给字段，当前帧结束会自动保存（NoiseSettingSave）
                // 2. tempChannelMark：一次性标记，需要下一帧再执行一次 非channel变换的分支，用于生成预览图
                NoiseSettingLoad(noiseTextureSetting);  // 这条语句会导致预览图重置（切换到未修改过的通道），通过下一句修复
                tempChannelMark = 1;
            }
            // 其他变换 直接生成并覆盖当前channel
            else
            {
                if (tempChannelMark == 1)
                {
                    tempChannelMark = 0;
                }
                textureForShow = GenerateTexture();
                Color[] tempCol_new = textureForShow.GetPixels();
                // if (!texture) texture = new Texture2D(width, height);
                if (SizeCheck(width, height)) texture.Reinitialize(width, height);
                Color[] tempCol_source = texture.GetPixels();
                
                NoiseSettingSave(); // 更新noiseTextureSetting，用于保存
                switch (channel)
                {
                    // R
                    case 0:
                        for (int i = 0; i < tempCol_new.Length; i++)
                        {
                            tempCol_new[i] = new Color(tempCol_new[i].r,
                                tempCol_source[i].g, 
                                tempCol_source[i].b,
                                tempCol_source[i].a);
                        }
                        noiseTextureSetting_channels[0] = noiseTextureSetting;
                        texture.SetPixels(tempCol_new);
                        texture.Apply();
                        break;
                    // G
                    case 1:
                        for (int i = 0; i < tempCol_new.Length; i++)
                        {
                            tempCol_new[i] = new Color(tempCol_source[i].r,
                                tempCol_new[i].g, 
                                tempCol_source[i].b,
                                tempCol_source[i].a);
                        }
                        noiseTextureSetting_channels[1] = noiseTextureSetting;
                        texture.SetPixels(tempCol_new);
                        texture.Apply();
                        break;
                    // B
                    case 2:
                        for (int i = 0; i < tempCol_new.Length; i++)
                        {
                            tempCol_new[i] = new Color(tempCol_source[i].r,
                                tempCol_source[i].g, 
                                tempCol_new[i].b,
                                tempCol_source[i].a);
                        }
                        noiseTextureSetting_channels[2] = noiseTextureSetting;
                        texture.SetPixels(tempCol_new);
                        texture.Apply();
                        break;
                    // A
                    case 3:
                        for (int i = 0; i < tempCol_new.Length; i++)
                        {
                            tempCol_new[i] = new Color(tempCol_source[i].r,
                                tempCol_source[i].g, 
                                tempCol_source[i].b, 
                                tempCol_new[i].r);  // 计算结果存在rgb通道中
                        }
                        noiseTextureSetting_channels[3] = noiseTextureSetting;
                        texture.SetPixels(tempCol_new);
                        texture.Apply();
                        break;
                    // RGBA
                    case 4:
                        textureForShow.SetPixels(tempCol_source);
                        textureForShow.Apply();
                        break;
                }
            }
            warningMark = 0;
        }
        
        // 纹理保存
        GUI.skin.button.wordWrap = true;
        if (GUILayout.Button("保存纹理"))
        {
            SaveTexture(texture);//保存图像
            // switch (noiseoption)
            // {
            //     case NOISEOPTIONS.PerlinNoise: 
            //     case NOISEOPTIONS.ValueNoise:
            //     case NOISEOPTIONS.WorleyNoise:
            //     case NOISEOPTIONS.PerlinFBM:
            //     case NOISEOPTIONS.WorleyFBM:
            //     case NOISEOPTIONS.PerlinWorley:
            //         SaveTexture(texture);//保存图像
            //     break;
            //     
            //     case NOISEOPTIONS.WorleyNoise3D:
            //     case NOISEOPTIONS.WorleyFBM3D:
            //     case NOISEOPTIONS.PerlinNoise3D:
            //     case NOISEOPTIONS.PerlinWorley3D:
            //         texture3D = GenerateTexture3D();
            //         SaveTexture(texture3D);//保存图像
            //     break;
            //     default:
            //         Debug.Log("Noise类型错误");
            //     break;
            // }
        }
        if (GUILayout.Button("保存3D纹理"))
        {
            if (CheckAll3DTexture(noiseTextureSetting_channels))
            {
                texture3D = GenerateTexture3D(noiseTextureSetting_channels);
                SaveTexture(texture3D);
                // 保存
            }
            else
            {
                warningMark = 1;
            }
        }
        if (warningMark == 1)
        {
            EditorGUILayout.HelpBox("请确认RGBA通道都是3D纹理", MessageType.Warning);
        }
        
        // 每帧缓存设置，设置变化时才重新生成贴图
        NoiseSettingSave();
    }

    // 缓存当前设置
    void NoiseSettingSave()
    {
        noiseTextureSetting.channel = channel;
        noiseTextureSetting.width = width;
        noiseTextureSetting.height = height;
        noiseTextureSetting.depth = depth;
        noiseTextureSetting.scale = scale;
        noiseTextureSetting.noiseoption = noiseoption;
        noiseTextureSetting.offsetX = offsetX;
        noiseTextureSetting.offsetY = offsetY;
        noiseTextureSetting.offsetZ = offsetZ;
        noiseTextureSetting.noisePow = noisePow;
        noiseTextureSetting.noiseContrast = noiseContrast;
        noiseTextureSetting.oneMinus = oneMinus;
    }

    void NoiseSettingLoad(NoiseTextureSetting noiseTextureSetting)
    {
        // 不能修改channel
        width = noiseTextureSetting.width;
        height = noiseTextureSetting.height;
        depth = noiseTextureSetting.depth;
        scale = noiseTextureSetting.scale;
        noiseoption = noiseTextureSetting.noiseoption;
        offsetX = noiseTextureSetting.offsetX;
        offsetY = noiseTextureSetting.offsetY;
        offsetZ = noiseTextureSetting.offsetZ;
        noisePow = noiseTextureSetting.noisePow;
        noiseContrast = noiseTextureSetting.noiseContrast;
        oneMinus = noiseTextureSetting.oneMinus;
    }
    
    // 检查设置是否发生变化，用于触发贴图的生成
    bool NoiseCheck(int channel,int width,int height,int depth,int scale,
        NOISEOPTIONS noiseoption,float offsetX,float offsetY, float offsetZ,
        float noisePow,float noiseContrast, bool oneMinus)
    {
        if (noiseTextureSetting.channel == channel
            && noiseTextureSetting.width == width 
            && noiseTextureSetting.height == height
            && noiseTextureSetting.scale == scale
            && noiseTextureSetting.noiseoption == noiseoption 
            && noiseTextureSetting.offsetX == offsetX 
            && noiseTextureSetting.offsetY == offsetY
            && noiseTextureSetting.offsetZ == offsetZ
            && noiseTextureSetting.noisePow == noisePow
            && noiseTextureSetting.noiseContrast == noiseContrast
            && noiseTextureSetting.oneMinus == oneMinus
            && tempChannelMark == 0)
        {
            return false;
        }
        return true;
    }

    bool ChannelCheck(int channel)
    {
        if (noiseTextureSetting.channel == channel)
        {
            return false;
        }
        return true;
    }

    bool SizeCheck(int width, int height)
    {
        if (noiseTextureSetting.width == width
            && noiseTextureSetting.height == height)
        {
            return false;
        }
        return true;
    }

    bool CheckAll3DTexture(NoiseTextureSetting[] noiseTextureSetting)
    {
        for (int i = 0; i < noiseTextureSetting.Length; i++)
        {
            // 含有任何2D Noise直接返回False
            if (noiseTextureSetting[i].noiseoption == NOISEOPTIONS.PerlinNoise
                || noiseTextureSetting[i].noiseoption == NOISEOPTIONS.ValueNoise
                || noiseTextureSetting[i].noiseoption == NOISEOPTIONS.WorleyNoise
                || noiseTextureSetting[i].noiseoption == NOISEOPTIONS.PerlinFBM
                || noiseTextureSetting[i].noiseoption == NOISEOPTIONS.WorleyFBM
                || noiseTextureSetting[i].noiseoption == NOISEOPTIONS.PerlinWorley)
            {
                return false;
            }
        }

        return true;
    }
    
    // ---------------- Main Function ----------------
    /// <summary>
    /// 生成2D纹理
    /// </summary>
    /// <returns></returns>
    Texture2D GenerateTexture()
    {
        Texture2D textureGen = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y, 0, noiseoption);
                textureGen.SetPixel(x, y, color);
            }
        }
        textureGen.Apply();

        return textureGen;
    }
    
    // /// <summary>
    // /// 生成3D纹理
    // /// （暂时不需要使用）
    // /// </summary>
    // /// <returns></returns>
    // Texture3D GenerateTexture3D()
    // {
    //     Texture3D texture3D = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);
    //     texture3D.wrapMode = TextureWrapMode.Clamp;
    //
    //     for (int x = 0; x < width; x++)
    //     {
    //         for (int y = 0; y < height; y++)
    //         {
    //             for (int z = 0; z < depth; z++)
    //             {
    //                 Color color = CalculateColor(x, y, z, noiseoption);
    //                 texture3D.SetPixel(x, y, z, color, 0);
    //             }
    //         }
    //     }
    //     texture3D.Apply();
    //
    //     return texture3D;
    // }
    
    /// <summary>
    /// 分通道生成 3D纹理
    /// </summary>
    /// <param name="setting"></param>
    /// <returns></returns>
    Texture3D GenerateTexture3D(NoiseTextureSetting[] setting)
    {
        // TODO: 3D纹理生成预览的处理
        Texture3D texture3DGen = new Texture3D(width, height, depth, TextureFormat.RGBA32, false);
        texture3DGen.wrapMode = TextureWrapMode.Clamp;
        // temp settings
        NoiseTextureSetting tempSetting = noiseTextureSetting;  // 暂存设置，之后还原
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    NoiseSettingLoad(setting[0]);
                    float r = CalculateColor(x, y, z, noiseoption).r;
                    NoiseSettingLoad(setting[1]);
                    float g = CalculateColor(x, y, z, noiseoption).r;
                    NoiseSettingLoad(setting[2]);
                    float b = CalculateColor(x, y, z, noiseoption).r;
                    NoiseSettingLoad(setting[3]);
                    float a = CalculateColor(x, y, z, noiseoption).r;

                    Color color = new Color(r, g, b, a);
                    texture3DGen.SetPixel(x, y, z, color, 0);
                }
            }
        }
        texture3DGen.Apply();

        noiseTextureSetting = tempSetting;
        NoiseSettingLoad(noiseTextureSetting);
    
        return texture3DGen;
    }


    /// <summary>
    /// 计算 Noise 数值
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    Color CalculateColor(int x, int y, int z, NOISEOPTIONS noiseOptionSetting)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;
        float zCoord = (float)z / depth * scale + offsetZ;
        float sample = 1;

        switch (noiseOptionSetting)
        {
            // 2D Perlin Noise:
            case NOISEOPTIONS.PerlinNoise:
                // Mathf.PerlinNoise -> discarded, because not tileable
                sample = Noise.PerlinNoise(new Vector2(xCoord, yCoord), scale);
                break; 
            // 2D Value Noise
            case NOISEOPTIONS.ValueNoise:
                sample = Noise.ValueNoise(new Vector2(xCoord, yCoord), scale);
                break;
            // 2D Worley Noise
            case NOISEOPTIONS.WorleyNoise:
                sample = Noise.WorleyNoise(new Vector2(xCoord, yCoord), scale);
                break;
            // 2D PerlinFBM
            case NOISEOPTIONS.PerlinFBM:
                sample = Noise.PerlinFBM(new Vector2(xCoord, yCoord), scale);
                break;
            // 2D WorleyFBM
            case NOISEOPTIONS.WorleyFBM:
                sample = Noise.WorleyFBM(new Vector2(xCoord, yCoord), scale);
                break;
            // 2D PerlinWorley
            case NOISEOPTIONS.PerlinWorley:
                sample = Noise.PerlinWorley(new Vector2(xCoord, yCoord), scale);
                break;
            // ============================ 3D Noise ============================
            // 3D Worley Noise
            case NOISEOPTIONS.WorleyNoise3D:
                sample = Noise.WorleyNoise3D(new Vector3(xCoord, yCoord, zCoord), scale);
                break;
            case NOISEOPTIONS.WorleyFBM3D:
                sample = Noise.WorleyFBM3D(new Vector3(xCoord, yCoord, zCoord), scale);
                break;
            case NOISEOPTIONS.PerlinNoise3D:
                sample = Noise.PerlinNoise3D(new Vector3(xCoord, yCoord, zCoord), scale);
                break;
            case NOISEOPTIONS.PerlinWorley3D:
                sample = Noise.PerlinWorley3D(new Vector3(xCoord, yCoord, zCoord), scale);
                break;
            default:
                break;
        }
        // pow
        sample = Mathf.Pow(sample, noisePow);
        // oneMinus
        if (oneMinus == true)
        {
            sample = 1.0f - sample;
        }
        // contrast
        sample = 0.5f + (sample - 0.5f) * 1.0f / (1.0f - noiseContrast);
        // saturate
        sample = Mathf.Max(Mathf.Min(sample, 1.0f), 0.0f);

        return new Color(sample, sample, sample);
    }
    
    // 保存纹理
    void SaveTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();                                   //读取图像为PNG
        var dirPath = Application.dataPath + "/" + AssetsName + "/";        //当前文件夹路径
        Debug.Log("生成路径:" + dirPath);                                   //生成路径位置
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);                                     //没有路径则生成
        }
        for (int i = 0; i < 1000; i++)
        {
            if (!File.Exists(dirPath + "Image" + "(" + i + ")" + ".png"))
            {
                File.WriteAllBytes(dirPath + "Image" + "(" + i + ")" + ".png", bytes);//写入文件里面
                break;
            }
        }
        AssetDatabase.Refresh();
    }
    
    void SaveTexture(Texture3D texture)
    {
        var dirPath = "Assets/" + AssetsName + "/";        //当前文件夹路径
        Debug.Log("生成路径:" + dirPath);                                   //生成路径位置
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);                                     //没有路径则生成
        }
        for (int i = 0; i < 1000; i++)
        {
            if (!File.Exists(dirPath + "Image" + "(" + i + ")" + ".asset"))
            {
                AssetDatabase.CreateAsset(texture, dirPath + "Image" + "(" + i + ")" + ".asset"); //写入
                break;
            }
        }
        AssetDatabase.Refresh();
    }
    // ---------------- ------------- ----------------
}


