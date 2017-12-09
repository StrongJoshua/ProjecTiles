using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootStep : MonoBehaviour {

    public AudioSource leftSound, rightSound;
    public DroneAnimationControl parent;

    void FootstepLeftEvent()
    {
        if (parent.IsGrounded && !leftSound.isPlaying && !rightSound.isPlaying)
            leftSound.Play();
    }

    void FootstepRightEvent()
    {
        if (parent.IsGrounded && !rightSound.isPlaying && !leftSound.isPlaying)
            rightSound.Play();
    }
}
