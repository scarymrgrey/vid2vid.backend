using System.IO;

public class Vid2VidService
{
    FFMPEG ffmpeg = new FFMPEG();
    CLIService cli = new CLIService();

    string vid2vidPath = "/home/scary/Sources/few-vid2vid-2";

    public void Translate(string inputVideo, string outputVideo)
    {
        var inputPath = Path.GetDirectoryName(inputVideo);
        var PATH_TO_SEQ = Path.Combine(inputPath,"test_images","0001");
        var PATH_TO_REF_IMG = Path.Combine(inputPath,"test_images","0002");;
        var PATH_TO_RESULTS = inputPath;

        Directory.CreateDirectory(PATH_TO_REF_IMG);
        cli.Bash($"cp /home/scary/Sources/Facial-Landmarks-Detection-with-DLIB/datasets/face/test_images/0002/00000.jpg {PATH_TO_REF_IMG}/");

        ffmpeg.ToFrames(inputVideo, PATH_TO_SEQ);
        var command = $@"source /home/scary/anaconda3/etc/profile.d/conda.sh &&
                        cd {vid2vidPath} &&
                        conda activate py36-3 &&
                        python data/detect_landmarks.py test {inputPath} &&
                        python test.py --name face_256 --dataset_mode fewshot_face --adaptive_spade --warp_ref --spade_combine --seq_path {PATH_TO_SEQ} --ref_img_path {PATH_TO_REF_IMG} --results_dir {PATH_TO_RESULTS} --gpu_ids 0,1";
        cli.Bash(command);
        var framesOut = Path.Combine(PATH_TO_RESULTS,"face_256","test_latest","test_images-0_test_images","synthesized");
        ffmpeg.FromFrames(framesOut, outputVideo);
    }
}