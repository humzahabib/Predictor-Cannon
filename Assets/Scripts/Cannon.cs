using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Cannon : MonoBehaviour
{
    //Targets pending to shoot, whenever a target spawns it gets updated in this list.
    //You must have a reference to target's velocity
    [SerializeField]
    public List<Target> targets = new List<Target>();


    //Bullet of the tank
    [SerializeField]
    Rigidbody cannonShell;


    //Initaial Velocity for bullet, leave it null 
    //because we're gonna set initial velocity
    //according to trajectory range
    float cannonShellVelocity;


    
    [Tooltip("The turret of the cannon"),SerializeField]
    Transform cannonHead;

    //Gravity is increased in trajectory calculation
    //to increase bullet's velocity
    //You should apply gravity manually on bullet
    float g = 30;

    [SerializeField]
    bool shouldAngleBeSteep = false;


    //The point where bullet will be instantiated
    [SerializeField]
    Transform spawnPoint;

    [Range(20,80)]
    [Tooltip("Angle of the arc, Greater the angle, lesser the accuracy"), SerializeField]
    float angle;

    // Use this for initialization
    void Start()
    {
        //Registery for UI events, I've removed UI so it is of no use.
        HUDMananger hudManager = FindObjectOfType<HUDMananger>();
        if (hudManager != null)
        {
            hudManager.HigherAngleToggleEvent.AddListener(OnHigherAngleToggleEvent);
            hudManager.bulletVelocityChangedEvent.AddListener(AngleChangedEvnetHander);
        }
    }



    bool canFire = true;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            canFire = true;

        if (targets.Count > 0)
        {
            
            TrajectoryInfo? preTrajectory = PredictTrajectory(targets[targets.Count - 1]);

            //The velocity of the bullet is predicted for the trajectory
            cannonShellVelocity = preTrajectory.Value.speed;
            if (canFire)
            {
                //rotating the tank
                transform.forward = (preTrajectory.Value.pointOfImpact - cannonHead.transform.position).normalized;
                //Measuring if the trajectory is accurate
                float accuracyMeasure = (targets[targets.Count - 1].transform.position - preTrajectory.Value.pointOfImpact).magnitude;
                if (canFire)
                {
                    //Setting turret,s rotation in accordance with angle,
                    //Useful if you decide to have variable angle
                    cannonHead.localEulerAngles = new Vector3(360f - preTrajectory.Value.angle, 0, 0);
                    ShootTrajectory();
                    canFire = false;
                }
            }
        }
    }



    /// <summary>
    /// Calculates Velocity for the projectile
    /// </summary>
    /// <param name="coordinates">Position of target</param>
    /// <param name="angle">Angle the trajectory must follow</param>
    /// <param name="distance">Range of the trajectory,
    /// Or distance of target</param>
    /// <returns></returns>
    TrajectoryInfo CalculateVelocity(Vector3 coordinates, float angle, float distance)
    {
        TrajectoryInfo info = new TrajectoryInfo(0, 0, 0, 0, Vector3.zero);
        info.angle = angle;
        info.speed = Mathf.Sqrt(g * distance);
        info.range = distance;
        info.time = ((Mathf.Sqrt(2) * info.speed) / g);
        Vector3 forwardVector = new Vector3(transform.forward.x, 0, transform.forward.z);
        Vector3 targetVector = coordinates - transform.position;
        Vector3 pointOfImpact = transform.forward + (targetVector);
        targetVector.y = 0;
        info.pointOfImpact = pointOfImpact;


        return info;
    }

    void AngleChangedEvnetHander(float value)
    {
        angle = value;
    }

    void OnHigherAngleToggleEvent()
    {
        shouldAngleBeSteep = !shouldAngleBeSteep;
    }

    void ShootTrajectory()
    {
        if (canFire)
        {
            Rigidbody fired = Rigidbody.Instantiate(cannonShell, cannonHead.position, Quaternion.identity);
            fired.velocity = cannonHead.forward * cannonShellVelocity;
            fired.transform.forward = fired.velocity;
        }
    }

    /// <summary>
    /// Predict the future trajectory with respect to target's velocity
    /// </summary>
    /// <param name="agent">Target you wanna shoot</param>
    /// <returns></returns>
    TrajectoryInfo? PredictTrajectory(Target agent)
    {

        //Calculating trajectory to the current location of target
        TrajectoryInfo? initialTrejectory = CalculateVelocity(cannonHead.InverseTransformDirection
            (targets[targets.Count-1].transform.position), angle,
            (agent.transform.position - cannonHead.position).magnitude);


        //Null check because we'll return null if there are any complex numbers in calculation
        //This will happen when we can,t hit target
        Debug.DrawLine(transform.position, initialTrejectory.Value.pointOfImpact, Color.cyan);
        if (initialTrejectory != null)
        {
            //Just defining a variable
            TrajectoryInfo? predictedTrajectory = new TrajectoryInfo(0, 0, 0, 0, Vector3.zero);
            for (int i = 0; i < 5; i++)
            {
                if (predictedTrajectory != null)
                {
                    //Here, w'll predict trajectory according to future location of target
                    float timeToReachTarget = initialTrejectory.Value.time;
                    Vector3 predictedLocation = agent.transform.position + (agent.Velocity * timeToReachTarget);
                    Debug.DrawLine(transform.position, predictedLocation, Color.red);
                    predictedTrajectory = CalculateVelocity(predictedLocation, angle, (predictedLocation - cannonHead.position).magnitude);
                    initialTrejectory = predictedTrajectory;
                }
            }


            if (predictedTrajectory != null)
                return predictedTrajectory;
        }
        if (initialTrejectory.HasValue)
            return initialTrejectory;
        
        return null;
        

    }



 
    /// <summary>
    /// Info of a trajectory
    /// </summary>
    public struct TrajectoryInfo
    {
        public float time, range, angle, speed ;
        public Vector3 pointOfImpact;

        /// <summary>
        /// Constructor of a Trajectory Info
        /// </summary>
        /// <param name="_time">Time taken by trajectory to reach target</param>
        /// <param name="_range">Range of distance of trajectory</param>
        /// <param name="_angle">Angle a which we'll shoot trajectory</param>
        /// <param name="_speed">Velocity for the projectile, but our method will predict this
        /// according to target's location and velocity</param>
        /// <param name="_pointOfImpact">The position where projectile will meet target</param>
        public TrajectoryInfo(float _time, float _range, float _angle, float _speed, Vector3 _pointOfImpact)
        {
            time = _time;
            range = _range;
            angle = _angle;
            speed = _speed;
            pointOfImpact = _pointOfImpact;
        }

    }


}
