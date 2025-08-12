public enum States {
    #region PlayerStates
    PlayerIdle,
    PlayerWalk,
    PlayerDash,
    PlayerRoll,
    PlayerBlock,
    PlayerHurt,
    PlayerAttack,
    PlayerChargedAttack,
    PlayerStun,
    PlayerWalkBlock,
    PlayerDeath,
    #endregion
    #region EnemiesStates
    EnemyIdle,
    //EnemyPatrol,
    EnemyChase,
    EnemyAttack,
    EnemyStagger,
    EnemyHurt,
    EnemyDeath,
    EnemyRangeAttack,
    EnemyKick,
    TargetLockWalk
    #endregion
    ,
    EnemyIntro,
    StompAttack,
    EnemyDistanceAttack,
    EnemyHeal,
    EnemyBuff,
    EnemySpawnMinions,
    PlayerInBonfire,
    PlayerHeal,
    Paused
}