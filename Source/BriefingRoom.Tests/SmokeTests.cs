using BriefingRoom4DCS.Mission;

namespace BriefingRoom4DCS.Tests;

public class SmokeTests
{
    [Fact]
    async public void SingleMission()
    {
        new BriefingRoom();
        DCSMission mission = await BriefingRoom.GenerateMission($"{BriefingRoom.GetBriefingRoomRootPath()}\\Default.brt");

        Assert.NotNull(mission);
        Assert.Equal("Caucasus", mission.TheaterID);
        Assert.NotNull(mission.Briefing.Name);
        Assert.NotNull(mission.Briefing.Description);
    }

    [Fact]
    async public void Campaign()
    {
        new BriefingRoom();
        DCSCampaign campaign = await BriefingRoom.GenerateCampaign($"{BriefingRoom.GetBriefingRoomRootPath()}\\Default.cbrt");

        Assert.NotNull(campaign);
        Assert.Equal(5, campaign.MissionCount);
        Assert.NotNull(campaign.Name);


        DCSMission mission = campaign.Missions.First();
        Assert.NotNull(mission);
        Assert.Equal("Caucasus", mission.TheaterID);
        Assert.NotNull(mission.Briefing.Name);
        Assert.NotNull(mission.Briefing.Description);
    }
}