briefingRoom.mission.features.friendlyBomberOnTarget = { }
briefingRoom.mission.features.friendlyBomberOnTarget.activated = { } -- was the bomber FG activated?

-- Activate aircraft and launch bombing run
function briefingRoom.mission.features.friendlyBomberOnTarget.beginBombingRun(index)
  briefingRoom.radioManager.play("Threats have been suppressed, you may begin your bombing run.", "RadioPilotBeginYourBombingRun")

  local bomberGroup = dcsExtensions.getGroupByID(briefingRoom.mission.featuresUnitGroups.FriendlyBomberOnTarget[index])
  if bomberGroup == nil then return end -- no group (destroyed? missing?), no radio answer

  -- bomber already activated
  if briefingRoom.mission.features.friendlyBomberOnTarget.activated[index] then
    briefingRoom.radioManager.play("Negative, bombing mission already in progress.", "RadioOtherPilotAlreadyBombing", briefingRoom.radioManager.getAnswerDelay())
    return
  end

  briefingRoom.radioManager.play("Affirm. Taking off, on my way to bomb the target location.", "RadioOtherPilotBeginBombing", briefingRoom.radioManager.getAnswerDelay())
  bomberGroup:activate()
  briefingRoom.mission.features.friendlyBomberOnTarget.activated[index] = true
end

-- Create F10 menu options
do
  for i,o in ipairs(briefingRoom.mission.objectives) do
    missionCommands.addCommandForCoalition($PLAYERCOALITION$, "Bomber, take off and begin bombing run", briefingRoom.f10Menu.objectives[i], briefingRoom.mission.features.friendlyBomberOnTarget.beginBombingRun, i)
    table.insert(briefingRoom.mission.features.friendlyBomberOnTarget.activated, false) -- set all activated indices to false
  end
end

