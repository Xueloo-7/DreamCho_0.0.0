using UnityEngine;

public class DashEnergyManager : Singleton<DashEnergyManager>
{
    [SerializeField] ParticleSystem energyPar; // 拾取体力后的爆开粒子
    
    public void PlayParticle(Vector2 position)
    {
        ParticleSystem par = ObjectPool.Instance.GetObjectFromPool(energyPar,transform);
        par.transform.position = position;
        par.Play();
        ObjectPool.Instance.ReturnObjectToPool(par, par.main.startLifetime.constant);
    }
}
