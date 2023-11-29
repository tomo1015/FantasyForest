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
    }

}