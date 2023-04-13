using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private Joystick _joystick;

    public void Construct(Joystick joystick)
    {
        _joystick = joystick;
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if(_joystick.Direction.sqrMagnitude != 0)
        {
            gameObject.SetActive(false);
        }   
    }
}