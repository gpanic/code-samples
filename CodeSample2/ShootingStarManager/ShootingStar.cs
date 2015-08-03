using UnityEngine;
using System.Collections;

public class ShootingStar : MonoBehaviour
{
    class AnimationParameters
    {
        public Vector3 scale;
        public float starTransparency;
        public float trailTransparency;
        public float flareBrightness;
    };

    public Vector3 startPos;
    public Vector3 endPos;
    public float growTime = 1.0f; // the shooting star grows and shrinks during travel
    public float shrinkTime = 1.5f;
    public Vector3 minScale = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 maxScale = new Vector3(2, 2, 2);
    public float minFlareBrightness = 0.0f;
    public float maxFlareBrightness = 1.0f;

    private TrailRenderer tr;
    private bool travelling = false;
    private bool growing = true;
    private float timer = 0.0f;
    private LensFlare flare;
    private Material starMat;
    private Material trailMat;
    private float origTrailTime;
    private AnimationParameters maxValues;
    private AnimationParameters minValues;

    private void Start()
    {
        flare = GetComponent<LensFlare>();
        startPos = transform.position;
        starMat = GetComponent<Renderer>().material;
        trailMat = GetComponent<TrailRenderer>().material;
        starMat.SetFloat("_Transparency", 0.0f);
        trailMat.SetFloat("_Transparency", 0.0f);
        flare.brightness = minFlareBrightness;
        tr = gameObject.GetComponent<TrailRenderer>();
        origTrailTime = tr.time;
        maxValues = new AnimationParameters() { scale = maxScale, starTransparency = 1.0f, trailTransparency = 1.0f, flareBrightness = maxFlareBrightness };
        minValues = new AnimationParameters() { scale = minScale, starTransparency = 0.0f, trailTransparency = 0.0f, flareBrightness = minFlareBrightness };
    }

    private void Update()
    {
        if (travelling)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, timer / (growTime + shrinkTime));

            // setting the time parameter to 0 for a frame effectively restarts the trail rendered, preventing
            // the problematic trail between the end of a shooting star and the start of a reused one
            // here we restore it to its oroginal value
            if (tr.time != origTrailTime && timer != 0)
            {
                tr.time = origTrailTime;
            }

          
            if (growing)
            {
                UpdateAnimations(timer / (growTime), minValues, maxValues);
            }
            else
            {
                UpdateAnimations((timer - growTime) / shrinkTime, maxValues, minValues);
            }


            if (timer >= growTime)
            {
                growing = false;
            }

            if (timer >= growTime + shrinkTime)
            {
                timer = 0.0f;
                travelling = false;
                growing = true;
                gameObject.SetActive(false);

                // reset the trail renderer
                tr.time = 0;
            }

        }
    }

    private void UpdateAnimations(float t, AnimationParameters fromParameters, AnimationParameters toParameters)
    {
        transform.localScale = Vector3.Lerp(fromParameters.scale, toParameters.scale, t);
        starMat.SetFloat("_Transparency", Mathf.Lerp(fromParameters.starTransparency, toParameters.starTransparency, t));
        trailMat.SetFloat("_Transparency", Mathf.Lerp(fromParameters.trailTransparency, toParameters.trailTransparency, t));
        flare.brightness = Mathf.Lerp(fromParameters.flareBrightness, toParameters.flareBrightness, t);
    }

    public void StartEffect()
    {
        startPos = transform.position;
        travelling = true;
    }
}
