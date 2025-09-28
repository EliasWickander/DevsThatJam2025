using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField]
    private PlayerInput m_playerInput;
    
    [SerializeField] 
    private float m_interactionRadius = 3f;

    [SerializeField]
    private float m_interactionWidth = 0.5f;

    [SerializeField]
    private LayerMask m_interactableLayer;

    private void OnValidate()
    {
        m_playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        m_playerInput.OnInteractInputEvent += OnInteractInput;
    }

    private void OnDisable()
    {
        m_playerInput.OnInteractInputEvent -= OnInteractInput;
    }

    private void TryInteract()
    {
        Camera currentCamera = CameraManager.Instance.CurrentCamera;
        Ray ray = currentCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        
        Debug.DrawRay(ray.origin, ray.direction * m_interactionRadius, Color.red, 4f);
        if (Physics.SphereCast(ray, m_interactionWidth, out RaycastHit hit, m_interactionRadius, m_interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null && interactable.CanInteract())
            {
                interactable.Interact();
            }
        }
    }
    
    private void OnInteractInput()
    {
        TryInteract();
    }
}
