using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RunComputeShader : MonoBehaviour {
    public ComputeShader shader;
    RenderTexture texture;
    int numThreadsGroupsX, numThreadsGroupsY, numThreadsGroupsZ = 1;

    void Start() {
        UpdateTextureSize();
    }

    void UpdateTextureSize() {
        var width = Screen.width;
        var height = Screen.height;
        if (texture != null && texture.width == width && texture.height == height) {
            return;
        }
        if (texture != null) {
            texture.Release();
        }

        texture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        texture.filterMode = FilterMode.Point;
        texture.enableRandomWrite = true;
        texture.Create();

        var rawImage = GetComponent<RawImage>();
        rawImage.rectTransform.anchorMin = Vector2.zero;
        rawImage.rectTransform.anchorMax = Vector2.one;
        rawImage.rectTransform.sizeDelta = Vector2.zero;
        rawImage.texture = texture;

        uint numThreadsX, numThreadsY, numThreadsZ;
        shader.GetKernelThreadGroupSizes(0, out numThreadsX, out numThreadsY, out numThreadsZ);
        numThreadsGroupsX = (int)(texture.width / numThreadsX);
        numThreadsGroupsY = (int)(texture.height / numThreadsY);

        shader.SetTexture(0, "Result", texture);
        shader.SetVector("iResolution", new Vector4(width, height));
    }

    private void Update() {
        UpdateTextureSize();

        shader.SetFloat("iGlobalTime", Time.time);
        shader.Dispatch(0, numThreadsGroupsX, numThreadsGroupsY, numThreadsGroupsZ);
    }
}
