using System.IO;

public class Vid2VidService
{
    FFMPEG ffmpeg = new FFMPEG();
    CLIService cli = new CLIService();

    public void Translate(string inputVideo, string outputVideo)
    {

        var framesOut = Path.GetDirectoryName(outputVideo);
        var PATH_TO_SEQ = "";
        var PATH_TO_REF_IMG = "";
        var PATH_TO_RESULTS = "";
        ffmpeg.ToFrames(inputVideo, framesOut);
        var command = $@"
            cd /home/scary/Sources/few-vid2vid-2 &&
            conda activate py36-3 &&
            python data/detect_landmarks.py test ${PATH_TO_SEQ} &&
            python test.py --name face --dataset_mode fewshot_face --adaptive_spade --warp_ref --spade_combine --seq_path ${PATH_TO_SEQ} --ref_img_path {PATH_TO_REF_IMG} --results_dir ${PATH_TO_RESULTS}
            ";
        cli.Bash(command);
        ffmpeg.FromFrames(framesOut, outputVideo);
    }
}