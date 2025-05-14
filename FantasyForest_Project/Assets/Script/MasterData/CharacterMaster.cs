using MasterMemory;
using MessagePack;

[MemoryTable("Character"),MessagePackObject(true)]
public sealed class CharacterMaster
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Name { get; set; }
    public int WeaponType { get; set; }
    public int Hp {  get; set; }
    public int Speed { get; set; }
}
