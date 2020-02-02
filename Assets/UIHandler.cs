using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UltimateCameraController.Cameras.Controllers;

public class UIHandler : MonoBehaviour
{
    public GameObject planetGeneratorObject;
    private PlanetGenerator planetGenerator;
    public InputField numberOfPlanetsInput;
    public InputField spawnRadiusInput;
    public InputField numberOfStarsInput;
    public InputField starSpawnRadiusInput;
    public Slider gravitationalConstantSlider;
    public Slider trailDurationSlider;
    public Slider trailThicknessSlider;
    public Toggle collisionToggle;
    public GameObject planet;
    private Orbit orbit;
    private TrailRenderer trail;

    void Start() {
        planetGenerator = planetGeneratorObject.GetComponent<PlanetGenerator>();
        orbit = planet.GetComponent<Orbit>();
        trail = planet.GetComponent<TrailRenderer>();
        
        planetGenerator.maxNumberOfPlanets = int.Parse(numberOfPlanetsInput.text);
        planetGenerator.spawnRadius = int.Parse(spawnRadiusInput.text);
        planetGenerator.maxNumberOfStars = int.Parse(numberOfStarsInput.text);
        planetGenerator.sunSpawnRadius = int.Parse(starSpawnRadiusInput.text);
        orbit.gravityScale = gravitationalConstantSlider.value;
        orbit.enableCollision = collisionToggle.isOn;
        trail.time = trailDurationSlider.value;
        trail.startWidth = trailThicknessSlider.value;
        trail.endWidth = trailThicknessSlider.value;
        planetGenerator.GenerateStars();
        planetGenerator.GeneratePlanets();
    }

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Debug.Log("Raycast!");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Camera.main.transform.position, 20f * ray.direction, Color.red, 2, false);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)) {
                if(hit.collider != null) {
                    Camera.main.GetComponent<CameraController>().targetObject = hit.collider.gameObject.transform;
                    Debug.Log($"Raycast collided with: {hit.collider.name}");
                }
            }
        }
    }

    public void RestartButton_OnClick() {
        planetGenerator.DeletePlanets();
        planetGenerator.DeleteStars();
        planetGenerator.maxNumberOfPlanets = int.Parse(numberOfPlanetsInput.text);
        planetGenerator.spawnRadius = int.Parse(spawnRadiusInput.text);
        planetGenerator.maxNumberOfStars = int.Parse(numberOfStarsInput.text);
        planetGenerator.sunSpawnRadius = int.Parse(starSpawnRadiusInput.text);
        planetGenerator.GenerateStars();
        planetGenerator.GeneratePlanets();
    }

    public void GravitationalConstantSlider_OnChange(Slider slider) {
        Debug.Log($"Gravitational constant: {slider.value}");
        orbit.gravityScale = slider.value;
        planetGenerator.SetGravitationalConstant(orbit.gravityScale);
    }

    public void TrailDurationSlider_OnChange(Slider slider) {
        Debug.Log($"Trail duration: {slider.value}");
        trail.time = slider.value;
        planetGenerator.SetTrailDuration(slider.value);
    }

    public void TrailThicknessSlider_OnChange(Slider slider) {
        Debug.Log($"Trail thickness {slider.value}");
        trail.startWidth = slider.value;
        trail.endWidth = 0;
        planetGenerator.SetTrailThickness(slider.value);
    }

    public void CollisionToggle_OnChange(Toggle toggle) {
        Debug.Log($"Toggle collision: {toggle.isOn}");
        planet.GetComponent<Orbit>().enableCollision = toggle.isOn;
        planetGenerator.SetCollision(toggle.isOn);
    }
}
