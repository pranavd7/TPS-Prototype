using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector] public CinemachineFreeLook playerCam;
    [HideInInspector] public Animator rigController;
    [SerializeField] Vector2[] recoil;
    float vRecoil;
    float hRecoil;
    [SerializeField] float duration;
    CinemachineImpulseSource camShake;

    float time;
    int index;
    public float recoilMultiplier;

    private void Awake()
    {
        camShake = GetComponent<CinemachineImpulseSource>();
    }
    public void GenerateRecoil(string gunName)
    {
        //time = duration;
        hRecoil = recoil[index].x;
        vRecoil = recoil[index].y;

        playerCam.m_YAxis.Value -= vRecoil * recoilMultiplier;
        playerCam.m_XAxis.Value -= hRecoil * recoilMultiplier;
        camShake.GenerateImpulse(Camera.main.transform.forward);
        rigController.Play("recoil_" + gunName, 1);

        index = (index + 1) % recoil.Length;
    }

    public void ResetRecoil()
    {
        index = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (time > 0)
        //{
        //    playerCam.m_YAxis.Value -= vRecoil * Time.deltaTime / duration;
        //    time -= Time.deltaTime;
        //}
    }
}
