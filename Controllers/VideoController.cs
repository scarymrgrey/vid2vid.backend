using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoController : ControllerBase
    {
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly string[] _permittedExtensions = { ".txt", ".png", ".avi", ".mov", ".mpg", ".mp4" };
        private string fileStoragePath = "/tmp/vid2vid/";
        private readonly long _fileSizeLimit = 1024 * 1024;
        private Vid2VidService vid2VidService = new Vid2VidService();
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<VideoController> _logger;

        public VideoController(ILogger<VideoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();

            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        //[Produces("application/json")]
        //[Consumes("application/binary")]
        [HttpPost]
        [DisableFormValueModelBinding]
        public async Task<IActionResult> UploadPhysical(IFormFile video)
        {
            if (video == null)
                return BadRequest("Input cannot be empty");

            var id = Guid.NewGuid().ToString();
            var fileExt = Path.GetExtension(video.FileName) ?? ".mp4";

            var inputVideoName = "input" + fileExt;

            var rootPath = Path.Combine(fileStoragePath, id);
            var inputVideoPath = Path.Combine(rootPath, inputVideoName);
            Directory.CreateDirectory(rootPath);
            using (var stream = new FileStream(inputVideoPath, FileMode.CreateNew))
                video.CopyTo(stream);

            var outputFilePath = rootPath + "/output.mp4";
            vid2VidService.Translate(inputVideoPath,outputFilePath);

            return Created("/video/" + id, id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideo(string id)
        {
            var outputFile = Path.Combine(fileStoragePath, id, "output.mp4");
            return PhysicalFile(outputFile, "video/mp4", enableRangeProcessing: true);
        }

        private async Task<IActionResult> download(string filename)
        {
            if (filename == null)
                return Content("filename not present");

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", filename);

            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
                await stream.CopyToAsync(memory);

            memory.Position = 0;

            return File(memory, "video/mp4", Path.GetFileName(path));
        }
    }
}
