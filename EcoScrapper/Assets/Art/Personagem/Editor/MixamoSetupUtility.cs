#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>
/// Utilitario de Editor responsavel por automatizar a configuracao de arquivos FBX do Mixamo
/// (Rig Humanoid e Loop Time nas animacoes ciclicas) e reconstruir o controlador ScrapE_Animator
/// conforme o contrato de integracao da Secao 5 e Secao 6 do CONTEXTO_PROJETO.md.
/// </summary>
public static class MixamoSetupUtility
{
    private const string PersonagemFolderPath = "Assets/Art/Personagem";
    private const string AnimatorControllerPath = "Assets/Art/Personagem/ScrapE_Animator.controller";

    /// <summary>
    /// Configura automaticamente todos os arquivos FBX dentro de Assets/Art/Personagem para Rig Humanoid
    /// e ativa Loop Time nos clipes ciclicos (Idle, Walking, Running, Falling Idle).
    /// </summary>
    /// <param name="nenhum">Esta funcao nao recebe parametros.</param>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// Menu de Editor acionavel em EcoScrapper/Rafael/1. Configurar FBX Mixamo (Humanoid + Loop).
    /// Garante que o modelo principal gere definicao de Avatar e que as animacoes In-Place tenham Loop Time correto.
    /// </remarks>
    [MenuItem("EcoScrapper/Rafael/1. Configurar FBX Mixamo (Humanoid + Loop)")]
    public static void ConfigureMixamoFbxAssets()
    {
        if (!AssetDatabase.IsValidFolder(PersonagemFolderPath))
        {
            Debug.LogWarning("[MixamoSetupUtility] Pasta nao encontrada: " + PersonagemFolderPath);
            return;
        }

        string[] fbxGuids = AssetDatabase.FindAssets("t:Model", new[] { PersonagemFolderPath });
        int configuredCount = 0;

        for (int i = 0; i < fbxGuids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(fbxGuids[i]);
            if (!assetPath.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            ModelImporter importer = AssetImporter.GetAtPath(assetPath) as ModelImporter;
            if (importer == null)
            {
                continue;
            }

            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

            string fileNameLower = Path.GetFileNameWithoutExtension(assetPath).ToLowerInvariant();
            bool shouldLoop = fileNameLower.Contains("idle") ||
                              fileNameLower.Contains("walk") ||
                              fileNameLower.Contains("run") ||
                              fileNameLower.Contains("fall");

            if (fileNameLower.Contains("jump"))
            {
                shouldLoop = false;
            }

            ModelImporterClipAnimation[] clips = importer.clipAnimations;
            if (clips == null || clips.Length == 0)
            {
                clips = importer.defaultClipAnimations;
            }

            if (clips != null && clips.Length > 0)
            {
                for (int c = 0; c < clips.Length; c++)
                {
                    clips[c].loopTime = shouldLoop;
                    clips[c].lockRootRotation = true;
                    clips[c].lockRootHeightY = true;
                    clips[c].keepOriginalPositionXZ = false;
                }

                importer.clipAnimations = clips;
            }

            importer.SaveAndReimport();
            configuredCount++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[MixamoSetupUtility] Configurados " + configuredCount + " arquivo(s) FBX em " + PersonagemFolderPath + ".");
    }

    /// <summary>
    /// Reconstroi ou atualiza as referencias de AnimationClip do ScrapE_Animator.controller,
    /// vinculando clipes FBX do Mixamo (caso presentes) ou os clipes nativos (.anim) do projeto.
    /// </summary>
    /// <param name="nenhum">Esta funcao nao recebe parametros.</param>
    /// <returns>void, sem retorno.</returns>
    /// <remarks>
    /// Menu de Editor acionavel em EcoScrapper/Rafael/2. Reconstruir ScrapE_Animator.
    /// Preserva integralmente os parametros Speed (Float), IsGrounded (Bool) e Jump (Trigger) exigidos por PlayerMovement.cs.
    /// </remarks>
    [MenuItem("EcoScrapper/Rafael/2. Reconstruir ScrapE_Animator")]
    public static void RebuildScrapEAnimator()
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AnimatorControllerPath);
        if (controller == null)
        {
            Debug.LogError("[MixamoSetupUtility] AnimatorController nao encontrado em: " + AnimatorControllerPath);
            return;
        }

        AnimationClip idleClip = FindBestClip("Idle", "Assets/Art/Personagem/Idle.anim");
        AnimationClip walkClip = FindBestClip("Walk", "Assets/Art/Personagem/Walking.anim");
        AnimationClip runClip = FindBestClip("Run", "Assets/Art/Personagem/Running.anim");
        AnimationClip jumpClip = FindBestClip("Jump", "Assets/Art/Personagem/Jump.anim");
        AnimationClip fallClip = FindBestClip("Fall", "Assets/Art/Personagem/FallingIdle.anim");

        AnimatorStateMachine rootSm = controller.layers[0].stateMachine;
        ChildAnimatorState[] states = rootSm.states;

        for (int i = 0; i < states.Length; i++)
        {
            AnimatorState state = states[i].state;
            if (state.name == "Idle" && idleClip != null)
            {
                state.motion = idleClip;
            }
            else if (state.name == "Walking" && walkClip != null)
            {
                state.motion = walkClip;
            }
            else if (state.name == "Running" && runClip != null)
            {
                state.motion = runClip;
            }
            else if (state.name == "Jump" && jumpClip != null)
            {
                state.motion = jumpClip;
            }
            else if (state.name == "Falling" && fallClip != null)
            {
                state.motion = fallClip;
            }
        }

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[MixamoSetupUtility] ScrapE_Animator atualizado com sucesso!");
    }

    /// <summary>
    /// Localiza o melhor AnimationClip disponivel na pasta do personagem para uma determinada palavra-chave,
    /// priorizando clipes importados de FBX do Mixamo e utilizando o arquivo .anim nativo como fallback.
    /// </summary>
    /// <param name="keyword">Palavra-chave do clipe desejado (ex.: Idle, Walk, Run, Jump, Fall).</param>
    /// <param name="fallbackAnimPath">Caminho relativo do clipe .anim nativo caso nenhum FBX correspondente exista.</param>
    /// <returns>O <see cref="AnimationClip"/> encontrado para associacao na maquina de estados.</returns>
    /// <remarks>
    /// Ignora clipes internos de preview (__preview__) gerados automaticamente pelo importador FBX da Unity.
    /// </remarks>
    private static AnimationClip FindBestClip(string keyword, string fallbackAnimPath)
    {
        string[] clipGuids = AssetDatabase.FindAssets("t:AnimationClip", new[] { PersonagemFolderPath });
        for (int i = 0; i < clipGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(clipGuids[i]);
            if (path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) &&
                path.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                UnityEngine.Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(path);
                for (int s = 0; s < subAssets.Length; s++)
                {
                    AnimationClip clip = subAssets[s] as AnimationClip;
                    if (clip != null && !clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase))
                    {
                        return clip;
                    }
                }
            }
        }

        return AssetDatabase.LoadAssetAtPath<AnimationClip>(fallbackAnimPath);
    }
}
#endif
