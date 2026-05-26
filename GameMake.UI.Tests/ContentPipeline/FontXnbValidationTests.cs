namespace GameMake.UI.Tests.ContentPipeline;

public class FontXnbValidationTests
{
    static readonly string s_contentDir = Path.GetFullPath(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            "../../../../GameSample/Content"));

    static readonly string[] s_fontAssets =
    {
        "MicrosoftYaHei-14.xnb",
        "MicrosoftYaHei-16.xnb",
        "MicrosoftYaHei-18.xnb",
        "MicrosoftYaHei-20.xnb",
        "MicrosoftYaHei-22.xnb",
        "MicrosoftYaHei-28.xnb",
        "MicrosoftYaHei-36.xnb",
    };

    [Fact]
    public void 所有字体Xnb文件存在且大小合理()
    {
        foreach (var asset in s_fontAssets)
        {
            var path = Path.Combine(s_contentDir, asset);
            Assert.True(File.Exists(path), $"缺少字体文件: {asset}");
            var size = new FileInfo(path).Length;
            Assert.True(size > 10_000, $"文件太小: {asset} ({size} bytes)");
        }
    }

    [Fact]
    public void 每个Xnb文件头格式有效()
    {
        foreach (var asset in s_fontAssets)
        {
            var path = Path.Combine(s_contentDir, asset);
            using var stream = File.OpenRead(path);
            var reader = new BinaryReader(stream);

            // XNB header: 3 bytes magic + 1 platform + 1 version + 1 flags
            var magic = new string(reader.ReadChars(3));
            Assert.Equal("XNB", magic);
            var platform = reader.ReadByte();
            Assert.True(platform is (byte)'w' or (byte)'m' or (byte)'x' or (byte)'p' or (byte)'d',
                $"未知平台代码: 0x{platform:x2}");
            var version = reader.ReadByte();
            Assert.Equal(5, version); // MonoGame/XNA 4.0 format
            reader.Close();
        }
    }
}
