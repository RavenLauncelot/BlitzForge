using System.Threading;
using UnityEngine;

public class speen : MonoBehaviour
{
    [SerializeField] private float speenSpeed;
    //https://tenor.com/en-GB/view/speen-spin-lain-lain-iwakura-iwakura-lain-gif-23510083

    private Transform speenTransform;
    private Vector3 speenVector;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        speenTransform = transform;
        speenVector = new Vector3(0, speenSpeed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        speenTransform.rotation *= Quaternion.Euler(speenVector * Time.deltaTime);   
    }
}
