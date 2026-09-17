using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Put this on a hotspot button object (a small mesh with a Collider and an
/// XR Simple Interactable component). Works automatically for BOTH controller
/// point-and-click (any Ray Interactor's select/trigger input) and gaze
/// dwell-select (an XR Gaze Interactor), because both fire the same
/// selectEntered event through XR Simple Interactable -- no separate code
/// path needed for each input method.
///
/// Setup on each hotspot object:
/// 1. Add a Collider (a Sphere Collider or Box Collider works well).
/// 2. Add Component > XR Simple Interactable.
/// 3. On that XR Simple Interactable, under "Interaction Layer Settings" /
///    gaze configuration, tick "Allow Gaze Interaction" and "Allow Gaze
///    Select" so an XR Gaze Interactor can hover/select it too.
/// 4. Add this Hotspot script.
/// 5. Set targetSphereName to the sphere this button should take the user
///    to (must match a sphereName entry in TourManager).
/// 6. Optionally drag the hotspot's Renderer into visualRenderer for a
///    color-change + scale-pulse hover effect (satisfies the "visual
///    feedback" requirement for both controller and gaze interaction).
/// </summary>
[RequireComponent(typeof(XRSimpleInteractable))]
public class Hotspot : MonoBehaviour
{
    [Tooltip("Must match a sphereName entry in TourManager, e.g. \"Cantina\".")]
    public string targetSphereName;

    [Header("Visual Feedback (optional)")]
    [Tooltip("Renderer whose material color changes on hover. Leave empty to skip color feedback.")]
    public Renderer visualRenderer;
    public Color normalColor = new Color(0.8f, 0.1f, 0.1f);
    public Color hoverColor = Color.yellow;

    [Tooltip("Local scale multiplier applied while hovered, for a simple pulse effect. 1 = no scale change.")]
    public float hoverScaleMultiplier = 1.15f;

    XRSimpleInteractable interactable;
    Vector3 baseScale;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        baseScale = transform.localScale;

        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.selectEntered.AddListener(OnSelectEntered);

        if (visualRenderer != null)
        {
            visualRenderer.material.color = normalColor;
        }
    }

    void OnDestroy()
    {
        if (interactable == null) return;
        interactable.hoverEntered.RemoveListener(OnHoverEntered);
        interactable.hoverExited.RemoveListener(OnHoverExited);
        interactable.selectEntered.RemoveListener(OnSelectEntered);
    }

    void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (visualRenderer != null) visualRenderer.material.color = hoverColor;
        transform.localScale = baseScale * hoverScaleMultiplier;
    }

    void OnHoverExited(HoverExitEventArgs args)
    {
        if (visualRenderer != null) visualRenderer.material.color = normalColor;
        transform.localScale = baseScale;
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (TourManager.Instance != null)
        {
            TourManager.Instance.SwitchToSphere(targetSphereName);
        }
        else
        {
            Debug.LogWarning("Hotspot: no TourManager found in scene. Add one and assign it before testing.");
        }
    }
}
