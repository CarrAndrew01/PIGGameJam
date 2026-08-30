using UnityEngine.UI;
using UnityEngine;

// Line renderer used for fishing line. Code taken from here: https://www.youtube.com/watch?v=--LB7URk60A and frankensteined by me

public class UILineRenderer : Graphic
{
    public float lineWidth = 5f;
    public bool tileTexture = true;
    public float textureRepeatPerUnit = 0.05f;
    public float scrollSpeed = 0f;
    public float currentOffset = 0f;
    public Vector2[] points;

    public override Texture mainTexture
    {
        get
        {
            if (material != null && material.mainTexture != null)
                return material.mainTexture;

            return Texture2D.whiteTexture;
        }
    }

    protected virtual void Update()
    {
        if (!Application.isPlaying)
            return;

        if (Mathf.Approximately(scrollSpeed, 0f))
            return;

        currentOffset += scrollSpeed * Time.unscaledDeltaTime;
        SetVerticesDirty();
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();

        if (points == null || points.Length < 2)
            return;

        float[] cumulativeLengths = new float[points.Length];
        cumulativeLengths[0] = 0f;
        for (int i = 1; i < points.Length; i++)
        {
            cumulativeLengths[i] = cumulativeLengths[i - 1] + Vector2.Distance(points[i - 1], points[i]);
        }

        float totalLength = cumulativeLengths[points.Length - 1];
        if (totalLength <= 0f)
            totalLength = 1f;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 start = points[i];
            Vector2 end = points[i + 1];

            Vector2 direction = (end - start).normalized;
            Vector2 normal = new Vector2(-direction.y, direction.x) * (lineWidth / 2f);

            Vector2 v1 = start + normal;
            Vector2 v2 = start - normal;
            Vector2 v3 = end - normal;
            Vector2 v4 = end + normal;

            int vertexIndex = vh.currentVertCount;

            float uStart;
            float uEnd;
            if (tileTexture)
            {
                uStart = cumulativeLengths[i] * textureRepeatPerUnit + currentOffset;
                uEnd = cumulativeLengths[i + 1] * textureRepeatPerUnit + currentOffset;
            }
            else
            {
                uStart = cumulativeLengths[i] / totalLength + currentOffset;
                uEnd = cumulativeLengths[i + 1] / totalLength + currentOffset;
            }

            vh.AddVert(v1, color, new Vector2(uStart, 0f));
            vh.AddVert(v2, color, new Vector2(uStart, 1f));
            vh.AddVert(v3, color, new Vector2(uEnd, 1f));
            vh.AddVert(v4, color, new Vector2(uEnd, 0f));

            vh.AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + 2);
            vh.AddTriangle(vertexIndex + 2, vertexIndex + 3, vertexIndex);
        }
    }
}
