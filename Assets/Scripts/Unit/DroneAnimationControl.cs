using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//require some things the bot control needs
[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class DroneAnimationControl : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rbody;

    private Transform leftFoot;
    private Transform rightFoot;

    public int groundContacts = 0;

    private float filteredForwardInput = 0f;
    private float filteredTurnInput = 0f;

    public float forwardInputFilter = 5f;
    public float turnInputFilter = 5f;

    private float forwardSpeedLimit = 1f;

    public bool IsGrounded
    {
        get { return groundContacts > 0; }
    }

    public float lifetime;
    public GameObject explodeParticle;
    public float explodeRange;
    public Unit origin;
    public int maxDamage;
    public UserControl userControl;
    public bool hasUserControl;

    public GameObject[] toHide;

    private float startTime;
    private bool exploded;
    private Vector3 offset, last;

    void Awake()
    {
        anim = GetComponent<Animator>();

        if (anim == null)
            Debug.Log("Animator could not be found");

        rbody = GetComponent<Rigidbody>();

        if (rbody == null)
            Debug.Log("Rigid body could not be found");

    }


    // Use this for initialization
    void Start()
    {
        //example of how to get access to certain limbs
        leftFoot = this.transform.Find("mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");
        rightFoot = this.transform.Find("mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");

        if (leftFoot == null || rightFoot == null)
            Debug.Log("One of the feet could not be found");

        if(userControl != null)
            userControl.mapControl = false;
        startTime = Time.timeSinceLevelLoad;
        offset = new Vector3(0, Camera.main.gameObject.transform.position.y - transform.position.y, Camera.main.gameObject.transform.position.z - transform.position.z);
    }

    //Update whenever physics updates with FixedUpdate()
    //Updating the animator here should coincide with "Animate Physics"
    //setting in Animator component under the Inspector
    void FixedUpdate()
    {
        if (!hasUserControl)
            return;

        //GetAxisRaw() so we can do filtering here instead of the InputManager
        float h = Input.GetAxisRaw("Horizontal");// setup h variable as our horizontal input axis
        float v = Input.GetAxisRaw("Vertical");	// setup v variables as our vertical input axis


        //enforce circular joystick mapping which should coincide with circular blendtree positions
        Vector2 vec = Vector2.ClampMagnitude(new Vector2(h, v), 1.0f);

        h = vec.x;
        v = vec.y;


        //BEGIN ANALOG ON KEYBOARD DEMO CODE
        if (Input.GetKey(KeyCode.Q))
            h = -0.5f;
        else if (Input.GetKey(KeyCode.E))
            h = 0.5f;

        if (Input.GetKeyUp(KeyCode.Alpha1))
            forwardSpeedLimit = 0.1f;
        else if (Input.GetKeyUp(KeyCode.Alpha2))
            forwardSpeedLimit = 0.2f;
        else if (Input.GetKeyUp(KeyCode.Alpha3))
            forwardSpeedLimit = 0.3f;
        else if (Input.GetKeyUp(KeyCode.Alpha4))
            forwardSpeedLimit = 0.4f;
        else if (Input.GetKeyUp(KeyCode.Alpha5))
            forwardSpeedLimit = 0.5f;
        else if (Input.GetKeyUp(KeyCode.Alpha6))
            forwardSpeedLimit = 0.6f;
        else if (Input.GetKeyUp(KeyCode.Alpha7))
            forwardSpeedLimit = 0.7f;
        else if (Input.GetKeyUp(KeyCode.Alpha8))
            forwardSpeedLimit = 0.8f;
        else if (Input.GetKeyUp(KeyCode.Alpha9))
            forwardSpeedLimit = 0.9f;
        else if (Input.GetKeyUp(KeyCode.Alpha0))
            forwardSpeedLimit = 1.0f;
        //END ANALOG ON KEYBOARD DEMO CODE  


        //do some filtering of our input as well as clamp to a speed limit
        filteredForwardInput = Mathf.Clamp(Mathf.Lerp(filteredForwardInput, v,
                Time.deltaTime * forwardInputFilter), -forwardSpeedLimit, forwardSpeedLimit);

        filteredTurnInput = Mathf.Lerp(filteredTurnInput, h,
            Time.deltaTime * turnInputFilter);


        //Debug.Log("Setting: " + filteredForwardInput + " " + filteredTurnInput);
        //finally pass the processed input values to the animator
        anim.SetFloat("turn", filteredTurnInput);	// set our animator's float parameter 'Speed' equal to the vertical input axis				
        anim.SetFloat("forward", filteredForwardInput); // set our animator's float parameter 'Direction' equal to the horizontal input axis		

        updateFall();


        if (Input.GetButtonDown("Fire1"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle Turn"))
            {
                anim.SetTrigger("ThrowNoMask");
            }
            else
            {
                anim.SetTrigger("ThrowThing");
            }
        }    //normally left-ctrl on keyboard

    }

    void updateFall()
    {
        bool isFalling = !IsGrounded;

        if (isFalling)
        {

            const float rayOriginOffset = 1f; //origin near bottom of collider, so need a fudge factor up away from there
            const float rayDepth = 1f; //how far down will we look for ground?
            const float totalRayLen = rayOriginOffset + rayDepth;

            Ray ray = new Ray(this.transform.position + Vector3.up * rayOriginOffset, Vector3.down);

            //visualize ray in the editor
            //I'm using DrawLine because Debug.DrawRay() doesn't allow setting ray length past a certain size
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * totalRayLen, Color.green);

            RaycastHit hit;


            //Cast ray and look for ground. If ground is close, then transition out of falling animation
            if (Physics.Raycast(ray, out hit, totalRayLen))
            {
                if (hit.collider.gameObject.CompareTag("ground"))
                {
                    isFalling = false; //turning falling back off because we are close to the ground

                    //draw an X that denotes where ray hit
                    const float ZBufFix = 0.01f;
                    const float edgeSize = 0.2f;
                    Color col = Color.red;

                    Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.forward * edgeSize, col);
                    Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.left * edgeSize, col);
                    Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.right * edgeSize, col);
                    Debug.DrawRay(hit.point + Vector3.up * ZBufFix, Vector3.back * edgeSize, col);
                }
            }
        }


        //anim.SetBool("IsFalling", isFalling);
    }


    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.gameObject.tag == "ground")
        {
            ++groundContacts;

            //Debug.Log("Player hit the ground at: " + collision.impulse.magnitude);

            if (collision.impulse.magnitude > 100f)
            {
                // landing
                //EventManager.TriggerEvent<PlayerLandsEvent, Vector3>(collision.contacts[0].point);
            }
        }

        TileManager tm = collision.gameObject.GetComponentInParent<TileManager>();
        if (tm != null && !tm.Destroyed)
            explode();
    }

    //This is a physics callback
    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.gameObject.tag == "ground")
            --groundContacts;
    }



    void OnAnimatorMove()
    {
        if (IsGrounded)
        {
            //use root motion as is if on the ground		
            this.transform.position = anim.rootPosition;

        }
        else
        {
            //Simple trick to keep model from climbing other rigidbodies that aren't the ground
            this.transform.position = new Vector3(anim.rootPosition.x, this.transform.position.y, anim.rootPosition.z);
        }

        //use rotational root motion as is
        this.transform.rotation = anim.rootRotation;

    }

    void OnTriggerEnter(Collider col)
    {
        Unit hitUnit = col.gameObject.GetComponent<Unit>();
        if (hitUnit != null && col.gameObject != origin.gameObject && !hitUnit.IsDead)
        {
            explode();
        }
    }

    private void explode()
    {
        Instantiate(explodeParticle, transform.position, transform.rotation);
        Collider[] allColliders = Physics.OverlapSphere(transform.position, explodeRange * MapGenerator.step);
        foreach (Collider c in allColliders)
        {
            int damage = (int)(maxDamage * (1 - Vector3.Distance(c.gameObject.transform.position, this.gameObject.transform.position) / (2 * explodeRange * MapGenerator.step)));
            Unit t = c.gameObject.GetComponent<Unit>();
            if (t != null && t.team != origin.team && c.gameObject != origin.gameObject)
            {
                t.takeDamage(damage);
            }

            TileManager tm = c.gameObject.GetComponent<TileManager>();
            if (tm != null && !tm.Destroyed)
                tm.hit(damage, gameObject);
        }
        exploded = true;
        gameObject.GetComponent<LineRenderer>().enabled = false;
        foreach (GameObject go in toHide)
            go.SetActive(false);
        anim.enabled = false;
        GetComponent<Rigidbody>().detectCollisions = false;

        last = Camera.main.gameObject.transform.position;

        StartCoroutine(returnControl());
    }

    public void DoRenderer()
    {
        float radius = explodeRange * MapGenerator.step;
        int numSegments = 128;
        LineRenderer lineRenderer = gameObject.GetComponent<LineRenderer>();
        Color c1 = new Color(1, 0f, 0f, 1);
        Color c2 = new Color(1, .8f, 0, 1);

        Gradient gradient = new Gradient();
        gradient.colorKeys = new GradientColorKey[] {
            new GradientColorKey (c1, 0),
            new GradientColorKey (c2, .5f),
            new GradientColorKey (c1, 1)
        };

        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.colorGradient = gradient;
        lineRenderer.startWidth = .2f;
        lineRenderer.endWidth = .2f;
        lineRenderer.positionCount = numSegments + 1;
        lineRenderer.useWorldSpace = false;

        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = Mathf.PI / 2;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, 0, z);
            lineRenderer.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    private void LateUpdate()
    {
        if (!hasUserControl)
            return;

        if (!exploded)
            Camera.main.gameObject.transform.position = transform.position + offset;
        else
            Camera.main.gameObject.transform.position = last;
    }

    // Update is called once per frame
    void Update()
    {
        if (!exploded)
        {
            DoRenderer();
            if (Time.timeSinceLevelLoad - startTime > lifetime)
            {
                explode();
            }
        }
    }

    IEnumerator returnControl()
    {
        float time = 1f;
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
        if(userControl != null)
            userControl.returnMapControl();
    }
}
