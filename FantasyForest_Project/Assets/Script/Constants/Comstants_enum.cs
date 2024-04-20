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
        DEFENSE,//�h�q
    }

    /// <summary>
    /// �L�����N�^�[�̃X�e�[�g
    /// </summary>
    public enum CHARACTER_STATUS
    {
        NONE,
        ALIVE,//�A�N�e�B�u���
        DEAD,//��A�N�e�B�u���
        RESPOWN_WAIT,//���X�|�[���ҋ@��
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

    public enum WEPON
    {
        NONE = 0,
        Sword, //��
        Bow,   //�|�i�����肠��j
        Arrow, //��
    }
}