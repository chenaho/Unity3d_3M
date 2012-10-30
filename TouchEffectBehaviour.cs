using UnityEngine;
using System.Collections;

public class TouchEffectBehaviour : MonoBehaviour {
	
//	int mShowSecond;
	
	float fSecStart;
	float fSecNow;
	
	
	//public GameObject Emitter;
	
	enum PARTICLE_STATUS{
		PARTICLE_STATUS_SHOW,
		PARTICLE_STATUS_STOP,
		PARTICLE_STATUS_DESTROY
	};
	
	PARTICLE_STATUS m_ParticleStatus;
	
	// Use this for initialization
	void Start () {
		transform.name = "ParticleEmitter_"+ (int)Time.time;
		m_ParticleStatus = PARTICLE_STATUS.PARTICLE_STATUS_SHOW;
		//mShowSecond = 5;
		
		fSecStart = Time.time;
		fSecNow =fSecStart;
	
			
		
	}
	
	// Update is called once per frame
	void Update () {
		fSecStart = Time.time;
		
		int nTimeDiff = (int)(fSecStart - fSecNow);
		//Debug.Log("time:"+nTimeDiff);
		if(  nTimeDiff  > 1)
			m_ParticleStatus = PARTICLE_STATUS.PARTICLE_STATUS_STOP;
		
		if(  nTimeDiff  > 5)
			m_ParticleStatus = PARTICLE_STATUS.PARTICLE_STATUS_DESTROY;		
		
		
		
		if (m_ParticleStatus == PARTICLE_STATUS.PARTICLE_STATUS_STOP)
		{
			//Emitter.GetComponent<ParticleEmitter>().enabled = false;
			transform.GetComponent<ParticleEmitter>().emit= false;
		}
		
		if (m_ParticleStatus == PARTICLE_STATUS.PARTICLE_STATUS_DESTROY)
		{
			//Destroy( this.gameObject , 3.0f);
			Destroy( this.gameObject );
		}
		
	}
		
		
		
		
		
}
