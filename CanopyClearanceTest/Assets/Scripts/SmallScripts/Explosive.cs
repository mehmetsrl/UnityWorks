using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : MonoBehaviour
{
    public bool destroyWhenExploded = true;
    public bool hideObjectWhenExploded = true;
    public GameObject explosionPrefab;
    public bool destroyEffectWhenEnded = true;
    public float explosionTime = 1f;
    public GameObject target = null;
    public float misileSpeed = 500f;

    [SerializeField] private AudioSource soundAudioSource;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip missileMovementSound;

    private Action<Vector3> collisionAction;


    void Start()
    {
        if (soundAudioSource == null)
        {
            soundAudioSource = gameObject.AddComponent<AudioSource>();

        }
    }

    public void FireTo(GameObject source, GameObject target,Action<Vector3> onCollided=null)
    {
        this.collisionAction = onCollided;
        this.target = target;

        transform.parent = source.transform;
        transform.localPosition = Vector3.zero;
        transform.parent = target.transform;

        if (soundAudioSource != null && missileMovementSound != null)
        {
            soundAudioSource.clip = missileMovementSound;
            soundAudioSource.loop = false;
            soundAudioSource.Play();
        }
    }
    
    public void Explode()
    {
        if (collisionAction != null)
            collisionAction.Invoke(target.transform.position);

        if (soundAudioSource != null && explosionSound != null)
        {
            soundAudioSource.clip = explosionSound;
            soundAudioSource.loop = false;
            soundAudioSource.Play();
        }

        StartCoroutine(DestroyExplosionEffect());
    }


    IEnumerator DestroyExplosionEffect()
    {

        foreach (var rend in GetComponents<Renderer>())
        {
            rend.enabled = false;
        }

        GameObject explosion = GameObject.Instantiate(explosionPrefab, transform);
        explosion.transform.localPosition = Vector3.zero;
        
        yield return new WaitForSeconds(explosionTime);

        if (destroyEffectWhenEnded)
        {
            Destroy(explosion);

            if (destroyWhenExploded)
                Destroy(gameObject);
        }
    }

    private bool exploded = false;

    void Update()
    {
        if (target != null)
        {

            transform.position += (target.transform.position - transform.position).normalized * misileSpeed * Time.deltaTime;
            if (misileSpeed < 2500f)
                misileSpeed *= 1.05f;

            transform.LookAt(target.transform);

            //Debug.Log(Vector3.Distance(transform.position, target.transform.position));
            if (Vector3.Distance(transform.position, target.transform.position) < 50f)
            {
                if (!exploded)
                {
                    exploded = true;
                    Explode();
                }
            }
        }

    }

}
