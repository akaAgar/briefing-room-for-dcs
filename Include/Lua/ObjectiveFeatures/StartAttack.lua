briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].startAttack = function ()
    briefingRoom.radioManager.play("$LANG_PILOT$: $LANG_LAUNCHATTACKREQUEST$", "RadioPilotBeginYourAttack")
    local groupNames = dcsExtensions.getGroupNamesContaining("%-STGT%-$OBJECTIVENAME$")
    briefingRoom.debugPrint("Activating Attack group $OBJECTIVEINDEX$: "..#groupNames, 1)
    for _, value in pairs(groupNames) do
        local acGroup = Group.getByName(value) -- get the group
        if acGroup ~= nil then -- activate the group, if it exists
            acGroup:activate()
            local Start = {
                id = 'Start',
                params = {}
            }
            acGroup:getController():setCommand(Start)
            briefingRoom.debugPrint("Activating Attack group $OBJECTIVEINDEX$: "..value, 1)
        end
    end
    briefingRoom.radioManager.play("$LANG_TROOP$: $LANG_BEGINATTACK$", "RadioOtherPilotBeginAttack")
    missionCommands.removeItemForCoalition(briefingRoom.playerCoalition, briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].startAttackCommand)
end

briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].startAttackCommand = missionCommands.addCommandForCoalition(briefingRoom.playerCoalition, "$LANG_LAUNCHATTACK$", briefingRoom.f10Menu.objectives[$OBJECTIVEINDEX$], briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].startAttack)