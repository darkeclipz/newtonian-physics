using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{

    private int numberOfPlanets = 0;
    public int maxNumberOfPlanets = 64;
    public float spawnRadius = 8f;
    public float planetMinSize = 0.1f;
    public float planetMaxSize = 0.6f;
    public GameObject planet;
    public List<Material> materials;
    private List<GameObject> planets = new List<GameObject>();
    private int numberOfStars = 0;
    public int maxNumberOfStars = 30;
    public float sunSpawnRadius = 80f;
    public GameObject starObject;
    private List<GameObject> stars = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = Vector3.zero;
        planet.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GeneratePlanets() {
        while(numberOfPlanets < maxNumberOfPlanets) {
            if(planet != null) {
                SpawnPlanet();
            }
            numberOfPlanets++;
        }
    }

    public void GenerateStars() {
        while(numberOfStars < maxNumberOfStars) {
            SpawnStar();
            numberOfStars++;
        }

        // Set the camera to the first star.
        stars[0].transform.position = Vector3.zero;
        Camera.main.GetComponent<UltimateCameraController.Cameras.Controllers.CameraController>().targetObject = stars[0].transform;
    }

    private void SpawnStar()
    {
        Vector3 position = sunSpawnRadius * UnityEngine.Random.insideUnitSphere;
        GameObject star = Instantiate(starObject, position, Quaternion.identity);
        stars.Add(star);
        star.SetActive(true);
    }

    private void SpawnPlanet() {
        Vector3 position = spawnRadius * UnityEngine.Random.insideUnitSphere;
        GameObject newPlanet = Instantiate(planet, position, Quaternion.identity);
        float scale = UnityEngine.Random.Range(planetMinSize, planetMaxSize);
        if(materials != null) {
            int materialId = UnityEngine.Random.Range(0, materials.Count);
            newPlanet.GetComponent<MeshRenderer>().material = materials[materialId];
        }
        foreach(GameObject star in stars) {
            newPlanet.GetComponent<Orbit>().targets.Add(star);
        }
        newPlanet.transform.localScale = Vector3.one * scale;
        planets.Add(newPlanet);
        newPlanet.SetActive(true);
    }

    public void DeletePlanets() {
        foreach(var planet in planets) {
            if(planet == null) continue;
            Destroy(planet);
        }
        planets = new List<GameObject>();
        numberOfPlanets = 0;
    }

    public void DeleteStars() {
        foreach(var star in stars) {
            Destroy(star);
        }
        stars = new List<GameObject>();
        numberOfStars = 0;
    }

    public void SetGravitationalConstant(float g) {
        foreach(var p in planets) {
            if(p == null) continue;
            p.GetComponent<Orbit>().gravityScale = g;
        }
    }

    public void SetTrailDuration(float t) {
        foreach(var p in planets) {
            if(p == null) continue;
            p.GetComponent<TrailRenderer>().time = t;
        }
    }

    public void SetTrailThickness(float w) {
        foreach(var p in planets) {
            if(p == null) continue;
            TrailRenderer tr = p.GetComponent<TrailRenderer>();
            tr.startWidth = w;
            tr.endWidth = 0;
            tr.Clear();
        }
    }

    public void AddStarToTarget(GameObject star) {
        foreach(var p in planets) {
            if(p == null) continue;
            Orbit orbit = p.GetComponent<Orbit>();
            orbit.targets.Add(star);
        }
    }

    public void SetCollision(bool value) {
        foreach(var p in planets) {
            if(p == null) continue;
            p.GetComponent<Orbit>().enableCollision = value;
        }
    }
}
