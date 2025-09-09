using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.U2D;

public class TextureResources : MonoBehaviour
{
    public SpriteAtlas SpriteAtlas;

    public TextAsset NormalMapSheet;
    public Data NormapData = new Data();

    [Serializable]
    public class Frames
    {
        public Frame frame;
        public bool rotated;
        public bool trimmed;
        public Frame spriteSourceSize;
        public Size sourceSize;
    }

    public class Frame
    {
        public int x, y, w, h;
    }

    [Serializable]
    public class Meta
    {
        public string app;
        public string version;
        public string image;
        public string format;
        public Size size;
        public string scale;

    }

    public class Size
    {
        public int w, h;
    }

    [Serializable]
    public class Data
    {
        public Dictionary<string, Frames> frames;
        public Meta meta;
    }

    private Dictionary<string, Vector4> cachedTilingOffsets = new Dictionary<string, Vector4>();
    void Awake()
    {
        // ListBlockTexture = Resources.LoadAll<Texture2D>("Texture");
        NormapData = JsonConvert.DeserializeObject<Data>(NormalMapSheet.text);
        PreCalculateAllTilingOffsets();
    }

    public Sprite TakeSprite(string name)
    {
        return SpriteAtlas.GetSprite(name);
    }

    // public void SetTexture(MaterialPropertyBlock material, string name)
    // {
    //     float atlasWidth = NormapData.meta.size.w;
    //     float atlasHeight = NormapData.meta.size.h;

    //     float spriteX = NormapData.frames[$"NormalMap{name}.png"].frame.x; ;
    //     float spriteYFromTop = NormapData.frames[$"NormalMap{name}.png"].frame.y;
    //     float spriteW = NormapData.frames[$"NormalMap{name}.png"].frame.w;
    //     float spriteH = NormapData.frames[$"NormalMap{name}.png"].frame.h;

    //     Vector2 tiling = new Vector2(spriteW / atlasWidth, spriteH / atlasHeight);

    //     float offsetX = spriteX / atlasWidth;
    //     float offsetY = (atlasHeight - spriteYFromTop - spriteH) / atlasHeight;

    //     Vector2 offset = new Vector2(offsetX, offsetY);
    //     Vector4 tilingOffset = new Vector4(tiling.x, tiling.y, offset.x, offset.y);

    //     material.SetVector("_BaseMap_ST", tilingOffset);
    //     material.SetVector("_NormalMap_ST", tilingOffset);
    // }

    void PreCalculateAllTilingOffsets()
    {
        float atlasWidth = NormapData.meta.size.w;
        float atlasHeight = NormapData.meta.size.h;

        foreach (var frame in NormapData.frames)
        {
            string originalKey = frame.Key; // "NormalMap1.2.png"
            string cleanKey = originalKey.Replace("NormalMap", "").Replace(".png", ""); // "1.2"

            float spriteX = frame.Value.frame.x;
            float spriteYFromTop = frame.Value.frame.y;
            float spriteW = frame.Value.frame.w;
            float spriteH = frame.Value.frame.h;

            Vector2 tiling = new Vector2(spriteW / atlasWidth, spriteH / atlasHeight);
            float offsetX = spriteX / atlasWidth;
            float offsetY = (atlasHeight - spriteYFromTop - spriteH) / atlasHeight;
            Vector2 offset = new Vector2(offsetX, offsetY);

            Vector4 tilingOffset = new Vector4(tiling.x, tiling.y, offset.x, offset.y);
            cachedTilingOffsets[cleanKey] = tilingOffset;
        }
    }
    
    public void SetTexture(MaterialPropertyBlock material, string name)
    {
        // Chỉ 1 dictionary lookup, không có tính toán!
        if (cachedTilingOffsets.TryGetValue(name, out Vector4 tilingOffset))
        {
            material.SetVector("_BaseMap_ST", tilingOffset);
            material.SetVector("_NormalMap_ST", tilingOffset);
        }
        else
        {
            Debug.LogError($"Texture not found: {name}");
        }
    }
    
    // public Texture TakeTexture(string name)
    // {
    //     return TextureAtlas.GetUVRect(name)
    // }


    // private async void LoadImageAsync()
    // {
    //     await LoadSpriteFromResourcesAsync("Items");
    //     Debug.Log("Items loaded");
    //     await LoadSpriteFromResourcesAsync("ListMiniGameItem");
    //     Debug.Log("ListMiniGameItem loaded");
    //     await LoadSpriteFromResourcesAsync("ListNewArea");
    //     Debug.Log("ListNewArea loaded");
    // }

    // private async Task LoadSpriteFromResourcesAsync(string path)
    // {
    //     ResourceRequest request = Resources.LoadAsync<Sprite>(path);

    //     while (!request.isDone)
    //     {
    //         Debug.Log("??");
    //         await Task.Yield();
    //     }
    // }

    // void TakeImagesFromResources(Sprite[] list, string path) => list = Resources.LoadAll<Sprite>(path); 

    // public Texture TakeTexture(Texture[] list, string name)
    // {
    //     Texture itemNeed = null;
    //     foreach (Texture item in list){
    //         if(item.name.ToLower() == name.ToLower()) 
    //         {
    //             itemNeed = item;
    //             break;
    //         }
    //     }
    //     return itemNeed;
    // }
}