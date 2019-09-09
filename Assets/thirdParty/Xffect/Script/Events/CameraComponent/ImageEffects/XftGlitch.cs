using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
public class XftGlitch : MonoBehaviour
{
    protected Material m_material;
    public Shader GlitchShader;
 
    public Vector3 Offset;
 
    public Texture2D Mask;

    public void Init (Shader glitch, Texture2D mask)
    {
        GlitchShader = glitch;
        Mask = mask;
    }
 
    public Material GlitchMaterial {
        get {
            if (m_material == null) {
                m_material = new Material (GlitchShader);
                m_material.hideFlags = HideFlags.HideAndDontSave;
            }
            return m_material;
        }
    }
 
    public bool CheckSupport ()
    {
        bool ret = true;
        if (!SystemInfo.supportsImageEffects)
            ret = false;
     
        if (!GlitchMaterial.shader.isSupported)
            ret = false;
     
        return ret;
    }
 
    void Awake ()
    {
        this.enabled = false;
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        if (Mask == null)
            return;
        
        GlitchMaterial.SetVector ("_displace", Offset);
        GlitchMaterial.SetTexture ("_Mask", Mask);
        Graphics.Blit (source, destination, GlitchMaterial);
    }
}
