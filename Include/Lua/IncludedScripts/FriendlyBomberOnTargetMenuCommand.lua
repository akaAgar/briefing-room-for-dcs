briefingRoom.mission.features.friendlyBomberOnTarget = { } -- was the bomber FG activated?

-- Activate aircraft and launch bombing run
function briefingRoom.mission.features.friendlyBomberOnTarget.beginBombingRun(index)
  briefingRoom.radioManager.play("Threats have been suppressed, you may begin your bombing run.", "RadioPilotBeginYourBombingRun")
  local bomberGroup = dcsExtensions.getGroupByID(briefingRoom.mission.featuresUnitGroups.FriendlyBomberOnTargetAndTakeOffCommand[index])
  if bomberGroup == nil then return end -- no group (destroyed? missing?), no radio answer

  briefingRoom.radioManager.play("Affirm. Taking off, on my way to bomb the target location.", "RadioOtherPilotBeginBombing", briefingRoom.radioManager.getAnswerDelay())
  bomberGroup:activate()
  missionCommands.removeItemForCoalition($PLAYERCOALITION$, briefingRoom.f10Menu.bomberCommands[index])
end

-- Create F10 menu options
briefingRoom.f10Menu.bomberCommands = {}
do
  for i,o in ipairs(briefingRoom.mission.objectives) do
    briefingRoom.f10Menu.bomberCommands[i] = missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Bomber, take off and begin bombing run", briefingRoom.f10Menu.objectives[i], briefingRoom.mission.features.friendlyBomberOnTarget.beginBombingRun, i)
  end
end

