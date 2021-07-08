using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BriefingRoom4DCS;
using BriefingRoom4DCS.Mission;
using BriefingRoom4DCS.Template;

namespace BriefingRoom4DCS.GUI.Web.API.Controllers
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
            using (BriefingRoom briefingRoom = new BriefingRoom())
            {
                DCSMission mission = briefingRoom.GenerateMission(template);
                byte[] mizBytes = mission.SaveToMizBytes();
                if (mizBytes == null) return null; // Something went wrong during the .miz export
                return File(mizBytes, "application/octet-stream", $"{mission.Briefing.Name}.miz");
            }
        }
    }
}
