using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ZoomOutOverTime : MonoBehaviour
{
    // 58 covers the entire Background Image
    public float maxZoom = 58f;
    public float zoomSpeed = 0.3f;
    
    private CinemachineVirtualCamera cinemachineVirtualCamera;

    // Start is called before the first frame update
    void Start()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cinemachineVirtualCamera.m_Lens.OrthographicSize < 58)
        {
            cinemachineVirtualCamera.m_Lens.OrthographicSize += 0.3f * Time.deltaTime;
        }
    }
}