using UnityEngine;

public class ApplyMaterialToChildren : MonoBehaviour
{
    public PhysicMaterial SlipperyTrees;
    public Material Green;

    void Start()
    {
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.material = SlipperyTrees;
        }

        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material = Green;
        }
    }
}
