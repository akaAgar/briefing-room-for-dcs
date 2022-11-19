do
  local unit = dcsExtensions.getUnitOrStatic(briefingRoom.mission.objectives[$OBJECTIVEINDEX$].unitNames[1])
  trigger.action.radioTransmission('l10n/DEFAULT/FXRadioSignal.ogg', unit:getPoint(), 0, true, 124000000, 100, "-- Morse Code --")
end
