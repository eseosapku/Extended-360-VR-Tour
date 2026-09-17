using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// Central manager for the 360 video tour. Keeps track of every sphere
/// environment (LivingRoom, Cantina, Cube, Mezzanine) and switches between
/// them, making sure only one sphere is active and only one video plays
/// at a time (per the assignment's performance note).
///
/// Setup:
/// 1. Create an empty GameObject in the scene called "TourManager".
/// 2. Add this script to it.
/// 3. In the Inspector, expand "Spheres" and set its size to 4.
/// 4. For each entry, set sphereName to match exactly what your hotspots
///    will target ("LivingRoom", "Cantina", "Cube", "Mezzanine"), and drag
///    the matching sphere GameObject into sphereObject. VideoPlayer will
///    be found automatically if left empty.
/// 5. Drag your XR Origin (VR) transform into the xrOrigin field so the
///    player gets repositioned into whichever sphere becomes active.
/// </summary>
public class TourManager : MonoBehaviour
{
    public static TourManager Instance { get; private set; }

    [Serializable]
    public class SphereEntry
    {
        [Tooltip("Must match the name used by hotspots that target this sphere, e.g. \"Cantina\".")]
        public string sphereName;

        [Tooltip("The sphere GameObject that holds the mesh, material, and Video Player for this location.")]
        public GameObject sphereObject;

        [Tooltip("The Video Player component on this sphere. Leave empty to auto-find it on sphereObject.")]
        public VideoPlayer videoPlayer;
    }

    [Header("Locations")]
    public List<SphereEntry> spheres = new List<SphereEntry>();

    [Header("Player Rig")]
    [Tooltip("Drag your XR Origin (VR) transform here so it gets repositioned into the active sphere.")]
    public Transform xrOrigin;

    [Header("Startup")]
    [Tooltip("Name of the sphere that should be active when the scene starts.")]
    public string startingSphere = "LivingRoom";

    void Awake()
    {
        Instance = this;

        // Auto-fill missing VideoPlayer references from the sphere object.
        foreach (var entry in spheres)
        {
            if (entry.videoPlayer == null && entry.sphereObject != null)
            {
                entry.videoPlayer = entry.sphereObject.GetComponent<VideoPlayer>();
            }
        }
    }

    void Start()
    {
        SwitchToSphere(startingSphere);
    }

    /// <summary>
    /// Activates the named sphere (and its video), deactivates all others,
    /// and moves the XR rig to that sphere's position. Called by Hotspot.cs
    /// when the player selects a hotspot button.
    /// </summary>
    public void SwitchToSphere(string targetName)
    {
        SphereEntry target = null;

        foreach (var entry in spheres)
        {
            bool isTarget = string.Equals(entry.sphereName, targetName, StringComparison.OrdinalIgnoreCase);

            if (entry.sphereObject != null)
            {
                entry.sphereObject.SetActive(isTarget);
            }

            if (entry.videoPlayer != null)
            {
                if (isTarget)
                {
                    entry.videoPlayer.Play();
                }
                else
                {
                    entry.videoPlayer.Stop();
                }
            }

            if (isTarget)
            {
                target = entry;
            }
        }

        if (target == null)
        {
            Debug.LogWarning($"TourManager: no sphere named \"{targetName}\" found. Check spelling against the Spheres list.");
            return;
        }

        if (xrOrigin != null && target.sphereObject != null)
        {
            xrOrigin.position = target.sphereObject.transform.position;
        }
    }
}
