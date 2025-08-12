
using UnityEngine;
public class MictlantecuhtliAnimatorController : EntityAnimatorController
{
    private Mictlantecuhtli _mictlantecuhtli;
    public override void Initialize(Entity owner)
    {
        base.Initialize(owner);

        _mictlantecuhtli = _owner as Mictlantecuhtli;
    }
    public void ActivateWeapon() => _mictlantecuhtli.enemyWeapon.EnableDisableWeapon(true);
    public void DeactivateWeapon() => _mictlantecuhtli.enemyWeapon.EnableDisableWeapon(false);
    public void Stomp() => _mictlantecuhtli.Stomp();
    public void Punch() => _mictlantecuhtli.Punch();
    public void Heal() => _mictlantecuhtli.StartHealing();
    public void Buff() => _mictlantecuhtli.StartBuffing();
}