using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Effects/Gradient")]

    #if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
    public class Gradient : BaseVertexEffect
    #else
    public class Gradient : BaseMeshEffect
    #endif
    {
        [SerializeField]
        private Color32 topColor = Color.white;
        [SerializeField]
        private Color32 bottomColor = Color.black;


        #if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
        public override void ModifyVertices(List<UIVertex> vertexList)
        {
            if(!IsActive() || vertexList == null || vertexList.Count == 0)
                return;
        #else
        public override void ModifyMesh(VertexHelper vh)
        {
            if(!IsActive())
                return;

            List<UIVertex> vertexList = new List<UIVertex>();
            vh.GetUIVertexStream(vertexList);

            if(vertexList == null || vertexList.Count == 0)
                return;
        #endif

            // get bound position.
            float topY = 0f;
            float bottomY = 0f;

            for(int i = 0; i < vertexList.Count; i++)
            {
                float y = vertexList[i].position.y;
                if(y > topY)
                    topY = y;
                else if(y < bottomY)
                    bottomY = y;
            }

            // set vertex color.
            float height = topY - bottomY;
            for(int i = 0; i < vertexList.Count; i++)
            {
                UIVertex vertext = vertexList[i];
                vertext.color = Color32.Lerp(bottomColor, topColor, (vertext.position.y - bottomY)/height);
                vertexList[i] = vertext;
            }

            #if !UNITY_4_6 && !UNITY_5_0 && !UNITY_5_1
            vh.Clear();
            vh.AddUIVertexTriangleStream(vertexList);
            #endif
        }
    }
}
