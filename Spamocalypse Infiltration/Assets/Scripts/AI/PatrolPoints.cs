using UnityEngine;

/// <summary>
/// A dummy MonoBehaviour that is attached to a patrol point to stop me trawling through Transforms.
/// </summary>
public class PatrolPoints : MonoBehaviour {

    Transform myTransform;

    void Start()
    {
        myTransform = transform;
    }

    public Vector3 GetPosition()
    {
        if (myTransform == null)
        {
            myTransform = transform;
        }
        return myTransform.position;
    }
}
