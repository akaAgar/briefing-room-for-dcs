do
  local unit = Unit.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  if unit == nil then -- no unit found with the ID, try searching for a static
    unit = StaticObject.getByName(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  end
 
  trigger.action.radioTransmission('l10n/DEFAULT/FXRadioSignal.ogg', unit:getPoint(), 0, true, 124000000, 100, "-- Morse Code --")
end
