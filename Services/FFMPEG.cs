using System.Diagnostics;
using System.IO;

public class FFMPEG
{
    private CLIService cli = new CLIService();
    private string ratio =  "25/1";
    public void ToFrames(string path,string output)
    {
        Directory.CreateDirectory(output);
        var cmd = $"ffmpeg -i {path} -r {ratio} {output}/$filename%03d.png";
        cli.Bash(cmd);
    }

    public void FromFrames(string inputPath,string outputPath)
    {
        var path = Path.GetDirectoryName(outputPath);
        var cmd = $"ffmpeg -r {ratio} -i {inputPath}/%03d.png -c:v libx264 -vf fps=25 -pix_fmt yuv420p {outputPath}";
        Directory.CreateDirectory(path);
        cli.Bash(cmd);
    }
}