using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;


namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Effects/Gradient")]
	#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
    public class GradientEffect : BaseVertexEffect
	#else
	public class GradientEffect : BaseMeshEffect
	#endif
	{
        public enum Direction
		{
            Horizontal,
			Vertical,			
		}

        public Direction direction = Direction.Vertical;
        public Gradient gradient;

		#if UNITY_4_6 || UNITY_5_0 || UNITY_5_1
		public override void ModifyVertices(List<UIVertex> vertexList)
		{
			// GameComponentとComponentのアクティブチェック・頂点リストの有無・頂点数が0ではないか、UnityEngineのGradientアクティブチェック.
			if (IsActive() == false || vertexList == null || vertexList.Count == 0 || _gradient == null)
			{
				return;
			}

		#else
		public override void ModifyMesh(VertexHelper vh)
		{
			// GameComponentとComponentのアクティブチェック.
			if (IsActive() == false || gradient == null)
			{
				return;
			}

			List<UIVertex> vertexList = new List<UIVertex>();
			vh.GetUIVertexStream(vertexList);

			//頂点リストの有無・頂点数が0ではないか・文字が1文字じゃないかチェック.
			if (vertexList == null || vertexList.Count == 0) 
			{
				return;
			}

		#endif
			float top = vertexList.Select(vertex => vertex.position.y).Max();
			float bottom = vertexList.Select(vertex => vertex.position.y).Min();
			float right = vertexList.Select(vertex => vertex.position.x).Max();
			float left = vertexList.Select(vertex => vertex.position.x).Min();

			for (int i = 0; i < vertexList.Count; i++)
			{
				UIVertex uiVertex = vertexList[i];
				if (direction == Direction.Vertical)
				{
					uiVertex.color = gradient.Evaluate((uiVertex.position.y - bottom) / (top - bottom));
				}
				else
				{
					uiVertex.color = gradient.Evaluate((uiVertex.position.x - left) / (right - left));
				}
				vertexList[i] = uiVertex;
			}

			#if !UNITY_4_6 && !UNITY_5_0 && !UNITY_5_1
			vh.Clear();
			vh.AddUIVertexTriangleStream(vertexList);
			#endif
		}
	}
}
