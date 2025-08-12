using UnityEngine;

public class ParticlesManager : BaseMonoSingleton<ParticlesManager>
{
    [SerializeField] private ParticleSystem[] particles;

    [SerializeField] private ParticleSystem parryParticles;
    [SerializeField] private ParticleSystem blockParticles;
    [SerializeField] private ParticleSystem feedbackParticles;
    [SerializeField] private ParticleSystem DeathParticles;
    [SerializeField] private ParticleSystem StompParticle;
    [SerializeField] private ParticleSystem AttackMudParticle;
    [SerializeField] private ParticleSystem AttackStoneParticle;
    [SerializeField] private ParticleSystem PunchParticles;

     ParticleSystem rewardParticles;
    public ParticleSystem GetHitParticle() => particles[0];
    public void SpawnParticles(Vector3 pos, string key)
    {
        ParticleSystem par = null;

        switch (key)
        {
            case "Parry":
                par = parryParticles;
                break;
            case "Block":
                par = blockParticles;
                break;
            case "FeedbackAttack":
                par = feedbackParticles;
                break;
            case "Death":
                par = DeathParticles;
                break;
            case "Stomp":
                par = StompParticle;
                break;
            case "AttackMud":
                par = AttackMudParticle;
                break;
            case "Punch":
                par = PunchParticles;
                break;
            default:
                par = particles[Random.Range(0, particles.Length)];
                break;
        }
        if (key == "AttackMud") par = AttackMudParticle;//por algun motivo no funciona el switch
        else if (key == "AttackStone") par = AttackStoneParticle;

        var part = Instantiate(par);

        part.transform.position = pos;
    }
    public void SpawnParticlesReward(Transform position)
    {
        var par = Instantiate(rewardParticles, position.position, Quaternion.identity);

        par.transform.parent = Player.Instance.transform;
    }
}
