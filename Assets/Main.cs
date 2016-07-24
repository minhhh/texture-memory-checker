using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text;
using System.Diagnostics;

public class Main : MonoBehaviour
{
    int textureNumber = 0;
    Text txtTextureNum = null;
    Text txtMemInfo = null;
    bool needToUpdateMem = true;

    const int BytesInMB = 1024 * 1024;


    // Use this for initialization
    void Start ()
    {
        txtTextureNum = GameObject.Find ("textureNum").GetComponent <Text> ();
        txtMemInfo = GameObject.Find ("memInfo").GetComponent <Text> ();
    }

    public static string GetProcessMemoryInfo (string delim = " ")
    {
        StringBuilder builder = new StringBuilder ();

        #if DEVELOPMENT_BUILD || UNITY_EDITOR
        uint monoUsed = Profiler.GetMonoUsedSize ();
        uint monoSize = Profiler.GetMonoHeapSize ();
        builder.AppendFormat ("Mono:{0}/{1}MB{2}", monoUsed / BytesInMB, monoSize / BytesInMB, delim);
        #endif

        uint totalUsed = Profiler.GetTotalAllocatedMemory ();
        uint totalSize = Profiler.GetTotalReservedMemory ();

        builder.AppendFormat ("Alloc:{0}/{1}MB{2}", totalUsed / BytesInMB, totalSize / BytesInMB, delim);
        long memory = GetProcessMemory ();
        builder.AppendFormat ("Proc:{0}/{1}MB{2}", (int)memory / BytesInMB, SystemInfo.systemMemorySize, delim);
        return builder.ToString ();
    }

    public static string GetProcessMemoryDetail ()
    {
        StringBuilder builder = new StringBuilder ();
        builder.Append (GetProcessMemoryInfo ()).Append ("\n");

        string memoryDetail = "";
        #if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass cls = new AndroidJavaClass("com.android.utils.androidmem.AndroidMem");
        memoryDetail = cls.CallStatic<string>("getMemoryDetailInfo");
        #else
        #endif
        builder.Append (memoryDetail);
        return builder.ToString ();
    }

    public static long GetProcessMemory ()
    {
        long processMem = 0;
        #if UNITY_ANDROID && !UNITY_EDITOR
        processMem = Process.GetCurrentProcess ().WorkingSet64;
        #else
        processMem = Profiler.GetTotalReservedMemory ();
        #endif
        return processMem;
    }


    // Update is called once per frame
    void Update ()
    {
        if (needToUpdateMem) {
            txtMemInfo.text = GetProcessMemoryDetail ();
            needToUpdateMem = false;
        }
    }

    public void AddMoreTexture ()
    {
        textureNumber += 1;

        var go = new GameObject ();
        var spriteRenderer = go.AddComponent< SpriteRenderer> ();
        spriteRenderer.sprite = Create2048Sprite ();
        spriteRenderer.material.color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
        go.transform.localScale = Vector3.one / 10;
        go.transform.position = new Vector3 (-1 + UnityEngine.Random.RandomRange (0, 15) / 10f, -2f - UnityEngine.Random.RandomRange (0, 15) / 10f, -textureNumber/100f);

        txtTextureNum.text = "Number of 2048x2048 textures " + textureNumber.ToString ();

        needToUpdateMem = true;
    }

    public Sprite Create2048Sprite ()
    {
        const int size = 2048;
        var texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
        int color = UnityEngine.Random.Range(0, 255);
        for (var i = 0; i < 2048; i++) {
            for (var j = 0; j < 2048; j++) {
                texture.SetPixel (i, j, Color.white);
            }
        }
            
        // Apply all SetPixel calls
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
