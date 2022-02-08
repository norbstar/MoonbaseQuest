using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatedScoreCanvasManager : ScoreCanvasManager
{
    private Animator animator;

    void Awake()
    {
        ResolveDependencies();
    }

    private void ResolveDependencies()
    {
        animator = GetComponent<Animator>() as Animator;
    }
}