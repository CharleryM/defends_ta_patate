using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    
    private Camera mainCamera;
    
    void Start()
    {
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        // Make the health bar face the camera
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }
    
    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        
        fill.color = gradient.Evaluate(1f);
    }
    
    public void SetHealth(float health)
    {
        slider.value = health;
        
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}