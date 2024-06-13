# MonoGame.Tool.BasisUniversal
MonoGame tooling build for basis_universal

This tool can be used to compress an image into various texture compression
formats. Examples are BC1-7, PVRTC, ETC1, ETC2, ASTC and ATC.

To use we must first create a "basis" file, this is an intermediate file containing
the image data. From this file ALL over texture formats can be created. It is probably
worth caching this "basis" file between builds.

```dotnetcli
basisu -file foo.png -ktx2
```

Adding a `-uastc` might give better results for some compression types.

Once you have the "basis" file you can then transform that into specific formats
by using the `-unpack` command. Providing the `-format_only` flag will produce
ONLY that format. Omitting this flag will result in ALL supported formats being
produced.
The values for `-format_only` are the numerical values for the [transcoder_texture_format]( https://github.com/BinomialLLC/basis_universal/blob/ad9386a4a1cf2a248f7bbd45f543a7448db15267/transcoder/basisu_transcoder.h#L49). For example ATC with an Alpha channel maps to [cTFATC_RGBA](https://github.com/BinomialLLC/basis_universal/blob/ad9386a4a1cf2a248f7bbd45f543a7448db15267/transcoder/basisu_transcoder.h#L73C3-L73C14) which has a numerical value of `17`.

```dotnetcli
basisu -unpack foo.ktx2 -no_ktx -linear -format_only 2
```

The output file(s) are listed in the output. There may be many output files. They will be stored in a .png files, but the internal image data will be compressed. We can use libraries like StbImageSharp to read the .png data.

```csharp
using (var fs = File.OpenRead(filename))
{
    ImageResult result = ImageResult.FromStream(fs, StbImageSharp.ColorComponents.Default);
    return result.Data;
}
```

If mipmap's are enabled, you will get one .png per mipmap level.
