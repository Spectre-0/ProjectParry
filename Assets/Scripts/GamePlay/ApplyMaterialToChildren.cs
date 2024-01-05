using UnityEngine;

public class ApplyMaterialToChildren : MonoBehaviour
{
    public Material materials;

    void Start()
    {

        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.material = materials;
        }
    }
}
