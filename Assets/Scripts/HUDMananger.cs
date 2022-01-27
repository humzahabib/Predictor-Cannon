using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HUDMananger : MonoBehaviour
{


    
    public UnityEvent<float> bulletVelocityChangedEvent = new UnityEvent<float>();
    public UnityEvent<float> targetVelocityChangedEvent = new UnityEvent<float>();

    [SerializeField]
    List<CinemachineVirtualCamera> cameras;

    int cameraIndex;

    [SerializeField]
    Slider timeSlider, bulletVelocitySlider, targetVelocitySlider;

    [SerializeField]
    Toggle higherAngleToggle;


    
    

    public void SwitchCamera()
    {
        if (cameraIndex < cameras.Count - 1)
            cameraIndex++;
        else
            cameraIndex = 0;
        cameras[cameraIndex].MoveToTopOfPrioritySubqueue();
    }

    public void OnTimeSliderValueChangedEventHandler()
    {
        Time.timeScale = timeSlider.value;
    }

    public void OnBulletVelocityChanged()
    {
        bulletVelocityChangedEvent.Invoke(bulletVelocitySlider.value);
    }
    
    public void OnTargetVelocityChanged()
    {
        targetVelocityChangedEvent.Invoke(targetVelocitySlider.value);
    }

    public void OnResetParametersButtonPressed()
    {
        timeSlider.value = 1;
        targetVelocitySlider.value = 5;
        bulletVelocitySlider.value = 20;
    }

    public UnityEvent HigherAngleToggleEvent = new UnityEvent();

    public void OnHigherAngleToggleEventHandler()
    {
        HigherAngleToggleEvent.Invoke();
    }

}
