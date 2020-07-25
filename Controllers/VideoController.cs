using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Utilities;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoController : ControllerBase
    {
        private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly string[] _permittedExtensions = { ".txt", ".png", ".avi", ".mov", ".mpg" };
        private readonly long _fileSizeLimit = 1024 * 1024;
        private FFMPEG ffmpeg = new FFMPEG();
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

        [HttpPost]
        [DisableFormValueModelBinding]
        public IActionResult UploadPhysical(IFormFile video)
        {
            if (video != null)
            {
                var id = Guid.NewGuid().ToString();
                //Set Key Name
                string ImageName = id + Path.GetExtension(video.FileName);

                //Get url To Save
                string savePath = Path.Combine("/Users/kpolunin/trash/", ImageName);
                string framesOut = Path.Combine("/Users/kpolunin/trash/", id);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    video.CopyTo(stream);
                }
                ffmpeg.ToFrames(savePath, framesOut);
                ffmpeg.FromFrames(framesOut,framesOut);
            }

            return Created(nameof(VideoController), "OK");
        }
    }
}
