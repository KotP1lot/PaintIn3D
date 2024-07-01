using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

public class PaintManager : MonoBehaviour
{
    const int TEXTURE_SIZE = 1024;
    private string filePath = Path.Combine(Application.dataPath, "SavedTexture.png");
    public float extendsIslandOffset = 1;

    RenderTexture extendIslandsRenderTexture;
    RenderTexture uvIslandsRenderTexture;
    RenderTexture maskRenderTexture;
    RenderTexture supportTexture;

    Renderer rend;

    int maskTextureID = Shader.PropertyToID("_MaskTexture");

    public Shader texturePaint;
    public Shader extendIslands;

    int prepareUVID = Shader.PropertyToID("_PrepareUV");
    int positionID = Shader.PropertyToID("_PainterPosition");
    int hardnessID = Shader.PropertyToID("_Hardness");
    int strengthID = Shader.PropertyToID("_Strength");
    int radiusID = Shader.PropertyToID("_Radius");
    int colorID = Shader.PropertyToID("_PainterColor");
    int textureID = Shader.PropertyToID("_MainTex");

    Material paintMaterial;

    CommandBuffer command;

    public void Awake()
    {
        paintMaterial = new Material(texturePaint);
        command = new CommandBuffer();
        command.name = "CommandBuffer - " + gameObject.name;
    }

    void Start()
    {
        maskRenderTexture = CreateRenderTexture();
        extendIslandsRenderTexture = CreateRenderTexture();

        uvIslandsRenderTexture = CreateRenderTexture();
        supportTexture = CreateRenderTexture();

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);
        LoadTextureFromFile();
    }

    void OnDisable()
    {
        maskRenderTexture.Release();
        uvIslandsRenderTexture.Release();
        extendIslandsRenderTexture.Release();
        supportTexture.Release();
    }

    RenderTexture CreateRenderTexture()
    {
        RenderTexture rt = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        rt.filterMode = FilterMode.Bilinear;
        rt.Create();
        return rt;
    }

    public void Paint(Vector3 pos, float radius = 1f, Color? color = null, float hardness = .5f, float strength = .5f)
    {
        paintMaterial.SetFloat(prepareUVID, 0);
        paintMaterial.SetVector(positionID, pos);
        paintMaterial.SetFloat(hardnessID, hardness);
        paintMaterial.SetFloat(strengthID, strength);
        paintMaterial.SetFloat(radiusID, radius);
        paintMaterial.SetTexture(textureID, supportTexture);
        paintMaterial.SetColor(colorID, color ?? Color.red);

        command.Blit(extendIslandsRenderTexture, supportTexture);

        command.SetRenderTarget(maskRenderTexture);
        command.DrawRenderer(rend, paintMaterial, 0);

        command.Blit(maskRenderTexture, extendIslandsRenderTexture);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }

    public void SaveTextureToFile()
    {
        Texture2D texture = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.RGBA32, false);
        RenderTexture.active = extendIslandsRenderTexture;
        texture.ReadPixels(new Rect(0, 0, TEXTURE_SIZE, TEXTURE_SIZE), 0, 0);
        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        RenderTexture.active = null;
        Destroy(texture);
    }

    public void LoadTextureFromFile()
    {
        if (!File.Exists(filePath))
        {
            return;
        }

        byte[] bytes = File.ReadAllBytes(filePath);
        Texture2D texture = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.RGBA32, false);

        if (!texture.LoadImage(bytes))
        {
            return;
        }
        command.SetRenderTarget(extendIslandsRenderTexture);
        command.Blit(texture, extendIslandsRenderTexture);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();

        rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);

        Destroy(texture);
    }
}
