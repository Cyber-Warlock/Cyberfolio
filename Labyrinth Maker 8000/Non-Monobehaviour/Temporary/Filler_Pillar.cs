using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filler_Pillar
{
    GameObject pillar;
    Graph.Edge[] associatedEdges;

    public Filler_Pillar(GameObject pillar)
    {
        this.pillar = pillar;
    }

    public bool ShouldBeDestroyed()
    {
        if (associatedEdges == null)
            return false;

        for (int i = 0; i < associatedEdges.Length; i++)
        {
            if (associatedEdges[i].Wall != null)
                return false;
        }

        if (Application.isEditor)
            Object.DestroyImmediate(pillar);
        else
            Object.Destroy(pillar);
        return true;
    }

    public void PopulateEdges(List<Graph.Edge> relatedEdges)
    {
        if (relatedEdges.Count > 4)
        {
            Debug.LogWarning("Related edges for a pillar should never exceed 4.");
            return;
        }

        associatedEdges = relatedEdges.ToArray();
    }
}
