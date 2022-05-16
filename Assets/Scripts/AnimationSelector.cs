using System.Collections.Generic;

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationSelector : MonoBehaviour
{
    [SerializeField] List<RuntimeAnimatorController> controllers;

    private Animator animator;
    public Animator Animator { get { return animator; } }

    void Awake() => ResolveDependencies();

    private void ResolveDependencies() => animator = GetComponent<Animator>() as Animator;
    
    public void SetController(int index) => animator.runtimeAnimatorController = controllers[index];
}