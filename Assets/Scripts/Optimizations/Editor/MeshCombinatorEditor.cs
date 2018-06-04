using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(MeshCombinator))]
public class MeshCombinatorEditor : Editor
{
    private MeshCombinator meshCombinator;

    private void OnEnable()
    {
        this.meshCombinator = (MeshCombinator)target;
    }

    public override void OnInspectorGUI()
    {
        if (AreAnyObjectsCombined())
        {
            AddCombineButton();
        }
        else
        {
            AddUncombineButton();
        }
    }

    private bool AreAnyObjectsCombined()
    {
        return meshCombinator.CombinedGameObjects == null || meshCombinator.CombinedGameObjects.Count == 0;
    }

    private void AddCombineButton()
    {
        meshCombinator.CombineAutomatically = EditorGUILayout.Toggle(
            "Combine When Launched",
            meshCombinator.CombineAutomatically);

        if (GUILayout.Button("Combine"))
        {
            this.meshCombinator.Combine();
        }
    }

    private void AddUncombineButton()
    {
        if (GUILayout.Button("Undo Combine"))
        {
            this.meshCombinator.UnCombine();
        }
    }
}
