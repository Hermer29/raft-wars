using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private Joystick _joystick;

    public void Construct(Joystick joystick)
    {
        _joystick = joystick;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
}