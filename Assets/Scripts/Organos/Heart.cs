using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Organ_Be
{
    // Start is called before the first frame update
    void Start()
    {
        canbeused = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerMovement.instance.animator.SetBool("TakeOrgan", true);
            PlayerMovement.instance.animator.SetBool("Idle", false);
            //PauseManager.isPaused = true;
            StartCoroutine( WaitForSeconds(1f));
        }
    }

    IEnumerator WaitForSeconds(float seconds)
    {
        Debug.Log("Waited for " + seconds + " seconds");
        yield return new WaitForSeconds(seconds);
       // PauseManager.isPaused = false;
        PlayerMovement.instance.animator.SetBool("TakeOrgan", false);
        PlayerMovement.instance.animator.SetBool("Idle", true);

    }
}
