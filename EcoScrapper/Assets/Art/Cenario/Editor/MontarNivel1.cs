using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Monta o cenário da Nivel1 (tarefa do Arthur, GUIA_ARTHUR.md) dentro do editor:
/// terreno com os 4 setores e 6 camadas de textura, 75 prefabs de cenário
/// low-poly, entulhos com física, poças de lodo, pontos de spawn, névoa,
/// partículas, sons e todas as ligações do SIstemaDeJogo e da Torre.
/// Menu: EcoScrapper → Montar Nivel1 (Arthur). Pode ser rodado de novo:
/// apaga o que gerou antes e monta tudo outra vez.
/// </summary>
public static class MontarNivel1
{
    const string ScenePath = "Assets/Scenes/Nivel1.unity";
    const string ArtRoot = "Assets/Art/Cenario";
    const string MatDir = ArtRoot + "/Materiais";
    const string TexDir = ArtRoot + "/Texturas";
    const string MeshDir = ArtRoot + "/Meshes";
    const string SoundDir = ArtRoot + "/Sons";
    const string PrefabRoot = "Assets/Prefabs/Cenario";
    const string AssetPrefabDir = PrefabRoot + "/Assets";

    // Dimensões do terreno (m). O vale corre ao longo de Z, centrado em X = 50.
    const float TerrainW = 100f, TerrainH = 60f, TerrainL = 240f, CenterX = 50f;

    static readonly string[] GeneratedRoots =
    {
        "Terreno", "Setor1_Despertar", "Setor2_Ravina", "Setor3_LeitoSeco", "Setor4_Represa",
        "SpawnPoints", "SomAmbiente", "LimitesInvisiveis", "SIstemaDeJogo", "Torre", "Main Camera"
    };

    static Dictionary<string, Material> mats = new Dictionary<string, Material>();
    static Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>();
    static Terrain terrain;
    static System.Random rng;

    [MenuItem("EcoScrapper/Montar Nivel1 (Arthur)")]
    public static void Montar()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        rng = new System.Random(2026);
        mats.Clear();
        prefabs.Clear();

        try
        {
            EditorUtility.DisplayProgressBar("Nivel1", "Pastas e materiais", 0.05f);
            CriarPastas();
            CriarMateriais();

            EditorUtility.DisplayProgressBar("Nivel1", "Sons", 0.15f);
            CriarSons();

            EditorUtility.DisplayProgressBar("Nivel1", "Partículas", 0.2f);
            CriarParticulas();

            EditorUtility.DisplayProgressBar("Nivel1", "Materiais dos pacotes da loja", 0.25f);
            ConverterMateriaisDaLoja();

            EditorUtility.DisplayProgressBar("Nivel1", "Prefabs de cenário", 0.3f);
            CriarPrefabsDeCenario();

            EditorUtility.DisplayProgressBar("Nivel1", "Cena", 0.6f);
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            LimparGerados(scene);

            EditorUtility.DisplayProgressBar("Nivel1", "Terreno", 0.65f);
            CriarTerreno();

            EditorUtility.DisplayProgressBar("Nivel1", "Montando os setores", 0.8f);
            MontarCena();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Debug.Log("Nivel1 montada: terreno, " + prefabs.Count + " prefabs de cenário, física, lodo, spawns, névoa, partículas e sons.");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    // ------------------------------------------------------------------ pastas

    static void CriarPastas()
    {
        foreach (var dir in new[] { ArtRoot, MatDir, TexDir, MeshDir, SoundDir, PrefabRoot, AssetPrefabDir })
        {
            Directory.CreateDirectory(dir);
        }
        AssetDatabase.Refresh();
    }

    // --------------------------------------------------------------- materiais

    static void CriarMateriais()
    {
        Mat("Rocha", new Color(0.45f, 0.40f, 0.35f));
        Mat("RochaClara", new Color(0.62f, 0.56f, 0.47f));
        Mat("RochaEscura", new Color(0.28f, 0.26f, 0.25f));
        Mat("Basalto", new Color(0.18f, 0.18f, 0.20f));
        Mat("Terra", new Color(0.50f, 0.36f, 0.22f));
        Mat("Cascalho", new Color(0.58f, 0.52f, 0.42f));
        Mat("Madeira", new Color(0.45f, 0.30f, 0.16f));
        Mat("MadeiraSeca", new Color(0.55f, 0.48f, 0.38f));
        Mat("Petrificado", new Color(0.52f, 0.50f, 0.46f));
        Mat("Queimado", new Color(0.12f, 0.10f, 0.08f));
        Mat("FolhaSeca", new Color(0.60f, 0.52f, 0.25f));
        Mat("FolhaVerde", new Color(0.25f, 0.55f, 0.20f));
        Mat("Grama", new Color(0.35f, 0.65f, 0.25f));
        Mat("Musgo", new Color(0.30f, 0.45f, 0.20f));
        Mat("Bioluminescente", new Color(0.3f, 1f, 0.6f), new Color(0.2f, 1.5f, 0.8f));
        Mat("EsporoAzul", new Color(0.3f, 0.6f, 1f), new Color(0.2f, 0.6f, 1.8f));
        Mat("Liquen", new Color(0.6f, 1f, 0.3f), new Color(0.5f, 1.2f, 0.2f));
        Mat("Fruto", new Color(0.85f, 0.2f, 0.25f));
        Mat("Ferrugem", new Color(0.55f, 0.27f, 0.12f), metal: 0.6f);
        Mat("Aco", new Color(0.55f, 0.57f, 0.60f), metal: 0.9f);
        Mat("AcoEscuro", new Color(0.25f, 0.26f, 0.28f), metal: 0.8f);
        Mat("Cobre", new Color(0.72f, 0.42f, 0.22f), metal: 0.9f);
        Mat("Borracha", new Color(0.08f, 0.08f, 0.08f));
        Mat("Amarelo", new Color(0.90f, 0.70f, 0.10f), metal: 0.3f);
        Mat("Vermelho", new Color(0.70f, 0.12f, 0.10f), metal: 0.3f);
        Mat("Azul", new Color(0.15f, 0.30f, 0.55f), metal: 0.3f);
        Mat("Verde", new Color(0.20f, 0.40f, 0.25f), metal: 0.3f);
        Mat("Concreto", new Color(0.60f, 0.60f, 0.58f));
        Mat("Torre", new Color(0.75f, 0.80f, 0.78f), metal: 0.5f);
        Mat("Energia", new Color(0.2f, 1f, 0.5f), new Color(0.3f, 2.5f, 1.0f));
        Mat("Tela", new Color(0.1f, 0.6f, 1f), new Color(0.1f, 0.9f, 2f));
        Mat("Lodo", new Color(0.35f, 0.75f, 0.10f), new Color(0.25f, 0.8f, 0.05f));
        Mat("Pele", new Color(0.70f, 0.55f, 0.35f));
        Mat("Pelo", new Color(0.62f, 0.50f, 0.38f));
        Mat("Pena", new Color(0.35f, 0.32f, 0.45f));
        Mat("Peixe", new Color(0.6f, 0.8f, 0.9f), new Color(0.1f, 0.3f, 0.4f), 0.4f);
        Mat("Preto", new Color(0.05f, 0.05f, 0.05f));
        Mat("Bota", new Color(0.30f, 0.20f, 0.12f));
    }

    static Material Mat(string name, Color color, Color? emission = null, float metal = 0f)
    {
        string path = MatDir + "/Cen_" + name + ".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.SetColor("_BaseColor", color);
        mat.SetFloat("_Metallic", metal);
        mat.SetFloat("_Smoothness", metal > 0 ? 0.45f : 0.15f);
        if (emission.HasValue)
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", emission.Value);
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        }
        EditorUtility.SetDirty(mat);
        mats[name] = mat;
        return mat;
    }

    // -------------------------------------------------------------------- sons

    static void CriarSons()
    {
        const int sr = 44100;
        var r = new System.Random(7);

        // Vento: ruído filtrado com rajadas lentas, 8 s em loop (começo e fim casam pelo fade cruzado).
        int n = sr * 8;
        var vento = new float[n];
        float lp = 0f, lp2 = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)sr;
            float white = (float)(r.NextDouble() * 2 - 1);
            lp += (white - lp) * 0.02f;
            lp2 += (lp - lp2) * 0.05f;
            float rajada = 0.55f + 0.45f * Mathf.Sin(t * Mathf.PI * 2f / 8f) * Mathf.Sin(t * Mathf.PI * 2f / 2.7f);
            float zumbido = 0.06f * Mathf.Sin(2f * Mathf.PI * 55f * t) + 0.03f * Mathf.Sin(2f * Mathf.PI * 110f * t);
            vento[i] = lp2 * 6f * rajada + zumbido;
        }
        int fade = sr / 2;
        for (int i = 0; i < fade; i++)
        {
            float a = i / (float)fade;
            vento[i] = vento[i] * a + vento[n - fade + i] * (1 - a);
        }
        Array.Resize(ref vento, n - fade);
        SalvarWav("Som_Ambiente_Vento", vento, sr);

        // Impacto metálico: parciais inarmônicos com decaimento + estalo de ruído.
        n = (int)(sr * 0.9f);
        var impacto = new float[n];
        float[] freqs = { 180f, 437f, 662f, 1033f, 1580f };
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)sr;
            float s = 0f;
            for (int k = 0; k < freqs.Length; k++) s += Mathf.Sin(2f * Mathf.PI * freqs[k] * t) * Mathf.Exp(-t * (4f + k * 2f)) / (k + 1);
            s += (float)(r.NextDouble() * 2 - 1) * Mathf.Exp(-t * 40f) * 0.8f;
            impacto[i] = s * 0.6f;
        }
        SalvarWav("Som_Pulso_Impacto", impacto, sr);

        // Bip de coleta: duas notas subindo.
        n = (int)(sr * 0.3f);
        var bip = new float[n];
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)sr;
            float f = t < 0.12f ? 880f : 1320f;
            float env = Mathf.Min(1f, t * 200f) * Mathf.Exp(-((t < 0.12f ? t : t - 0.12f)) * 12f);
            bip[i] = Mathf.Sin(2f * Mathf.PI * f * t) * env * 0.5f;
        }
        SalvarWav("Som_Coleta_Bip", bip, sr);

        // Ativação: varredura subindo + água correndo (ruído filtrado crescente).
        n = sr * 3;
        var ativ = new float[n];
        float phase = 0f, w = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)sr;
            float f = 120f + 600f * Mathf.Clamp01(t / 1.5f);
            phase += 2f * Mathf.PI * f / sr;
            float tom = Mathf.Sin(phase) * Mathf.Exp(-Mathf.Max(0f, t - 1.5f) * 2f) * 0.35f;
            float white = (float)(r.NextDouble() * 2 - 1);
            w += (white - w) * 0.15f;
            float agua = w * Mathf.Clamp01((t - 0.8f) / 1f) * 1.2f * (1f - Mathf.Clamp01((t - 2.5f) / 0.5f));
            ativ[i] = tom + agua;
        }
        SalvarWav("Som_Torre_Ativacao", ativ, sr);

        // Confirmação do cheat: arpejo rápido.
        n = (int)(sr * 0.45f);
        var cheat = new float[n];
        float[] notas = { 523f, 659f, 784f, 1047f };
        for (int i = 0; i < n; i++)
        {
            float t = i / (float)sr;
            int k = Mathf.Min(3, (int)(t / 0.09f));
            float lt = t - k * 0.09f;
            cheat[i] = Mathf.Sign(Mathf.Sin(2f * Mathf.PI * notas[k] * t)) * Mathf.Exp(-lt * 10f) * 0.18f;
        }
        SalvarWav("Som_Cheat_Confirmacao", cheat, sr);

        AssetDatabase.Refresh();
    }

    static void SalvarWav(string name, float[] samples, int sampleRate)
    {
        float peak = 0.0001f;
        foreach (var s in samples) peak = Mathf.Max(peak, Mathf.Abs(s));
        float gain = 0.9f / peak;

        using (var ms = new MemoryStream())
        using (var bw = new BinaryWriter(ms))
        {
            int dataLen = samples.Length * 2;
            bw.Write(new[] { 'R', 'I', 'F', 'F' });
            bw.Write(36 + dataLen);
            bw.Write(new[] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' });
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(sampleRate);
            bw.Write(sampleRate * 2);
            bw.Write((short)2);
            bw.Write((short)16);
            bw.Write(new[] { 'd', 'a', 't', 'a' });
            bw.Write(dataLen);
            foreach (var s in samples) bw.Write((short)Mathf.Clamp(s * gain * 32767f, -32768f, 32767f));
            File.WriteAllBytes(SoundDir + "/" + name + ".wav", ms.ToArray());
        }
    }

    static AudioClip Som(string name)
    {
        return AssetDatabase.LoadAssetAtPath<AudioClip>(SoundDir + "/" + name + ".wav");
    }

    // --------------------------------------------------------------- partículas

    static void CriarParticulas()
    {
        var circulo = TexturaCirculo();

        var matFumaca = MatParticula("Part_Fumaca", circulo, new Color(0.55f, 0.53f, 0.48f, 0.5f));
        var matPedra = MatParticula("Part_Pedrinhas", circulo, new Color(0.45f, 0.36f, 0.26f, 1f));
        var matVerde = MatParticula("Part_Purificacao", circulo, new Color(0.4f, 1f, 0.5f, 1f));
        var matPetala = MatParticula("Part_Petalas", circulo, new Color(1f, 0.85f, 0.95f, 1f));

        // Fumaça: fica na cena, em loop.
        var go = new GameObject("Fumaca");
        var ps = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.loop = true;
        main.playOnAwake = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(4f, 6f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.5f, 1f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.8f, 1.4f);
        main.startColor = new Color(0.55f, 0.53f, 0.48f, 0.6f);
        main.maxParticles = 200;
        var em = ps.emission;
        em.rateOverTime = 6f;
        var sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Cone;
        sh.angle = 12f;
        sh.radius = 0.4f;
        go.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        var sol = ps.sizeOverLifetime;
        sol.enabled = true;
        sol.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 0.6f, 1f, 3f));
        var col = ps.colorOverLifetime;
        col.enabled = true;
        col.color = Gradiente(new Color(0.6f, 0.58f, 0.5f), new Color(0.4f, 0.4f, 0.38f), 0.7f);
        var rot = ps.rotationOverLifetime;
        rot.enabled = true;
        rot.z = new ParticleSystem.MinMaxCurve(-0.5f, 0.5f);
        go.GetComponent<ParticleSystemRenderer>().sharedMaterial = matFumaca;
        SalvarPrefab(go, PrefabRoot + "/Fumaca.prefab");

        // Pedrinhas: criadas pelo código no impacto do pulso. Play On Awake LIGADO, sem loop.
        go = new GameObject("Particula_Pedrinhas");
        ps = go.AddComponent<ParticleSystem>();
        main = ps.main;
        main.loop = false;
        main.playOnAwake = true;
        main.duration = 1f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.8f, 1.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.2f);
        main.gravityModifier = 1f;
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.45f, 0.35f, 0.25f), new Color(0.55f, 0.53f, 0.5f));
        em = ps.emission;
        em.rateOverTime = 0f;
        em.SetBursts(new[] { new ParticleSystem.Burst(0f, 20) });
        sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Sphere;
        sh.radius = 0.2f;
        go.GetComponent<ParticleSystemRenderer>().sharedMaterial = matPedra;
        SalvarPrefab(go, PrefabRoot + "/Particula_Pedrinhas.prefab");

        // Purificação: explosão verde ao ativar a torre, com pétalas num segundo sistema.
        go = new GameObject("Particula_Purificacao");
        ps = go.AddComponent<ParticleSystem>();
        main = ps.main;
        main.loop = false;
        main.playOnAwake = true;
        main.duration = 2f;
        main.startLifetime = new ParticleSystem.MinMaxCurve(1.6f, 2.2f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(5f, 10f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.25f, 0.6f);
        main.startColor = new ParticleSystem.MinMaxGradient(new Color(0.4f, 1f, 0.5f), new Color(0.75f, 1f, 0.6f));
        main.gravityModifier = 0.1f;
        main.maxParticles = 400;
        em = ps.emission;
        em.rateOverTime = 0f;
        em.SetBursts(new[] { new ParticleSystem.Burst(0f, 200) });
        sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Sphere;
        sh.radius = 0.5f;
        col = ps.colorOverLifetime;
        col.enabled = true;
        col.color = Gradiente(new Color(0.6f, 1f, 0.6f), new Color(0.2f, 0.9f, 0.4f), 1f);
        go.GetComponent<ParticleSystemRenderer>().sharedMaterial = matVerde;

        var petalas = new GameObject("Petalas");
        petalas.transform.SetParent(go.transform, false);
        var ps2 = petalas.AddComponent<ParticleSystem>();
        var main2 = ps2.main;
        main2.loop = false;
        main2.playOnAwake = true;
        main2.duration = 2f;
        main2.startLifetime = new ParticleSystem.MinMaxCurve(2.5f, 3.5f);
        main2.startSpeed = new ParticleSystem.MinMaxCurve(2f, 5f);
        main2.startSize = new ParticleSystem.MinMaxCurve(0.15f, 0.3f);
        main2.gravityModifier = 0.25f;
        main2.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.8f, 0.9f), new Color(1f, 1f, 0.7f));
        var em2 = ps2.emission;
        em2.rateOverTime = 0f;
        em2.SetBursts(new[] { new ParticleSystem.Burst(0.1f, 60) });
        var sh2 = ps2.shape;
        sh2.shapeType = ParticleSystemShapeType.Sphere;
        sh2.radius = 1f;
        var rot2 = ps2.rotationOverLifetime;
        rot2.enabled = true;
        rot2.z = new ParticleSystem.MinMaxCurve(-3f, 3f);
        petalas.GetComponent<ParticleSystemRenderer>().sharedMaterial = matPetala;
        SalvarPrefab(go, PrefabRoot + "/Particula_Purificacao.prefab");
    }

    static Gradient Gradiente(Color a, Color b, float alpha0)
    {
        var g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(a, 0f), new GradientColorKey(b, 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(alpha0, 0.15f), new GradientAlphaKey(alpha0, 0.6f), new GradientAlphaKey(0f, 1f) });
        return g;
    }

    static Texture2D TexturaCirculo()
    {
        string path = TexDir + "/Particula_Circulo.png";
        var tex = new Texture2D(64, 64, TextureFormat.RGBA32, false);
        for (int y = 0; y < 64; y++)
            for (int x = 0; x < 64; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(31.5f, 31.5f)) / 32f;
                float a = Mathf.Clamp01(1f - d);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a * a));
            }
        File.WriteAllBytes(path, tex.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);
        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.alphaIsTransparency = true;
        imp.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
    }

    static Material MatParticula(string name, Texture2D tex, Color color)
    {
        string path = MatDir + "/" + name + ".mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
            AssetDatabase.CreateAsset(mat, path);
        }
        mat.SetTexture("_BaseMap", tex);
        mat.SetColor("_BaseColor", color);
        // Transparente com alpha blend.
        mat.SetFloat("_Surface", 1f);
        mat.SetFloat("_Blend", 0f);
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        EditorUtility.SetDirty(mat);
        return mat;
    }

    static GameObject SalvarPrefab(GameObject go, string path)
    {
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        UnityEngine.Object.DestroyImmediate(go);
        return prefab;
    }

    // --------------------------------------------------- prefabs de cenário (75)

    class Def
    {
        public string nome;
        public int categoria;
        public float massa; // > 0: entulho com física (Rigidbody + tag HeavyDebris)
        public Action<Transform> montar;
        public bool atravessavel; // sem collider (grama, líquen...)
    }

    static List<Def> defs = new List<Def>();

    static void D(string nome, int cat, Action<Transform> montar, float massa = 0f, bool atravessavel = false)
    {
        defs.Add(new Def { nome = nome, categoria = cat, montar = montar, massa = massa, atravessavel = atravessavel });
    }

    static void CriarPrefabsDeCenario()
    {
        defs.Clear();

        // Categoria 1: terreno, geologia e formações rochosas (20)
        D("Rocha_Falesia_A", 1, t => Rocha(t, "Falesia_A", 11, new Vector3(8, 10, 6), "Rocha", 0.35f));
        D("Rocha_Falesia_B", 1, t => Rocha(t, "Falesia_B", 12, new Vector3(6, 13, 7), "RochaEscura", 0.4f));
        D("Pedregulho_Pontiagudo", 1, t => Rocha(t, "Pontiagudo", 13, new Vector3(2, 2.6f, 2), "Rocha", 0.55f));
        D("Pedregulho_Arredondado", 1, t => Rocha(t, "Arredondado", 14, new Vector3(2.2f, 1.8f, 2.2f), "RochaClara", 0.12f));
        D("Laje_Pedra_Chata", 1, t => Rocha(t, "Laje", 15, new Vector3(5, 0.7f, 4), "RochaClara", 0.15f));
        D("Seixos_Conjunto", 1, t =>
        {
            for (int i = 0; i < 7; i++)
                Rocha(t, "Seixo_" + i, 100 + i, Vector3.one * (0.25f + i * 0.04f), i % 2 == 0 ? "Cascalho" : "RochaClara", 0.2f)
                    .localPosition = new Vector3(Mathf.Cos(i * 2.4f) * (0.3f + i * 0.12f), 0f, Mathf.Sin(i * 2.4f) * (0.3f + i * 0.12f));
        }, atravessavel: true);
        D("Arco_Rochoso", 1, t =>
        {
            Rocha(t, "Arco_PernaA", 16, new Vector3(2, 7, 2.5f), "Rocha", 0.3f).localPosition = new Vector3(-3.5f, 0, 0);
            Rocha(t, "Arco_PernaB", 17, new Vector3(2, 7, 2.5f), "Rocha", 0.3f).localPosition = new Vector3(3.5f, 0, 0);
            Rocha(t, "Arco_Topo", 18, new Vector3(9.5f, 2, 2.6f), "Rocha", 0.25f).localPosition = new Vector3(0, 6.2f, 0);
        });
        D("Paredao_Canion", 1, t => Rocha(t, "Paredao", 19, new Vector3(14, 12, 4), "RochaEscura", 0.25f));
        D("Coluna_Basaltica", 1, t =>
        {
            for (int i = 0; i < 5; i++)
                P(t, PrimitiveType.Cylinder, new Vector3((i % 3 - 1) * 0.9f, (2f + i * 0.6f) / 2f, (i / 3) * 0.8f),
                    new Vector3(0.9f, (2f + i * 0.6f) / 2f, 0.9f), Vector3.zero, "Basalto");
        });
        D("Bloco_Rochoso_Quebravel", 1, t => Rocha(t, "BlocoQuebravel", 20, new Vector3(1.6f, 1.6f, 1.6f), "Rocha", 0.3f), massa: 12f);
        D("Rampa_Terra_Batida", 1, t => P(t, PrimitiveType.Cube, new Vector3(0, 0.6f, 0), new Vector3(4, 0.4f, 7), new Vector3(-12, 0, 0), "Terra"));
        D("Talude_Cascalho", 1, t => Rocha(t, "Talude", 21, new Vector3(6, 1.5f, 3), "Cascalho", 0.2f));
        D("Calha_Fluvial_Seca", 1, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.1f, 0), new Vector3(3, 0.2f, 8), Vector3.zero, "Cascalho");
            P(t, PrimitiveType.Cube, new Vector3(-1.8f, 0.35f, 0), new Vector3(0.8f, 0.4f, 8), new Vector3(0, 0, 25), "RochaClara");
            P(t, PrimitiveType.Cube, new Vector3(1.8f, 0.35f, 0), new Vector3(0.8f, 0.4f, 8), new Vector3(0, 0, -25), "RochaClara");
        });
        D("Margem_Seixos_Rio", 1, t =>
        {
            for (int i = 0; i < 10; i++)
                Rocha(t, "Margem_" + i, 200 + i, new Vector3(0.6f, 0.35f, 0.5f), "RochaClara", 0.25f).localPosition = new Vector3(i * 0.55f - 2.5f, 0, Mathf.Sin(i) * 0.4f);
        }, atravessavel: true);
        D("Cratera_Erosao", 1, t =>
        {
            for (int i = 0; i < 9; i++)
            {
                float a = i / 9f * Mathf.PI * 2f;
                Rocha(t, "Cratera_" + i, 300 + i, new Vector3(1.4f, 0.6f, 1f), "Terra", 0.3f).localPosition = new Vector3(Mathf.Cos(a) * 2.5f, 0, Mathf.Sin(a) * 2.5f);
            }
        });
        D("Rocha_Lascada", 1, t => Rocha(t, "Lascada", 22, new Vector3(1.4f, 2.2f, 0.9f), "RochaEscura", 0.6f));
        D("Pedra_Pequena", 1, t => Rocha(t, "PedraPequena", 23, new Vector3(0.7f, 0.5f, 0.6f), "Rocha", 0.3f), atravessavel: true);
        D("Pedra_Alongada", 1, t => Rocha(t, "Alongada", 24, new Vector3(4, 1.2f, 1.2f), "RochaClara", 0.25f));
        D("Rocha_Musgo", 1, t =>
        {
            Rocha(t, "RochaMusgo", 25, new Vector3(2.5f, 1.6f, 2.2f), "Rocha", 0.2f);
            Rocha(t, "RochaMusgo_Capa", 26, new Vector3(2.3f, 0.5f, 2f), "Musgo", 0.2f).localPosition = new Vector3(0, 1.3f, 0);
        });
        D("Pilha_Pedras", 1, t =>
        {
            Rocha(t, "Pilha_1", 27, new Vector3(1.4f, 0.8f, 1.2f), "Rocha", 0.2f);
            Rocha(t, "Pilha_2", 28, new Vector3(1f, 0.6f, 0.9f), "RochaClara", 0.2f).localPosition = new Vector3(0.1f, 0.7f, 0);
            Rocha(t, "Pilha_3", 29, new Vector3(0.6f, 0.5f, 0.6f), "Rocha", 0.2f).localPosition = new Vector3(0, 1.2f, 0.05f);
        });

        // Categoria 2: flora árida e bioativa (16)
        D("Arvore_Petrificada", 2, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 3, 0), new Vector3(0.6f, 3, 0.6f), Vector3.zero, "Petrificado");
            P(t, PrimitiveType.Cylinder, new Vector3(0.9f, 5.2f, 0), new Vector3(0.25f, 1.3f, 0.25f), new Vector3(0, 0, -40), "Petrificado");
            P(t, PrimitiveType.Cylinder, new Vector3(-0.8f, 4.5f, 0.3f), new Vector3(0.2f, 1f, 0.2f), new Vector3(20, 0, 45), "Petrificado");
        });
        D("Arvore_Caida", 2, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.4f, 0), new Vector3(0.7f, 4f, 0.7f), new Vector3(0, 0, 90), "MadeiraSeca");
            P(t, PrimitiveType.Cylinder, new Vector3(2.5f, 0.9f, 0.5f), new Vector3(0.25f, 1f, 0.25f), new Vector3(40, 0, 60), "MadeiraSeca");
        });
        D("Toco_Oco", 2, t =>
        {
            for (int i = 0; i < 6; i++)
            {
                float a = i / 6f * Mathf.PI * 2f;
                P(t, PrimitiveType.Cube, new Vector3(Mathf.Cos(a) * 0.45f, 0.5f, Mathf.Sin(a) * 0.45f), new Vector3(0.35f, 1f + (i % 2) * 0.3f, 0.2f), new Vector3(0, -a * Mathf.Rad2Deg + 90, 0), "Madeira");
            }
        });
        D("Raiz_Gigante", 2, t =>
        {
            for (int i = 0; i < 4; i++)
                P(t, PrimitiveType.Capsule, new Vector3(Mathf.Cos(i * 1.6f) * 1.4f, 0.3f, Mathf.Sin(i * 1.6f) * 1.4f), new Vector3(0.4f, 1.6f, 0.4f), new Vector3(80, i * 92f, 0), "MadeiraSeca");
        });
        D("Arbusto_Seco_Espinhoso", 2, t => Galhos(t, 9, 0.9f, "FolhaSeca", 1));
        D("Espinheiro_Rasteiro", 2, t => Galhos(t, 12, 0.5f, "MadeiraSeca", 2), atravessavel: true);
        D("Arbusto_Queimado", 2, t => Galhos(t, 8, 1.1f, "Queimado", 3));
        D("Muda_Bioluminescente", 2, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.35f, 0), new Vector3(0.06f, 0.35f, 0.06f), Vector3.zero, "Bioluminescente");
            P(t, PrimitiveType.Sphere, new Vector3(0.12f, 0.7f, 0), new Vector3(0.25f, 0.08f, 0.15f), Vector3.zero, "FolhaVerde");
            P(t, PrimitiveType.Sphere, new Vector3(-0.12f, 0.6f, 0), new Vector3(0.25f, 0.08f, 0.15f), Vector3.zero, "FolhaVerde");
        }, atravessavel: true);
        D("Flor_Esporos_Azuis", 2, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.4f, 0), new Vector3(0.05f, 0.4f, 0.05f), Vector3.zero, "FolhaVerde");
            for (int i = 0; i < 5; i++)
                P(t, PrimitiveType.Sphere, new Vector3(Mathf.Cos(i * 1.26f) * 0.15f, 0.82f, Mathf.Sin(i * 1.26f) * 0.15f), Vector3.one * 0.14f, Vector3.zero, "EsporoAzul");
        }, atravessavel: true);
        D("Moita_Grama_Alta", 2, t =>
        {
            for (int i = 0; i < 14; i++)
                P(t, PrimitiveType.Cube, new Vector3(Mathf.Cos(i * 2.1f) * 0.3f, 0.45f, Mathf.Sin(i * 2.1f) * 0.3f), new Vector3(0.05f, 0.9f, 0.12f), new Vector3((i % 3 - 1) * 15, i * 25, (i % 2) * 12), "Grama");
        }, atravessavel: true);
        D("Trepadeiras_Encosta", 2, t =>
        {
            for (int i = 0; i < 8; i++)
                P(t, PrimitiveType.Capsule, new Vector3(i * 0.35f - 1.2f, 1.2f + Mathf.Sin(i) * 0.4f, 0), new Vector3(0.12f, 1.2f, 0.12f), new Vector3(0, 0, (i % 2 == 0 ? 10 : -10)), "FolhaVerde");
        }, atravessavel: true);
        D("Cogumelo_Filtrador", 2, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.3f, 0), new Vector3(0.15f, 0.3f, 0.15f), Vector3.zero, "MadeiraSeca");
            P(t, PrimitiveType.Sphere, new Vector3(0, 0.62f, 0), new Vector3(0.6f, 0.25f, 0.6f), Vector3.zero, "Bioluminescente");
        }, atravessavel: true);
        D("Liquen_Fluorescente", 2, t =>
        {
            for (int i = 0; i < 6; i++)
                P(t, PrimitiveType.Sphere, new Vector3(Mathf.Cos(i) * 0.4f, 0.03f, Mathf.Sin(i * 1.7f) * 0.4f), new Vector3(0.4f, 0.06f, 0.3f), Vector3.zero, "Liquen");
        }, atravessavel: true);
        D("Juncos_Aquaticos", 2, t =>
        {
            for (int i = 0; i < 10; i++)
            {
                var x = Mathf.Cos(i * 2.4f) * 0.35f;
                var z = Mathf.Sin(i * 2.4f) * 0.35f;
                P(t, PrimitiveType.Cylinder, new Vector3(x, 0.7f, z), new Vector3(0.04f, 0.7f, 0.04f), new Vector3(i % 3 * 5, 0, i % 2 * 6), "Musgo");
                if (i % 3 == 0) P(t, PrimitiveType.Capsule, new Vector3(x, 1.3f, z), new Vector3(0.08f, 0.15f, 0.08f), Vector3.zero, "Madeira");
            }
        }, atravessavel: true);
        D("Arbusto_Frutifero", 2, t =>
        {
            P(t, PrimitiveType.Sphere, new Vector3(0, 0.7f, 0), new Vector3(1.4f, 1.1f, 1.4f), Vector3.zero, "FolhaVerde");
            for (int i = 0; i < 7; i++)
                P(t, PrimitiveType.Sphere, new Vector3(Mathf.Cos(i) * 0.65f, 0.7f + Mathf.Sin(i * 3) * 0.3f, Mathf.Sin(i) * 0.65f), Vector3.one * 0.15f, Vector3.zero, "Fruto");
        });
        D("Cacto_Seco", 2, t =>
        {
            P(t, PrimitiveType.Capsule, new Vector3(0, 1.2f, 0), new Vector3(0.5f, 1.2f, 0.5f), Vector3.zero, "Musgo");
            P(t, PrimitiveType.Capsule, new Vector3(0.45f, 1.4f, 0), new Vector3(0.3f, 0.5f, 0.3f), new Vector3(0, 0, -30), "Musgo");
        });

        // Categoria 3: resíduos industriais e sucatas com física (19)
        D("Tambor_Oleo_Amassado", 3, t => P(t, PrimitiveType.Cylinder, new Vector3(0, 0.5f, 0), new Vector3(0.75f, 0.5f, 0.68f), new Vector3(0, 0, 4), "Azul"), massa: 15f);
        D("Barril_Contrapeso", 3, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.65f, 0), new Vector3(1f, 0.65f, 1f), Vector3.zero, "Amarelo");
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.65f, 0), new Vector3(1.04f, 0.06f, 1.04f), Vector3.zero, "Preto", false);
        }, massa: 50f);
        D("Cacamba_Enferrujada", 3, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.1f, 0), new Vector3(4, 0.2f, 2.2f), Vector3.zero, "Ferrugem");
            P(t, PrimitiveType.Cube, new Vector3(0, 0.8f, 1.05f), new Vector3(4, 1.6f, 0.1f), Vector3.zero, "Ferrugem");
            P(t, PrimitiveType.Cube, new Vector3(0, 0.8f, -1.05f), new Vector3(4, 1.6f, 0.1f), Vector3.zero, "Ferrugem");
            P(t, PrimitiveType.Cube, new Vector3(1.95f, 0.8f, 0), new Vector3(0.1f, 1.6f, 2.2f), Vector3.zero, "Ferrugem");
            P(t, PrimitiveType.Cube, new Vector3(-1.95f, 0.8f, 0), new Vector3(0.1f, 1.6f, 2.2f), Vector3.zero, "Ferrugem");
        });
        D("Duto_Curvo_Quebrado", 3, t =>
        {
            for (int i = 0; i < 5; i++)
            {
                float a = i / 4f * Mathf.PI / 2f;
                P(t, PrimitiveType.Cylinder, new Vector3(Mathf.Sin(a) * 2f, 0.6f, Mathf.Cos(a) * 2f - 2f), new Vector3(1.2f, 0.45f, 1.2f), new Vector3(90, a * Mathf.Rad2Deg, 0), "AcoEscuro");
            }
        });
        D("Duto_Reto_Quebrado", 3, t => P(t, PrimitiveType.Cylinder, new Vector3(0, 0.6f, 0), new Vector3(1.2f, 2.5f, 1.2f), new Vector3(90, 0, 0), "Aco"));
        D("Caixa_Madeira", 3, t => P(t, PrimitiveType.Cube, new Vector3(0, 0.6f, 0), Vector3.one * 1.2f, Vector3.zero, "Madeira"), massa: 6f);
        D("Caixote_Aco_Reforcado", 3, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.75f, 0), Vector3.one * 1.5f, Vector3.zero, "AcoEscuro");
            P(t, PrimitiveType.Cube, new Vector3(0, 0.75f, 0), new Vector3(1.55f, 0.2f, 1.55f), Vector3.zero, "Amarelo", false);
        });
        D("Viga_I_Torcida", 3, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.15f, 0), new Vector3(6, 0.1f, 0.5f), new Vector3(0, 0, 6), "Ferrugem");
            P(t, PrimitiveType.Cube, new Vector3(0, 0.45f, 0), new Vector3(6, 0.6f, 0.08f), new Vector3(8, 0, 6), "Ferrugem");
            P(t, PrimitiveType.Cube, new Vector3(0, 0.75f, 0), new Vector3(6, 0.1f, 0.5f), new Vector3(16, 0, 6), "Ferrugem");
        });
        D("Engrenagem_Desgastada", 3, t => Engrenagem(t, 0.75f, 14, "Ferrugem"));
        D("Roda_Dentada_Pequena", 3, t => Engrenagem(t, 0.3f, 10, "Aco"), atravessavel: true);
        D("Rolo_Cabos_Cobre", 3, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.6f, 0), new Vector3(1.2f, 0.4f, 1.2f), new Vector3(90, 0, 0), "Cobre");
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.6f, 0), new Vector3(1.5f, 0.05f, 1.5f), new Vector3(90, 0, 0), "Madeira", false);
        });
        D("Pneu_Industrial", 3, t => Toro(t, "Pneu", 0.9f, 0.4f, "Borracha", new Vector3(0, 0.4f, 0), Vector3.zero));
        D("Placa_Corrugada", 3, t =>
        {
            for (int i = 0; i < 8; i++)
                P(t, PrimitiveType.Cube, new Vector3(i * 0.3f - 1.05f, 0.05f + (i % 2) * 0.08f, 0), new Vector3(0.32f, 0.04f, 2.2f), new Vector3(0, 0, (i % 2 == 0 ? 25 : -25)), "Aco");
        });
        D("Chapa_Gangorra", 3, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.55f, 0), new Vector3(5, 0.12f, 1.2f), new Vector3(0, 0, 10), "AcoEscuro");
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.25f, 0), new Vector3(0.5f, 0.6f, 0.5f), new Vector3(90, 0, 0), "Concreto");
        });
        D("Gerador_Desativado", 3, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.8f, 0), new Vector3(2.5f, 1.4f, 1.4f), Vector3.zero, "Verde");
            P(t, PrimitiveType.Cylinder, new Vector3(0.8f, 1.9f, 0.3f), new Vector3(0.2f, 0.4f, 0.2f), Vector3.zero, "Preto");
            P(t, PrimitiveType.Cube, new Vector3(0, 0.05f, 0), new Vector3(2.8f, 0.1f, 1.6f), Vector3.zero, "AcoEscuro");
        });
        D("Tanque_Pressao_Esferico", 3, t =>
        {
            P(t, PrimitiveType.Sphere, new Vector3(0, 1.8f, 0), Vector3.one * 2.4f, Vector3.zero, "Torre");
            for (int i = 0; i < 4; i++)
                P(t, PrimitiveType.Cylinder, new Vector3((i % 2 * 2 - 1) * 0.8f, 0.6f, (i / 2 * 2 - 1) * 0.8f), new Vector3(0.15f, 0.6f, 0.15f), Vector3.zero, "AcoEscuro");
        });
        D("Escada_Incendio_Torcida", 3, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(-0.5f, 1.5f, 0), new Vector3(0.08f, 3, 0.08f), new Vector3(0, 0, 8), "Vermelho");
            P(t, PrimitiveType.Cube, new Vector3(0.5f, 1.5f, 0), new Vector3(0.08f, 3, 0.08f), new Vector3(0, 0, 14), "Vermelho");
            for (int i = 0; i < 7; i++)
                P(t, PrimitiveType.Cube, new Vector3(i * 0.06f, 0.3f + i * 0.4f, 0), new Vector3(1f, 0.05f, 0.05f), new Vector3(0, i * 4, i * 2), "Vermelho", false);
        });
        D("Conteiner_Leve", 3, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.8f, 0), new Vector3(3f, 1.6f, 1.6f), Vector3.zero, "Vermelho");
            for (int i = 0; i < 5; i++)
                P(t, PrimitiveType.Cube, new Vector3(i * 0.6f - 1.2f, 0.8f, 0.82f), new Vector3(0.08f, 1.5f, 0.05f), Vector3.zero, "Ferrugem", false);
        }, massa: 12f);
        D("Pedra_Entulho_Fisica", 3, t => Rocha(t, "PedraEntulho", 30, new Vector3(1.5f, 1.2f, 1.4f), "RochaEscura", 0.35f), massa: 10f);

        // Categoria 4: infraestrutura, hidráulica e mecanismos (12)
        D("Torre_Purificadora_Modelo", 4, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 6, 0), new Vector3(3, 6, 3), Vector3.zero, "Torre");
            P(t, PrimitiveType.Cylinder, new Vector3(0, 12.4f, 0), new Vector3(4.2f, 0.4f, 4.2f), Vector3.zero, "AcoEscuro");
            P(t, PrimitiveType.Sphere, new Vector3(0, 13.8f, 0), Vector3.one * 2.6f, Vector3.zero, "Energia");
            for (int i = 0; i < 4; i++)
                P(t, PrimitiveType.Cube, new Vector3(Mathf.Cos(i * Mathf.PI / 2) * 1.6f, 6, Mathf.Sin(i * Mathf.PI / 2) * 1.6f), new Vector3(0.3f, 11f, 0.3f), new Vector3(0, -i * 90, 0), "Energia", false);
        });
        D("Console_Terminal", 4, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.55f, 0), new Vector3(1.4f, 1.1f, 0.7f), Vector3.zero, "AcoEscuro");
            P(t, PrimitiveType.Cube, new Vector3(0, 1.25f, -0.1f), new Vector3(1.2f, 0.6f, 0.08f), new Vector3(-20, 0, 0), "Tela");
        });
        D("Comporta_Represa", 4, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 4, 0), new Vector3(14, 8, 2), Vector3.zero, "Concreto");
            P(t, PrimitiveType.Cube, new Vector3(0, 2.2f, -1.05f), new Vector3(4, 4.4f, 0.2f), Vector3.zero, "Ferrugem");
        });
        D("Valvula_Manual", 4, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.5f, 0), new Vector3(0.2f, 0.5f, 0.2f), Vector3.zero, "AcoEscuro");
            Toro(t, "Valvula", 0.4f, 0.05f, "Vermelho", new Vector3(0, 1.05f, 0), Vector3.zero);
        });
        D("Passarela_Grade", 4, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 1.5f, 0), new Vector3(1.6f, 0.08f, 8), Vector3.zero, "AcoEscuro");
            for (int i = 0; i < 2; i++)
            {
                P(t, PrimitiveType.Cube, new Vector3(0, 0.75f, i == 0 ? -3.6f : 3.6f), new Vector3(0.2f, 1.5f, 0.2f), Vector3.zero, "Aco");
            }
        });
        D("Pilar_Concreto", 4, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 3, 0), new Vector3(1.2f, 6, 1.2f), Vector3.zero, "Concreto");
            P(t, PrimitiveType.Cylinder, new Vector3(0.3f, 6.3f, 0), new Vector3(0.06f, 0.4f, 0.06f), new Vector3(0, 0, 15), "Ferrugem", false);
        });
        D("Corrimao_Metalico", 4, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 1f, 0), new Vector3(0.06f, 2f, 0.06f), new Vector3(0, 0, 90), "Ferrugem");
            for (int i = 0; i < 4; i++)
                P(t, PrimitiveType.Cylinder, new Vector3(i * 1.3f - 1.95f, 0.5f, 0), new Vector3(0.06f, 0.5f, 0.06f), new Vector3(0, 0, i == 2 ? 12 : 0), "Ferrugem");
        });
        D("Tubo_Drenagem", 4, t =>
        {
            Toro(t, "Boca_Tubo", 0.9f, 0.18f, "Concreto", new Vector3(0, 0.9f, 0), new Vector3(90, 0, 0));
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.9f, 1.5f), new Vector3(1.6f, 1.5f, 1.6f), new Vector3(90, 0, 0), "Concreto");
        });
        D("Caixa_Eletrica_Quebrada", 4, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.9f, 0), new Vector3(0.9f, 1.2f, 0.45f), Vector3.zero, "Aco");
            P(t, PrimitiveType.Cube, new Vector3(0.5f, 0.9f, -0.3f), new Vector3(0.05f, 1.1f, 0.8f), new Vector3(0, -50, 0), "Aco", false);
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.15f, 0), new Vector3(0.1f, 0.15f, 0.1f), Vector3.zero, "Preto");
        });
        D("Poste_Sinalizacao_Inclinado", 4, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0.6f, 2.5f, 0), new Vector3(0.15f, 2.5f, 0.15f), new Vector3(0, 0, -14), "AcoEscuro");
            P(t, PrimitiveType.Cube, new Vector3(1.2f, 4.6f, 0), new Vector3(0.9f, 0.6f, 0.05f), new Vector3(0, 0, -14), "Amarelo");
        });
        D("Placa_Aviso_Toxico", 4, t =>
        {
            P(t, PrimitiveType.Cube, new Vector3(0, 0.6f, 0), new Vector3(0.08f, 1.2f, 0.08f), Vector3.zero, "Madeira");
            P(t, PrimitiveType.Cube, new Vector3(0, 1.3f, 0), new Vector3(0.8f, 0.6f, 0.04f), new Vector3(0, 0, 45), "Amarelo");
        });
        D("Cerca_Arame", 4, t =>
        {
            for (int i = 0; i < 4; i++)
                P(t, PrimitiveType.Cylinder, new Vector3(i * 1.5f - 2.25f, 0.7f, 0), new Vector3(0.08f, 0.7f, 0.08f), new Vector3(0, 0, (i - 1.5f) * 4), "Madeira");
            for (int k = 0; k < 3; k++)
                P(t, PrimitiveType.Cube, new Vector3(0, 0.4f + k * 0.4f, 0), new Vector3(4.6f, 0.02f, 0.02f), Vector3.zero, "AcoEscuro", false);
        });

        // Categoria 5: entidades vivas, fauna e easter eggs (8)
        D("Drone_ARIA", 5, t =>
        {
            P(t, PrimitiveType.Sphere, new Vector3(0, 2.2f, 0), Vector3.one * 0.6f, Vector3.zero, "Torre", false);
            P(t, PrimitiveType.Sphere, new Vector3(0, 2.25f, -0.25f), new Vector3(0.35f, 0.15f, 0.15f), Vector3.zero, "Tela", false);
            Toro(t, "Anel_ARIA", 0.5f, 0.04f, "Energia", new Vector3(0, 2.2f, 0), Vector3.zero, false);
        }, atravessavel: true);
        D("Lagarto_Rochas", 5, t =>
        {
            P(t, PrimitiveType.Capsule, new Vector3(0, 0.08f, 0), new Vector3(0.12f, 0.2f, 0.12f), new Vector3(90, 0, 0), "Pele");
            P(t, PrimitiveType.Sphere, new Vector3(0, 0.09f, 0.25f), new Vector3(0.12f, 0.08f, 0.14f), Vector3.zero, "Pele");
            P(t, PrimitiveType.Capsule, new Vector3(0, 0.06f, -0.33f), new Vector3(0.05f, 0.15f, 0.05f), new Vector3(90, 0, 0), "Pele");
        }, atravessavel: true);
        D("Passaro_Canyon_Swift", 5, t =>
        {
            P(t, PrimitiveType.Capsule, new Vector3(0, 0, 0), new Vector3(0.15f, 0.2f, 0.15f), new Vector3(90, 0, 0), "Pena");
            P(t, PrimitiveType.Cube, new Vector3(0.25f, 0.02f, 0), new Vector3(0.4f, 0.02f, 0.15f), new Vector3(0, 0, 15), "Pena");
            P(t, PrimitiveType.Cube, new Vector3(-0.25f, 0.02f, 0), new Vector3(0.4f, 0.02f, 0.15f), new Vector3(0, 0, -15), "Pena");
        }, atravessavel: true);
        D("Lebre_Prado", 5, t =>
        {
            P(t, PrimitiveType.Sphere, new Vector3(0, 0.2f, 0), new Vector3(0.3f, 0.28f, 0.42f), Vector3.zero, "Pelo");
            P(t, PrimitiveType.Sphere, new Vector3(0, 0.38f, 0.2f), Vector3.one * 0.18f, Vector3.zero, "Pelo");
            P(t, PrimitiveType.Capsule, new Vector3(0.04f, 0.56f, 0.18f), new Vector3(0.04f, 0.1f, 0.03f), new Vector3(-10, 0, 8), "Pelo");
            P(t, PrimitiveType.Capsule, new Vector3(-0.04f, 0.56f, 0.18f), new Vector3(0.04f, 0.1f, 0.03f), new Vector3(-10, 0, -8), "Pelo");
        }, atravessavel: true);
        D("Besouro_Blindado", 5, t =>
        {
            P(t, PrimitiveType.Sphere, new Vector3(0, 0.1f, 0), new Vector3(0.3f, 0.15f, 0.4f), Vector3.zero, "Aco");
            P(t, PrimitiveType.Sphere, new Vector3(0, 0.08f, 0.22f), Vector3.one * 0.12f, Vector3.zero, "Preto");
        }, atravessavel: true);
        D("Peixe_Cintilante", 5, t =>
        {
            P(t, PrimitiveType.Sphere, new Vector3(0, 0.15f, 0), new Vector3(0.12f, 0.15f, 0.4f), Vector3.zero, "Peixe");
            P(t, PrimitiveType.Cube, new Vector3(0, 0.15f, -0.25f), new Vector3(0.02f, 0.15f, 0.12f), new Vector3(45, 0, 0), "Peixe");
        }, atravessavel: true);
        D("EasterEgg_Esteira_Bota", 5, t =>
        {
            for (int i = 0; i < 6; i++)
                P(t, PrimitiveType.Cube, new Vector3(0, 0.05f, i * 0.32f - 0.8f), new Vector3(0.7f, 0.1f, 0.28f), Vector3.zero, "AcoEscuro");
            P(t, PrimitiveType.Cube, new Vector3(0.9f, 0.15f, 0), new Vector3(0.3f, 0.3f, 0.55f), Vector3.zero, "Bota");
            P(t, PrimitiveType.Cylinder, new Vector3(0.9f, 0.45f, -0.1f), new Vector3(0.28f, 0.2f, 0.28f), Vector3.zero, "Bota");
            P(t, PrimitiveType.Cylinder, new Vector3(0.9f, 0.75f, -0.1f), new Vector3(0.03f, 0.12f, 0.03f), Vector3.zero, "FolhaVerde", false);
            P(t, PrimitiveType.Sphere, new Vector3(0.95f, 0.88f, -0.1f), new Vector3(0.1f, 0.04f, 0.06f), Vector3.zero, "FolhaVerde", false);
        });
        D("EasterEgg_Extintor_Vintage", 5, t =>
        {
            P(t, PrimitiveType.Cylinder, new Vector3(0, 0.3f, 0), new Vector3(0.22f, 0.3f, 0.22f), new Vector3(0, 0, 70), "Vermelho");
            P(t, PrimitiveType.Cylinder, new Vector3(0.38f, 0.32f, 0), new Vector3(0.05f, 0.06f, 0.05f), new Vector3(0, 0, 70), "Ferrugem", false);
        }, atravessavel: true);

        AdicionarModelosDaLoja();

        int i2 = 0;
        foreach (var def in defs)
        {
            EditorUtility.DisplayProgressBar("Nivel1", "Prefab " + def.nome, 0.3f + 0.3f * i2++ / defs.Count);
            prefabs[def.nome] = CriarPrefab(def);
        }
    }

    static GameObject CriarPrefab(Def def)
    {
        var root = new GameObject(def.nome);
        if (!Modelos.TryGetValue(def.nome, out var modelo) || !ModeloLoja(root.transform, modelo.caminho, modelo.tamanho))
        {
            def.montar(root.transform);
        }

        if (def.atravessavel)
        {
            foreach (var c in root.GetComponentsInChildren<Collider>()) UnityEngine.Object.DestroyImmediate(c);
        }

        if (def.massa > 0f)
        {
            var rb = root.AddComponent<Rigidbody>();
            rb.mass = def.massa;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            // O pulso testa a tag no objeto do collider, então todos os filhos levam a tag.
            foreach (var tr in root.GetComponentsInChildren<Transform>()) tr.gameObject.tag = "HeavyDebris";
        }

        foreach (var tr in root.GetComponentsInChildren<Transform>())
        {
            if (def.massa <= 0f) GameObjectUtility.SetStaticEditorFlags(tr.gameObject, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
        }

        return SalvarPrefab(root, AssetPrefabDir + "/" + def.nome + ".prefab");
    }

    // ------------------------------------------------- modelos da Asset Store

    // Pastas dos pacotes grátis importados (ver Apêndice B).
    static readonly string[] PastasLoja =
    {
        "Assets/BrokenVector", "Assets/JC_StylizedRocks_Lite", "Assets/SimpleNaturePack", "Assets/Dry_Trees",
        "Assets/Donthrukat", "Assets/PBS Barrels and Crates", "Assets/AssetsStore"
    };

    struct Modelo
    {
        public string caminho; // trecho final do caminho do prefab do pacote, sem ".prefab"
        public float tamanho;  // maior dimensão desejada, em metros
        public Modelo(string c, float t) { caminho = c; tamanho = t; }
    }

    // Prefab de cenário → modelo da loja que substitui as primitivas. Quem não está aqui
    // (ou cujo pacote não foi importado) continua com o modelo gerado pelo script.
    static readonly Dictionary<string, Modelo> Modelos = new Dictionary<string, Modelo>
    {
        { "Rocha_Falesia_A", new Modelo("LowPolyCliffPack/Prefabs/Clifftile Straight 1", 12f) },
        { "Rocha_Falesia_B", new Modelo("LowPolyCliffPack/Prefabs/Clifftile Convex", 12f) },
        { "Paredao_Canion", new Modelo("LowPolyCliffPack/Prefabs/Clifftile Straight 2", 14f) },
        { "Arco_Rochoso", new Modelo("LowPolyCliffPack/Prefabs/Clifftile Pass 1", 12f) },
        { "Talude_Cascalho", new Modelo("LowPolyCliffPack/Prefabs/Rocktile Straight 1", 6f) },
        { "Pedregulho_Pontiagudo", new Modelo("Rock Type1 01", 2.6f) },
        { "Pedregulho_Arredondado", new Modelo("Rock Type2 01", 2.2f) },
        { "Laje_Pedra_Chata", new Modelo("Rock Type3 02", 5f) },
        { "Bloco_Rochoso_Quebravel", new Modelo("Rock Type4 01", 1.6f) },
        { "Rocha_Lascada", new Modelo("Rock Type5 01", 2.2f) },
        { "Pedra_Alongada", new Modelo("Rock Type6 01", 4f) },
        { "Pedra_Entulho_Fisica", new Modelo("Rock Type4 03", 1.5f) },
        { "Rocha_Musgo", new Modelo("SM_Rocks_01", 2.5f) },
        { "Pilha_Pedras", new Modelo("SM_Rocks_03", 1.6f) },
        { "Pedra_Pequena", new Modelo("SimpleNaturePack/Prefabs/Rock_01", 0.7f) },
        { "Rampa_Terra_Batida", new Modelo("Ground_01", 7f) },
        { "Arvore_Petrificada", new Modelo("Dry3333", 7f) },
        { "Arvore_Caida", new Modelo("Branch_01", 5f) },
        { "Toco_Oco", new Modelo("Stump_01", 1.2f) },
        { "Arbusto_Seco_Espinhoso", new Modelo("Bush_02", 1.2f) },
        { "Moita_Grama_Alta", new Modelo("Grass_01", 0.9f) },
        { "Cogumelo_Filtrador", new Modelo("Mushroom_01", 0.7f) },
        { "Flor_Esporos_Azuis", new Modelo("Flowers_01", 0.8f) },
        { "Arbusto_Frutifero", new Modelo("Bush_01", 1.5f) },
        { "Tambor_Oleo_Amassado", new Modelo("barrel_blue", 1f) },
        { "Barril_Contrapeso", new Modelo("Barrel Hazmat", 1.3f) },
        { "Caixa_Madeira", new Modelo("woodenbox", 1.2f) },
        { "Caixote_Aco_Reforcado", new Modelo("Crate Metal", 1.5f) },
        { "Conteiner_Leve", new Modelo("bigcrate_red", 3f) },
        { "Duto_Curvo_Quebrado", new Modelo("pipe_metal_corner", 3f) },
        { "Duto_Reto_Quebrado", new Modelo("bighightpipe", 5f) },
        { "Rolo_Cabos_Cobre", new Modelo("coil_dark", 1.4f) },
        { "Gerador_Desativado", new Modelo("conditioner", 2.5f) },
        { "Tanque_Pressao_Esferico", new Modelo("spherecan_orange", 3f) },
        { "Escada_Incendio_Torcida", new Modelo("Garage_props/Assets/Prefabs/stairs", 3f) },
        { "Caixa_Eletrica_Quebrada", new Modelo("electrolocker", 1.6f) },

        // Assets que só existem pelos pacotes (somam aos 75).
        { "Arvore_Seca_B", new Modelo("Dry4195", 6f) },
        { "Arvore_Seca_C", new Modelo("Dry7509", 5f) },
        { "Arvore_Seca_D", new Modelo("Dry8872", 4f) },
        { "Arvore_Viva_Restaurada", new Modelo("Tree_01", 5f) },
        { "Arbusto_Verde_Baixo", new Modelo("Bush_03", 1f) },
        { "Grama_Rasteira", new Modelo("Grass_02", 0.6f) },
        { "Rocha_Estilizada_B", new Modelo("SM_Rocks_02", 2f) },
        { "Rocha_Estilizada_D", new Modelo("SM_Rocks_04", 3f) },
        { "Rocha_Tipo1_Grande", new Modelo("Rock Type1 03", 3.5f) },
        { "Rocha_Tipo3_Media", new Modelo("Rock Type3 01", 2.4f) },
        { "Rocha_Tipo5_Lasca", new Modelo("Rock Type5 03", 1.8f) },
        { "Rocha_Tipo6_Baixa", new Modelo("Rock Type6 02", 2.5f) },
        { "Rochedo_Pass", new Modelo("LowPolyCliffPack/Prefabs/Rocktile Pass", 8f) },
        { "Tanque_Cilindrico", new Modelo("cylindertank_green", 2.5f) },
        { "Tanque_Grande_Amarelo", new Modelo("bigtank_yellow", 4f) },
        { "Tanque_Longo", new Modelo("longtank_orange", 4f) },
        { "Lixeira_Industrial", new Modelo("trashcan_green", 1.3f) },
        { "Palete_Madeira", new Modelo("pallet", 1.2f) },
        { "Prateleira_Tombada", new Modelo("shelf2", 2f) },
        { "Caixa_Ferramentas", new Modelo("toolcrate", 0.8f) },
        { "Armario_Metal", new Modelo("metal cabinet", 2f) },
        { "Ventilacao_Quebrada", new Modelo("vent", 2f) },
        { "Barril_Velho_Garagem", new Modelo("Barrels_Old/barrel", 1f) },
        { "Tanque_Garagem", new Modelo("Barrels_Old/tank", 1.8f) },
        { "Caixote_Quebrado", new Modelo("Crate Dry_BrokenLarge", 1.3f) },
        { "Barril_Umido", new Modelo("Barrel Wet", 1.2f) },
    };

    static readonly string[] SoLoja =
    {
        "Arvore_Seca_B", "Arvore_Seca_C", "Arvore_Seca_D", "Arvore_Viva_Restaurada", "Arbusto_Verde_Baixo", "Grama_Rasteira",
        "Rocha_Estilizada_B", "Rocha_Estilizada_D", "Rocha_Tipo1_Grande", "Rocha_Tipo3_Media", "Rocha_Tipo5_Lasca",
        "Rocha_Tipo6_Baixa", "Rochedo_Pass", "Tanque_Cilindrico", "Tanque_Grande_Amarelo", "Tanque_Longo",
        "Lixeira_Industrial", "Palete_Madeira", "Prateleira_Tombada", "Caixa_Ferramentas", "Armario_Metal",
        "Ventilacao_Quebrada", "Barril_Velho_Garagem", "Tanque_Garagem", "Caixote_Quebrado", "Barril_Umido"
    };

    static void AdicionarModelosDaLoja()
    {
        foreach (var nome in SoLoja)
        {
            if (AcharPrefabLoja(Modelos[nome].caminho) == null) continue;
            int cat = nome.StartsWith("Arvore") || nome.StartsWith("Arbusto") || nome.StartsWith("Grama") ? 2
                : nome.StartsWith("Roch") ? 1 : 3;
            D(nome, cat, t => { }, atravessavel: nome.StartsWith("Grama"));
        }
    }

    static GameObject AcharPrefabLoja(string caminho)
    {
        string nome = Path.GetFileName(caminho);
        var pastas = Array.FindAll(PastasLoja, AssetDatabase.IsValidFolder);
        if (pastas.Length == 0) return null;
        foreach (var guid in AssetDatabase.FindAssets(nome, pastas))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid).Replace('\\', '/');
            if (!path.EndsWith(".prefab")) continue;
            if (path.EndsWith("/" + caminho + ".prefab", StringComparison.OrdinalIgnoreCase))
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }
        return null;
    }

    /// Põe o modelo do pacote como filho, escalado para o tamanho pedido, com a base em y = 0.
    /// Tira scripts e Rigidbody que o pacote traga e garante collider.
    static bool ModeloLoja(Transform parent, string caminho, float tamanho)
    {
        var src = AcharPrefabLoja(caminho);
        if (src == null) return false;

        var go = (GameObject)PrefabUtility.InstantiatePrefab(src);
        PrefabUtility.UnpackPrefabInstance(go, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        go.name = "Modelo_" + src.name;
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;

        foreach (var j in go.GetComponentsInChildren<Joint>(true)) UnityEngine.Object.DestroyImmediate(j);
        foreach (var mb in go.GetComponentsInChildren<MonoBehaviour>(true)) UnityEngine.Object.DestroyImmediate(mb);
        foreach (var rb in go.GetComponentsInChildren<Rigidbody>(true)) UnityEngine.Object.DestroyImmediate(rb);
        foreach (var lod in go.GetComponentsInChildren<LODGroup>(true)) UnityEngine.Object.DestroyImmediate(lod);

        var rends = go.GetComponentsInChildren<Renderer>();
        if (rends.Length == 0)
        {
            UnityEngine.Object.DestroyImmediate(go);
            return false;
        }
        var b = rends[0].bounds;
        foreach (var rr in rends) b.Encapsulate(rr.bounds);
        float maior = Mathf.Max(b.size.x, Mathf.Max(b.size.y, b.size.z));
        float k = maior > 0.0001f ? tamanho / maior : 1f;
        go.transform.localScale = go.transform.localScale * k;
        go.transform.position = new Vector3(-b.center.x * k, -b.min.y * k, -b.center.z * k);

        if (go.GetComponentInChildren<Collider>() == null)
        {
            foreach (var mf in go.GetComponentsInChildren<MeshFilter>())
            {
                if (mf.sharedMesh == null) continue;
                var mc = mf.gameObject.AddComponent<MeshCollider>();
                mc.sharedMesh = mf.sharedMesh;
                mc.convex = true;
            }
        }
        else
        {
            foreach (var mc in go.GetComponentsInChildren<MeshCollider>()) mc.convex = true;
        }

        go.transform.SetParent(parent, true);
        return true;
    }

    /// Os pacotes vêm com o shader Standard (ou um shader que nem veio junto), que no URP fica rosa.
    /// Troca para URP/Lit levando a textura, a cor, o normal map e o recorte de alpha.
    static void ConverterMateriaisDaLoja()
    {
        var pastas = Array.FindAll(PastasLoja, AssetDatabase.IsValidFolder);
        if (pastas.Length == 0) return;
        var lit = Shader.Find("Universal Render Pipeline/Lit");
        foreach (var guid in AssetDatabase.FindAssets("t:Material", pastas))
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
            if (mat == null || mat.shader == null) continue;
            string sh = mat.shader.name;
            if (sh.StartsWith("Universal Render Pipeline") || sh.StartsWith("Shader Graphs")) continue;
            if (sh.Contains("Particle")) continue;

            // Lê das propriedades salvas: funciona mesmo quando o shader original não existe.
            var so = new SerializedObject(mat);
            Texture main = TexturaSalva(so, "_MainTex"), normal = TexturaSalva(so, "_BumpMap");
            Color cor = CorSalva(so, "_Color", Color.white);
            float cutoff = FloatSalvo(so, "_Cutoff", -1f);
            bool recorte = sh.Contains("Cutout") || sh.Contains("Leaves") || sh.Contains("Tree") || sh.Contains("InternalError")
                || mat.renderQueue == 2450 || FloatSalvo(so, "_Mode", 0f) == 1f;

            mat.shader = lit;
            if (main != null) mat.SetTexture("_BaseMap", main);
            mat.SetColor("_BaseColor", cor);
            if (normal != null)
            {
                mat.SetTexture("_BumpMap", normal);
                mat.EnableKeyword("_NORMALMAP");
            }
            if (recorte)
            {
                mat.SetFloat("_AlphaClip", 1f);
                mat.SetFloat("_Cutoff", cutoff > 0f ? cutoff : 0.5f);
                mat.EnableKeyword("_ALPHATEST_ON");
                mat.SetFloat("_Cull", 0f);
                mat.renderQueue = 2450;
            }
            mat.SetFloat("_Smoothness", 0.2f);
            EditorUtility.SetDirty(mat);
        }
        AssetDatabase.SaveAssets();
    }

    static SerializedProperty Salva(SerializedObject so, string lista, string nome)
    {
        var arr = so.FindProperty("m_SavedProperties." + lista);
        if (arr == null) return null;
        for (int i = 0; i < arr.arraySize; i++)
        {
            var el = arr.GetArrayElementAtIndex(i);
            if (el.FindPropertyRelative("first").stringValue == nome) return el.FindPropertyRelative("second");
        }
        return null;
    }

    static Texture TexturaSalva(SerializedObject so, string nome)
    {
        var p = Salva(so, "m_TexEnvs", nome);
        return p == null ? null : p.FindPropertyRelative("m_Texture").objectReferenceValue as Texture;
    }

    static Color CorSalva(SerializedObject so, string nome, Color padrao)
    {
        var p = Salva(so, "m_Colors", nome);
        return p == null ? padrao : p.colorValue;
    }

    static float FloatSalvo(SerializedObject so, string nome, float padrao)
    {
        var p = Salva(so, "m_Floats", nome);
        return p == null ? padrao : p.floatValue;
    }

    // ---------------------------------------------------- blocos de construção

    static Transform P(Transform parent, PrimitiveType type, Vector3 pos, Vector3 scale, Vector3 euler, string mat, bool collider = true)
    {
        var go = GameObject.CreatePrimitive(type);
        go.name = type.ToString();
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localEulerAngles = euler;
        go.GetComponent<Renderer>().sharedMaterial = mats[mat];
        if (!collider) UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());
        return go.transform;
    }

    /// Rocha low-poly: icosfera deformada por ruído, com faces chapadas. A malha é salva como asset.
    static Transform Rocha(Transform parent, string meshName, int seed, Vector3 size, string mat, float rugosidade)
    {
        var mesh = SalvarMesh(MeshRocha(seed, rugosidade), "Rocha_" + meshName);
        var go = new GameObject(meshName);
        go.transform.SetParent(parent, false);
        go.transform.localScale = size;
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = mats[mat];
        var mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        mc.convex = true;
        return go.transform;
    }

    static Mesh MeshRocha(int seed, float rug)
    {
        var r = new System.Random(seed);
        // Icosaedro subdividido uma vez.
        float t = (1f + Mathf.Sqrt(5f)) / 2f;
        var v = new List<Vector3>
        {
            new Vector3(-1, t, 0), new Vector3(1, t, 0), new Vector3(-1, -t, 0), new Vector3(1, -t, 0),
            new Vector3(0, -1, t), new Vector3(0, 1, t), new Vector3(0, -1, -t), new Vector3(0, 1, -t),
            new Vector3(t, 0, -1), new Vector3(t, 0, 1), new Vector3(-t, 0, -1), new Vector3(-t, 0, 1)
        };
        var f = new List<int>
        {
            0,11,5, 0,5,1, 0,1,7, 0,7,10, 0,10,11, 1,5,9, 5,11,4, 11,10,2, 10,7,6, 7,1,8,
            3,9,4, 3,4,2, 3,2,6, 3,6,8, 3,8,9, 4,9,5, 2,4,11, 6,2,10, 8,6,7, 9,8,1
        };
        var cache = new Dictionary<long, int>();
        Func<int, int, int> mid = (a, b) =>
        {
            long key = a < b ? ((long)a << 32) + b : ((long)b << 32) + a;
            if (cache.TryGetValue(key, out int idx)) return idx;
            v.Add((v[a] + v[b]) * 0.5f);
            cache[key] = v.Count - 1;
            return v.Count - 1;
        };
        var f2 = new List<int>();
        for (int i = 0; i < f.Count; i += 3)
        {
            int a = mid(f[i], f[i + 1]), b = mid(f[i + 1], f[i + 2]), c = mid(f[i + 2], f[i]);
            f2.AddRange(new[] { f[i], a, c, f[i + 1], b, a, f[i + 2], c, b, a, b, c });
        }
        float ox = (float)r.NextDouble() * 100f, oy = (float)r.NextDouble() * 100f;
        for (int i = 0; i < v.Count; i++)
        {
            var n = v[i].normalized;
            float noise = Mathf.PerlinNoise(n.x * 1.7f + ox, n.y * 1.7f + n.z * 0.9f + oy) - 0.5f;
            float jitter = (float)(r.NextDouble() - 0.5) * 0.5f;
            v[i] = n * 0.5f * (1f + (noise * 1.4f + jitter) * rug);
        }
        // Base achatada para assentar no chão: nada abaixo de -0.35, depois sobe para pivô no chão.
        for (int i = 0; i < v.Count; i++)
        {
            var p = v[i];
            if (p.y < -0.35f) p.y = -0.35f;
            p.y += 0.35f;
            v[i] = p;
        }
        // Faces chapadas: um vértice por canto de triângulo.
        var verts = new Vector3[f2.Count];
        var tris = new int[f2.Count];
        for (int i = 0; i < f2.Count; i++)
        {
            verts[i] = v[f2[i]];
            tris[i] = i;
        }
        var mesh = new Mesh { vertices = verts, triangles = tris };
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    static Mesh SalvarMesh(Mesh mesh, string name)
    {
        string path = MeshDir + "/" + name + ".asset";
        var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
        mesh.name = name;
        if (existing != null)
        {
            existing.Clear();
            EditorUtility.CopySerialized(mesh, existing);
            UnityEngine.Object.DestroyImmediate(mesh);
            return existing;
        }
        mesh.name = name;
        AssetDatabase.CreateAsset(mesh, path);
        return mesh;
    }

    static void Galhos(Transform parent, int quantidade, float altura, string mat, int seed)
    {
        var r = new System.Random(seed);
        for (int i = 0; i < quantidade; i++)
        {
            float a = (float)r.NextDouble() * 360f;
            float incl = 20f + (float)r.NextDouble() * 50f;
            P(parent, PrimitiveType.Cylinder, new Vector3(0, altura * 0.45f, 0), new Vector3(0.05f, altura * 0.5f, 0.05f), new Vector3(incl, a, 0), mat);
        }
    }

    static void Engrenagem(Transform parent, float raio, int dentes, string mat)
    {
        P(parent, PrimitiveType.Cylinder, new Vector3(0, raio + 0.1f, 0), new Vector3(raio * 2f, 0.1f, raio * 2f), new Vector3(90, 0, 0), mat);
        for (int i = 0; i < dentes; i++)
        {
            float a = i / (float)dentes * Mathf.PI * 2f;
            P(parent, PrimitiveType.Cube, new Vector3(Mathf.Cos(a) * raio * 1.08f, raio + 0.1f + Mathf.Sin(a) * raio * 1.08f, 0),
                new Vector3(raio * 0.25f, raio * 0.25f, 0.2f), new Vector3(0, 0, a * Mathf.Rad2Deg), mat, false);
        }
    }

    static void Toro(Transform parent, string name, float R, float r, string mat, Vector3 pos, Vector3 euler, bool collider = true)
    {
        int seg = 20, lados = 8;
        var verts = new List<Vector3>();
        var tris = new List<int>();
        for (int i = 0; i < seg; i++)
        {
            float a0 = i / (float)seg * Mathf.PI * 2f, a1 = (i + 1) / (float)seg * Mathf.PI * 2f;
            for (int j = 0; j < lados; j++)
            {
                float b0 = j / (float)lados * Mathf.PI * 2f, b1 = (j + 1) / (float)lados * Mathf.PI * 2f;
                Vector3 p00 = TP(R, r, a0, b0), p10 = TP(R, r, a1, b0), p01 = TP(R, r, a0, b1), p11 = TP(R, r, a1, b1);
                int k = verts.Count;
                verts.AddRange(new[] { p00, p10, p11, p00, p11, p01 });
                tris.AddRange(new[] { k, k + 2, k + 1, k + 3, k + 5, k + 4 });
            }
        }
        var mesh = new Mesh { vertices = verts.ToArray(), triangles = tris.ToArray() };
        mesh.RecalculateNormals();
        mesh = SalvarMesh(mesh, "Toro_" + name);
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localEulerAngles = euler;
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshRenderer>().sharedMaterial = mats[mat];
        if (collider)
        {
            var mc = go.AddComponent<MeshCollider>();
            mc.sharedMesh = mesh;
            mc.convex = true;
        }
    }

    static Vector3 TP(float R, float r, float a, float b)
    {
        return new Vector3((R + r * Mathf.Cos(b)) * Mathf.Cos(a), r * Mathf.Sin(b), (R + r * Mathf.Cos(b)) * Mathf.Sin(a));
    }

    // ------------------------------------------------------------------ terreno

    // Meia-largura do vale (m) ao longo de Z.
    static float MeiaLargura(float z)
    {
        if (z < 34f) return 10f;
        if (z < 46f) return 3.5f;                              // saída estreita da plataforma, com a barricada
        if (z < 100f) return 6f;                               // Ravina Entulhada
        if (z < 108f) return Mathf.Lerp(6f, 22f, (z - 100f) / 8f);
        if (z < 166f) return 22f;                              // Leito Fluvial Seco
        if (z < 172f) return Mathf.Lerp(22f, 15f, (z - 166f) / 6f);
        return 15f;                                            // Base da Represa
    }

    // Altura do chão (m) no meio do vale.
    static float Piso(float z)
    {
        if (z < 20f) return 9.2f;                              // onde o robô nasce
        if (z < 48f) return 10f;                               // degrau de 0,8 m: ensina o pulo
        if (z < 62f) return Mathf.Lerp(10f, 5f, Smooth((z - 48f) / 14f));
        if (z < 100f) return 5f;
        if (z < 110f) return Mathf.Lerp(5f, 3f, Smooth((z - 100f) / 10f));
        if (z < 164f) return 3f;
        if (z < 174f) return Mathf.Lerp(3f, 5f, Smooth((z - 164f) / 10f));
        return 5f;
    }

    static float Smooth(float t)
    {
        t = Mathf.Clamp01(t);
        return t * t * (3f - 2f * t);
    }

    static float AlturaEm(float x, float z)
    {
        float piso = Piso(z);
        float ruido = (Mathf.PerlinNoise(x * 0.08f, z * 0.08f) - 0.5f) * (z > 108f && z < 164f ? 0.3f : 0.5f);
        float d = Mathf.Abs(x - CenterX);
        float lateral = (d - MeiaLargura(z)) / 3f;
        float fundo = Mathf.Max((4f - z) / 3f, (z - 230f) / 3f);
        float parede = Mathf.Clamp01(Mathf.Max(lateral, fundo));
        float topo = 42f + Mathf.PerlinNoise(x * 0.05f + 30f, z * 0.05f) * 12f;
        // Degrau do pulo fica nítido (sem ruído) na faixa z 18–22.
        if (z > 17f && z < 23f) ruido *= 0.2f;
        return Mathf.Lerp(piso + ruido, topo, Smooth(parede));
    }

    static void CriarTerreno()
    {
        var td = new TerrainData();
        td.heightmapResolution = 257;
        td.alphamapResolution = 256;
        td.size = new Vector3(TerrainW, TerrainH, TerrainL);

        int res = td.heightmapResolution;
        var h = new float[res, res];
        for (int iz = 0; iz < res; iz++)
            for (int ix = 0; ix < res; ix++)
            {
                float x = ix / (float)(res - 1) * TerrainW;
                float z = iz / (float)(res - 1) * TerrainL;
                h[iz, ix] = AlturaEm(x, z) / TerrainH;
            }
        td.SetHeights(0, 0, h);

        // Camadas: 0 solo árido, 1 cascalho, 2 rocha, 3 lodo, 4 seixos de rio, 5 grama seca.
        td.terrainLayers = new[]
        {
            Camada("Solo_Arido_Fendido", new Color(0.62f, 0.48f, 0.32f), new Color(0.42f, 0.30f, 0.18f), 0.06f, 1),
            Camada("Cascalho_Areia", new Color(0.66f, 0.60f, 0.50f), new Color(0.45f, 0.42f, 0.38f), 0.35f, 2),
            Camada("Rocha_Sedimentar", new Color(0.50f, 0.44f, 0.38f), new Color(0.33f, 0.29f, 0.26f), 0.12f, 3),
            Camada("Lodo_Toxico", new Color(0.40f, 0.70f, 0.12f), new Color(0.20f, 0.40f, 0.05f), 0.08f, 4),
            Camada("Seixos_Rio", new Color(0.70f, 0.68f, 0.62f), new Color(0.48f, 0.46f, 0.42f), 0.25f, 5),
            Camada("Grama_Seca", new Color(0.62f, 0.58f, 0.30f), new Color(0.42f, 0.42f, 0.20f), 0.2f, 6)
        };

        int ar = td.alphamapResolution;
        var alphas = new float[ar, ar, 6];
        for (int iz = 0; iz < ar; iz++)
            for (int ix = 0; ix < ar; ix++)
            {
                float nx = ix / (float)(ar - 1), nz = iz / (float)(ar - 1);
                float x = nx * TerrainW, z = nz * TerrainL;
                float slope = td.GetSteepness(nx, nz);
                float d = Mathf.Abs(x - CenterX);
                var w = new float[6];
                float n = Mathf.PerlinNoise(x * 0.15f, z * 0.15f);

                w[0] = 1f;
                if (z < 46f) w[5] = 0.6f + n * 0.6f;
                if (z >= 46f && z < 104f) w[1] = 1.2f + n;
                if (z >= 104f && z < 168f && d < 9f) w[4] = 1.5f * (1f - d / 9f) + n * 0.5f;
                if (z >= 168f) w[1] = 0.5f * n;
                foreach (var poca in Pocas)
                {
                    float dist = Vector2.Distance(new Vector2(x, z), poca);
                    if (dist < 4.5f) w[3] += 3f * (1f - dist / 4.5f);
                }
                if (slope > 28f)
                {
                    float k = Mathf.Clamp01((slope - 28f) / 12f) * 4f;
                    w[2] += k;
                }

                float sum = 0f;
                foreach (var x2 in w) sum += x2;
                for (int l = 0; l < 6; l++) alphas[iz, ix, l] = w[l] / sum;
            }
        td.SetAlphamaps(0, 0, alphas);

        string tdPath = ArtRoot + "/Nivel1_TerrainData.asset";
        AssetDatabase.DeleteAsset(tdPath);
        AssetDatabase.CreateAsset(td, tdPath);

        var go = Terrain.CreateTerrainGameObject(td);
        go.name = "Terreno";
        go.transform.position = Vector3.zero;
        terrain = go.GetComponent<Terrain>();
        var terMat = AssetDatabase.LoadAssetAtPath<Material>(MatDir + "/Cen_Terreno.mat");
        if (terMat == null)
        {
            terMat = new Material(Shader.Find("Universal Render Pipeline/Terrain/Lit"));
            AssetDatabase.CreateAsset(terMat, MatDir + "/Cen_Terreno.mat");
        }
        terrain.materialTemplate = terMat;
        GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.BatchingStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic);
    }

    static TerrainLayer Camada(string name, Color a, Color b, float freq, int seed)
    {
        string texPath = TexDir + "/Terreno_" + name + ".png";
        const int size = 256;
        var tex = new Texture2D(size, size, TextureFormat.RGB24, false);
        var r = new System.Random(seed);
        float o = (float)r.NextDouble() * 100f;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                // Ruído periódico (tileável): amostra em círculo.
                float u = x / (float)size * Mathf.PI * 2f, v = y / (float)size * Mathf.PI * 2f;
                float n = 0f, amp = 1f, tot = 0f;
                for (int oct = 1; oct <= 4; oct++)
                {
                    float s = freq * 40f * oct / Mathf.PI;
                    n += amp * Mathf.PerlinNoise(o + Mathf.Cos(u) * s + Mathf.Sin(v) * s * 0.5f, o + Mathf.Sin(u) * s + Mathf.Cos(v) * s * 0.5f);
                    tot += amp;
                    amp *= 0.5f;
                }
                n /= tot;
                if (name.StartsWith("Solo"))
                {
                    // Fendas: linhas escuras onde o ruído cruza 0,5.
                    float crack = Mathf.Abs(n - 0.5f) < 0.015f ? 0.45f : 1f;
                    tex.SetPixel(x, y, Color.Lerp(b, a, n) * crack);
                }
                else
                {
                    float grain = (float)r.NextDouble() * 0.12f;
                    tex.SetPixel(x, y, Color.Lerp(b, a, Mathf.Clamp01(n + grain - 0.06f)));
                }
            }
        File.WriteAllBytes(texPath, tex.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(texPath);

        string layerPath = TexDir + "/Camada_" + name + ".terrainlayer";
        var layer = AssetDatabase.LoadAssetAtPath<TerrainLayer>(layerPath);
        if (layer == null)
        {
            layer = new TerrainLayer();
            AssetDatabase.CreateAsset(layer, layerPath);
        }
        layer.diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texPath);
        layer.tileSize = new Vector2(6f, 6f);
        layer.smoothness = name.StartsWith("Lodo") ? 0.7f : 0.05f;
        EditorUtility.SetDirty(layer);
        return layer;
    }

    // Centros das poças de lodo (x, z) no Setor 3.
    static readonly Vector2[] Pocas =
    {
        new Vector2(42f, 116f), new Vector2(58f, 124f), new Vector2(47f, 136f),
        new Vector2(60f, 146f), new Vector2(40f, 152f), new Vector2(54f, 160f)
    };

    static float Chao(float x, float z)
    {
        return terrain.SampleHeight(new Vector3(x, 0, z)) + terrain.transform.position.y;
    }

    // --------------------------------------------------------------------- cena

    static void LimparGerados(UnityEngine.SceneManagement.Scene scene)
    {
        foreach (var go in scene.GetRootGameObjects())
        {
            if (Array.IndexOf(GeneratedRoots, go.name) >= 0) UnityEngine.Object.DestroyImmediate(go);
        }
    }

    static GameObject Colocar(string nome, Transform grupo, float x, float z, float rotY, float escala = 1f, float afundar = 0.05f)
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefabs[nome], grupo);
        go.transform.position = new Vector3(x, Chao(x, z) - afundar, z);
        go.transform.rotation = Quaternion.Euler(0f, rotY, 0f);
        go.transform.localScale = Vector3.one * escala;
        return go;
    }

    static float R(float a, float b) => a + (float)rng.NextDouble() * (b - a);

    /// Coloca enfeites espalhados perto das paredes de um setor (sem bloquear o caminho do meio).
    static void Enfeitar(Transform grupo, string[] nomes, int quantos, float z0, float z1, float margem, float escMin = 0.8f, float escMax = 1.3f, bool noMeio = false)
    {
        for (int i = 0; i < quantos; i++)
        {
            string nome = nomes[i % nomes.Length];
            float z = R(z0, z1);
            float hw = MeiaLargura(z);
            float lado = rng.NextDouble() < 0.5 ? -1f : 1f;
            float x = noMeio ? CenterX + R(-hw + 1.5f, hw - 1.5f) : CenterX + lado * R(Mathf.Max(0f, hw - margem), hw + 0.5f);
            Colocar(nome, grupo, x, z, R(0f, 360f), R(escMin, escMax));
        }
    }

    static void MontarCena()
    {
        var setor1 = new GameObject("Setor1_Despertar").transform;
        var setor2 = new GameObject("Setor2_Ravina").transform;
        var setor3 = new GameObject("Setor3_LeitoSeco").transform;
        var setor4 = new GameObject("Setor4_Represa").transform;
        var fumacaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabRoot + "/Fumaca.prefab");

        // ---------- Setor 1: Plataforma de Despertar (z 4–46)
        Colocar("Arco_Rochoso", setor1, 50f, 5.5f, 0f, 1.1f);
        Colocar("Rocha_Falesia_A", setor1, 39f, 14f, 30f);
        Colocar("Rocha_Falesia_B", setor1, 61f, 26f, 200f);
        Colocar("Drone_ARIA", setor1, 46f, 12f, 30f);
        Colocar("Placa_Aviso_Toxico", setor1, 54f, 30f, 180f);
        Colocar("Toco_Oco", setor1, 43f, 28f, 0f);
        Colocar("Arvore_Petrificada", setor1, 57f, 9f, 40f);
        Enfeitar(setor1, new[] { "Arbusto_Seco_Espinhoso", "Espinheiro_Rasteiro", "Pedra_Pequena", "Seixos_Conjunto", "Cacto_Seco", "Liquen_Fluorescente", "Muda_Bioluminescente", "Lagarto_Rochas", "Pilha_Pedras" }, 18, 6f, 44f, 3f);

        // Barricada de caixotes leves (5 a 8 kg) fechando a saída estreita (meia-largura 3,5 m).
        for (int fila = 0; fila < 2; fila++)
            for (int i = 0; i < 5; i++)
            {
                var c = Colocar("Caixa_Madeira", setor1, 47.4f + i * 1.3f, 40f + fila * 0.05f, 0f, 1f, 0f);
                c.transform.position += Vector3.up * (fila * 1.22f + 0.02f);
                c.GetComponent<Rigidbody>().mass = 5f + (i + fila) % 4;
                c.name = "Barricada_Caixa_" + fila + "_" + i;
            }

        // ---------- Setor 2: Ravina Entulhada (z 46–104)
        Colocar("Paredao_Canion", setor2, 41f, 70f, 90f);
        Colocar("Paredao_Canion", setor2, 59f, 88f, 270f);
        Colocar("Coluna_Basaltica", setor2, 45f, 58f, 0f);
        Colocar("Talude_Cascalho", setor2, 55f, 64f, 80f);
        Colocar("Viga_I_Torcida", setor2, 52.5f, 78f, 70f);
        Colocar("Rocha_Lascada", setor2, 45f, 80f, 0f);
        Colocar("Escada_Incendio_Torcida", setor2, 55f, 95f, 10f);
        Colocar("Caixa_Eletrica_Quebrada", setor2, 45f, 92f, 90f);
        Colocar("Poste_Sinalizacao_Inclinado", setor2, 55f, 55f, 180f);
        Colocar("Rampa_Terra_Batida", setor2, 46f, 98f, 0f);
        Colocar("Passaro_Canyon_Swift", setor2, 52f, 66f, 0f).transform.position += Vector3.up * 6f;
        Colocar("Placa_Corrugada", setor2, 47f, 86f, 20f);
        Enfeitar(setor2, new[] { "Pedra_Pequena", "Seixos_Conjunto", "Espinheiro_Rasteiro", "Rocha_Musgo", "Pedregulho_Pontiagudo", "Trepadeiras_Encosta", "Roda_Dentada_Pequena", "Besouro_Blindado", "Arbusto_Queimado" }, 22, 50f, 100f, 1.5f, 0.6f, 1f);

        // Bloqueios com física: dois contêineres atravessados, pedras e tambores.
        Colocar("Conteiner_Leve", setor2, 47f, 70f, 0f, 1f, 0f);
        Colocar("Conteiner_Leve", setor2, 53f, 70.5f, 0f, 1f, 0f);
        Colocar("Tambor_Oleo_Amassado", setor2, 44.9f, 70f, 0f, 1f, 0f);
        Colocar("Tambor_Oleo_Amassado", setor2, 55.1f, 70.5f, 0f, 1f, 0f);
        Colocar("Pedra_Entulho_Fisica", setor2, 46.5f, 84f, 30f, 1f, 0f);
        Colocar("Pedra_Entulho_Fisica", setor2, 50f, 84.5f, 80f, 1.1f, 0f);
        Colocar("Pedra_Entulho_Fisica", setor2, 53.5f, 84f, 150f, 1f, 0f);
        Colocar("Bloco_Rochoso_Quebravel", setor2, 50f, 60f, 0f, 1f, 0f);
        Colocar("Tambor_Oleo_Amassado", setor2, 48f, 76f, 0f, 1f, 0f);
        Colocar("Tambor_Oleo_Amassado", setor2, 52f, 92f, 0f, 1f, 0f);
        Colocar("Tambor_Oleo_Amassado", setor2, 49f, 93f, 0f, 1f, 0f);

        // ---------- Setor 3: Leito Fluvial Seco (z 104–168)
        var slimePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gameplay/Lodo.prefab");
        int iPoca = 1;
        foreach (var p in Pocas)
        {
            var lodo = (GameObject)PrefabUtility.InstantiatePrefab(slimePrefab, setor3);
            lodo.name = "Lodo_" + iPoca++;
            // Collider de 1 m de altura encostado no chão: pisar dentro ativa o dano.
            lodo.transform.position = new Vector3(p.x, Chao(p.x, p.y) + 0.3f, p.y);
            lodo.transform.localScale = new Vector3(5f, 1f, 5f);
            var visual = P(lodo.transform, PrimitiveType.Cylinder, new Vector3(0, -0.28f, 0), new Vector3(1f, 0.02f, 1f), Vector3.zero, "Lodo", false);
            visual.name = "Poca_Visual";
            var fum = (GameObject)PrefabUtility.InstantiatePrefab(fumacaPrefab, setor3);
            fum.transform.position = new Vector3(p.x + 2.2f, Chao(p.x + 2.2f, p.y), p.y);
            Colocar("Placa_Aviso_Toxico", setor3, p.x - 3f, p.y - 2.5f, R(0, 360));
        }

        Colocar("Calha_Fluvial_Seca", setor3, 50f, 132f, 0f);
        Colocar("Margem_Seixos_Rio", setor3, 45f, 120f, 80f);
        Colocar("Margem_Seixos_Rio", setor3, 56f, 140f, 100f);
        Colocar("Laje_Pedra_Chata", setor3, 35f, 128f, 20f);
        Colocar("Cratera_Erosao", setor3, 63f, 112f, 0f);
        Colocar("Arvore_Caida", setor3, 36f, 142f, 70f);
        Colocar("Raiz_Gigante", setor3, 66f, 150f, 0f);
        Colocar("Cacamba_Enferrujada", setor3, 31f, 160f, 60f);
        Colocar("Duto_Curvo_Quebrado", setor3, 68f, 130f, 200f);
        Colocar("Duto_Reto_Quebrado", setor3, 33f, 112f, 30f);
        Colocar("Engrenagem_Desgastada", setor3, 64f, 158f, 40f);
        Colocar("Rolo_Cabos_Cobre", setor3, 38f, 122f, 0f);
        Colocar("Pneu_Industrial", setor3, 61f, 120f, 0f);
        Colocar("Chapa_Gangorra", setor3, 44f, 146f, 30f);
        Colocar("Gerador_Desativado", setor3, 67f, 140f, 260f);
        Colocar("Tanque_Pressao_Esferico", setor3, 30f, 136f, 0f);
        Colocar("Caixote_Aco_Reforcado", setor3, 57f, 152f, 15f);
        Colocar("Pedra_Alongada", setor3, 52f, 113f, 60f);
        Colocar("Pedregulho_Arredondado", setor3, 40f, 132f, 0f);
        Colocar("Juncos_Aquaticos", setor3, 49f, 128f, 0f);
        Colocar("Juncos_Aquaticos", setor3, 53f, 146f, 0f);
        Colocar("Peixe_Cintilante", setor3, 50f, 133f, 70f);
        Colocar("Lebre_Prado", setor3, 34f, 150f, 120f);
        Colocar("EasterEgg_Extintor_Vintage", setor3, 66f, 116f, 30f);
        Colocar("EasterEgg_Esteira_Bota", setor3, 32f, 120f, 210f);
        Colocar("Tubo_Drenagem", setor3, 70f, 122f, 270f);
        var fumDreno = (GameObject)PrefabUtility.InstantiatePrefab(fumacaPrefab, setor3);
        fumDreno.transform.position = new Vector3(69f, Chao(69f, 122f) + 0.9f, 122f);
        Colocar("Barril_Contrapeso", setor3, 44f, 147f, 0f, 1f, 0f);
        Colocar("Barril_Contrapeso", setor3, 41f, 160f, 0f, 1f, 0f);
        Colocar("Barril_Contrapeso", setor3, 60f, 113f, 0f, 1f, 0f);
        Colocar("Tambor_Oleo_Amassado", setor3, 63f, 148f, 0f, 1f, 0f);
        Enfeitar(setor3, new[] { "Pedra_Pequena", "Seixos_Conjunto", "Arbusto_Seco_Espinhoso", "Cogumelo_Filtrador", "Liquen_Fluorescente", "Flor_Esporos_Azuis", "Muda_Bioluminescente", "Pedregulho_Pontiagudo", "Rocha_Musgo", "Moita_Grama_Alta" }, 36, 106f, 166f, 4f);
        Enfeitar(setor3, new[] { "Rocha_Falesia_A", "Rocha_Falesia_B", "Paredao_Canion" }, 6, 108f, 164f, 0f, 0.9f, 1.2f);

        // Assets que vieram só dos pacotes da loja, espalhados pelos setores.
        var soLoja = Array.FindAll(SoLoja, prefabs.ContainsKey);
        for (int i = 0; i < soLoja.Length; i++)
        {
            var grupo = i % 4 == 0 ? setor2 : i % 4 == 3 ? setor4 : setor3;
            float z0 = grupo == setor2 ? 52f : grupo == setor3 ? 108f : 178f;
            float z1 = grupo == setor2 ? 98f : grupo == setor3 ? 164f : 226f;
            Enfeitar(grupo, new[] { soLoja[i] }, 2, z0, z1, 3f, 0.9f, 1.1f);
        }

        // ---------- Setor 4: Base da Represa e Torre (z 168–232)
        Colocar("Comporta_Represa", setor4, 50f, 226f, 0f, 1.5f);
        Colocar("Pilar_Concreto", setor4, 39f, 216f, 0f);
        Colocar("Pilar_Concreto", setor4, 61f, 216f, 0f);
        Colocar("Passarela_Grade", setor4, 39f, 207f, 0f);
        Colocar("Corrimao_Metalico", setor4, 44f, 190f, 90f);
        Colocar("Corrimao_Metalico", setor4, 56f, 190f, 90f);
        Colocar("Valvula_Manual", setor4, 57f, 205f, 0f);
        Colocar("Arbusto_Frutifero", setor4, 40f, 196f, 0f);
        Colocar("Moita_Grama_Alta", setor4, 60f, 198f, 0f);
        Colocar("Trepadeiras_Encosta", setor4, 36f, 210f, 90f);
        Colocar("Cerca_Arame", setor4, 60f, 183f, 90f);
        Colocar("Poste_Sinalizacao_Inclinado", setor4, 42f, 182f, 0f);
        Enfeitar(setor4, new[] { "Pedra_Pequena", "Pilha_Pedras", "Rocha_Musgo", "Flor_Esporos_Azuis", "Cogumelo_Filtrador", "Lagarto_Rochas", "Besouro_Blindado" }, 16, 176f, 228f, 3f);

        // Os 3 entulhos que bloqueiam o canal, entre o Setor 3 e o 4, dentro da AreaCanal.
        var canal = GameObject.CreatePrimitive(PrimitiveType.Cube);
        canal.name = "AreaCanal";
        canal.transform.SetParent(setor4, true);
        UnityEngine.Object.DestroyImmediate(canal.GetComponent<MeshRenderer>());
        UnityEngine.Object.DestroyImmediate(canal.GetComponent<MeshFilter>());
        canal.transform.position = new Vector3(50f, Chao(50f, 176f) + 1.5f, 176f);
        canal.transform.localScale = new Vector3(14f, 4f, 6f);
        var canalCol = canal.GetComponent<BoxCollider>();
        canalCol.isTrigger = true;
        var bloqueios = new List<GameObject>();
        foreach (var x in new[] { 45.5f, 50f, 54.5f })
        {
            var e = Colocar("Pedra_Entulho_Fisica", setor4, x, 176f, R(0, 360), 1.1f, 0f);
            e.name = "Entulho_Canal_" + bloqueios.Count;
            e.GetComponent<Rigidbody>().mass = 8f;
            bloqueios.Add(e);
        }

        // Torre: área de ativação na frente do console; modelos como filhos.
        var torrePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gameplay/Torre.prefab");
        var torre = (GameObject)PrefabUtility.InstantiatePrefab(torrePrefab);
        torre.transform.position = new Vector3(50f, Chao(50f, 205f) + 1f, 205f);
        var modeloTorre = (GameObject)PrefabUtility.InstantiatePrefab(prefabs["Torre_Purificadora_Modelo"], torre.transform);
        modeloTorre.transform.localPosition = new Vector3(0f, -1f, 7f);
        var console = (GameObject)PrefabUtility.InstantiatePrefab(prefabs["Console_Terminal"], torre.transform);
        console.transform.localPosition = new Vector3(0f, -1f, 2.4f);
        console.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
        var effectPoint = new GameObject("EffectPoint").transform;
        effectPoint.SetParent(torre.transform, false);
        effectPoint.localPosition = new Vector3(0f, 13f, 7f);

        var torreSO = new SerializedObject(torre.GetComponent<PurifierTower>());
        torreSO.FindProperty("purificationEffectPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabRoot + "/Particula_Purificacao.prefab");
        torreSO.FindProperty("effectPoint").objectReferenceValue = effectPoint;
        torreSO.FindProperty("activationSound").objectReferenceValue = Som("Som_Torre_Ativacao");
        torreSO.FindProperty("channelArea").objectReferenceValue = canalCol;
        var deb = torreSO.FindProperty("blockingDebris");
        deb.arraySize = bloqueios.Count;
        for (int i = 0; i < bloqueios.Count; i++) deb.GetArrayElementAtIndex(i).objectReferenceValue = bloqueios[i];
        torreSO.ApplyModifiedPropertiesWithoutUndo();

        // ---------- Pontos de spawn (Setores 2 e 3), 0,5 m acima do chão
        var spawnRoot = new GameObject("SpawnPoints").transform;
        var pontos = new List<Transform>();
        var posicoes = new[]
        {
            new Vector2(50f, 64f), new Vector2(48f, 79f), new Vector2(51f, 97f), new Vector2(46f, 90f),
            new Vector2(36f, 118f), new Vector2(64f, 126f), new Vector2(50f, 140f), new Vector2(33f, 146f),
            new Vector2(66f, 155f), new Vector2(48f, 163f), new Vector2(57f, 108f), new Vector2(42f, 130f)
        };
        for (int i = 0; i < posicoes.Length; i++)
        {
            var sp = new GameObject("SpawnPoint_" + (i + 1).ToString("00")).transform;
            sp.SetParent(spawnRoot, false);
            sp.position = new Vector3(posicoes[i].x, Chao(posicoes[i].x, posicoes[i].y) + 0.5f, posicoes[i].y);
            pontos.Add(sp);
        }

        // ---------- SIstemaDeJogo
        var sistemaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Gameplay/SIstemaDeJogo.prefab");
        var sistema = (GameObject)PrefabUtility.InstantiatePrefab(sistemaPrefab);
        sistema.transform.position = Vector3.zero;
        var player = sistema.transform.Find("Player");
        player.position = new Vector3(50f, Chao(50f, 12f) + 1.1f, 12f);
        player.rotation = Quaternion.identity;

        Ligar(player.GetComponent<InteractionController>(), "impactParticlePrefab", AssetDatabase.LoadAssetAtPath<GameObject>(PrefabRoot + "/Particula_Pedrinhas.prefab"));
        Ligar(player.GetComponent<InteractionController>(), "pulseSound", Som("Som_Pulso_Impacto"));
        Ligar(player.GetComponent<PlayerInventory>(), "collectSound", Som("Som_Coleta_Bip"));
        Ligar(player.GetComponent<CheatCode>(), "cheatSound", Som("Som_Cheat_Confirmacao"));

        var spawn = sistema.transform.Find("SpawnManager").GetComponent<SpawnManager>();
        var spSO = new SerializedObject(spawn);
        foreach (var campo in new[] { "filterSpawnPoints", "scrapSpawnPoints" })
        {
            var arr = spSO.FindProperty(campo);
            arr.arraySize = pontos.Count;
            for (int i = 0; i < pontos.Count; i++) arr.GetArrayElementAtIndex(i).objectReferenceValue = pontos[i];
        }
        spSO.FindProperty("filtersToSpawn").intValue = 5;
        spSO.FindProperty("scrapToSpawn").intValue = pontos.Count - 5;
        spSO.ApplyModifiedPropertiesWithoutUndo();

        // ---------- Som ambiente
        var amb = new GameObject("SomAmbiente").AddComponent<AudioSource>();
        amb.clip = Som("Som_Ambiente_Vento");
        amb.loop = true;
        amb.playOnAwake = true;
        amb.spatialBlend = 0f;
        amb.volume = 0.5f;

        // ---------- Limites invisíveis (reforço das paredes do terreno)
        var limites = new GameObject("LimitesInvisiveis").transform;
        Parede(limites, new Vector3(-0.5f, 50f, TerrainL / 2f), new Vector3(1f, 100f, TerrainL));
        Parede(limites, new Vector3(TerrainW + 0.5f, 50f, TerrainL / 2f), new Vector3(1f, 100f, TerrainL));
        Parede(limites, new Vector3(TerrainW / 2f, 50f, -0.5f), new Vector3(TerrainW, 100f, 1f));
        Parede(limites, new Vector3(TerrainW / 2f, 50f, TerrainL + 0.5f), new Vector3(TerrainW, 100f, 1f));
        Parede(limites, new Vector3(TerrainW / 2f, -3f, TerrainL / 2f), new Vector3(TerrainW, 1f, TerrainL)); // piso de segurança

        // ---------- Câmera da cena (o prefab traz a do jogador) e névoa do Lighting
        var cam = GameObject.Find("Main Camera");
        if (cam != null && cam.transform.root == cam.transform) UnityEngine.Object.DestroyImmediate(cam);

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogDensity = 0.02f;
        RenderSettings.fogColor = new Color(0.62f, 0.60f, 0.48f);
    }

    static void Ligar(UnityEngine.Object componente, string campo, UnityEngine.Object valor)
    {
        if (componente == null) return;
        var so = new SerializedObject(componente);
        var prop = so.FindProperty(campo);
        if (prop == null) return;
        prop.objectReferenceValue = valor;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void Parede(Transform parent, Vector3 pos, Vector3 size)
    {
        var go = new GameObject("Limite");
        go.transform.SetParent(parent, false);
        go.transform.position = pos;
        go.AddComponent<BoxCollider>().size = size;
    }

    // ------------------------------------------------------ lista para o Apêndice B

    [MenuItem("EcoScrapper/Listar assets da Nivel1 (Apêndice B)")]
    public static void ListarAssets()
    {
        var contagem = new SortedDictionary<string, int>();
        foreach (var go in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (!PrefabUtility.IsOutermostPrefabInstanceRoot(go)) continue;
            var src = PrefabUtility.GetCorrespondingObjectFromSource(go);
            if (src == null) continue;
            string path = AssetDatabase.GetAssetPath(src);
            contagem.TryGetValue(path, out int n);
            contagem[path] = n + 1;
        }
        var sb = new System.Text.StringBuilder();
        foreach (var kv in contagem) sb.AppendLine(Path.GetFileNameWithoutExtension(kv.Key) + " x" + kv.Value + "  (" + kv.Key + ")");
        Debug.Log("Assets diferentes na cena: " + contagem.Count + "\n" + sb);
    }
}
