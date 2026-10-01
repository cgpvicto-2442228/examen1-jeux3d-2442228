using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Objet représentant une boule contrôlée par le joueur.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class Boule : MonoBehaviour
{
    [SerializeField, Tooltip("La cible pour le suvi de la caméra")]
    private Transform cibleCamera;

    [SerializeField, Tooltip("Force de déplacement de la boule.")]
    private float forceDeplacement;

    [SerializeField]
    private PlayerInput controles;

    private float charge = 15f;

    private int nombreDeCharge = 0;

    // Force appliquée à la boule pour le déplacement à chaque frame.
    private Vector3 forceAppliquee;

    // Référence au Rigidbody de la boule pour appliquer la physique.
    private Rigidbody rigidbody;

    /// <summary>
    /// Obtient la vélocité actuelle de la boule.
    /// </summary>
    public Vector3 Velocite => rigidbody.linearVelocity;

    private void Start()
    {
        controles.actions.FindAction("Commencer").performed += CommencerJeu;
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Fait commencer le jeu
    /// </summary>
    /// <param name="contexte"></param>
    private void CommencerJeu(InputAction.CallbackContext contexte)
    {
        if (nombreDeCharge > 0)
        {
            controles.actions.FindAction("Charge").performed += CommencerCharge;
        }
        controles.actions.FindAction("Diriger").performed += CommencerDirection;
        controles.actions.FindAction("Diriger").canceled += ArreterDirection;
        rigidbody.useGravity = true;
    }

    private void OnDestroy()
    {
        if (ControleurJeu.Instance == null)
            return;

        PlayerInput controles = ControleurJeu.Instance.Controles;

        if (controles == null) 
            return;

        controles.actions.FindAction("Charge").performed -= CommencerCharge;
        controles.actions.FindAction("Diriger").performed -= CommencerDirection;
        controles.actions.FindAction("Diriger").canceled -= ArreterDirection;
        controles.actions.FindAction("Commencer").performed -= CommencerJeu;
    }

    private void Update()
    {
        if (cibleCamera != null)
        {
            cibleCamera.position = rigidbody.position;
        }
    }

    private void FixedUpdate()
    {
        Diriger();
    }

    private void CommencerDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee += contexte.ReadValue<float>() * forceDeplacement * Vector3.right;
    }

    private void ArreterDirection(InputAction.CallbackContext contexte)
    {
        forceAppliquee = Vector3.zero;
    }

    private void Diriger()
    {
        if(!Mathf.Approximately(forceAppliquee.sqrMagnitude, 0.0f))
        {
            rigidbody.AddForce(forceAppliquee, ForceMode.Force);
        }
    }

    private void CommencerCharge(InputAction.CallbackContext contexte)
    {
        Coroutine maCoroutine = StartCoroutine(MethodeCoroutine());
        StopCoroutine(maCoroutine);
    }

    private IEnumerator MethodeCoroutine()
    {
        rigidbody.AddForce(Velocite.normalized * charge, ForceMode.Acceleration);
        yield return new WaitForSeconds(1.0f);
        charge = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Acceleration"))
        {
            if (nombreDeCharge < 3)
            {
                nombreDeCharge++;
            }
        }
    }
}
