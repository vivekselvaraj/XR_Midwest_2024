using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableSpawner : MonoBehaviour
{
    public GameObject tablePrefab;
    public OVRSkeleton LeftSkeleton;
    public OVRSkeleton RightSkeleton;
    public GameObject DebugUI;

    public void RemoveDebugUI() {
        if (DebugUI.activeSelf) {
            DebugUI.SetActive(false);
        }
        else {
            DebugUI.SetActive(true);
        }
    }
    
    public void SpawnTable() {
        Transform leftThumbTipTransform = GetFingerTransformForSkeleton(LeftSkeleton, OVRSkeleton.BoneId.Hand_ThumbTip);
        Transform rightThumbTipTransform = GetFingerTransformForSkeleton(RightSkeleton, OVRSkeleton.BoneId.Hand_ThumbTip);
        
        Vector3 midpoint = (leftThumbTipTransform.position + rightThumbTipTransform.position) / 2f;

        Vector3 direction = leftThumbTipTransform.position - rightThumbTipTransform.position;
        Vector3 perpendicular = -Vector3.Cross(direction.normalized, Vector3.up);
        Quaternion rotation = Quaternion.LookRotation(perpendicular);
        
        // if a object with tag table exists, destroy it
        GameObject oldTable = GameObject.FindGameObjectWithTag("Table");
        if (oldTable != null) {
            // Destoy(oldTable);
            Debug.Log("Table already exists");
            return;
        }
        GameObject table = Instantiate(tablePrefab, midpoint, rotation);
        
        
        // Passthrough enable for keyboard
        GameObject scripts = GameObject.Find("Scripts");
        OVRPassthroughLayer layer = scripts.GetComponent<OVRPassthroughLayer>();
        GameObject passthroughSurface = table.transform.Find("Plane").gameObject;
        layer.AddSurfaceGeometry(passthroughSurface, false);

        // Disable the mesh renderer to avoid rendering the surface within Unity
        MeshRenderer mr = passthroughSurface.GetComponent<MeshRenderer>();
        if (mr) {
            mr.enabled = false;
        }
    }

    public Transform GetFingerTransformForSkeleton(OVRSkeleton skeleton, OVRSkeleton.BoneId boneId) {
        foreach (var b in skeleton.Bones) {
            if (b.Id == boneId) {
                return b.Transform;
            }
        }

        return null;
    }

}
