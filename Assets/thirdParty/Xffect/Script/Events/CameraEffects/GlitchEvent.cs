using UnityEngine;
using System.Collections;

namespace Xft
{
    //modified from here:http://va.lent.in/blog/2010/01/25/how-i-made-the-tv-glitch-effect-for-va-lent-in/
    public class WaveRandom
    {
        protected int seed = 0;
        protected float[] dSeeds = new float[3];
        protected float[] seeds = new float[3];
        protected Vector3 disp = Vector3.zero;
        
        public void Reset ()
        {
            seeds [0] = Random.Range (1f, 2f);
            seeds [1] = Random.Range (1f, 2f);
            seeds [2] = Random.Range (1f, 2f);
            seed = 0;
        }       
        
        
        //must call Reset Before!
        public Vector3 GetRandom(float minAmp, float maxAmp, float minRand, float maxRand, int len)
        {
            
            float difAmp = maxAmp - minAmp;
            seed++;
            if (seed >= len) {
                seed = 0;
            }
     
            float v = Mathf.PI / len * seed;
            float sin = 1.27323954f * v - 0.405284735f * v * v;
            float amp = minAmp + sin * difAmp;
     
            float pi2 = 6.28318531f;
     
            
            for (int i = 0; i < 3; i++) {
                if (seeds [i] >= 1f) {
                    seeds [i] = seeds [i] - 1f;
                    dSeeds [i] = Random.Range (minRand, maxRand);
                }
                
                seeds [i] += dSeeds [i];
                v = seeds [i] * pi2;
                
                if (v > Mathf.PI)
                    v -= pi2;
                if (v < 0f)
                    sin = 1.27323954f * v + 0.405284735f * v * v;
                else
                    sin = 1.27323954f * v - 0.405284735f * v * v;
                disp [i] = sin * amp;
            }
            
            return disp;
        }
    }
    
    
    
    public class GlitchEvent : CameraEffectEvent
    {
        protected XftGlitch m_glitchComp;
        protected bool m_supported = true;
     
        protected WaveRandom m_random;
     
        public GlitchEvent (XftEventComponent owner)
            : base(CameraEffectEvent.EType.Glitch, owner)
        {
            m_random = new WaveRandom();
        }
        public override void ToggleCameraComponent (bool flag)
        {
            m_glitchComp = MyCamera.gameObject.GetComponent<XftGlitch> ();
            if (m_glitchComp == null) {
                m_glitchComp = MyCamera.gameObject.AddComponent<XftGlitch> ();
            }
            m_glitchComp.Init (m_owner.GlitchShader,m_owner.GlitchMask);
            m_glitchComp.enabled = flag;
        }

        public override void Initialize ()
        {
            ToggleCameraComponent (false);
            m_supported = m_glitchComp.CheckSupport ();
            if (!m_supported)
                Debug.LogWarning ("can't support Image Effect: Glitch on this device!");
            m_random.Reset();
        }

        public override void Reset ()
        {
            base.Reset();
            m_glitchComp.enabled = false;
            m_elapsedTime = 0f;
            ResetMyCamera();
            m_random.Reset();
        }
     
        public override void Update (float deltaTime)
        {
            if (!m_supported) {
                m_owner.enabled = false;
                return;
            }
            m_elapsedTime += deltaTime;
            m_glitchComp.Offset = m_random.GetRandom(m_owner.MinAmp,m_owner.MaxAmp,m_owner.MinRand,m_owner.MaxRand,m_owner.WaveLen);
            m_glitchComp.enabled = true;
        }
    }
}

