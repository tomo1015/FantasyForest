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
        NONE,//何もしていない
        SEARCH,//探索
        CAPTURE,//占領
        MOVE,//移動
        ATTACK,//攻撃
        DEFENSE,//防衛
    }

    /// <summary>
    /// キャラクターのステート
    /// </summary>
    public enum CHARACTER_STATUS
    {
        NONE,
        ALIVE,//アクティブ状態
        DEAD,//非アクティブ状態
        RESPOWN_WAIT,//リスポーン待機中
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

    public enum WEPON
    {
        NONE = 0,
        Sword, //剣
        Bow,   //弓（直殴りあり）
        Arrow, //矢
    }
}