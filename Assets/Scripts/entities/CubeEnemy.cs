﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CubeEnemy : AIEnemy
{
    [SerializeField] private ParticleSystem collisionParticles;
    [SerializeField] private ParticleSystem deathParticles;
    [SerializeField] private float acceleration = 1;
    [SerializeField] private float maxSpeed = 1;
    private Rigidbody _rigidbody;
    private ParticleSystem _particles;

    void Start()
    {
        health = 5;
        _rigidbody = GetComponent<Rigidbody>();
        _particles = Instantiate(collisionParticles);
    }

    // Start is called before the first frame update

    protected void doStep(Vector3 targetPoint, Vector3 gravityVector)
    {
        Vector3 fwd = (targetPoint - transform.position).normalized;
        Vector3 up = -gravityVector.normalized;

        Vector3 applyPoint = transform.position - fwd * transform.localScale.z + up * transform.localScale.y;
        if (acceleration > maxSpeed) acceleration = maxSpeed;
        _rigidbody.AddForceAtPosition(fwd * acceleration * Time.deltaTime * 1000, applyPoint, ForceMode.Impulse);
        preventExceedMaxSpeed();
    }

    protected void preventExceedMaxSpeed()
    {
        _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, maxSpeed);
    }

    protected override void StepTowardsTarget()
    {
        if(GetPath() == null) return;
        Node target = GetNextPathPoint();
        Vector3 gravityDir = (currentPlanet.transform.position - transform.position).normalized;
        doStep(target.vertex, gravityDir);
        UpdateNextPathPoint();
    }

    protected override bool IsTargetReached()
    {
        if (GetPath() == null) return false;
        return false;
    }

    protected override void OnTargetReached()
    {
        return;
    }

    public override void DestroySelf(){

        Destroy(_particles);

        ParticleSystem deathParticles = Instantiate(this.deathParticles);
        deathParticles.transform.LookAt(transform.position);
        deathParticles.transform.position = transform.position;

        GameObject.Find("CameraController").GetComponent<CameraController>().ShakeCamera();

        Destroy(deathParticles, 3);
        Destroy(transform.root.gameObject);

    }

}