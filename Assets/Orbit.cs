using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{

    public List<GameObject> targets;
    public GameObject sun;
    private Rigidbody rigidbody;
    public float gravityScale = 80f;
    public bool enableCollision = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Vector3 difference = sun.transform.position - this.transform.position;
        Vector3 normal = new Vector3(difference.x, difference.y, difference.z);
        // Pick a random normal vector.
        switch(Random.Range(0, 3)) {
            case 0: normal = new Vector3(-normal.x, normal.y, normal.z); break;
            case 1: normal = new Vector3(normal.x, -normal.y, normal.z); break;
            case 2: normal = new Vector3(normal.x, normal.y, -normal.z); break;
        }
        rigidbody.velocity = normal.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forceVector = Vector3.zero;

        foreach(GameObject target in targets) {
            Vector3 difference = target.transform.position - this.transform.position;
            float distanceSquared = difference.sqrMagnitude;
            Vector3 gravityDirection = difference.normalized;
            float gravity = (this.transform.localScale.x * target.transform.localScale.x * gravityScale) / distanceSquared;
            Vector3 gravityVector = gravityDirection * gravity;
            forceVector += gravityVector;
        }

        rigidbody.AddForce(forceVector, ForceMode.Acceleration);
    }

    private float CalculateSphereVolume(float radius) {
        return 4 / 3 * Mathf.PI * Mathf.Pow(radius, 3);
    }

    private float CalculateSphereRadius(float volume) {
        return Mathf.Pow((3f * volume) / (4f * Mathf.PI), 1f / 3f);
    }

    void OnCollisionEnter(Collision collision) {
        if(enableCollision) {
            GameObject smallerObject = this.gameObject;
            GameObject biggerObject = collision.gameObject;
            if(smallerObject.transform.localScale.x > biggerObject.transform.localScale.x) {
                GameObject temp = smallerObject;
                smallerObject = biggerObject;
                biggerObject = temp;
            }

            const int starLayer = 8;
            if(biggerObject.layer == starLayer) {
                return;
            }

            float radiusA = smallerObject.transform.localScale.x / 2f;
            float volumeA = CalculateSphereVolume(radiusA);
            float radiusB = biggerObject.transform.localScale.x / 2f;
            float volumeB = CalculateSphereVolume(radiusB);
            float totalVolume = volumeA + volumeB;
            float radius = CalculateSphereRadius(totalVolume);
            biggerObject.transform.localScale = 1.08f * 2f * radius * Vector3.one;

            float massA = smallerObject.transform.localScale.x;
            float massB = biggerObject.transform.localScale.x;
            float totalMass = massA + massB;
            float coefA = massA / totalMass;
            float coefB = massB / totalMass;

            Rigidbody rb = biggerObject.GetComponent<Rigidbody>();
            rb.velocity = smallerObject.GetComponent<Rigidbody>().velocity * coefA + rb.velocity * coefB;

            Destroy(smallerObject);
            //smallerObject.SetActive(false);

            Debug.Log($"Mass A: {volumeA} Mass B: {volumeB} Mass tot: {totalVolume}, Ra: {radiusA} Rb: {radiusB} Rt: {radius} A: {smallerObject.name} B: {biggerObject.name}");
        }
    }
}
