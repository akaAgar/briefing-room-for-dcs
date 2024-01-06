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
    local idx = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].startAttackCommandIndex
    missionCommands.removeItemForCoalition(briefingRoom.playerCoalition, briefingRoom.mission.objectives[$OBJECTIVEINDEX$].f10Commands[idx].commandPath)
end

table.insert(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].f10Commands, {text = "$LANG_LAUNCHATTACK$", func = briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].startAttack, args =  nil})
briefingRoom.mission.objectiveFeatures[$OBJECTIVEINDEX$].startAttackCommandIndex = #briefingRoom.mission.objectives[$OBJECTIVEINDEX$].f10Commands