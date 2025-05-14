using System.IO;
using MasterMemory;
using MessagePack;
using MessagePack.Resolvers;
using UnityEditor;

public static class BinaryGenerator
{
    [MenuItem("Window/Generate Binary")]
    public static void Run()
    {
        //MessagePack�̏�����
        var messagePackResolvers = CompositeResolver.Create(
            MasterMemoryResolver.Instance,
            StandardResolver.Instance);
        var option = MessagePackSerializerOptions.Standard.WithResolver(messagePackResolvers);
        MessagePackSerializer.DefaultOptions = option;

        //�f�[�^������
        var characterMasters = new[]
        {
            new CharacterMaster
            {
                Id = 1,
                Name = "CAT",
                WeaponType = 1,
                Hp = 100,
                Speed = 45
            },
            new CharacterMaster
            {
                Id = 2,
                Name = "ELF",
                WeaponType = 2,
                Hp = 150,
                Speed = 40
            },
            new CharacterMaster
            {
                Id = 3,
                Name = "GOLEM",
                WeaponType = 3,
                Hp = 300,
                Speed = 20
            }
        };

        // DatabaseBuilder���g���ăo�C�i���f�[�^�𐶐�����
        var databaseBuilder = new DatabaseBuilder();
        databaseBuilder.Append(characterMasters);
        var binary = databaseBuilder.Build();

        //�ł����o�C�i���͉i�������Ă���
        var path = "Assets/Binary/CharacterMaster.bytes";
        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        File.WriteAllBytes(path, binary);
        AssetDatabase.Refresh();
    }
}
