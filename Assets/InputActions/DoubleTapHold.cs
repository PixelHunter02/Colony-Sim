using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class DoubleTapHold : IInputInteraction
{
    public float duration = 5f;
    public float tapDuration = 0.2f;
    public float minHoldDuration = 0.5f;

    static DoubleTapHold()
    {
        InputSystem.RegisterInteraction<DoubleTapHold>();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize()
    {
    }

    public void Process(ref InputInteractionContext context)
    {
        if (context.timerHasExpired)
        {
            context.Canceled();
            return;
        }

        switch (context.phase)
        {
            case InputActionPhase.Waiting:
                if (context.ControlIsActuated(1))
                {
                    context.Performed();
                    context.SetTimeout(duration);
                }
                break;

            // case InputActionPhase.Started:
            //     if (context.action.ReadValue<float>() == 0)
            //         if(context.action.ReadValue<float>() == 1) 
            //             // context.Performed();
            //     break;
        }
    }

    private InputControl m_control;
    
    public void Reset()
    {
        throw new System.NotImplementedException();
    }
}
