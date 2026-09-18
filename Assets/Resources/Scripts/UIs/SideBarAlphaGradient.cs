using UnityEngine;
using UnityEngine.UI;

public class SideBarAlphaGradient : BaseMeshEffect
{
    public float gradientWidth = 100f;

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!IsActive() || vh.currentVertCount == 0)
            return;

        var rect = graphic.rectTransform.rect;
        float leftEdge = rect.xMin;

        for (int i = 0; i < vh.currentVertCount; i++)
        {
            UIVertex vertex = new UIVertex();
            vh.PopulateUIVertex(ref vertex, i);

            float t = (vertex.position.x - leftEdge) / gradientWidth;
            t = Mathf.Clamp01(t);

            Color c = vertex.color;
            c.a *= t;
            vertex.color = c;

            vh.SetUIVertex(vertex, i);
        }
    }
}
