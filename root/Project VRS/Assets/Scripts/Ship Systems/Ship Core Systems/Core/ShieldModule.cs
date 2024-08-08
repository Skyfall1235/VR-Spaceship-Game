using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class ShieldModule : BC_CoreModule
{
    [SerializeField] Material m_shieldMaterial;
    Health m_shieldHealth;
    [SerializeField] Transform transformToTrack;
    [SerializeField] List<Vector4> m_positionsAndDistortion = new List<Vector4>();
    public override void Awake()
    {
        base.Awake();
        m_shieldHealth = GetComponent<Health>();
        if(m_shieldHealth != null)
        {
            m_shieldHealth.OnDamage.AddListener(OnDamageTaken);
        }
        if(GetComponent<Renderer>() != null)
        {
            m_shieldMaterial = GetComponent<Renderer>().sharedMaterial;
        }
    }
    public void OnDamageTaken(DamageData damageData, uint oldHealth, uint newHealth)
    {
        if(m_shieldMaterial != null)
        {
            m_shieldMaterial.SetVector("_Hit_Position", damageData.DamageLocation);
        }
    }

    private void Update()
    {
        m_positionsAndDistortion.Clear();
        m_positionsAndDistortion.Add(new Vector4(transformToTrack.position.x, transformToTrack.position.y, transformToTrack.position.z, 1));

        Texture2D input = new Texture2D(m_positionsAndDistortion.Count, 1, TextureFormat.RGBAFloat, false);
        input.filterMode = FilterMode.Point;
        input.wrapMode = TextureWrapMode.Clamp;
        for (int i = 0; i < m_positionsAndDistortion.Count; i++)
        {
            input.SetPixel(i, 0, m_positionsAndDistortion[i]);
        }
        input.Apply();
        m_shieldMaterial.SetTexture("_IncomingTexture", input);
        m_shieldMaterial.SetInteger("_ArrayLength", m_positionsAndDistortion.Count);
    }
}
