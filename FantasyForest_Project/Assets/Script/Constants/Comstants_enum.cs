namespace Constants
{
    /// <summary>
    /// �`�[���J���[
    /// </summary>
    public enum TEAM_COLOR
    {
        BLUE,
        RED,
        NATURAL
    }

    /// <summary>
    /// AI�̃X�e�[�g
    /// </summary>
    public enum AI_STATUS
    {
        NONE,//�������Ă��Ȃ�
        SEARCH,//�T��
        CAPTURE,//���
        MOVE,//�ړ�
        ATTACK,//�U��
    }

    /// <summary>
    /// �A�j���[�V�����̃X�e�[�g
    /// </summary>
    public enum ANIMATION_STATE
    {
        IDLE = 0,
        RUN,
        ATTACK,
        DOWN
    }

}