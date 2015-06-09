using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MusicSatalite : MonoBehaviour
{
    public AudioClip[] NormAudio;
    public AudioClip[] MenuAudio;

    List<Satalite> _satalites = new List<Satalite>();
    ParticleSystem _particles;

    public void Awake()
    {
        _particles = transform.GetChild(0).GetComponent<ParticleSystem>();
        var randAMount = 0.1f;
        for(var i = 0; i < 5; ++i)
        {
            var s = new Satalite();
            s.Rotation = UnityEngine.Random.Range(0, Mathf.PI * 2);
            s.Pos = new Vector2(Mathf.PI * 2f * (i / 5f) + UnityEngine.Random.Range(-randAMount * 5f, randAMount * 5f), UnityEngine.Random.Range(-randAMount, randAMount));
            //s.Vel = 0.1f * new Vector2(1f + UnityEngine.Random.Range(-randAMount / 10f, randAMount / 10f), UnityEngine.Random.Range(-randAMount / 10f, randAMount / 10f));
            s.WorldPosition = s.Pos.ToWorld(MainGame.Radius);

            s.NormClip = s.GameObject.AddComponent<AudioSource>();
            s.NormClip.volume = 0f;
            s.NormClip.loop = true;
            if (i < NormAudio.Length)
            {
                s.NormClip.clip = NormAudio[i];
                s.NormClip.Play();
            }
            s.MenuClip = s.GameObject.AddComponent<AudioSource>();
            s.MenuClip.volume = 0f;
            s.MenuClip.loop = true;
            if (i < MenuAudio.Length)
            {
                s.MenuClip.clip = MenuAudio[i];
                s.MenuClip.Play();
            }
            _satalites.Add(s);
        }
    }
    
    public void Update()
    {
        var volSpeed = 1f;
        if(MainGame.S.GameState == GameState.Playing)
        {
            foreach(var s in _satalites)
            {
                s.Pos += s.Vel * Time.deltaTime;
                s.WorldPosition = s.Pos.ToWorld(MainGame.Radius);
                s.Rotation += Time.deltaTime * 90f;

                s.NormClip.volume += volSpeed * Time.deltaTime;
                s.MenuClip.volume -= volSpeed * Time.deltaTime;
            }
        }
        else
        {
            foreach (var s in _satalites)
            {
                s.Pos += s.Vel * Time.deltaTime;
                s.WorldPosition = s.Pos.ToWorld(MainGame.Radius);
                s.Rotation += Time.deltaTime * 90f;

                s.NormClip.volume -= volSpeed * Time.deltaTime;
                s.MenuClip.volume += volSpeed * Time.deltaTime;
            }
        }
        var size = 1f + Mathf.Cos((Time.time / 60) * 120);

        var particles = _satalites.Select(s => new ParticleSystem.Particle()
        {
            rotation = s.Rotation,
            color = Color.Lerp(Color.white, Color.blue, 0.2f) * 0.3f,
            position = s.Pos.ToWorld(MainGame.Radius),
            size = 1f,
            lifetime = 1000,
            startLifetime = 500,
        }).ToArray();
        _particles.SetParticles(particles, particles.Length);
    }

    [Serializable]
    class Satalite : UnityObject
    {
        public Vector2 Vel;
        public Vector2 Pos;
        public AudioSource MenuClip;
        public AudioSource NormClip;
        public float Rotation;
    }
}
