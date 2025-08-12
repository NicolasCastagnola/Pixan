using Audio;
public class MictlantecuhtliSpawnMinionsState : IState
{

    private readonly Mictlantecuhtli _owner;
    private readonly AudioConfigurationData _audioConfigurationData;
    public MictlantecuhtliSpawnMinionsState(Mictlantecuhtli owner, AudioConfigurationData audioConfigurationData)
    {
        _owner = owner;
        _audioConfigurationData = audioConfigurationData;
    }
    public void OnEnter()
    {
        _audioConfigurationData.Play3D();
    }
    public void OnUpdate()
    {
    }
    public void OnExit()
    {
    }
    public void OnFixedUpdate()
    {
    }
}