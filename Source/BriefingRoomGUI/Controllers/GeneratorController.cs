using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BriefingRoom4DCS.Generator;
using BriefingRoom4DCS.Template;
using BriefingRoom4DCS.Miz;


namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeneratorController : ControllerBase
    {
        private readonly ILogger<GeneratorController> _logger;

        public GeneratorController(ILogger<GeneratorController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public FileContentResult Post(MissionTemplate template)
        {
            using (MissionGenerator generator = new MissionGenerator())
            {
                var mission = generator.Generate(template);
                using (MizFile miz = mission.ExportToMiz())
                {

                    if (miz == null) // Something went wrong during the .miz export
                        return null;
                    return File(miz.ZipFiles(), "application/octet-stream", $"{mission.MissionName}.miz");
                }
            }
        }
    }
}
