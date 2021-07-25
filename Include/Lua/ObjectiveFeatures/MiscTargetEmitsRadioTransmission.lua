do
  local unit = dcsExtensions.getUnitByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = dcsExtensions.getStaticByID(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitsID[1])
  end
 
  trigger.action.radioTransmission('l10n/DEFAULT/FXRadioSignal.ogg', unit:getPoint(), 0, true, 124000000, 100)
end
