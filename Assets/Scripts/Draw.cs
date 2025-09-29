using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Draw : MonoBehaviour
{
   GameObject[] body;
   BodyProperty[] b;
   TrailRenderer[] trails;
   public Material[] material;
   private int numberOfSphere = 200; // start small, add later
   private float timeflow = 0.1f;
   float radius = 10f;
   private const float G = 500f;


   struct BodyProperty // why struct?
   {                   // https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/choosing-between-class-and-struct
       public float mass;
       public Vector3 velocity;
       public Vector3 acceleration;
   }
   void Start()
   {
       body = new GameObject[numberOfSphere];
       b = new BodyProperty[numberOfSphere];
       trails = new TrailRenderer[numberOfSphere];


       // Loop generating the gameobject and assign initial conditions
       for (int i = 0; i < numberOfSphere; i++)
       {
           // Our gameobjects are created here:
           body[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere); // why sphere? try different options.
           // https://docs.unity3d.com/ScriptReference/GameObject.CreatePrimitive.html


           // initial position
           body[i].transform.position = new Vector3(radius * Mathf.Sin(timeflow * i), // changes the position of every sphere
                                                   radius * Mathf.Cos(timeflow * i),
                                                   180);
           // initial color
           var meshRenderer = body[i].GetComponent<Renderer>();
           meshRenderer.material.SetColor("_Color", new Color(i / 255f, (255 - i) / 255f, 255 / 255f));


           // trails
           trails[i] = body[i].AddComponent<TrailRenderer>();
           trails[i].time = 1.0f;                   
           trails[i].startWidth = 0.5f;
           trails[i].endWidth = 0.0f;
           trails[i].material = new Material(Shader.Find("Sprite/Default"));
           // Gradient gradient = new Gradient();
           // gradient.SetKeys(


           //     new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f), new GradientColorKey(Color.red, 1.0f) },


           //     new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }


           // );
           // trails[i].colorGradient = gradient;


           // adjust the mass
           b[i].mass = 1f;
        //    b[i].velocity = Vector3.zero;
           b[i].acceleration = Vector3.zero;


       }
   }


   void Update()
   {
       timeflow += Time.deltaTime;
       // How to make them move over the time
       // Loop for N-body gravity
       // How should we design the loop?


       // reset accelerations
       for (int i = 0; i < numberOfSphere; i++) {
           b[i].acceleration = new Vector3 (0f, 0f, 0f);
       }


       // how to make them move over time
       for (int i = 0; i < numberOfSphere; i++){
           // calculate accelerations
           for (int j = i + 1; j < numberOfSphere; j++){


               // F = G * m1 * m2 / r^2
               float distance = Vector3.Distance(body[i].transform.position, body[j].transform.position);


               Vector3 direction = (body[j].transform.position - body[i].transform.position).normalized;
               Vector3 gravity = G * b[i].mass * b[j].mass / (distance * distance) * direction;


               b[i].acceleration += gravity / b[i].mass;
               b[j].acceleration -= gravity / b[j].mass;
               Debug.Log(b[i].acceleration);
           }
           // update positions


           b[i].velocity += b[i].acceleration * Time.deltaTime;
           body[i].transform.position += b[i].velocity * Time.deltaTime;
       }

   }
}


// internal class constant
// {
// }
