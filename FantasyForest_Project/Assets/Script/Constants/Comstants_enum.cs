namespace Constants
{
    /// <summary>
    /// チームカラー
    /// </summary>
    public enum TEAM_COLOR
    {
        BLUE,
        RED,
        NATURAL
    }

    /// <summary>
    /// AIのステート
    /// </summary>
    public enum AI_STATUS
    {
        NONE,   // 何もしていない
        SEARCH, // 探索
        CAPTURE,// 占領
        MOVE,   // 移動
        ATTACK, // 攻撃
        DEFENSE,// 防衛
    }

    /// <summary>
    /// アニメーションのステート
    /// </summary>
    public enum ANIMATION_STATE
    {
        IDLE = 0,
        RUN,
        ATTACK,
        DOWN
    }

    /// <summary>
    /// キャラクターのタイプ
    /// </summary>
    public enum CHARACTER_TYPE
    {
        NONE,
        CAT =1,
        ELF = 2,
        GOLEM = 3,
    }
}