using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class PrefabCreator : MonoBehaviour
{
    [SerializeField] private GameObject prefab1; // Prefab for trigger_1
    [SerializeField] private GameObject prefab2; // Prefab for trigger_2
    [SerializeField] private GameObject prefab3; // Prefab for trigger_3
    [SerializeField] private Vector3 prefabOffset;

    private ARTrackedImageManager arTrackedImageManager;
    private Dictionary<string, GameObject> instantiatedPrefabs = new Dictionary<string, GameObject>();

    private void OnEnable()
    {
        arTrackedImageManager = gameObject.GetComponent<ARTrackedImageManager>();
        arTrackedImageManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable()
    {
        arTrackedImageManager.trackedImagesChanged -= OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs obj)
    {
        foreach (ARTrackedImage image in obj.added)
        {
            HandleTrackedImage(image);
        }

        foreach (ARTrackedImage image in obj.updated)
        {
            HandleTrackedImage(image);
        }

        foreach (ARTrackedImage image in obj.removed)
        {
            if (instantiatedPrefabs.ContainsKey(image.referenceImage.name))
            {
                Destroy(instantiatedPrefabs[image.referenceImage.name]);
                instantiatedPrefabs.Remove(image.referenceImage.name);
            }
        }
    }

    private void HandleTrackedImage(ARTrackedImage trackedImage)
    {
        if (!instantiatedPrefabs.ContainsKey(trackedImage.referenceImage.name))
        {
            GameObject prefabToInstantiate = null;

            // Check the reference image name and assign the corresponding prefab
            if (trackedImage.referenceImage.name == "trigger_1")
            {
                prefabToInstantiate = prefab1;
            }
            else if (trackedImage.referenceImage.name == "trigger_2")
            {
                prefabToInstantiate = prefab2;
            }
            else if (trackedImage.referenceImage.name == "trigger_3")
            {
                prefabToInstantiate = prefab3;
            }

            if (prefabToInstantiate != null)
            {
                GameObject instantiatedPrefab = Instantiate(prefabToInstantiate, trackedImage.transform);
                instantiatedPrefab.transform.position += prefabOffset;
                instantiatedPrefabs[trackedImage.referenceImage.name] = instantiatedPrefab;
            }
        }
        else
        {
            // Update the position of the existing prefab if needed
            GameObject existingPrefab = instantiatedPrefabs[trackedImage.referenceImage.name];
            existingPrefab.transform.position = trackedImage.transform.position + prefabOffset;
        }
    }
}
