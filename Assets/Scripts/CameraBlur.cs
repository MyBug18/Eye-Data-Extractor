using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBlur : MonoBehaviour
{
    public Material blurMat;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        var temporaryTexture = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit(source, temporaryTexture, blurMat, 0);
        Graphics.Blit(temporaryTexture, destination, blurMat, 1);
        RenderTexture.ReleaseTemporary(temporaryTexture);
    }
}
