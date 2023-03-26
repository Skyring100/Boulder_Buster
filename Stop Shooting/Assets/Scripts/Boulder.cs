using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    private Vector3 playerPos;
    public float speed;
    private float originalSpeed;
    //public float health;
    //public float maxHealth;
    [SerializeField] private int maxSpeed = 10;
    [SerializeField] private int minSpeed = 10;
    private Vector3 target;
    [SerializeField] private Vector3 direction;
    public List<Vector3> directionChangeLog;
    [SerializeField] private float maxSize = 14;
    [SerializeField] private float minSize = 2;
    [SerializeField] private float despawnOffset = 4;
    [SerializeField] private float explosionForce = 10;
    private SpawnManager manager;
    public bool isHouse = false;
    [SerializeField] private Material normalColor;
    [SerializeField] private Material homeColour;
    //[SerializeField] private Material flashColour;
    private Renderer render;
    [SerializeField] float homeTimeDelay = 5;
    [SerializeField] private float shrinkAmount = 10;
    [SerializeField] private float shrinkMultipler = 1.25f;
    [SerializeField] private bool hasPriority = false;
    private SpawnManager spM;
    public GameObject healthBarPrefab;
    private AudioSource boulderAudio;
    [SerializeField] private AudioClip hitSFX;
    [SerializeField] private AudioClip homeSFX;
    [SerializeField] private float shielding;
    [SerializeField] private float maxShielding;
    //public bool isTurning;
    //[SerializeField] private float healthLoss;
    void Start()
    {
        spM = GameObject.Find("Game Manager").GetComponent<SpawnManager>();
        manager = GameObject.Find("Game Manager").GetComponent<SpawnManager>();
        render = GetComponent<MeshRenderer>();
        isHouse = false;
        if (spM.GetGameState())
        {
            playerPos = GameObject.Find("Player").GetComponent<Transform>().position;
        }
        else
        {
            playerPos = new Vector3(0,0,0);
        }
        target = playerPos;
        target.x += Random.Range(-5,6);
        target.z += Random.Range(-5,6);
        //Get some nice RNG for fun
        SetDirection((target - transform.position).normalized);
        //randomize size
        transform.localScale = new Vector3(Random.Range(minSize,maxSize), 3, Random.Range(minSize, maxSize));
        speed = Random.Range(minSpeed,maxSpeed);
        originalSpeed = speed;
        boulderAudio = GetComponent<AudioSource>();
        shielding = 0;
        /*change color depending on the spawner count (difficulty)
        Color previousColor = normalColor.color;
        int colorIncrement = spM.GetSpawnerCount() * 10;
        Color matColor = new Color(previousColor.r + colorIncrement, previousColor.g, previousColor.b + colorIncrement, 1);
        render.material.color = matColor;
        Debug.Log(render.material.color.r);
        */
        //health = CalculateTotalSize(gameObject) / speed;
        //maxHealth = health;
        //a faster target will have less health.
        //isTurning = false;
    }

    //For home exponental increase bug, i am destroying it when it gets too big
    /*Other solutions could include:
     * Checking if there are too many triggers happening at once
     * Making it a power up so the super size is supposed to happen (Might do this cuz id like the player dodging more)s Could also add limits to bullets for same effect
     * Adding invincibility frames when it gets too big (encourage it)
     */

    //Pushback direction bug
    /*
     * Children have different independent direction variable to parent
     * To calculate pushback, we use the first boulder and first home detected and dont check if they have a parent intitally
     * What happens when a priority boulder hits the home again?
     * Might have found out the cause!
     *      when there is pushback, we use the child boulder position. This is ok, until the home grows to a point the boulder gets inside it and the boulder bounces inside it.
     *      this explains the riduculous amounts of direction changes and it flings in a random direction because when the home is destroyed, it flings in the last bounced direction
     *SOULUTION: DELETE THE BOUDLER THAT GETS STUCK INSIDE THE HOME 
     */
    /*Boulder stuck solution -Somehow detect if boulder is inside of home and delete boulder
     * testing for multiple triggers to the boulder parent, caused by individual parts of home (if 2 parts of home are tiggering boulder, delete it)
     * use a polymesh for the entire boulder and home. If this works, code will need some mass revamping as we no longer need to worry about children collison. This speeds up code but will make 90% of my work deleted
     * Add an empty gamobject parent to each boulder/home and use it's center positon to make another hitbox, proportial to it's size, for detectig if something goes to far in. This is flawed tho as there are different shapes 
     * 
     * 
     */
    //Boulder not being childed properly (child inception happening) (FIXED)
    /*
     * transform.parent boolean could be an issue 
     *  there is a check in CalculateTotalSize for this
     *  in the triggerEnter for boulder and boulder, there is a check
     *maybe try switching boolean as soon as possilbe (might be too many trigger happening for booleans to switch correctly) 
     *could add a failSafe check where a function in update checks if a child has another child -EASIEST SOLUTION but not the most optimized
     */
    //FEATURES TO BE ADDED
    /* Add the extra stats to be displayed when game over (DONE)
     * Add a ui score during runtime (DONE)
     * Add a lined path when boulders are fling (mostly for debugging purposes)
     * New boulder health system (you shoot the boulder to slow them first. Once a boulder is no longer moving, you will shoot their health) (REDACTED)
         * Max health might need to be reworked
         *  This makes it so player can make boulders completely stack up around them, meaning player can survive infinitely. Although there us a score system to disencourage it, we might want to add an addiotnal insentive to destroy still boulders
         *      -Have a time limit for still boulders to be destroyed. Will either automaticlly turn into home or just be destroyed
         *      -This time limit will be reset if a boulder hits it 
     * Restirict how fast boulders will be bounced off (change the vector). May or may not implement this
     * Add a visual indicator on how much "health" boulder has (the amount of speed boulder has compared to its original speed) (DONE)
     * Add some way to tell player to stand inside homes and not be afraid of them -tutorial?
     * Add sound effects (DONE)
     * Add particle effects
     * Add a file save for highscores with usernames
     * Add keybinding (DONE)
     * Add function where when children detached, children grab info like speed and stuff from parent
     * BIG FEATURES:
     * Add an airstike that will target and then land a couple secs later that drops a power up. Airstike will destroy player and boulders, pushing all its children when airstrike hits
     * Add sheild system where if boulders collide, the parent gets a "sheild", an extra bar on top of health bar. This will have to be removed before speed reduction (DONE)
     * Add a way to disourage constant shooting - overheating?
     * Level up system?
     */
    //PRIORTY BUG
    //when a child of a prioity boulder hits another boulder, it is not passed on
    //Direction might need to be passed on to children as well
    void FixedUpdate()
    {
        //ChildFixer();
        if (!transform.parent) {
            transform.Translate(speed * Time.deltaTime * GetDirection());
        }
        bool isGone = IsOutOfBounds();
        if (isGone && !isHouse)
        {
            BecomeOrphan();
        }
        /*
        if (isGone && transform.childCount ------------- || isGone && !transform.parent)
        {
            Destroy(gameObject);
        }
        */
        if (isHouse)
        {
            //if the home is a parent or single, reduce its scale
            if (!transform.parent)
            {
                transform.localScale -= new Vector3(shrinkAmount * Time.deltaTime, 0, shrinkAmount * Time.deltaTime);
                if (transform.localScale.x <= 0 || transform.localScale.z <= 0)
                {
                    HomeDestruction(gameObject);
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Boulder"))
        {
            if (CompareTag("Boulder")) {
                float selftotalSize = CalculateTotalSize(gameObject);
                float otherTotalSize = CalculateTotalSize(other.gameObject);
                if (selftotalSize < otherTotalSize)
                {
                    //Check if the collided boulder is a child. If so, get its parent
                    GameObject newParent = FindParent(other.gameObject);
                    //Get the current parent
                    GameObject oldParent = FindParent();
                    //for every prexisting child of this parent, make the other parent their new parent. This stops heirarchy complexites
                    Transform[] ownedChildren = oldParent.GetComponentsInChildren<Transform>();
                    foreach (Transform child in ownedChildren)
                    {
                        child.SetParent(newParent.transform, true);
                        child.GetComponent<Boulder>().speed = newParent.GetComponent<Boulder>().speed;
                    }
                    oldParent.transform.SetParent(newParent.transform, true);
                    Boulder newParentScript = newParent.GetComponent<Boulder>();
                    //Shielding is based on the new boulder's size and new boulder's shielding. However, we need to reduce the amount gained as it gets bigger, as its already to get shielding due to sheer size
                    newParentScript.shielding += (selftotalSize+oldParent.GetComponent<Boulder>().shielding)/(otherTotalSize/2);
                    /*
                    if (newParentScript.shielding > newParentScript.maxShielding)
                    {
                        newParentScript.shielding = newParentScript.maxShielding;
                    }
                    */
                    //check if there is a health bar to display a change on
                    if (HealthBar.IsTarget(newParent) != null)
                    {
                        newParentScript.ShieldChange();
                    }
                    //Checks if this boulder was flung from a home. If so, we push the other boulder in the same direction. If the other boulder also has priority, there is no effect
                    if (hasPriority && !newParentScript.hasPriority)
                    {
                        //Debug.Log("Before Priority: "+transform.parent.GetComponent<Boulder>().GetDirection());
                        newParentScript.SetDirection(GetDirection());
                        //Debug.Log("After Priority: " + transform.parent.GetComponent<Boulder>().GetDirection());
                        newParentScript.hasPriority = true;
                        Transform[] childTransforms = newParent.GetComponentsInChildren<Transform>();
                        foreach (Transform t in childTransforms)
                        {
                            Boulder script = t.GetComponent<Boulder>();
                            script.hasPriority = true;
                        }
                    }
                }
                /*
                if (FindParent(gameObject).GetComponent<Boulder>().isTurning)
                {
                    FindParent(gameObject).GetComponent<Boulder>().SetAllToMaterial(flashColour);
                }
                else
                {
                    FindParent(gameObject).GetComponent<Boulder>().SetAllToMaterial(normalColor);
                }
                */

            }
            else if (CompareTag("Home"))
            {
                //The home parent will pulse out. If there isn't one, it will be its own parent
                GameObject homeParent = FindParent();
                homeParent.transform.localScale += new Vector3(shrinkAmount, 0, shrinkAmount);
                homeParent.GetComponent<Boulder>().shrinkAmount *= shrinkMultipler;
                //if we dont increase shrink amount, the force field will be up forever
                //This is because everytime its hit, it gets bigger, which makes it easier to hit. 

                //In case the field get exponentally big. Ill find some way to make this a gameplay mechanic or ill find another soulution---------------------------------------------------------------------------------------------------------
                if (CalculateTotalSize(homeParent) > 500)
                {
                    HomeDestruction(homeParent);
                    manager.GrowthEvent();
                }
            }
        }
        else if (other.CompareTag("Home") && CompareTag("Boulder"))
        {
            PlayHitAudio();
            //If the boulder hits a home, this boulder will pushed back
            Vector3 pushBack = (transform.position - other.transform.position).normalized * explosionForce;
            pushBack.y = 0;
            GameObject p = FindParent();
            Boulder pScript = GetComponent<Boulder>();
            //if boulder is a child, push the parent
            //Debug.Log("Before PushBack: " + p.GetComponent<Boulder>().GetDirection());
            pScript.SetDirection(pushBack);
            //Debug.Log("After PushBack: " + p.GetComponent<Boulder>().GetDirection());
            pScript.hasPriority = true;
            //for each child of this, give them priority
            Transform[] childTransforms = p.GetComponentsInChildren<Transform>();
            foreach (Transform t in childTransforms)
            {
                Boulder script = t.GetComponent<Boulder>();
                script.hasPriority = true;
                script.SetDirection(p.GetComponent<Boulder>().GetDirection());
            }
            /*
            if (!transform.parent)
            {
                //If boulder is a parent or alone, push them
                //Debug.Log("Before PushBack: " + GetDirection());
                SetDirection(pushBack);
                //Debug.Log("After PushBack: " + GetDirection());
                hasPriority = true;
                //for each child of this, give them priority. Wont effect alone boulders
                Transform[] childTransforms = GetComponentsInChildren<Transform>();
                foreach (Transform t in childTransforms)
                {
                    Boulder script = t.GetComponent<Boulder>();
                    script.hasPriority = true;
                }
            }
            else
            {
                //if boulder is a child, push the parent
                Debug.Log("Before PushBack: " + transform.parent.GetComponent<Boulder>().GetDirection());
                transform.parent.GetComponent<Boulder>().SetDirection(pushBack);
                Debug.Log("After PushBack: " + transform.parent.GetComponent<Boulder>().GetDirection());
                transform.parent.GetComponent<Boulder>().hasPriority = true;
                //for each child of this, give them priority
                Transform[] childTransforms = transform.parent.GetComponentsInChildren<Transform>();
                foreach (Transform t in childTransforms)
                {
                    Boulder script = t.GetComponent<Boulder>();
                    script.hasPriority = true;
                }
            }
            */
        }
        else if (other.CompareTag("Bullet") && CompareTag("Boulder"))
        {
            //If this is a boulder and it collides with a bullet, decrease its speed. If 0 or less, it will turn into a home
            Boulder script = FindParent().GetComponent<Boulder>();
            if (script.shielding > 0)
            {
                script.shielding -= other.GetComponent<BulletMove>().GetDamage();
                script.ShieldChange();
            }
            else
            {
                script.speed -= other.GetComponent<BulletMove>().GetDamage();
                script.DamageBoulder();
                if (script.speed <= 0)
                {
                    script.speed = 0;
                    script.TurnHome();
                    //b.BecomeIsTurning();
                    //b.ChangeHealth(other.GetComponent<BulletMove>().GetDamage());
                }
            }
        }
 
        /*
        if (!transform.parent && !transform.childCount -------------) {
            Destroy(gameObject);
        }
        else {
            Transform parent = transform.parent;
            if (parent == null)
            {
                //if no parent present, means this is the parent
                parent = transform;
            }
            Debug.Log("Parent is "+parent);
            Transform[] children = GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                child.transform.parent = null;
                child.transform.Translate((transform.position - parent.position).normalized * explosionForce);
            }
        }
        */
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Home") && CompareTag("Boulder"))
        {
            PlayHitAudio();
            //If the boulder hits a home, this boulder will pushed back
            Vector3 pushBack = (transform.position - other.transform.position).normalized * explosionForce;
            pushBack.y = 0;
            GameObject p = FindParent();
            //if boulder is a child, push the parent
            //Debug.Log("Before PushBack: " + p.GetComponent<Boulder>().GetDirection());
            p.GetComponent<Boulder>().SetDirection(pushBack);
            //Debug.Log("After PushBack: " + p.GetComponent<Boulder>().GetDirection());
            p.GetComponent<Boulder>().hasPriority = true;
            //for each child of this, give them priority
            Transform[] childTransforms = p.GetComponentsInChildren<Transform>();
            foreach (Transform t in childTransforms)
            {
                Boulder script = t.GetComponent<Boulder>();
                script.hasPriority = true;
                script.SetDirection(p.GetComponent<Boulder>().GetDirection());
            }
        }
    }
    private bool IsOutOfBounds()
    {
        //Despawn offset to make deletion more smooth 
        if (transform.position.x > SpawnManager.GetxRange() + despawnOffset || transform.position.x < -SpawnManager.GetxRange() - despawnOffset)
        {
            return true;
        }
        else if (transform.position.z > SpawnManager.GetzRange() + despawnOffset || transform.position.z < -SpawnManager.GetzRange() - despawnOffset)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void TurnHome()
    {
        isHouse = true;
        gameObject.tag = "Home";
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material = homeColour;
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in childRenderers)
        {
            r.material = homeColour;
            r.gameObject.tag = "Home";
            r.GetComponent<Boulder>().isHouse = true;
        }
        transform.localScale.Set(transform.localScale.x, transform.localScale.y+2, transform.localScale.z);
        PlayHomeAudio();
        //increase the y value so there are no clipping concerns between boulders and homes
        SetAllContinuous();
        spM.BouldersTurned(1 + childRenderers.Length,CalculateTotalSize(gameObject));
        //All the boulders, which includes the parent boulder and its children
    }

    private void SetAllToMaterial(Material mat)
    {
        GetComponent<MeshRenderer>().material = mat;
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in childRenderers)
        {
            r.material = mat;
        }
    }
    private void SetAllContinuous()
    {
        GameObject parent = FindParent(gameObject);
        parent.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
        Rigidbody[] childRigidBody= parent.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody r in childRigidBody)
        {
            r.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
    }
    private float CalculateTotalSize(GameObject obj)
    {
        float totalSize;
        GameObject parent = FindParent(obj);
        totalSize = parent.transform.localScale.x + parent.transform.localScale.z;
        //Add to this for each child's size
        Transform[] childTransforms = parent.GetComponentsInChildren<Transform>();
        foreach (Transform transform in childTransforms)
        {
            totalSize += transform.localScale.x + transform.localScale.z;
        }
        return totalSize;
    }
    private void SetDirection(Vector3 d)
    {
        d.y = 0;
        direction = d;
        directionChangeLog.Add(d);
    }
    private Vector3 GetDirection()
    {
        return direction;
    }
    private void HomeDestruction(GameObject homeParent)
    {
        GameObject ff = GameObject.Find("Force Field");
        if (ff && PlayerController.inHome)
        {
            Debug.Log("Home destroyed, force field leaving...");
            Debug.Log("In home? "+PlayerController.inHome);
            if (PlayerController.inHome)
            {
                ff.GetComponent<ForceField>().TimeRunningOut();
                PlayerController.inHome = false;
            }
        }
        int size = (int)CalculateTotalSize(homeParent);
        Debug.Log("Size: "+size);
        int powerupChance = Random.Range(0, 50-size);
        Debug.Log("Chance number: "+powerupChance);
        if (powerupChance <= 0)
        {
            spM.ExternalPowerUpCall(homeParent.transform.position);
        }
        Destroy(homeParent);
    }
    /*
    private void ChildFixer()
    {
        //runs if this boulder has a parent and also has a child, which shouldn't happen
        if (transform.parent && transform.childCount > 0)
        {
            Transform[] incorrectChildren = gameObject.GetComponentsInChildren<Transform>();
            foreach (Transform mistake in incorrectChildren)
            {
                mistake.SetParent(transform.parent);
            }
        }
    }
    /*
 public void ChangeHealth(float amount)
 {
     health -= amount;
     Debug.Log("Health: "+health+"/"+ maxHealth);
     if (health <= 0)
     {
         TurnHome();
     }
 }
 public void ChangeHealth()
 {
     health -= healthLoss;
     Debug.Log("Health: " + health + "/" + maxHealth);
     if (health <= 0)
     {
         Debug.Log("House time!");
         CancelInvoke("ChangeHealth");
         TurnHome();
     }
 }
 private void BecomeIsTurning()
 {
     if (!isTurning)
     {
         hasPriority = true;
         isTurning = true;
         InvokeRepeating("ChangeHealth",0,1);
     }
 }
 */

    /*
    private IEnumerator HomeCountDown()
    {
        isTurning = true;
        SetAllToMaterial(flashColour);
        yield return new WaitForSeconds(homeTimeDelay);
        //check if isTurning was cancels by an action
        if (!isHouse && isTurning)
        {
            TurnHome();
        }
    }
    private void CancelHomeCountDown()
    {
        isTurning = false;
        StopCoroutine(HomeCountDown());
    }
    */
    private GameObject FindParent(GameObject obj)
    {
        GameObject parent;
        if (obj.transform.parent != null)
        {
            parent =  obj.transform.parent.gameObject;
        }
        else
        {
            parent = obj;
        }
        return parent;
    }
    private GameObject FindParent()
    {
        GameObject obj = gameObject;
        GameObject parent;
        if (obj.transform.parent != null)
        {
            parent = transform.parent.gameObject;
        }
        else
        {
            parent = obj;
        }
        return parent;
    }
    private GameObject GetHealthBar()
    {
        GameObject healthBar = HealthBar.IsTarget(gameObject);
        GameObject p = FindParent(gameObject);
        if (healthBar == null)
        {
            healthBar = Instantiate(healthBarPrefab, p.transform.position, p.transform.rotation);
            healthBar.GetComponent<HealthBar>().SetTarget(gameObject);
        }
        return healthBar;
    }
    private void ShieldChange()
    {
        //Debug.Log("Current Shielding: "+shielding);
        GameObject shieldBar = GetHealthBar();
        Transform shield = shieldBar.transform.Find("Shield").transform;
        Vector3 shieldScale = shield.localScale;
        if (shield.gameObject.activeSelf == false)
        {
            shield.transform.localScale = new Vector3(0, shieldScale.y, shieldScale.z);
            shield.gameObject.SetActive(true);
        }
        float currrentShield = shielding / maxShielding;
        shield.localScale = new Vector3(currrentShield, shieldScale.y, shieldScale.z);
        if (shield.transform.localScale.x <= 0)
        {
            shield.gameObject.SetActive(false);
        }
    }
    private void DamageBoulder()
    {
        GameObject healthBar = GetHealthBar();
        Transform health = healthBar.transform.Find("Health").transform;
        Vector3 healthScale = health.localScale;
        float currrentHealth = speed / originalSpeed;
        if (speed < 0)
        {
            speed = 0;
        }
        health.localScale = new Vector3(currrentHealth,healthScale.y,healthScale.z);
    }
    private void PlayHitAudio()
    {
        boulderAudio.clip = hitSFX;
        boulderAudio.volume = spM.GetVolume();
        boulderAudio.Play();
    }
    private void PlayHomeAudio()
    {
        boulderAudio.clip = homeSFX;
        boulderAudio.volume = spM.GetVolume();
        boulderAudio.Play();
    }
    public void BecomeOrphan()
    {
        if (transform.childCount > 0)
        {
            //for every child of this boulder, we will make them have the parent's original direction along with changing the boolean correctly here
            Transform[] childTransforms = GetComponentsInChildren<Transform>();
            foreach (Transform t in childTransforms)
            {
                Boulder script = t.GetComponent<Boulder>();
                script.SetDirection(GetDirection());
            }
            transform.DetachChildren();
        }
        Destroy(gameObject);
    }
}

