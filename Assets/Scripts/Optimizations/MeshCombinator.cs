using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using System.Collections;

public class MeshCombinator : MonoBehaviour
{
    [DefaultValue(true)]
    public bool CombineAutomatically;

    public List<GameObject> CombinedGameObjects;

    private List<GameObject> objectsUsed;

    private const float MaxVertices = 65000.0f;

    public MeshCombinator()
    {
        this.CombineAutomatically = true;
        this.CombinedGameObjects = new List<GameObject>();
        this.objectsUsed = new List<GameObject>();
    }

    private void Start()
    {
        if (CombineAutomatically)
        {
            if (this.CombinedGameObjects == null)
            {
                this.CombinedGameObjects = new List<GameObject>();
            }

            this.Combine();
        }
    }

    private void Update()
    {

    }

    public void Combine()
    {
        Dictionary<string, List<MeshFilter>> filtersDictionary = this.GetEnabledMeshes();

        foreach (KeyValuePair<string, List<MeshFilter>> filtersPair in filtersDictionary)
        {
            int filterVerticesCount = filtersPair.Value.Sum(filter => filter.transform.GetComponent<MeshFilter>().sharedMesh.vertexCount);
            int splitsCount = Mathf.CeilToInt(filterVerticesCount / MaxVertices);
            int filtersInSplit = filtersPair.Value.Count / splitsCount;

            for (int splitIndex = 0; splitIndex < splitsCount; splitIndex++)
            {

                List<Material> sharedMaterials = new List<Material>();
                List<CombineInstance> combineInstances = new List<CombineInstance>();

                for (int filterIndex = 0; filterIndex < filtersInSplit; filterIndex++)
                {
                    Transform filterTransform = GetTransformFromFiltersPair(filtersPair, splitIndex, filtersInSplit, filterIndex);

                    sharedMaterials.AddRange(filterTransform.gameObject.GetComponent<MeshRenderer>().sharedMaterials);

                    combineInstances.Add(
                        new CombineInstance
                        {
                            mesh = filterTransform.GetComponent<MeshFilter>().sharedMesh,
                            transform = filterTransform.localToWorldMatrix
                        });

                    SaveToObjectsUsed(filterTransform.gameObject);
                }

                GameObject combinedGameObject = SetupCombinedGameObject(combineInstances, sharedMaterials, filtersPair.Value.FirstOrDefault(), splitIndex);
                this.CombinedGameObjects.Add(combinedGameObject);
            }
        }

        this.DisableRenderers(this.objectsUsed);
    }

    private GameObject SetupCombinedGameObject(
        List<CombineInstance> combineInstances,
        List<Material> materials,
        MeshFilter nameFilter,
        int nameIndex)
    {
        GameObject combinedGameObject = new GameObject();
        combinedGameObject.name = "_Combined Mesh [" + nameFilter + "]_" + nameIndex;
        combinedGameObject.transform.parent = this.transform;

        AddMeshFilterToCombinedGameObject(combineInstances.ToArray(), combinedGameObject);
        AddMeshRendererToCombinedGameObject(materials, combinedGameObject);

        return combinedGameObject;
    }

    private static void AddMeshRendererToCombinedGameObject(List<Material> sharedMaterials, GameObject combinedGameObject)
    {
        MeshRenderer meshRenderer = combinedGameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterials = sharedMaterials.ToArray();
    }

    private static void AddMeshFilterToCombinedGameObject(CombineInstance[] combineInstances, GameObject combinedGameObject)
    {
        MeshFilter meshFilter = combinedGameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.sharedMesh.CombineMeshes(combineInstances);
    }

    private static Transform GetTransformFromFiltersPair(KeyValuePair<string, List<MeshFilter>> filtersPair, int i, int filtersInSplit, int j)
    {
        return filtersPair.Value[i * filtersInSplit + j].transform;
    }

    private void SaveToObjectsUsed(GameObject gameObject)
    {
        this.objectsUsed.Add(gameObject);
    }

    public void UnCombine()
    {
        this.EnableRenderers(this.objectsUsed);

        foreach (GameObject combinedGameObject in this.CombinedGameObjects)
        {
            DestroyImmediate(combinedGameObject);
        }

        this.CombinedGameObjects.Clear();
    }

    private Dictionary<string, List<MeshFilter>> GetEnabledMeshes()
    {
        MeshFilter[] filters;
        filters = this.transform.GetComponentsInChildren<MeshFilter>();

        return GetEnabledMeshesFromFilters(filters);
    }

    private Dictionary<string, List<MeshFilter>> GetEnabledMeshesFromFilters(MeshFilter[] filters)
    {
        Dictionary<string, List<MeshFilter>> filtersDictionary = new Dictionary<string, List<MeshFilter>>();

        foreach (MeshFilter filter in filters)
        {
            MeshRenderer renderer = filter.GetComponent<MeshRenderer>();

            if (IsRendererEnabled(renderer))
            {
                foreach (string materialName in renderer.sharedMaterials.Select(material => material.name))
                {
                    if (filtersDictionary.ContainsKey(materialName))
                    {
                        filtersDictionary[materialName].Add(filter);
                    }
                    else
                    {
                        filtersDictionary.Add(materialName, new List<MeshFilter>() { filter });
                    }
                }
            }
        }

        return filtersDictionary;
    }

    private static bool IsRendererEnabled(MeshRenderer renderer)
    {
        return renderer != null && renderer.enabled;
    }

    private void EnableRenderers(List<GameObject> gameObjects)
    {
        foreach (GameObject combinedGameObject in gameObjects)
        {
            combinedGameObject.GetComponent<Renderer>().enabled = true;
        }
    }

    private void DisableRenderers(List<GameObject> gameObjects)
    {
        foreach (GameObject combinedGameObject in gameObjects)
        {
            combinedGameObject.GetComponent<Renderer>().enabled = false;
        }
    }
}
