using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BendImage : MaskableGraphic
{
    public int segmentCount = 8;
    public float bendAmount = 0;

    [SerializeField]
    Texture m_Texture;

    protected BendImage()
    {
        useLegacyMeshGeneration = false;
    }

    /// <summary>
    /// Returns the texture used to draw this Graphic.
    /// </summary>
    public override Texture mainTexture
    {
        get
        {
            if (m_Texture == null)
            {
                if (material != null && material.mainTexture != null)
                {
                    return material.mainTexture;
                }
                return s_WhiteTexture;
            }

            return m_Texture;
        }
    }

    /// <summary>
    /// Texture to be used.
    /// </summary>
    public Texture texture
    {
        get
        {
            return m_Texture;
        }
        set
        {
            if (m_Texture == value)
                return;

            m_Texture = value;
            SetVerticesDirty();
            SetMaterialDirty();
        }
    }

    /// <summary>
    /// Adjust the scale of the Graphic to make it pixel-perfect.
    /// </summary>

    public override void SetNativeSize()
    {
        Texture tex = mainTexture;
        if (tex != null)
        {
            int w = tex.width;
            int h = tex.height;
            rectTransform.anchorMax = rectTransform.anchorMin;
            rectTransform.sizeDelta = new Vector2(w, h);
        }
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        Texture tex = mainTexture;
        vh.Clear();
        if (tex != null && segmentCount > 0)
        {
            var r = GetPixelAdjustedRect();
            var v = new Vector4(r.x, r.y, r.x + r.width, r.y + r.height);
            //var scaleX = tex.width * tex.texelSize.x;
            //var scaleY = tex.height * tex.texelSize.y;
            var color32 = color;

            float segmentHeight = r.height/segmentCount;
            v.w = r.y + segmentHeight;

            float uvV = 0;
            float uvIncrement = 1.0f / segmentCount;

            for (int i = 0; i < segmentCount; i++)
            {
                int baseVertIndex = vh.currentVertCount;

                float ratioBottom = (float)i / segmentCount;
                float ratioTop = ((float)i+1) / segmentCount;

                float bendBottom = Mathf.Sin(ratioBottom * Mathf.PI);
                float bendBottomOffset = bendBottom * bendAmount;

                float bendTop = Mathf.Sin(ratioTop * Mathf.PI);
                float bendTopOffset = bendTop * bendAmount;

                vh.AddVert(new Vector3(v.x + bendBottomOffset, v.y), color32, new Vector2(0, uvV));
                vh.AddVert(new Vector3(v.x + bendTopOffset, v.w), color32, new Vector2(0, uvV + uvIncrement));
                vh.AddVert(new Vector3(v.z + bendTopOffset, v.w), color32, new Vector2(1, uvV + uvIncrement));
                vh.AddVert(new Vector3(v.z + bendBottomOffset, v.y), color32, new Vector2(1, uvV));

                v.y += segmentHeight;
                v.w += segmentHeight;
                uvV += uvIncrement;
                vh.AddTriangle(baseVertIndex + 0, baseVertIndex + 1, baseVertIndex + 2);
                vh.AddTriangle(baseVertIndex + 2, baseVertIndex + 3, baseVertIndex + 0);
            }
        }
    }
}
