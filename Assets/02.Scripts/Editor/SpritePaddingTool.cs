#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

public class SpritePaddingTool : EditorWindow
{
    private int _padding = 10;

    [MenuItem("Tools/Sprite Padding Tool")]
    private static void ShowWindow()
    {
        GetWindow<SpritePaddingTool>("Sprite Padding");
    }

    private void OnGUI()
    {
        GUILayout.Label("선택한 텍스처에 투명 여백 추가", EditorStyles.boldLabel);
        _padding = EditorGUILayout.IntSlider("Padding (px)", _padding, 1, 50);

        if (GUILayout.Button("Apply Padding"))
        {
            ApplyPaddingToSelected();
        }
    }

    private void ApplyPaddingToSelected()
    {
        foreach (Object obj in Selection.objects)
        {
            if (obj is not Texture2D texture)
            {
                continue;
            }

            string path = AssetDatabase.GetAssetPath(texture);
            SetTextureReadable(path, true);

            // 리임포트 후 다시 가져오기.
            texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            AddPadding(texture, path);
        }

        AssetDatabase.Refresh();
        Debug.Log("Padding 적용 완료.");
    }

    private void SetTextureReadable(string path, bool readable)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            return;
        }

        importer.isReadable = readable;
        importer.SaveAndReimport();
    }

    private void AddPadding(Texture2D source, string path)
    {
        int newWidth = source.width + _padding * 2;
        int newHeight = source.height + _padding * 2;

        var padded = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);

        // 전체를 투명으로 초기화.
        var clearPixels = new Color[newWidth * newHeight];
        padded.SetPixels(clearPixels);

        // 원본을 중앙에 복사.
        Color[] sourcePixels = source.GetPixels();
        padded.SetPixels(_padding, _padding, source.width, source.height, sourcePixels);
        padded.Apply();

        byte[] pngData = padded.EncodeToPNG();
        File.WriteAllBytes(path, pngData);

        // Read/Write 끄기.
        SetTextureReadable(path, false);
    }
}
#endif