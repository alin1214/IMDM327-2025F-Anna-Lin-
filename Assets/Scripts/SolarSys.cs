using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSys : MonoBehaviour
{
    GameObject[] body;
    BodyProperty[] b;
    TrailRenderer[] trails;
    public Material[] material;
    private int numberOfSphere = 200;
    private float timeflow = 500000f;
    float radius = 5f;
    private const float G = 5000f;

    struct BodyProperty
    {
        public string name;
        public string type;
        public double mass;
        public float radius_meters;
        public double speed_ms;           // mean orbital speed (m/s)
        public Vector3 initVelocity;      // initial velocity vector (Unity units/s)
        public float omega;              // angular speed (rad/s) = v / r
        public float angle;              // current angle (rad)
        public float size;
        public Color color;
    }

    void Start()
    {
        // scaling down in unity 
        float unityUnit = 5e9f;
        float positionScale = 1f / unityUnit;

        string[] names = { "Sun", "Mercury", "Venus", "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", "Pluto" };
        string[] types = { "Star", "Planet", "Planet", "Planet", "Planet", "Planet", "Planet", "Planet", "Planet", "Dwarf" };

        double[] massesKg = {
            1.98847e30, 3.3011e23, 4.8675e24, 5.9722e24, 6.4171e23,
            1.89813e27, 5.6834e26, 8.6813e25, 1.02413e26, 1.303e22
        };

        float[] radii = {  // orbital radii from sun
            0f,
            5.7894375961e10f,
            1.08159260516e11f,
            1.495978707e11f,
            2.27987154947e11f,
            7.78357721252e11f,
            1.426714892866e12f,
            2.870932736604e12f,
            4.498258374078e12f,
            5.906423130977e12f
        };

        double[] orbital_speed = { 0, 47360, 35020, 29780, 24070, 13070, 9680, 6800, 5430, 4740 };

        // sizing the diff planets 
        float sunSize = 3f;
        float mercurySize = 0.5f;
        float venusSize = 0.7f;
        float earthSize = 0.8f;
        float marsSize = 0.6f;
        float jupiterSize = 1.6f;
        float saturnSize = 1.4f;
        float uranusSize = 1.2f;
        float neptuneSize = 1.2f;
        float plutoSize = 0.1f;

        // size array
        float[] sizes = {
            sunSize,
            mercurySize, venusSize, earthSize, marsSize,
            jupiterSize, saturnSize, uranusSize, neptuneSize, plutoSize
        };

        // setting all planets white except sun 
        Color[] colors = {
            Color.yellow,
            Color.white,
            Color.white,
            Color.white,
            Color.white,
            Color.white,
            Color.white,
            Color.white,
            Color.white,
            Color.white
        };

        // allocating arrays for each property to initialize 
        numberOfSphere = names.Length;
        body = new GameObject[numberOfSphere];
        b = new BodyProperty[numberOfSphere];
        trails = new TrailRenderer[numberOfSphere];

        // angle spacing so they're in a circle 
        float initAngle = 0.35f;

        // initalizing each planet 

        for (int i = 0; i < numberOfSphere; i++)
        {
            b[i].name = names[i];
            b[i].type = types[i];
            b[i].mass = massesKg[i];
            b[i].radius_meters = radii[i];
            b[i].speed_ms = orbital_speed[i];
            b[i].omega = (radii[i] > 0f) ? (float)(orbital_speed[i] / radii[i]) : 0f; // omega = velocity / radii
            b[i].angle = i * initAngle;
            b[i].size = sizes[i];
            b[i].color = colors[i];


            // creating sphere 
            body[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            body[i].name = b[i].name;
            body[i].transform.localScale = Vector3.one * b[i].size;

            // assign meterial and color to each 
            var rend = body[i].GetComponent<Renderer>();
            if (rend != null)
            {
                if (material != null && i < material.Length && material[i] != null)
                    rend.material = material[i];
                rend.material.color = b[i].color; // color from array 
            }

            // initial position from angle & scaled radius 
            if (b[i].radius_meters > 0f)
            {
                float scaledDistance = Mathf.Sqrt(b[i].radius_meters / 1e8f);
                float x = scaledDistance * Mathf.Cos(b[i].angle);
                float z = scaledDistance * Mathf.Sin(b[i].angle);
                body[i].transform.position = new Vector3(x, 0f, z);

                // tangent initial velocity 
                float vU = (float)(b[i].speed_ms * positionScale);
                Vector3 tangent = new Vector3(-Mathf.Sin(b[i].angle), 0f, Mathf.Cos(b[i].angle));
                b[i].initVelocity = vU * tangent;
            }

            else
            {
                // for sun
                body[i].transform.position = Vector3.zero;
                b[i].initVelocity = Vector3.zero;
            }

            // trail colors 
            Color[] trailColors = {
                Color.yellow,   
                Color.gray,    
                new Color(1f, 0.5f, 0f), 
                Color.blue,     
                Color.red,    
                new Color(1f, 0.8f, 0.6f), 
                new Color(1f, 1f, 0.5f),   
                new Color(0.5f, 1f, 1f),   
                new Color(0.3f, 0.5f, 1f),
                Color.white   
            };

            // trail
            TrailRenderer trailRenderer = body[i].AddComponent<TrailRenderer>();

            // configure properties
            trailRenderer.time = 100.0f;        // duration of the trail
            trailRenderer.startWidth = 0.5f;
            trailRenderer.endWidth = 0.1f;

            // material of trail
            trailRenderer.material = new Material(Shader.Find("Sprites/Default"));

            // set trail color over time
            Color trailColor = trailColors[i];
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                new GradientColorKey(trailColor, 0.0f),
                new GradientColorKey(trailColor, 1.0f)
                },
                new GradientAlphaKey[] {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(0.0f, 1.0f)
                }
            );
            trailRenderer.colorGradient = gradient;
        }
        // sun is at origin 
        if (numberOfSphere > 0) body[0].transform.position = Vector3.zero;
    }

    void Update()
    {
        // speed up simulation 
        float deltaT = Time.deltaTime * timeflow;
        if (deltaT <= 0f || body == null) return; // checking if deltaT is valid and body exists 

        float metersPerUnityUnit = 2e10f;
        float positionScale = 1f / metersPerUnityUnit;

        for (int i = 1; i < numberOfSphere; i++) // all planets except the sun
        {
            b[i].angle += b[i].omega * deltaT; // increase planet's angle by angular speed times timestep
            float scaledDistance = Mathf.Sqrt(b[i].radius_meters / 1e8f);
            float x = scaledDistance * Mathf.Cos(b[i].angle);
            float z = scaledDistance * Mathf.Sin(b[i].angle);
            body[i].transform.position = new Vector3(x, 0f, z);
        }

        // make sun stable 
        body[0].transform.position = Vector3.zero;

    }
}
