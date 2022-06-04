using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.Chicken;

public class InteractiveNPC : MonoBehaviour
{
    [SerializeField] SO_Dialog dialog;

    public Outline outline;

    [Space, SerializeField, Tooltip("Time in seconds")]
    private float coolDown = 60f;
    [SerializeField, NaughtyAttributes.ReadOnly]
    private bool inCoolDown = false;

    [Space]
    public UnityEvent correctAnswer;
    public UnityEvent wrongAnswer;
    
    
    private float outLineStrenth;
    private void Start()
    {
        outLineStrenth = outline.OutlineWidth;
        outline.OutlineWidth = 0f;
    }
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<Chicken>();
        if (player)
        {
            outline.OutlineWidth = outLineStrenth;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        var player = other.gameObject.GetComponent<Chicken>();
        if (player)
        {
            outline.OutlineWidth = 0f;
            DialogManager.Instance.CloseDialog();
        }
    }
    public void ActivateDialog()
    {
        if (inCoolDown) return;
        StartCoroutine(CoolDownCount());
        DialogManager.Answered += GetAnswer;
        DialogManager.Instance.StartDialog(dialog);
    }

    private void GetAnswer(bool assertion)
    {
        if(assertion)
            correctAnswer.Invoke();
        else
            wrongAnswer.Invoke();

        DialogManager.Answered -= GetAnswer;
    }

    private IEnumerator CoolDownCount()
    {
        inCoolDown = true;
        yield return new WaitForSeconds(coolDown);
        inCoolDown = false;
    }
}
