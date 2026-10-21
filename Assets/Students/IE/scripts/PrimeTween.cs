using UnityEngine;
using PrimeTween;

public class SpriteTween : MonoBehaviour
{

    [SerializeField] private bool tweenScale = true;
    [SerializeField] private Vector3 targetScale = new Vector3(1.2f, 1.2f, 1f);

    [SerializeField] private bool tweenPosition = false;
    [SerializeField] private Vector3 positionOffset = new Vector3(0f, 1f, 0f);


    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float startDelay = 0f;
    [SerializeField] private Ease ease = Ease.OutBack;
    [SerializeField] private bool infiniteRepeats = false;
    [SerializeField] private int repetitions = 1;
    [SerializeField] private CycleMode cycleMode = CycleMode.Restart;

    private void Start()
    {
        int cycles = repetitions;
        if (infiniteRepeats) cycles = -1;
        if (tweenScale)
        {
            Tween.Scale(
                transform,
                transform.localScale,
                targetScale,
                duration,
                ease: ease,
                startDelay: startDelay,
                cycles: cycles,
                cycleMode: cycleMode
            );
        }

        if (tweenPosition)
        {
            Tween.LocalPosition(
                transform,
                transform.localPosition,
                transform.localPosition + positionOffset,
                duration,
                ease: ease,
                startDelay: startDelay,
                cycles: cycles,
                cycleMode: cycleMode
            );
        }
    }
}