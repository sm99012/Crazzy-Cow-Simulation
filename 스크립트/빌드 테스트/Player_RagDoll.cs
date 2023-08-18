using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_RagDoll : MonoBehaviour
{
    [SerializeField] Rigidbody m_Rigidbody;

    [SerializeField] GameObject m_gParticleSystem_Effect;
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            m_Rigidbody.AddForce(Vector3.up * 10000);
        }
    }

    public void FlyAway(Vector3 dir)
    {
        m_Rigidbody.AddForce(dir * 7500);

        GameObject gm = Instantiate(m_gParticleSystem_Effect);
        gm.transform.position = this.transform.position;
        gm.GetComponent<ParticleSystem>().Play();
    }
}
