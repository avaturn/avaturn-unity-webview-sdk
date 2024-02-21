using UnityEngine;
using UnityEngine.Rendering;

namespace Avaturn.Samples.Runtime._Data.Plugins
{
  [ExecuteAlways]
  public class MaterialsSetup : MonoBehaviour
  {
    private enum PipelineType
    {
      Unsupported,
      BuiltInPipeline,
      UniversalPipeline,
      HDPipeline
    }

    public Material[] Materials;

    private void Awake() => 
      Setup();

    private void Setup() 
    {
      PipelineType pipeline = GetPipeline();

      if (pipeline == PipelineType.UniversalPipeline)
      {
        SwitchMaterials("Universal Render Pipeline/Lit");
        Debug.Log("Samples Material Setup: URP");
      }
      else if (pipeline == PipelineType.HDPipeline)
      {
        SwitchMaterials("HDRP/Lit");
        Debug.Log("Samples Material Setup: HDRP");
      }
      else if (pipeline == PipelineType.BuiltInPipeline)
      {
        SwitchMaterials("Standard");
        Debug.Log("Samples Material Setup: BRP");
      }
    }

    private static PipelineType GetPipeline()
    {
#if UNITY_2019_1_OR_NEWER
      if (GraphicsSettings.renderPipelineAsset != null)
      {
        string srpType = GraphicsSettings.renderPipelineAsset.GetType().ToString();

        if (srpType.Contains("HDRenderPipelineAsset"))
          return PipelineType.HDPipeline;
        else if (srpType.Contains("UniversalRenderPipelineAsset") || srpType.Contains("LightweightRenderPipelineAsset"))
          return PipelineType.UniversalPipeline;
        else
          return PipelineType.Unsupported;
      }
#elif UNITY_2017_1_OR_NEWER
        if (GraphicsSettings.renderPipelineAsset != null)
          return PipelineType.Unsupported;
#endif
      return PipelineType.BuiltInPipeline;
    }

    private void SwitchMaterials(string shaderName)
    {
      if(Materials == null)
        return;
      
      foreach (Material material in Materials)
      {
        Shader shader = Shader.Find(shaderName);

        if (shader != null)
          material.shader = shader; 
      }
    }
  }
}