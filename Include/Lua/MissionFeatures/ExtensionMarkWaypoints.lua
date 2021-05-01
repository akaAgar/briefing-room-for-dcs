briefingRoom.ExtensionMarkWaypoints = $PLAYERWAYPOINTS$
for i,v in ipairs(briefingRoom.ExtensionMarkWaypoints) do
    trigger.action.markToAll(i, v["name"], v["location"], true, "")
end