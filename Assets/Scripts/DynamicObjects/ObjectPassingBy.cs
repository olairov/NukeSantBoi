using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPassingBy : MonoBehaviour
{
    private MapGenerator mapScript;

    private Transform cameraRiserTransform, playerTransform;

    public static float speedMultiplier, realSpeedMultiplier;
    public float passingSpeed, realPassingSpeed, myYpos;
    private float appearingDistance = 8, lastCameraYpos, movementFixSpeed, originalSceneMovement;
    // movementFixMultiplier is used for assigning an initial speed to the moving object, so that it moves accordingly to the original velocity.
    public float OriginalMovement
    {
        set { originalSceneMovement = value; }
    }

    [SerializeField] private bool background, fakePassingSpeed;
    [SerializeField] public bool moveInFixedUpdate;
    public bool appearingObject, imSkystraperPart;

    private void Start()
    {
        if (background) appearingDistance = 30;

        if (!appearingObject) transform.position = new Vector3(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x + appearingDistance, transform.position.y, transform.position.z);

        imSkystraperPart = name.Contains("Skystraper") && gameObject.layer != 3;
        cameraRiserTransform = Camera.main.transform.parent;
        if (fakePassingSpeed)
        {
            playerTransform = GameObject.Find("Player").transform;
            mapScript = GameObject.Find("__________________Map___________________").GetComponent<MapGenerator>();
            lastCameraYpos = cameraRiserTransform.position.y;
        }

        //passingSpeed = 0;
        //realPassingSpeed = 0;
    }

    private void Update()
    {
        if (!moveInFixedUpdate) UpdateXpos();
        UpdateYpos();
    }

    private void FixedUpdate()
    {
        // Make objects that also move using their own methods are unaffected by the change of speedMultiplier when in loops.
        MovementFix();

        if (moveInFixedUpdate) UpdateXpos();
    }

    void UpdateXpos()
    {
        transform.position += new Vector3(-passingSpeed * Time.deltaTime * speedMultiplier * MapGenerator.playerDistanceToStandardPos, 0, 0);
        transform.position += new Vector3(-realPassingSpeed * Time.deltaTime * realSpeedMultiplier * MapGenerator.playerDistanceToStandardPos, 0, 0);

        // Multiplied by 3 so that the bombs have time to hit passed buildings and some other reasons.
        if (transform.position.x < Camera.main.ScreenToWorldPoint(Vector3.zero).x - appearingDistance * 3) Destroy(gameObject);
    }

    void MovementFix() // This is a semi-futile attempt of making dynamic objects look like they're moving independently from the player
    {
        if (!fakePassingSpeed || playerTransform == null) return;
        
        if (!PlayerController.dead)
        {
            float actualPlayerRotationIndex = Mathf.Cos((playerTransform.eulerAngles.z - 180) / 57.3f) + 0.25f;
            if (actualPlayerRotationIndex > 1) actualPlayerRotationIndex = 1;

            movementFixSpeed = originalSceneMovement - (actualPlayerRotationIndex * 5); // Multiply by 5 because it's buildings' default passing speed.
        }

        transform.position += new Vector3(movementFixSpeed * realSpeedMultiplier * Time.deltaTime, 0, 0);
    }

    void UpdateYpos()
    {
        myYpos = transform.position.y - (cameraRiserTransform.position.y - lastCameraYpos) * (!fakePassingSpeed ? passingSpeed : 6);
        // If fake passing speed is on, the object's passingSpeed is 0, but it really is passing, too, in the layer of the buildings plus camera's movement (that gives 6).
        transform.position = new Vector3(transform.position.x, myYpos, transform.position.z);

        lastCameraYpos = cameraRiserTransform.position.y;
    }
}
