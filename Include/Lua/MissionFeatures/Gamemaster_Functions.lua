--Gamemaster_Functions v. 2.4, created by Cake, CTLD-Functions contributed by fargo007

GMFunc = {} -- Never delete this!!!

--################################################################Start of Config-Section################################################################

--General options
GMFunc.DebugMode = false --Toggles the display of status messages, only needed for debugging
GMFunc.CmdSymbol = "-" --The symbol or string the script looks for to separate the marker text into the command and the different parameters. You can change this to any string or symbol. DO NOT remove the quotation marks! 
GMFunc.RestrToCoal = nil --restricts the usage of the commands to one coalition only, enter 0 to restrict to neutral, 1 to restrict to red, 2 to restrict to blue, nil allows players from all coalitions to use the commands
GMFunc.PW = nil --Here you can specify a password that has to be entered into the marker before any commands in the following text are recognized. PW needs to be put in quotation marks. Set to nil to disable! 

--Spawn command options
GMFunc.DefaultSkill = "Random" --Default skill of units spawned via the "-s" command, possible values are: Random, Excellent, High, Good, Average. Does NOT override the skill set by the parameter of the "-s" command.
GMFunc.DefaultCountry = nil --All units spawned via the "-s" and "-sta" command will be spawned as units of the country specified here. For countrynames see documentation. Does NOT override the country set by the parameter of the "-s" command.
                            --Nil defaults to USAF aggressors for all spawns that are included in Gamemaster_Templates. For all other spawns nil defaults to the country set up in the ME. 
GMFunc.EPLRS = true --Toggles EPLRS (Datalink) for Groups spawned via the "-s" command, true = on, false = off. Gets overriden by the "keep tasking" parameter
GMFunc.DefaultROE = "free" --Set a default ROE that all Groups spawned via the "-s" command will obey: "free" = weapons free, "hold" = hold fire, "return" = return fire

--Text command options
GMFunc.MsgDispTime = 30 --Display time for messages sent through the "-text" and "-list" commands 
GMFunc.MsgBorderL = ">>>> " --Enter any collection of letters, numbers and/or signs to precede (BorderL) and/or follow (BorderR) Messages sent by the "-text" command. Makes it look nice ;)
GMFunc.MsgBorderR = " <<<<"
GMFunc.MsgSound = nil --Sound that plays when a message is sent through the "-text" command, needs to be loaded into the mission at mission start with a trigger using one of the "Sound To .." actions. 
                      --Enter the exact name of the soundfile in the following format: "Filename.ending"! Leave at nil if no sound is meant to be played.

--Draw command options
GMFunc.ColorPreset = { Red = {1,0,0}, --Adjust RGB values of the default colors for map drawings here, or use Custom1,2,3 to add colours you feel are missing
                       Blue = {0,0,1},
                       Green = {0,1,0},
                       Black = {0,0,0},
                       White = {1,1,1},
                       Yellow = {1,0.8,0},
                       Orange = {1,0.65,0},
                       Purple = {0.41,0.13,0.55},
                       Custom1 = {0,0,0},
                       Custom2 = {0,0,0},
                       Custom3 = {0,0,0} }
                      
GMFunc.ShapeColor = "Blue" --Default outline color of all shapes drawn onto the F10 map, all colors defined in GMFunc.ColorPreset can be entered. Don't remove the quotation marks!
GMFunc.ShapeFillColor = "Blue" --Default fill color of all shapes drawn onto the F10 map, all colors defined in GMFunc.ColorPreset can be entered. Don't remove the quotation marks!

GMFunc.TextColor =  "Black" --Default text color of all text boxes drawn onto the F10 map, all colors defined in GMFunc.ColorPreset can be entered. Don't remove the quotation marks!
GMFunc.TextShapeColor =  "White" --Default background color of all text boxes drawn onto the F10 map, all colors defined in GMFunc.ColorPreset can be entered. Don't remove the quotation marks!

GMFunc.ShapeAlpha = 0.7 --Transparency setting for the outline of shapes drawn on the F10 map, values can range from 0 to 1 (0 = full transparency)
GMFunc.TextAlpha = 1 --Transparency setting for text drawn on the F10 map, values can range from 0 to 1 (0 = full transparency)

GMFunc.ShapeFillAlpha = 0.15 --Transparency setting for the fill area of shapes drawn on the F10 map, values can range from 0 to 1 (0 = full transparency)
GMFunc.TextFillAlpha = 0.15 --Transparency setting for the background of text boxes drawn on the F10 map, values can range from 0 to 1 (0 = full transparency)

GMFunc.LineType = 1 --Default line type used to outline shapes drawn on the F10 map, 0=No line, 1=Solid, 2=Dashed, 3=Dotted, 4=Dot dash, 5=Long dash, 6=Double dash

GMFunc.TextOffsetLine = 150 --Offset in meters from start point, end point or middle point for textfields drawn as labels by -drawline and -drawarrow. For end and start points the offset 
                            --gets applied in line with the orientation of the line/arrow, for middle points the offset is applied in the direction that has been specified by parameter
                          
GMFunc.TextOffsetShape = { N = 0,     --North offset in meters from center of shape for textfields drawn as labels by -drawpoly, -drawrect and -drawcircle, negative values move the text to south
                           E = -150 } --East offset in meters from shape center, negative values move the text to west

GMFunc.FontSize = 14 --Default font size of all text fields drawn onto the F10 map

--################################################################End of Config, don't change anything below this line, unless you know what you're doing!!!!################################################################

_SETTINGS:SetPlayerMenuOff()
    
--Init Variables----------------------------------------------------------------------------------------------------------------------------------------

GMFunc.lastPolyID = 1000
GMFunc.deleteZoneNum = 0
GMFunc.queryZoneNum = 0
GMFunc.Spawns = {}

if GMFunc.DefaultROE ~= nil

  then if GMFunc.DefaultROE == "free" then GMFunc.DefaultROE = ENUMS.ROE.WeaponFree
       elseif GMFunc.DefaultROE == "hold" then GMFunc.DefaultROE = ENUMS.ROE.WeaponHold
       elseif GMFunc.DefaultROE == "return" then GMFunc.DefaultROE = ENUMS.ROE.ReturnFire
       end

end

--Init Service-Functions, these are only used as utilies by other GMFunctions and are not supposed to be called by map markers

function GMFunc.getTACANFrequency(channel, channelMode) --lifted from ED file "BeaconTypes.lua"
  local A = 1151 -- 'X', channel >= 64
  local B = 64   -- channel >= 64
  
  if channel < 64 then
    B = 1
  end
  
  if channelMode == "Y" then
    A = 1025
    if channel < 64 then
      A = 1088
    end
  else -- 'X'
    if channel < 64 then
      A = 962
    end
  end
  
  return (A + channel - B) * 1000000
end

function GMFunc.ShuffleFillColor(fillColor)

  local newFillColor = {}
            
        if fillColor[1] - 0.1 <= 0 
            
         then newFillColor[1] = fillColor[1] + 0.1
                   
        else newFillColor[1] = fillColor[1] - 0.1
                  
        end
                 
        if fillColor[2] - 0.1 <= 0 
            
          then newFillColor[2] = fillColor[2] + 0.1
                   
        else newFillColor[2] = fillColor[2] - 0.1
                  
        end
                 
        if fillColor[3] - 0.1 <= 0 
            
          then newFillColor[3] = fillColor[3] + 0.1
                   
        else newFillColor[3] = fillColor[3] - 0.1
                  
        end

return newFillColor

end

--Init DrawText-Function

function GMFunc.DrawText(param1, param2, param3, param4, param5, param6, param7, mcoord)
  
  local paramTbl = { param2 = param2,
                     param3 = param3,
                     param4 = param4,
                     param5 = param5,
                     param6 = param6,
                     param7 = param7 }
  
  local coal = -1
  
  local color = GMFunc.ColorPreset[GMFunc.TextColor]
  local fillColor = GMFunc.ColorPreset[GMFunc.TextShapeColor]
  
  local alpha = GMFunc.TextAlpha
  local fillAlpha = GMFunc.TextFillAlpha
  
  local shapeText = nil
  local fontSize = GMFunc.FontSize
  
  shapeText = param1
    
  for i, param in pairs(paramTbl)
  
    do if param == "red" then coal = 1
       elseif param == "blue" then coal = 2
       elseif param == "neutral" then coal = 0
       end
    
       if param == "r" then color = GMFunc.ColorPreset.Red
       elseif param == "b" then color = GMFunc.ColorPreset.Blue
       elseif param == "g" then color = GMFunc.ColorPreset.Green
       elseif param == "bl" then color = GMFunc.ColorPreset.Black
       elseif param == "w" then color = GMFunc.ColorPreset.White
       elseif param == "y" then color = GMFunc.ColorPreset.Yellow
       elseif param == "o" then color = GMFunc.ColorPreset.Orange
       elseif param == "p" then color = GMFunc.ColorPreset.Purple
       elseif param == "c1" then color = GMFunc.ColorPreset.Custom1
       elseif param == "c2" then color = GMFunc.ColorPreset.Custom2
       elseif param == "c3" then color = GMFunc.ColorPreset.Custom2
       end
    
       if tonumber(param) 
      
        then alpha = tonumber(param)
      
       end
    
       if param == "fr" then fillColor = GMFunc.ColorPreset.Red
       elseif param == "fb" then fillColor = GMFunc.ColorPreset.Blue
       elseif param == "fg" then fillColor = GMFunc.ColorPreset.Green
       elseif param == "fbl" then fillColor = GMFunc.ColorPreset.Black
       elseif param == "fw" then fillColor = GMFunc.ColorPreset.White
       elseif param == "fy" then fillColor = GMFunc.ColorPreset.Yellow
       elseif param == "fo" then fillColor = GMFunc.ColorPreset.Orange
       elseif param == "fp" then fillColor = GMFunc.ColorPreset.Purple
       elseif param == "fc1" then fillColor = GMFunc.ColorPreset.Custom1
       elseif param == "fc2" then fillColor = GMFunc.ColorPreset.Custom2
       elseif param == "fc3" then fillColor = GMFunc.ColorPreset.Custom2
       end
    
       if param ~= nil and string.find(param, "f") and tonumber(string.sub(param, 2)) 
    
        then fillAlpha = tonumber(string.sub(param, 2))
   
       end
  
       if param ~= nil and string.find(param2, "t") and tonumber(string.sub(param2, 2)) 
  
        then fontSize = tonumber(string.sub(param2, 2))
  
       end
  
  end
  
  if fillAlpha == 0
    
     then if color[1] == fillColor[1]
     
            then local newFillColor = {}
            
                 if fillColor[1] - 0.1 <= 0 
            
                   then newFillColor[1] = fillColor[1] + 0.1
                   
                  else newFillColor[1] = fillColor[1] - 0.1
                  
                 end
                 
                 if fillColor[2] - 0.1 <= 0 
            
                   then newFillColor[2] = fillColor[2] + 0.1
                   
                  else newFillColor[2] = fillColor[2] - 0.1
                  
                 end
                 
                 if fillColor[3] - 0.1 <= 0 
            
                   then newFillColor[3] = fillColor[3] + 0.1
                   
                  else newFillColor[3] = fillColor[3] - 0.1
                  
                 end
                 
                 fillColor = newFillColor
                 
          end
          
    end
    
  mcoord:TextToAll(shapeText, coal, color, alpha, fillColor, fillAlpha, fontSize, false)   

end

--Init DrawArrow-Function

function GMFunc.DrawArrow(param1, param2, param3, param4, param5, param6, param7, mcoord)

  local endPoint = {}
  local corners = {}
  
  local paramTbl = { param1 = param1,
                     param2 = param2,
                     param3 = param3,
                     param4 = param4,
                     param5 = param5,
                     param6 = param6,
                     param7 = param7 }
  
  local allMarks = world.getMarkPanels()

  local coal = -1
  
  local color = GMFunc.ColorPreset[GMFunc.ShapeColor]
  local fillColor = GMFunc.ColorPreset[GMFunc.ShapeFillColor]
  
  local alpha = GMFunc.ShapeAlpha
  local fillAlpha = GMFunc.ShapeFillAlpha
  
  local lineType = GMFunc.LineType
  
  local shapeText = nil
  local fontSize = GMFunc.FontSize
  local textPos = {}
  local vector3 = {}
  local osetBearing = nil
  
  for i, mark in pairs(allMarks)
  
    do if string.find(mark.text, "c") and tonumber(string.sub(mark.text, 2))
    
        then local pointInd = tonumber(string.sub(mark.text, 2))
             
             corners[pointInd] = COORDINATE:New(mark.pos.x, mark.pos.y, mark.pos.z)
       
       elseif mark.text == "end"
       
        then endPoint = COORDINATE:New(mark.pos.x, mark.pos.y, mark.pos.z)
             
       end
       
  end
    
  for i, param in pairs(paramTbl)
  
    do if param == "red" then coal = 1
       elseif param == "blue" then coal = 2
       elseif param == "neutral" then coal = 0
       end
    
       if param == "r" then color = GMFunc.ColorPreset.Red
       elseif param == "b" then color = GMFunc.ColorPreset.Blue
       elseif param == "g" then color = GMFunc.ColorPreset.Green
       elseif param == "bl" then color = GMFunc.ColorPreset.Black
       elseif param == "w" then color = GMFunc.ColorPreset.White
       elseif param == "y" then color = GMFunc.ColorPreset.Yellow
       elseif param == "o" then color = GMFunc.ColorPreset.Orange
       elseif param == "p" then color = GMFunc.ColorPreset.Purple
       elseif param == "c1" then color = GMFunc.ColorPreset.Custom1
       elseif param == "c2" then color = GMFunc.ColorPreset.Custom2
       elseif param == "c3" then color = GMFunc.ColorPreset.Custom2
       end
    
       if tonumber(param) 
      
        then alpha = tonumber(param)
      
       end
    
       if param == "fr" then fillColor = GMFunc.ColorPreset.Red
       elseif param == "fb" then fillColor = GMFunc.ColorPreset.Blue
       elseif param == "fg" then fillColor = GMFunc.ColorPreset.Green
       elseif param == "fbl" then fillColor = GMFunc.ColorPreset.Black
       elseif param == "fw" then fillColor = GMFunc.ColorPreset.White
       elseif param == "fy" then fillColor = GMFunc.ColorPreset.Yellow
       elseif param == "fo" then fillColor = GMFunc.ColorPreset.Orange
       elseif param == "fp" then fillColor = GMFunc.ColorPreset.Purple
       elseif param == "fc1" then fillColor = GMFunc.ColorPreset.Custom1
       elseif param == "fc2" then fillColor = GMFunc.ColorPreset.Custom2
       elseif param == "fc3" then fillColor = GMFunc.ColorPreset.Custom2
       end
    
       if param ~= nil and string.find(param, "f") and tonumber(string.sub(param, 2)) 
    
        then fillAlpha = tonumber(string.sub(param, 2))
   
       end
    
       if param == "n" then lineType = 0
       elseif param == "s" then lineType = 1
       elseif param == "d" then lineType = 2
       elseif param == "dot" then lineType = 3
       elseif param == "dd" then lineType = 4
       elseif param == "ld" then lineType = 5
       elseif param == "2d" then lineType = 6
       end
    
       if string.sub(param, 1, 4) == "beg "
    
        then if #corners == 0 
        
              then vector3 = endPoint:GetDirectionVec3(mcoord)
                   osetBearing = endPoint:GetAngleDegrees(vector3)
                   textPos = mcoord:Translate(GMFunc.TextOffsetLine, osetBearing, false, false)
                   shapeText = string.sub(param, 5)
                   
             elseif #corners > 0
             
              then vector3 = corners[1]:GetDirectionVec3(mcoord)
                   osetBearing = corners[1]:GetAngleDegrees(vector3)
                   textPos = mcoord:Translate(GMFunc.TextOffsetLine, osetBearing, false, false)
                   shapeText = string.sub(param, 5)
                   
             end
             
       elseif string.sub(param, 1, 4) == "end "
       
        then if #corners == 0
        
              then vector3 = mcoord:GetDirectionVec3(endPoint)
                   osetBearing = mcoord:GetAngleDegrees(vector3)
                   textPos = endPoint:Translate(GMFunc.TextOffsetLine, osetBearing, false, false)
                   shapeText = string.sub(param, 5)
                   
             elseif #corners > 0
             
              then vector3 = corners[#corners]:GetDirectionVec3(endPoint)
                   osetBearing = corners[#corners]:GetAngleDegrees(vector3)
                   textPos = endPoint:Translate(GMFunc.TextOffsetLine, osetBearing, false, false)
                   shapeText = string.sub(param, 5)
                   
             end
             
       elseif string.sub(param, 1, 2) == "ct"
       
        then textPos = { x = mcoord.x + (endPoint.x - mcoord.x)/2, z = mcoord.z + (endPoint.z - mcoord.z)/2 }
             textPos.y = land.getHeight({textPos.x, textPos.z})
             textPos = COORDINATE:New(textPos.x, textPos.y, textPos.z)
             shapeText = string.sub(param, 5)
            
             if string.sub(param, 1, 4) == "ctn " then textPos:Translate(GMFunc.TextOffsetLine, 0, false, true)
             elseif string.sub(param, 1, 4) == "cts " then textPos:Translate(GMFunc.TextOffsetLine, 180, false, true)
             elseif string.sub(param, 1, 4) == "cte " then textPos:Translate(GMFunc.TextOffsetLine, 90, false, true)
             elseif string.sub(param, 1, 4) == "ctw " then textPos:Translate(GMFunc.TextOffsetLine, 270, false, true)
             end
            
       end
       
  end     
    
  if fillAlpha == 0
    
   then if color[1] == fillColor[1] or color[2] == fillColor[2] or color[3] == fillColor[3]
     
          then fillColor = GMFunc.ShuffleFillColor(fillColor)
                 
        end
          
  end
  
  if #corners > 0 
  
    then for i = 1, #corners, 1
  
          do if corners[i] ~= nil
          
              then local nextIndex = i + 1
                   
                   if i == 1
                   
                    then mcoord:ArrowToAll(corners[i], coal, color, alpha, fillColor, fillAlpha, lineType, false)
                    
                         if corners[nextIndex] ~= nil
                         
                          then corners[i]:ArrowToAll(corners[nextIndex], coal, color, alpha, fillColor, fillAlpha, lineType, false)
                          
                         else corners[i]:ArrowToAll(endPoint, coal, color, alpha, fillColor, fillAlpha, lineType, false)
                         
                         end
                   
                   elseif corners[nextIndex] ~= nil 
        
                    then corners[i]:ArrowToAll(corners[nextIndex], coal, color, alpha, fillColor, fillAlpha, lineType, false)
              
                   else corners[i]:ArrowToAll(endPoint, coal, color, alpha, fillColor, fillAlpha, lineType, false)
              
                   end
            
             end
             
         end
              
  elseif #corners == 0
      
    then mcoord:ArrowToAll(endPoint, coal, color, alpha, fillColor, fillAlpha, lineType, false)
     
  end
  
  if GMFunc.DebugMode == true
      
    then trigger.action.outText("Shape gezeichnet, coal = " .. coal .. " color = " .. color[1] .. "," .. color[2] .. "," .. color[3] .. " fillColor = " .. fillColor[1] .. "," .. fillColor[2] .. "," .. fillColor[3] .. " alpha = " .. alpha .. " fillAlpha = " .. fillAlpha .. " lineType = " .. lineType, 60)
      
  end
    
  if shapeText ~= nil
    
   then if color[1] == fillColor[1] or color[2] == fillColor[2] or color[3] == fillColor[3]
     
          then fillColor = GMFunc.ShuffleFillColor(fillColor)
                 
        end
           
        textPos:TextToAll(shapeText, coal, color, alpha, fillColor, 0, GMFunc.FontSize, false)
           
        if GMFunc.DebugMode == true
      
          then trigger.action.outText("Textbox gezeichnet, x = " .. textPos.x .. " z = " .. textPos.z .. " y = " .. textPos.y .. " coal = " .. coal .. " alpha = " .. alpha .. " fillAlpha = " .. fillAlpha .. " shapeText = " .. shapeText, 60)
      
        end    

  end

end

--Init DrawLine-Function

function GMFunc.DrawLine(param1, param2, param3, param4, param5, mcoord)

  local paramTbl = { param1 = param1,
                     param2 = param2,
                     param3 = param3,
                     param4 = param4,
                     param5 = param5 }
  
  local endPoint = {}
  local corners = {}
  
  local allMarks = world.getMarkPanels()
  
  local coal = -1
  
  local color = GMFunc.ColorPreset[GMFunc.ShapeColor]
  local fillColor = GMFunc.ColorPreset[GMFunc.ShapeFillColor]
  
  local alpha = GMFunc.ShapeAlpha
  local fillAlpha = GMFunc.ShapeFillAlpha
  
  local lineType = GMFunc.LineType
  
  local shapeText = nil
  local fontSize = GMFunc.FontSize
  local textPos = {}
  local vector3 = {}
  local osetBearing = nil
  
  for i, mark in pairs(allMarks)
  
    do if string.find(mark.text, "c") and tonumber(string.sub(mark.text, 2))
    
        then local pointInd = tonumber(string.sub(mark.text, 2))
             
             corners[pointInd] = COORDINATE:New(mark.pos.x, mark.pos.y, mark.pos.z)
       
       elseif mark.text == "end"
       
        then endPoint = COORDINATE:New(mark.pos.x, mark.pos.y, mark.pos.z)
             
       end
       
  end
    
  for i, param in pairs(paramTbl)
  
    do if param == "red" then coal = 1
       elseif param == "blue" then coal = 2
       elseif param == "neutral" then coal = 0
       end
    
       if param == "r" then color = GMFunc.ColorPreset.Red
       elseif param == "b" then color = GMFunc.ColorPreset.Blue
       elseif param == "g" then color = GMFunc.ColorPreset.Green
       elseif param == "bl" then color = GMFunc.ColorPreset.Black
       elseif param == "w" then color = GMFunc.ColorPreset.White
       elseif param == "y" then color = GMFunc.ColorPreset.Yellow
       elseif param == "o" then color = GMFunc.ColorPreset.Orange
       elseif param == "p" then color = GMFunc.ColorPreset.Purple
       elseif param == "c1" then color = GMFunc.ColorPreset.Custom1
       elseif param == "c2" then color = GMFunc.ColorPreset.Custom2
       elseif param == "c3" then color = GMFunc.ColorPreset.Custom2
       end
    
       if tonumber(param) 
      
        then alpha = tonumber(param)
      
       end
    
       if param == "n" then lineType = 0
       elseif param == "s" then lineType = 1
       elseif param == "d" then lineType = 2
       elseif param == "dot" then lineType = 3
       elseif param == "dd" then lineType = 4
       elseif param == "ld" then lineType = 5
       elseif param == "2d" then lineType = 6
       end
    
       if string.sub(param, 1, 4) == "beg "
    
        then if #corners == 0 
        
              then vector3 = endPoint:GetDirectionVec3(mcoord)
                   osetBearing = endPoint:GetAngleDegrees(vector3)
                   textPos = mcoord:Translate(GMFunc.TextOffsetLine, osetBearing, false, false)
                   shapeText = string.sub(param, 5)
                   
             elseif #corners > 0
             
              then vector3 = corners[1]:GetDirectionVec3(mcoord)
                   osetBearing = corners[1]:GetAngleDegrees(vector3)
                   textPos = mcoord:Translate(GMFunc.TextOffsetLine, osetBearing, false, false)
                   shapeText = string.sub(param, 5)
                   
             end 
             
       elseif string.sub(param, 1, 4) == "end "
       
        then if #corners == 0
        
              then vector3 = mcoord:GetDirectionVec3(endPoint)
                   osetBearing = mcoord:GetAngleDegrees(vector3)
                   textPos = endPoint:Translate(GMFunc.TextOffsetLine, osetBearing, false, false)
                   shapeText = string.sub(param, 5)
                   
             elseif #corners > 0
             
              then vector3 = corners[#corners]:GetDirectionVec3(endPoint)
                   osetBearing = corners[#corners]:GetAngleDegrees(vector3)
                   textPos = endPoint:Translate(GMFunc.TextOffsetLine, osetBearing, false, false)
                   shapeText = string.sub(param, 5)
                   
             end
             
       elseif string.sub(param, 1, 2) == "ct"
       
        then textPos = { x = mcoord.x + (endPoint.x - mcoord.x)/2, z = mcoord.z + (endPoint.z - mcoord.z)/2 }
             textPos.y = land.getHeight({textPos.x, textPos.z})
             textPos = COORDINATE:New(textPos.x, textPos.y, textPos.z)
             shapeText = string.sub(param, 5)
            
             if string.sub(param, 1, 4) == "ctn " then textPos:Translate(GMFunc.TextOffsetLine, 0, false, true)
             elseif string.sub(param, 1, 4) == "cts " then textPos:Translate(GMFunc.TextOffsetLine, 180, false, true)
             elseif string.sub(param, 1, 4) == "cte " then textPos:Translate(GMFunc.TextOffsetLine, 90, false, true)
             elseif string.sub(param, 1, 4) == "ctw " then textPos:Translate(GMFunc.TextOffsetLine, 270, false, true)
             end
            
       end
       
  end 
  
  if #corners > 0 
  
    then for i = 1, #corners, 1
  
          do if corners[i] ~= nil
          
              then local nextIndex = i + 1
                   
                   if i == 1
                   
                    then mcoord:LineToAll(corners[i], coal, color, alpha, lineType, false)
                    
                         if corners[nextIndex] ~= nil
                         
                          then corners[i]:LineToAll(corners[nextIndex], coal, color, alpha, lineType, false)
                          
                         else corners[i]:LineToAll(endPoint, coal, color, alpha, lineType, false)
                         
                         end
                   
                   elseif corners[nextIndex] ~= nil 
        
                    then corners[i]:LineToAll(corners[nextIndex], coal, color, alpha, lineType, false)
              
                   else corners[i]:LineToAll(endPoint, coal, color, alpha, lineType, false)
              
                   end
            
             end
             
         end
              
  elseif #corners == 0
      
    then mcoord:LineToAll(endPoint, coal, color, alpha, lineType, false)
     
  end
    
  if GMFunc.DebugMode == true
      
    then trigger.action.outText("Shape gezeichnet, coal = " .. coal .. " color = " .. color[1] .. "," .. color[2] .. "," .. color[3] .. " fillColor = " .. fillColor[1] .. "," .. fillColor[2] .. "," .. fillColor[3] .. " alpha = " .. alpha .. " fillAlpha = " .. fillAlpha .. " lineType = " .. lineType, 60)
      
  end
    
  if shapeText ~= nil
    
   then if color[1] == fillColor[1] or color[2] == fillColor[2] or color[3] == fillColor[3]
     
          then fillColor = GMFunc.ShuffleFillColor(fillColor)
                 
        end
           
        textPos:TextToAll(shapeText, coal, color, alpha, fillColor, 0, GMFunc.FontSize, false)
           
        if GMFunc.DebugMode == true
      
          then trigger.action.outText("Textbox gezeichnet, x = " .. textPos.x .. " z = " .. textPos.z .. " y = " .. textPos.y .. " coal = " .. coal .. " alpha = " .. alpha .. " fillAlpha = " .. fillAlpha .. " shapeText = " .. shapeText, 60)
      
        end    

  end

end

--Init DrawCircle-Function

function GMFunc.DrawCircle(param1, param2, param3, param4, param5, param6, param7, mcoord)

  local paramTbl = { param1 = param1,
                     param2 = param2,
                     param3 = param3,
                     param4 = param4,
                     param5 = param5,
                     param6 = param6,
                     param7 = param7 }
  
  local radius = nil
  
  local allMarks = world.getMarkPanels()
  
  local coal = -1
  
  local color = GMFunc.ColorPreset[GMFunc.ShapeColor]
  local fillColor = GMFunc.ColorPreset[GMFunc.ShapeFillColor]
  
  local alpha = GMFunc.ShapeAlpha
  local fillAlpha = GMFunc.ShapeFillAlpha
  
  local lineType = GMFunc.LineType
  
  local shapeText = nil
  local fontSize = GMFunc.FontSize
  
  for i, mark in pairs(allMarks)
  
    do if mark.text == "rad"
    
        then local radPoint = COORDINATE:New(mark.pos.x, mark.pos.y, mark.pos.z)
             
             radius = radPoint:Get2DDistance(mcoord)
        
       end
       
  end
 
  for i, param in pairs(paramTbl)
  
    do if param == "red" then coal = 1
       elseif param == "blue" then coal = 2
       elseif param == "neutral" then coal = 0
       end
    
       if param == "r" then color = GMFunc.ColorPreset.Red
       elseif param == "b" then color = GMFunc.ColorPreset.Blue
       elseif param == "g" then color = GMFunc.ColorPreset.Green
       elseif param == "bl" then color = GMFunc.ColorPreset.Black
       elseif param == "w" then color = GMFunc.ColorPreset.White
       elseif param == "y" then color = GMFunc.ColorPreset.Yellow
       elseif param == "o" then color = GMFunc.ColorPreset.Orange
       elseif param == "p" then color = GMFunc.ColorPreset.Purple
       elseif param == "c1" then color = GMFunc.ColorPreset.Custom1
       elseif param == "c2" then color = GMFunc.ColorPreset.Custom2
       elseif param == "c3" then color = GMFunc.ColorPreset.Custom2
       end
    
       if tonumber(param) 
      
        then alpha = tonumber(param)
      
       end
       
       if param == "fr" then fillColor = GMFunc.ColorPreset.Red
       elseif param == "fb" then fillColor = GMFunc.ColorPreset.Blue
       elseif param == "fg" then fillColor = GMFunc.ColorPreset.Green
       elseif param == "fbl" then fillColor = GMFunc.ColorPreset.Black
       elseif param == "fw" then fillColor = GMFunc.ColorPreset.White
       elseif param == "fy" then fillColor = GMFunc.ColorPreset.Yellow
       elseif param == "fo" then fillColor = GMFunc.ColorPreset.Orange
       elseif param == "fp" then fillColor = GMFunc.ColorPreset.Purple
       elseif param == "fc1" then fillColor = GMFunc.ColorPreset.Custom1
       elseif param == "fc2" then fillColor = GMFunc.ColorPreset.Custom2
       elseif param == "fc3" then fillColor = GMFunc.ColorPreset.Custom2
       end
    
       if param ~= nil and string.find(param, "f") and tonumber(string.sub(param, 2)) 
    
        then fillAlpha = tonumber(string.sub(param, 2))
   
       end
       
       if param == "n" then lineType = 0
       elseif param == "s" then lineType = 1
       elseif param == "d" then lineType = 2
       elseif param == "dot" then lineType = 3
       elseif param == "dd" then lineType = 4
       elseif param == "ld" then lineType = 5
       elseif param == "2d" then lineType = 6
       end
    
       if param ~= nil and string.find(param, "t ") 
  
        then shapeText = string.sub(param, 3)

       end
       
  end
    
  if fillAlpha == 0
    
   then if color[1] == fillColor[1] or color[2] == fillColor[2] or color[3] == fillColor[3]
     
          then fillColor = GMFunc.ShuffleFillColor(fillColor)
                 
        end
          
  end
  
  mcoord:CircleToAll(radius, coal, color, alpha, fillColor, fillAlpha, lineType, false)
    
  if GMFunc.DebugMode == true
      
    then trigger.action.outText("Shape gezeichnet, coal = " .. coal .. " color = " .. color[1] .. "," .. color[2] .. "," .. color[3] .. " fillColor = " .. fillColor[1] .. "," .. fillColor[2] .. "," .. fillColor[3] .. " alpha = " .. alpha .. " fillAlpha = " .. fillAlpha .. " lineType = " .. lineType, 60)
      
  end
    
  if shapeText ~= nil
    
   then if color[1] == fillColor[1] or color[2] == fillColor[2] or color[3] == fillColor[3]
     
          then fillColor = GMFunc.ShuffleFillColor(fillColor)
                 
        end
        
        mcoord.x = mcoord.x + GMFunc.TextOffsetShape.N
        mcoord.z = mcoord.z + GMFunc.TextOffsetShape.E
           
        mcoord:TextToAll(shapeText, coal, color, alpha, fillColor, 0, GMFunc.FontSize, false)
           
        if GMFunc.DebugMode == true
      
          then trigger.action.outText("Textbox gezeichnet, x = " .. mcoord.x .. " z = " .. mcoord.z .. " y = " .. mcoord.y .. " coal = " .. coal .. " alpha = " .. alpha .. " fillAlpha = " .. fillAlpha .. " shapeText = " .. shapeText, 60)
      
        end    

  end

end

--Init Rect-Function

function GMFunc.DrawRect(param1, param2, param3, param4, param5, param6, param7, mcoord)

  local paramTbl = { param1 = param1,
                     param2 = param2,
                     param3 = param3,
                     param4 = param4,
                     param5 = param5,
                     param6 = param6,
                     param7 = param7 }

  local opCorner = {}
  
  local allMarks = world.getMarkPanels()
  
  local coal = -1
  
  local color = GMFunc.ColorPreset[GMFunc.ShapeColor]
  local fillColor = GMFunc.ColorPreset[GMFunc.ShapeFillColor]
  
  local alpha = GMFunc.ShapeAlpha
  local fillAlpha = GMFunc.ShapeFillAlpha
  
  local lineType = GMFunc.LineType
  
  local shapeText = nil
  local fontSize = GMFunc.FontSize
  
  for i, mark in pairs(allMarks)
  
    do if mark.text == "c"
    
        then opCorner = COORDINATE:New(mark.pos.x, mark.pos.y, mark.pos.z)
        
       end
       
  end
 
  for i, param in pairs(paramTbl)
  
    do if param == "red" then coal = 1
       elseif param == "blue" then coal = 2
       elseif param == "neutral" then coal = 0
       end
    
       if param == "r" then color = GMFunc.ColorPreset.Red
       elseif param == "b" then color = GMFunc.ColorPreset.Blue
       elseif param == "g" then color = GMFunc.ColorPreset.Green
       elseif param == "bl" then color = GMFunc.ColorPreset.Black
       elseif param == "w" then color = GMFunc.ColorPreset.White
       elseif param == "y" then color = GMFunc.ColorPreset.Yellow
       elseif param == "o" then color = GMFunc.ColorPreset.Orange
       elseif param == "p" then color = GMFunc.ColorPreset.Purple
       elseif param == "c1" then color = GMFunc.ColorPreset.Custom1
       elseif param == "c2" then color = GMFunc.ColorPreset.Custom2
       elseif param == "c3" then color = GMFunc.ColorPreset.Custom2
       end
    
       if tonumber(param) 
      
        then alpha = tonumber(param)
      
       end
       
       if param == "fr" then fillColor = GMFunc.ColorPreset.Red
       elseif param == "fb" then fillColor = GMFunc.ColorPreset.Blue
       elseif param == "fg" then fillColor = GMFunc.ColorPreset.Green
       elseif param == "fbl" then fillColor = GMFunc.ColorPreset.Black
       elseif param == "fw" then fillColor = GMFunc.ColorPreset.White
       elseif param == "fy" then fillColor = GMFunc.ColorPreset.Yellow
       elseif param == "fo" then fillColor = GMFunc.ColorPreset.Orange
       elseif param == "fp" then fillColor = GMFunc.ColorPreset.Purple
       elseif param == "fc1" then fillColor = GMFunc.ColorPreset.Custom1
       elseif param == "fc2" then fillColor = GMFunc.ColorPreset.Custom2
       elseif param == "fc3" then fillColor = GMFunc.ColorPreset.Custom2
       end
    
       if param ~= nil and string.find(param, "f") and tonumber(string.sub(param, 2)) 
    
        then fillAlpha = tonumber(string.sub(param, 2))
   
       end
       
       if param == "n" then lineType = 0
       elseif param == "s" then lineType = 1
       elseif param == "d" then lineType = 2
       elseif param == "dot" then lineType = 3
       elseif param == "dd" then lineType = 4
       elseif param == "ld" then lineType = 5
       elseif param == "2d" then lineType = 6
       end
    
       if param ~= nil and string.find(param, "t ") 
  
        then shapeText = string.sub(param, 3)

       end
       
  end
    
  if fillAlpha == 0
    
   then if color[1] == fillColor[1] or color[2] == fillColor[2] or color[3] == fillColor[3]
     
          then fillColor = GMFunc.ShuffleFillColor(fillColor)
                 
        end
          
  end
  
  mcoord:RectToAll(opCorner, coal, color, alpha, fillColor, fillAlpha, lineType, false)
    
  if GMFunc.DebugMode == true
      
    then trigger.action.outText("Shape gezeichnet, coal = " .. coal .. " color = " .. color[1] .. "," .. color[2] .. "," .. color[3] .. " fillColor = " .. fillColor[1] .. "," .. fillColor[2] .. "," .. fillColor[3] .. " alpha = " .. alpha .. " fillAlpha = " .. fillAlpha .. " lineType = " .. lineType, 60)
      
  end
    
  if shapeText ~= nil
    
   then local middlePoint = { x = mcoord.x + (opCorner.x - mcoord.x)/2 + GMFunc.TextOffsetShape.N,
                              z = mcoord.z + (opCorner.z - mcoord.z)/2 + GMFunc.TextOffsetShape.E }
          
        middlePoint.y = land.getHeight({middlePoint.x, middlePoint.z})
           
        local middleCoord = COORDINATE:New(middlePoint.x, middlePoint.y, middlePoint.z)
          
        if color[1] == fillColor[1] or color[2] == fillColor[2] or color[3] == fillColor[3]
     
          then fillColor = GMFunc.ShuffleFillColor(fillColor)
                 
        end
           
        middleCoord:TextToAll(shapeText, coal, color, alpha, fillColor, 0, GMFunc.FontSize, false)
           
        if GMFunc.DebugMode == true
      
          then trigger.action.outText("Textbox gezeichnet, x = " .. middleCoord.x .. " z = " .. middleCoord.z .. " y = " .. middleCoord.y .. " coal = " .. coal .. " alpha = " .. alpha .. " fillAlpha = " .. fillAlpha .. " shapeText = " .. shapeText, 60)
      
        end    

  end

end

--Init Poly-Function

function GMFunc.DrawPoly(param1, param2, param3, param4, param5, param6, param7, mcoord)

  local paramTbl = { param1 = param1,
                     param2 = param2,
                     param3 = param3,
                     param4 = param4,
                     param5 = param5,
                     param6 = param6,
                     param7 = param7 }

  local polyCorners = {}
  
  local allMarks = world.getMarkPanels()
  
  GMFunc.lastPolyID = GMFunc.lastPolyID + 1
  
  local coal = -1
  
  local color = GMFunc.ColorPreset[GMFunc.ShapeColor]
  local fillColor = GMFunc.ColorPreset[GMFunc.ShapeFillColor]
  
  local alpha = GMFunc.ShapeAlpha
  local fillAlpha = GMFunc.ShapeFillAlpha
  
  local lineType = GMFunc.LineType
  
  local shapeText = nil
  local fontSize = GMFunc.FontSize
  
  for i, mark in pairs(allMarks)
  
    do if string.find(mark.text, "c")
    
        then if tonumber(string.sub(mark.text, 2))
        
                then local cornerInd = tonumber(string.sub(mark.text, 2)) - 1
                     
                     polyCorners[cornerInd] = COORDINATE:New(mark.pos.x, mark.pos.y, mark.pos.z)
                     
             end
        
       end
       
    end
    
  for i, param in pairs(paramTbl)
  
    do if param == "red" then coal = 1
       elseif param == "blue" then coal = 2
       elseif param == "neutral" then coal = 0
       end
    
       if param == "r" then color = GMFunc.ColorPreset.Red
       elseif param == "b" then color = GMFunc.ColorPreset.Blue
       elseif param == "g" then color = GMFunc.ColorPreset.Green
       elseif param == "bl" then color = GMFunc.ColorPreset.Black
       elseif param == "w" then color = GMFunc.ColorPreset.White
       elseif param == "y" then color = GMFunc.ColorPreset.Yellow
       elseif param == "o" then color = GMFunc.ColorPreset.Orange
       elseif param == "p" then color = GMFunc.ColorPreset.Purple
       elseif param == "c1" then color = GMFunc.ColorPreset.Custom1
       elseif param == "c2" then color = GMFunc.ColorPreset.Custom2
       elseif param == "c3" then color = GMFunc.ColorPreset.Custom2
       end
    
       if tonumber(param) 
      
        then alpha = tonumber(param)
      
       end
       
       if param == "fr" then fillColor = GMFunc.ColorPreset.Red
       elseif param == "fb" then fillColor = GMFunc.ColorPreset.Blue
       elseif param == "fg" then fillColor = GMFunc.ColorPreset.Green
       elseif param == "fbl" then fillColor = GMFunc.ColorPreset.Black
       elseif param == "fw" then fillColor = GMFunc.ColorPreset.White
       elseif param == "fy" then fillColor = GMFunc.ColorPreset.Yellow
       elseif param == "fo" then fillColor = GMFunc.ColorPreset.Orange
       elseif param == "fp" then fillColor = GMFunc.ColorPreset.Purple
       elseif param == "fc1" then fillColor = GMFunc.ColorPreset.Custom1
       elseif param == "fc2" then fillColor = GMFunc.ColorPreset.Custom2
       elseif param == "fc3" then fillColor = GMFunc.ColorPreset.Custom2
       end
    
       if param ~= nil and string.find(param, "f") and tonumber(string.sub(param, 2)) 
    
        then fillAlpha = tonumber(string.sub(param, 2))
   
       end
       
       if param == "n" then lineType = 0
       elseif param == "s" then lineType = 1
       elseif param == "d" then lineType = 2
       elseif param == "dot" then lineType = 3
       elseif param == "dd" then lineType = 4
       elseif param == "ld" then lineType = 5
       elseif param == "2d" then lineType = 6
       end
    
       if param ~= nil and string.find(param, "t ") 
  
        then shapeText = string.sub(param, 3)

       end
       
  end
    
    if fillAlpha == 0
    
     then if color[1] == fillColor[1] or color[2] == fillColor[2] or color[3] == fillColor[3]
     
            then fillColor = GMFunc.ShuffleFillColor(fillColor)
                 
          end
          
    end
    
    color[4] = alpha
    fillColor[4] = fillAlpha
    
    if #polyCorners == 2 then trigger.action.markupToAll(7, coal, GMFunc.lastPolyID, mcoord, polyCorners[1], polyCorners[2], color, fillColor, lineType, false)
    elseif #polyCorners == 3 then trigger.action.markupToAll(7, coal, GMFunc.lastPolyID, mcoord, polyCorners[1], polyCorners[2], polyCorners[3], color, fillColor, lineType, false)
    elseif #polyCorners == 4 then trigger.action.markupToAll(7, coal, GMFunc.lastPolyID, mcoord, polyCorners[1], polyCorners[2], polyCorners[3], polyCorners[4], color, fillColor, lineType, false)
    elseif #polyCorners == 5 then trigger.action.markupToAll(7, coal, GMFunc.lastPolyID, mcoord, polyCorners[1], polyCorners[2], polyCorners[3], polyCorners[4], polyCorners[5], color, fillColor, lineType, false)
    elseif #polyCorners == 6 then trigger.action.markupToAll(7, coal, GMFunc.lastPolyID, mcoord, polyCorners[1], polyCorners[2], polyCorners[3], polyCorners[4], polyCorners[5], polyCorners[6], color, fillColor, lineType, false)
    elseif #polyCorners == 7 then trigger.action.markupToAll(7, coal, GMFunc.lastPolyID, mcoord, polyCorners[1], polyCorners[2], polyCorners[3], polyCorners[4], polyCorners[5], polyCorners[6], polyCorners[7], color, fillColor, lineType, false)
    elseif #polyCorners == 8 then trigger.action.markupToAll(7, coal, GMFunc.lastPolyID, mcoord, polyCorners[1], polyCorners[2], polyCorners[3], polyCorners[4], polyCorners[5], polyCorners[6], polyCorners[7], polyCorners[8], color, fillColor, lineType, false)
    elseif #polyCorners == 9 then trigger.action.markupToAll(7, coal, GMFunc.lastPolyID, mcoord, polyCorners[1], polyCorners[2], polyCorners[3], polyCorners[4], polyCorners[5], polyCorners[6], polyCorners[7], polyCorners[8], polyCorners[9], color, fillColor, lineType, false)
    elseif #polyCorners == 10 then trigger.action.markupToAll(7, coal, GMFunc.lastPolyID, mcoord, polyCorners[1], polyCorners[2], polyCorners[3], polyCorners[4], polyCorners[5], polyCorners[6], polyCorners[7], polyCorners[8], polyCorners[9], polyCorners[10], color, fillColor, lineType, false)
    end--there MUST be a more elegant way to handle the varying number of corners used as parameters for .markupToAll  
    
    if shapeText ~= nil
    
     then local middlePoint = {}
     
          middlePoint.x = mcoord.x
          middlePoint.z = mcoord.z
          
          for i = 1, #polyCorners, 1
          
            do middlePoint.x = middlePoint.x + polyCorners[i].x
               middlePoint.z = middlePoint.z + polyCorners[i].z
               
          end
          
          middlePoint.x = middlePoint.x / (#polyCorners + 1) + GMFunc.TextOffsetShape.N
          middlePoint.z = middlePoint.z / (#polyCorners + 1) + GMFunc.TextOffsetShape.E
               
          
          
          middlePoint.y = land.getHeight({middlePoint.x, middlePoint.z})
           
          local middleCoord = COORDINATE:New(middlePoint.x, middlePoint.y, middlePoint.z)
          
          if color[1] == fillColor[1] or color[2] == fillColor[2] or color[3] == fillColor[3]
     
            then fillColor = GMFunc.ShuffleFillColor(fillColor)
                 
          end
           
          middleCoord:TextToAll(shapeText, coal, color, alpha, fillColor, 0, GMFunc.FontSize, false)
           
          if GMFunc.DebugMode == true
      
            then trigger.action.outText("Textbox gezeichnet, x = " .. middleCoord.x .. " z = " .. middleCoord.z .. " y = " .. middleCoord.y .. " coal = " .. coal .. " alpha = " .. alpha .. " fillAlpha = " .. fillAlpha .. " shapeText = " .. shapeText, 60)
      
          end    

    end

end

--Init DeleteDrawing-Function

function GMFunc.DeleteDrawing(param1, mcoord)
  
  local delRadius = 500
  
  if tonumber(param1) ~= nil then delRadius = tonumber(param1) end
  
  local allMarks = world.getMarkPanels()
  
  for i, MarkPanel in pairs(allMarks)
  
    do local MarkCoord = COORDINATE:New(MarkPanel.pos.x, MarkPanel.pos.y, MarkPanel.pos.z)
    
       if MarkCoord:IsInRadius(mcoord, delRadius) == true
       
        then trigger.action.removeMark(MarkPanel.idx)
        
       end
    
  end
  
end

--Init Query-Function

function GMFunc.WhatsThis(param1, mcoord)

  local queryZoneName = string.format("QueryZone %d", GMFunc.queryZoneNum)
  GMFunc.queryZoneNum = GMFunc.queryZoneNum + 1
  local queryRadius = 500
  
  if tonumber(param1) 
    
    then queryRadius = tonumber(param1)

  end  
  
  local queryZone = ZONE_RADIUS:New(queryZoneName, mcoord:GetVec2(), queryRadius)
  queryZone:Scan(Object.Category) 
  local queryTable = queryZone:GetScannedUnits()
  
  for i, unit in pairs(queryTable)
  
   do local scanUName = unit:getName()
      local scanGrName = "No Group"
      local unitCategory = unit:getCategory()
      
      if unitCategory ~= 3 and unitCategory ~= 6 
        then scanGrName = unit:getGroup():getName()
      end  
          
      local queryMarker = MARKER:New(mcoord, "Unit: " .. scanUName .. ", Group: " .. scanGrName)
             
      if GMFunc.RestrToCoal == 1
       then queryMarker:ToCoaliton(coaliton.side.RED)
      elseif GMFunc.RestrToCoal == 2
       then queryMarker:ToCoaliton(coaliton.side.BLUE)
      else queryMarker:ToAll()
      end
      
     --break
          
   end

end

--Init Flag-Function---------------------------------------------------------------------------------------------------------------------------------

function GMFunc.UserFlagSet(param1, param2)

  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Flaggen-Funktion aktiv!", 10)
    
  end
  
  local FlagValue = nil
  
  if param2 == "true" then FlagValue = true
  elseif param2 == "false" then FlagValue = false
  elseif tonumber(param2) then FlagValue = param2
  end
  
  trigger.action.setUserFlag(param1, FlagValue)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Flagge " .. param1 .. " auf Wert " .. param2 .. " gesetzt!", 10)
    
  end
  
end

--Init Flare-Function-----------------------------------------------------------------------------------------------------------------------------------

function GMFunc.Flare(param1, param2, param3, mcoord)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Flare-Funktion aktiv!", 10)
    
  end
  
  local flareParams = {}
  
  flareParams.color = nil
  flareParams.counter = nil
  flareParams.hdg = nil
  
  if param2 == "ne" then flareParams.hdg = 45
  elseif param2 == "e" then flareParams.hdg = 90
  elseif param2 == "se" then flareParams.hdg = 135
  elseif param2 == "s" then flareParams.hdg = 180
  elseif param2 == "sw" then flareParams.hdg = 225
  elseif param2 == "w" then flareParams.hdg = 270
  elseif param2 == "nw" then flareParams.hdg = 315
  elseif tonumber(param2) then param3 = param2
  end

  if param1 == "g" then flareParams.color = "Green"
  elseif param1 == "r" then flareParams.color = "Red"
  elseif param1 == "w" then flareParams.color = "White"
  elseif param1 == "y" then flareParams.color = "Yellow"
  end
  
  if param3 ~= nil
  
    then flareParams.counter = tonumber(param3)
    
         local function FlareMult(flareParams)
          
          if flareParams.counter > 0
          
            then flareParams.counter = flareParams.counter - 1
                 mcoord:Flare(FLARECOLOR[flareParams.color], flareParams.hdg)
                 timer.scheduleFunction(FlareMult, flareParams, timer.getTime() + 1)
                 
                 if GMFunc.DebugMode == true
                 
                  then trigger.action.outText("DEBUG: " .. flareParams.counter .. " Flare(s) noch zu schiessen!", 10)
                  
                 end
                 
          end
          
         end
         
         FlareMult(flareParams)
  
  else mcoord:Flare(FLARECOLOR[flareParams.color], flareParams.hdg)
       
       if GMFunc.DebugMode == true
       
        then trigger.action.outText("DEBUG: Einzelne Flare geschossen!", 10)
        
       end
              
  end
  
end

--Init Smoke-Function----------------------------------------------------------------------------------------------------------------------------------------

function GMFunc.Smoke(param1, param2, mcoord)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Smoke-Funktion aktiv!", 10)
    
  end
  
  local smokeParams = {}
  
  smokeParams.color = nil
  smokeParams.counter = nil
  
  if param1 == "b" then smokeParams.color = "Blue"
  elseif param1 == "g" then smokeParams.color = "Green"
  elseif param1 == "o" then smokeParams.color = "Orange"
  elseif param1 == "r" then smokeParams.color = "Red"
  elseif param1 == "w" then smokeParams.color = "White"
  end
  
  if tonumber(param2)
    
    then smokeParams.counter = math.floor((tonumber(param2)/5)+0.5)
         
         if GMFunc.DebugMode == true
                 
          then trigger.action.outText("DEBUG: " .. smokeParams.counter .. " mal wird der Rauch noch erneuert!", 10)
                  
         end
         
         mcoord:Smoke(SMOKECOLOR[smokeParams.color])
    
         local function smokeMult(smokeParams)
         
          if smokeParams.counter > 0 
          
            then smokeParams.counter = smokeParams.counter - 1
                 mcoord:Smoke(SMOKECOLOR[smokeParams.color])
                 timer.scheduleFunction(smokeMult, smokeParams, timer.getTime() + 300)

          end
          
         end
         
         timer.scheduleFunction(smokeMult, smokeParams, timer.getTime() + 300)
         
  else mcoord:Smoke(SMOKECOLOR[smokeParams.color])
    
  end
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: ".. smokeParams.color .."-Smoke aktiv!", 10)
    
  end
  
end

--Init Explode-Function------------------------------------------------------------------------------------------------------------------------------

function GMFunc.ExplodeAtMark(param1, param2, param3, mcoord)
 
 if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Explosions-Funktion aktiv!", 10)
    
 end
 
 local expDelay = 1
 local expYield = 100
 local expGroup = nil
 
 if param1 ~= nil and GROUP:FindByName(param1) == nil
 
  then if string.find(param1, "d")
 
        then expDelay = tonumber(string.sub(param1, 2))
        
       elseif tonumber(param1)
       
        then expYield = tonumber(param1)
       
       end
       
 end
  
 if param2 ~= nil and GROUP:FindByName(param2) == nil
 
  then if string.find(param2, "d")
  
        then expDelay = tonumber(string.sub(param2, 2))
        
       elseif tonumber(param2)
       
        then expYield = tonumber(param2)
        
       end
  
 end
 
 if param3 ~= nil and GROUP:FindByName(param3) == nil
 
  then if string.find(param3, "d")
  
        then expDelay = tonumber(string.sub(param3, 2))
        
       elseif tonumber(param3)
       
        then expYield = tonumber(param3)
        
       end
  
 end
 
 if GROUP:FindByName(param1) then expGroup = GROUP:FindByName(param1)
 elseif GROUP:FindByName(param2) then expGroup = GROUP:FindByName(param2)
 elseif GROUP:FindByName(param3) then expGroup = GROUP:FindByName(param3)
 end
 
 if expGroup ~= nil
 
  then local expUnits = expGroup:GetUnits()
    
       for i, Victim in pairs(expUnits)
       
        do Victim:Explode(expYield, expDelay)
        
       end
  
 else mcoord:Explosion(expYield, expDelay)
       
 end      

 if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Explosion gezuendet! Delay = " .. expDelay .. ", Yield = " .. expYield .. "!", 10)
    
 end

end

--Init Illumination-Function---------------------------------------------------------------------------------------------------------------------------

function GMFunc.IllumAtMark(param1, param2, mcoord)
 
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Leuchtgranaten-Funktion aktiv!", 10)
    
 end
 
 if param1 == nil 
 
  then mcoord.y = mcoord.y + 650
 
 else mcoord.y = mcoord.y + param1
 
 end
 
 if param2 == nil 
 
  then param2 = 10000
  
 end
 
 mcoord:IlluminationBomb(param2)

 if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Leuchtgranate gezuendet!", 10)
    
 end

end

--Init SmokeFX-Function-------------------------------------------------------------------------------------------------------------------------------

function GMFunc.FXFireSmoke(param1, param2, mvec3)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Rauch und Feuer Funktion aktiv!", 10)
    
  end
  
  local SmokePreset = nil
  
  if param1 == "ssf" then SmokePreset = 1
  elseif param1 == "msf" then SmokePreset = 2
  elseif param1 == "lsf" then SmokePreset = 3
  elseif param1 == "hsf" then SmokePreset = 4
  elseif param1 == "ss" then SmokePreset = 5
  elseif param1 == "ms" then SmokePreset = 6
  elseif param1 == "ls" then SmokePreset = 7
  elseif param1 == "hs" then SmokePreset = 8
  end
 
  if param2 ~= nil
    
    then param2 = param2 / 100
    
  end
  
  trigger.action.effectSmokeBig(mvec3, SmokePreset, param2 )
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Es brennt, Modus " .. param1 .. "!", 10)
    
  end

end

--Init Sound-Function--------------------------------------------------------------------------------------------------------------------------------------

function GMFunc.PlaySound(param1, param2)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Sound-Funktion aktiv!", 10)
    
  end
  
  if param1 ~= nil
    
    then if param2 ~= nil
    
           then if param2 == "b"
           
                 then trigger.action.outSoundForCoalition(2 ,param1)
                      
                      if GMFunc.DebugMode == true
                        
                        then trigger.action.outText("DEBUG: Sound an Blau gespielt!", 10)
                        
                      end
           
                elseif param2 == "r"
           
                 then trigger.action.outSoundForCoalition(1, param1)
                      
                      if GMFunc.DebugMode == true
                        
                        then trigger.action.outText("DEBUG: Sound an Rot gespielt!", 10)
                        
                      end
           
                elseif string.len(param2) > 1
          
                 then local RecGroup = Group.getByName(param2)
                      local RecID = RecGroup:getID()
                      trigger.action.outSoundForGroup(RecID, param1)
                      
                      if GMFunc.DebugMode == true
                      
                        then trigger.action.outText("DEBUG: Sound an Gruppe " .. RecID .." gespielt!", 10)
                        
                      end
                
                end
                
         else trigger.action.outSound(param1)
              
              if GMFunc.DebugMode == true
              
                then trigger.action.outText("DEBUG: Sound an Alle gespielt!", 10)
                
              end
         
         end 
  
  end
  
end

--Init Text-Function--------------------------------------------------------------------------------------------------------------------

function GMFunc.ShowText(param1, param2, param3, param4)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Text-Funktion aktiv!", 10)
    
  end
  
  if param1 ~= nil
    
    then local coalition = nil
         local clearView = false
         local DispTime = GMFunc.MsgDispTime
         local RecGroup = nil
         
         if param2 == "b" or param3 == "b" or param4 == "b"
         
          then coalition = 2
               
               if param2 == "b" then param2 = nil
               elseif param3 == "b" then param3 = nil
               elseif param4 == "b" then param4 = nil
               end
               
          
         elseif param2 == "r" or param3 == "r" or param4 == "r"
         
          then coalition = 1
          
               if param2 == "r" then param2 = nil
               elseif param3 == "r" then param3 = nil
               elseif param4 == "r" then param4 = nil
               end
          
         end
         
         if param2 ~= nil and Group.getByName(param2) 
            then RecGroup = Group.getByName(param2)
                 param2 = nil
         elseif param3 ~= nil and Group.getByName(param3) 
            then RecGroup = Group.getByName(param3)
                 param3= nil
         elseif param4 ~= nil and Group.getByName(param4) 
            then RecGroup = Group.getByName(param4)
                 param4 = nil
         end
         
         if tonumber(param2) then DispTime = param2
         elseif tonumber(param3) then DispTime = param3
         elseif tonumber(param4) then DispTime = param4
         end
         
         if param2 == "c" or param3 == "c" or param4 == "c"

           then clearView = true
                
                if param2 == "c" then param2 = nil
                elseif param3 == "c" then param3 = nil
                elseif param4 == "c" then param4 = nil
                end
         end
         
         
                    
         if coalition ~= nil 
         
          then trigger.action.outTextForCoalition(coalition, GMFunc.MsgBorderL .. param1 .. GMFunc.MsgBorderR, DispTime, clearView)
           
                  if GMFunc.DebugMode == true
                  
                    then trigger.action.outText("DEBUG: Text an Koalition " .. coalition .. " ausgegeben!", 10)
                    
                  end
                  
                  if GMFunc.MsgSound ~= nil
                  
                    then trigger.action.outSoundForCoalition(coalition, GMFunc.MsgSound)
                  
                  end
           
         elseif  RecGroup ~= nil
          
                  then local RecID = RecGroup:getID()
                       trigger.action.outTextForGroup(RecID, GMFunc.MsgBorderL .. param1 .. GMFunc.MsgBorderR, DispTime, clearView)
                       
                       if GMFunc.DebugMode == true
                       
                        then trigger.action.outText("DEBUG: Text an Gruppe " .. RecID .." ausgegeben!", 10)
                        
                       end
                       
                       if GMFunc.MsgSound ~= nil
                  
                          then trigger.action.outSoundForGroup(RecID, GMFunc.MsgSound)
                  
                       end
            
         else trigger.action.outText(GMFunc.MsgBorderL .. param1 .. GMFunc.MsgBorderR, DispTime, clearView)
              
              if GMFunc.DebugMode == true
              
                then trigger.action.outText("DEBUG: Text an Alle ausgegeben!", 10)
                
              end
              
              if GMFunc.MsgSound ~= nil
                  
                then trigger.action.outSound(GMFunc.MsgSound)
                  
              end
      
         end
      
  end
  
end

--Init Invisible-Function---------------------------------------------------------------------------------------------------------------------------------

function GMFunc.SetInvisible(param1, param2)

  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Invisible-Funktion aktiv!", 10)
    
  end
    
  if param2 == "on"
  
    then param2 = true
    
  elseif param2 == "off"
  
    then param2 = false
    
  end
  
  local InvGroup = nil
    
  if Group.getByName(param1)
  
    then InvGroup = Group.getByName(param1)
         
         local InvCommand = { id = 'SetInvisible', 
                              params = { value = param2 } }
                              
         InvGroup:getController():setCommand(InvCommand)
         
         if GMFunc.DebugMode == true
         
          then trigger.action.outText("DEBUG: Invisible-Befehl an Gruppe " .. InvGroup:getName() .. " gesendet!", 10)
          
         end
  
  end    
  
end

--Init Immortal-Function-----------------------------------------------------------------------------------------------------------------------------------

function GMFunc.SetImmortal(param1, param2)
  
  if GMFunc.DebugMode == true
    
    then trigger.action.outText("DEBUG: Immortal-Funktion aktiv!", 10)
    
  end
    
  if param2 == "on"
  
    then param2 = true
    
  elseif param2 == "off"
  
    then param2 = false
    
  end
  
  local ImmGroup = nil
    
  if Group.getByName(param1)
  
    then ImmGroup = Group.getByName(param1)
         
         local ImmCommand = { id = 'SetImmortal', 
                              params = { value = param2 } }
                              
         ImmGroup:getController():setCommand(ImmCommand)
         
         if GMFunc.DebugMode == true
         
          then trigger.action.outText("DEBUG: Immortal-Befehl an Gruppe " .. ImmGroup:getName() .. " gesendet!", 10)
          
         end
  
  end    
  
end

--Init AI-Togglefunction------------------------------------------------------------------------------------------------------------------------------

function GMFunc.ToggleAI(param1, param2)
  
  if GMFunc.DebugMode == true
    
    then trigger.action.outText("DEBUG: KI-Togglefunktion aktiv!", 10)
    
  end
    
  if param2 == "on"
  
    then param2 = true
    
  elseif param2 == "off"
  
    then param2 = false
    
  end
  
  local AIMGroup = nil
    
  if Group.getByName(param1)
  
    then AIMGroup = GROUP:FindByName(param1)
                              
         AIMGroup:SetAIOnOff(param2)
         
         if GMFunc.DebugMode == true
         
          then trigger.action.outText("DEBUG: KI der Gruppe " .. param1 .. " umgeschaltet!", 10)
          
         end
  
  end    
  
end

--Init Pickup-Function---------------------------------------------------------------------------------------------------------------------------

function GMFunc.CargoPickup(param1, param2, mcoord)

  if GMFunc.DebugMode == true
 
    then trigger.action.outText("DEBUG: Pickup-Funktion aktiv!", 10)
  
  end
  
  if GROUP:FindByName(param2)
  
    then local CarrierGrp = GROUP:FindByName(param2)
         local LoadObject = CARGO:FindByName(param1)
  
         if LoadObject:CanBoard()
         
          then LoadObject:Board(CarrierGrp, 10)
          
         elseif LoadObject:CanLoad()
         
          then LoadObject:Load(CarrierGrp)
         
         end
                        
         if GMFunc.DebugMode == true
 
            then trigger.action.outText("DEBUG: Gruppe " .. param1 .. " wird eingeladen in Transporter " .. param2 .. " !", 10)
  
         end
  
  end
  
end

--Init Unload-Function---------------------------------------------------------------------------------------------------------------------------

function GMFunc.CargoDrop(param1, mcoord)

  if GMFunc.DebugMode == true
 
    then trigger.action.outText("DEBUG: Aussteige-Funktion aktiv!", 10)
  
  end
  
  if CARGO:FindByName(param1)
  
    then local DropObject = CARGO:FindByName(param1)
    
         if DropObject:CanUnboard()
         
          then DropObject:UnBoard(mcoord)
               
               if GMFunc.DebugMode == true
 
                then trigger.action.outText("DEBUG: Gruppe " .. param1 .. " steigt aus!", 10)
  
               end
          
         elseif DropObject:CanUnload()
         
          then DropObject:UnLoad(mcoord)
          
               if GMFunc.DebugMode == true
 
                then trigger.action.outText("DEBUG: Statische Fracht " .. param1 .. " entladen!", 10)
  
               end
               
         end
          
  end
  
end

--Init RTB-Function----------------------------------------------------------------------------------------------------------------------------------

function GMFunc.AIReturnToBase(param1, param2, mcoord)

  if GMFunc.DebugMode == true
 
    then trigger.action.outText("DEBUG: RTB-Funktion aktiv!", 10)
  
  end
  
  local AItoRTB = nil
  local RTBSpeed = nil
  local RTBAirfield = nil
  
  if GROUP:FindByName(param1)
  
    then AItoRTB = GROUP:FindByName(param1)
          
         if tonumber(param2)
  
          then RTBSpeed = UTILS.KnotsToKmph(param2)
          
          else RTBSpeed = AItoRTB:GetVelocityKMH()
          
         end
  
         RTBAirfield = mcoord:GetClosestAirbase()
         
         if RTBAirfield ~= nil 
         
          then AItoRTB:RouteRTB(RTBAirfield, RTBSpeed)
         
         end
         
         if GMFunc.DebugMode == true
 
          then trigger.action.outText("DEBUG: Gruppe " .. param1 .. " geht RTB!", 10)
  
         end
         
  end

end

--Init Helicopter LZ Function-----------------------------------------------------------------------------------------------------------------------------

function GMFunc.HeloLand(param1, param2, mcoord)

  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Helikopter-Landefunktion aktiv!", 10)
  
 end

  if GROUP:FindByName(param1)

    then local AIHelo = GROUP:FindByName(param1)
         local LZVec2 = mcoord:GetVec2()
         local DTime = 120
       
         if tonumber(param2)
       
          then DTime = tonumber(param2)
        
         end
       
         local LandingTask = AIHelo:TaskLandAtVec2(LZVec2, DTime)
         AIHelo:SetTask(LandingTask, 1)
       
         if GMFunc.DebugMode == true
 
          then trigger.action.outText("DEBUG: Gruppe " .. param1 .. " landet!", 10)
  
         end
       
  
  end

end

--Init Waypoint-Function-------------------------------------------------------------------------------------------------------------------------------

function GMFunc.RouteAI(param1, param2, param3, param4, mcoord)
 
 if GMFunc.DebugMode == true
 
  then trigger.action.outText("DEBUG: Wegpunkt-Funktion aktiv!", 10)
  
 end
 
 local AIDesc = Group.getByName(param1):getUnit(1):getDesc()
 local AIMGroup = GROUP:FindByName(param1)
 local AIRoute = nil
 
 if AIDesc.category == 3
  
  then if mcoord:GetSurfaceType() == 2 or mcoord:GetSurfaceType() == 3 
  
        then local mvec2 = POINT_VEC2:New(mcoord.x, mcoord.y, mcoord.z)
             AIRoute = AIMGroup:RouteToVec2(mvec2, param2)
       
       end
 
 elseif AIDesc.category == 2
 
  then if mcoord:GetSurfaceType() ~= 2 and mcoord:GetSurfaceType() ~= 3
  
        then local UseRoad = false
             local TrSpeed = 20
             local Form = nil
             
             if param2 == "road" or param3 == "road" or param4 == "road" 
             
              then UseRoad = true

             end
             
             if tonumber(param2) then TrSpeed = param2
             elseif tonumber(param3) then TrSpeed = param3
             elseif tonumber(param4) then TrSpeed = param4 
             end
             
             if param2 == "v" or param3 == "v" or param4 == "v" then Form = "Vee"
             elseif param2 == "c" or param3 == "c" or param4 == "c" then Form = "Cone"
             elseif param2 == "d" or param3 == "d" or param4 == "d" then Form = "Diamond"
             elseif param3 == "r" or param3 == "r" or param4 == "r" then Form = "Rank"
             elseif param3 == "el" or param3 == "el" or param4 == "el" then Form = "EchelonL"
             elseif param3 == "er" or param3 == "er" or param4 == "er" then Form = "EchelonR"
             end
             
             if UseRoad == true 
             
              then AIRoute = AIMGroup:RouteGroundOnRoad(mcoord, TrSpeed, 1, "Off Road")
              
             else AIRoute = AIMGroup:RouteGroundTo(mcoord, TrSpeed, Form, 1)
             
             end
             

             
       end
 
 end
 
 AIMGroup:SetTask(AIRoute, 1)
 
 if GMFunc.DebugMode == true
 
  then trigger.action.outText("DEBUG: WP fuer Gruppe " .. param1 .. " zugewiesen!", 10)
  
 end
 
end

--Init Orbit-Function--------------------------------------------------------------------------------------------------------------------------

function GMFunc.Orbit(param1, param2, param3, param4, mcoord)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Orbit-Funktion aktiv!", 10)
  
  end
  
  local Orbiter = GROUP:FindByName(param1)
  local OrbiterVec3 = Orbiter:GetPositionVec3()
  local OrbiterCoord = COORDINATE:NewFromVec3(OrbiterVec3)
  
  if param4 == "r"
  
    then local OrbitTask = Orbiter:TaskOrbit(OrbiterCoord, param2, param3 / 1.94, mcoord)
         Orbiter:SetTask(OrbitTask, 1)
         
         if GMFunc.DebugMode == true
         
          then trigger.action.outText("DEBUG: Racetrack-Orbit fuer Gruppe " .. param1 .. " zugewiesen!", 10)
        
         end
        
  else local OrbitTask = Orbiter:TaskOrbit(mcoord, param2, param3 / 1.94)
       Orbiter:SetTask(OrbitTask, 1)
        
        if GMFunc.DebugMode == true
         
          then trigger.action.outText("DEBUG: Orbit fuer Gruppe " .. param1 .. " zugewiesen!", 10)
        
        end
        
  end

end

--Init Follow-Function--------------------------------------------------------------------------------------------------------------------------------------

function GMFunc.AirFollowAI(param1, param2, param3, param4, param5)
  
  if GMFunc.DebugMode == true
         
    then trigger.action.outText("DEBUG: Follow-Funktion aktiv!", 10)
        
  end
  
  local AirLeader = GROUP:FindByName(param1)
  local AirFollower = GROUP:FindByName(param2)
  local FollowOffset = {x = -100, y = 0, z = 0}

  if param3 ~= nil 
  
    then if string.find(param3, "f") then FollowOffset.x = tonumber(string.sub(param3, 2))
         elseif string.find(param3, "b") then FollowOffset.x = tonumber(string.sub(param3, 2)) * -1
         elseif string.find(param3, "r") then FollowOffset.z = tonumber(string.sub(param3, 2))
         elseif string.find(param3, "l") then FollowOffset.z = tonumber(string.sub(param3, 2)) * -1
         elseif string.find(param3, "a") then FollowOffset.y = tonumber(string.sub(param3, 2))
         elseif string.find(param3, "u") then FollowOffset.y = tonumber(string.sub(param3, 2)) * -1
         end
         
  end
  
  if param4 ~= nil 
  
    then if string.find(param4, "f") then FollowOffset.x = tonumber(string.sub(param4, 2))
         elseif string.find(param4, "b") then FollowOffset.x = tonumber(string.sub(param4, 2)) * -1
         elseif string.find(param4, "r") then FollowOffset.z = tonumber(string.sub(param4, 2))
         elseif string.find(param4, "l") then FollowOffset.z = tonumber(string.sub(param4, 2)) * -1
         elseif string.find(param4, "a") then FollowOffset.y = tonumber(string.sub(param4, 2))
         elseif string.find(param4, "u") then FollowOffset.y = tonumber(string.sub(param4, 2)) * -1
         end
         
  end
  
  if param5 ~= nil 
  
    then if string.find(param5, "f") then FollowOffset.x = tonumber(string.sub(param5, 2))
         elseif string.find(param5, "b") then FollowOffset.x = tonumber(string.sub(param5, 2)) * -1
         elseif string.find(param5, "r") then FollowOffset.z = tonumber(string.sub(param5, 2))
         elseif string.find(param5, "l") then FollowOffset.z = tonumber(string.sub(param5, 2)) * -1
         elseif string.find(param5, "a") then FollowOffset.y = tonumber(string.sub(param5, 2))
         elseif string.find(param5, "u") then FollowOffset.y = tonumber(string.sub(param5, 2)) * -1
         end
         
  end
  
  local AirFollowTask = AirFollower:TaskFollow(AirLeader, FollowOffset, nil)
  AirFollower:SetTask( AirFollowTask, 2)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Lead zugewiesen!", 10)
    
  end

end

--Init Escort-Function--------------------------------------------------------------------------------------------------------------------------------------

function GMFunc.AirEscortAI(param1, param2, param3, param4, param5, param6)
  
  if GMFunc.DebugMode == true
         
    then trigger.action.outText("DEBUG: Eskorten-Funktion aktiv!", 10)
        
  end
  
  local AirEscort = GROUP:FindByName(param1)
  local AirEscorted = GROUP:FindByName(param2)
  local AirEscortOffset = {x = -100, y = 0, z = 100}
  local AirEscortER = 83340
  
  if param3 ~= nil 
    
    then if string.find(param3, "f") then AirEscortOffset.x = tonumber(string.sub(param3, 2))
         elseif string.find(param3, "b") then AirEscortOffset.x = tonumber(string.sub(param3, 2)) * -1
         elseif string.find(param3, "r") then AirEscortOffset.z = tonumber(string.sub(param3, 2))
         elseif string.find(param3, "l") then AirEscortOffset.z = tonumber(string.sub(param3, 2)) * -1
         elseif string.find(param3, "a") then AirEscortOffset.y = tonumber(string.sub(param3, 2))
         elseif string.find(param3, "u") then AirEscortOffset.y = tonumber(string.sub(param3, 2)) * -1
         
         elseif tonumber(param3) then AirEscortER = tonumber(param3) * 1852
         
         end
  
  end
  
  if param4 ~= nil 
  
    then if string.find(param4, "f") then AirEscortOffset.x = tonumber(string.sub(param4, 2))
         elseif string.find(param4, "b") then AirEscortOffset.x = tonumber(string.sub(param4, 2)) * -1
         elseif string.find(param4, "r") then AirEscortOffset.z = tonumber(string.sub(param4, 2))
         elseif string.find(param4, "l") then AirEscortOffset.z = tonumber(string.sub(param4, 2)) * -1
         elseif string.find(param4, "a") then AirEscortOffset.y = tonumber(string.sub(param4, 2))
         elseif string.find(param4, "u") then AirEscortOffset.y = tonumber(string.sub(param4, 2)) * -1
         
         elseif tonumber(param4) then AirEscortER = tonumber(param4) * 1852
         
         end
         
  end
  
  if param5 ~= nil 
  
    then if string.find(param5, "f") then AirEscortOffset.x = tonumber(string.sub(param5, 2))
         elseif string.find(param5, "b") then AirEscortOffset.x = tonumber(string.sub(param5, 2)) * -1
         elseif string.find(param5, "r") then AirEscortOffset.z = tonumber(string.sub(param5, 2))
         elseif string.find(param5, "l") then AirEscortOffset.z = tonumber(string.sub(param5, 2)) * -1
         elseif string.find(param5, "a") then AirEscortOffset.y = tonumber(string.sub(param5, 2))
         elseif string.find(param5, "u") then AirEscortOffset.y = tonumber(string.sub(param5, 2)) * -1
         
         elseif tonumber(param5) then AirEscortER = tonumber(param5) * 1852
         
         end
         
  end
  
  if param6 ~= nil 
  
    then if string.find(param6, "f") then AirEscortOffset.x = tonumber(string.sub(param6, 2))
         elseif string.find(param6, "b") then AirEscortOffset.x = tonumber(string.sub(param6, 2)) * -1
         elseif string.find(param6, "r") then AirEscortOffset.z = tonumber(string.sub(param6, 2))
         elseif string.find(param6, "l") then AirEscortOffset.z = tonumber(string.sub(param6, 2)) * -1
         elseif string.find(param6, "o") then AirEscortOffset.y = tonumber(string.sub(param6, 2))
         elseif string.find(param6, "u") then AirEscortOffset.y = tonumber(string.sub(param6, 2)) * -1
         
         elseif tonumber(param6) then AirEscortER = tonumber(param6) * 1852
         
         end
         
  end
  
  local AirEscortTask = AirEscort:TaskEscort(AirEscorted, AirEscortOffset, nil, AirEscortER, {"Air"} )
  AirEscort:SetTask( AirEscortTask, 2)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Eskorte zugewiesen!", 10)
    
  end

end

--Init Delete-Function-----------------------------------------------------------------------------------------------------------------------------

function GMFunc.Delete(param1, mcoord)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Loeschen-Funktion aktiv!", 10)
    
  end
  
  if param1 == nil or tonumber(param1)
  
    then if param1 == nil
    
          then param1 = 100
         
         end
   
         local deleteZoneName = string.format("DeleteZone %d", GMFunc.deleteZoneNum)
         GMFunc.deleteZoneNum = GMFunc.deleteZoneNum + 1
  
         local deleteZone = ZONE_RADIUS:New(deleteZoneName, mcoord:GetVec2(), param1)
         deleteZone:Scan(Object.Category) 
         local deleteTable = deleteZone:GetScannedUnits()
  
         for i, unit in pairs(deleteTable)
  
          do if unit:getPlayerName() == nil 
      
              then unit:destroy()
        
             end
    
             if GMFunc.DebugMode == true
    
              then trigger.action.outText("DEBUG: Eine Einheit/Static wurde geloescht!", 10)
      
             end
  
         end
  
  elseif Group.getByName(param1)
  
    then local DestroyGroup = Group.getByName(param1)
         DestroyGroup:destroy()
         
         if GMFunc.DebugMode == true
    
          then trigger.action.outText("DEBUG: Eine Gruppe wurde geloescht!", 10)
      
         end
         
  end
  
end

--Init Control Toggle Function----------------------------------------------------------------------------------------------------------------------------

function GMFunc.ControlToggle(param1)

  if GMFunc.DebugMode == true
    
    then trigger.action.outText("Control-Toggle-Funktion aktiv", 10)
         trigger.action.outText("Param1 = " .. param1 .. " !", 10)
    
  end
    
  if GROUP:FindByName(param1)
         
   then local UctGroup = GROUP:FindByName(param1)
        UctGroup:RespawnAtCurrentAirbase(nil, SPAWN.Takeoff.Cold, false)
        
        if GMFunc.DebugMode == true
    
          then trigger.action.outText("Gruppe " .. param1 .. " mit Pilot neu gespawnt!", 10)
    
        end
        
  end

end

--Init Late Activation Function----------------------------------------------------------------------------------------------------------------------------

function GMFunc.LateActivate(param1)
  
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Late Activation Funktion aktiv!", 10)
    
  end
  
  if Group.getByName(param1)
  
    then local ActGroup = Group.getByName(param1)
         trigger.action.activateGroup(ActGroup)
         
         if GMFunc.DebugMode == true
         
          then trigger.action.outText("DEBUG: Gruppe " .. param1 .. " aktiviert!", 10)
          
         end
         
  end

end

--Init Static Spawn Function----------------------------------------------------------------------------------------------------------------------------------

function GMFunc.StaticSpawn(param1, param2, param3, param4, mcoord)

  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Static Spawn-Funktion aktiv!", 10)
    
  end
  
  local StaOriCountry = nil
  local StaOriCountryID = nil
  local StaSetCountry = nil
  local StaSpawnObject = nil
  local StaHeading = nil
  local StaIsCargo = nil
  local StaticSpawn = nil
  
  if StaticObject.getByName(param1)
  
    then if GMFunc.DefaultCountry == nil 
         
          then StaOriCountryID = StaticObject.getByName(param1):getCountry()
               StaOriCountry = country.name[StaOriCountryID]
               
         else StaOriCountryID = country.id[GMFunc.DefaultCountry]
              StaOriCountry = GMFunc.DefaultCountry
         
         end
         
         if GMFunc.DebugMode == true
  
          then trigger.action.outText("DEBUG: LandID = " .. StaOriCountryID .. ", Landname =  " .. StaOriCountry, 10)
    
         end
         
         if country.id[param2] ~= nil then StaSetCountry = country.id[param2]
         elseif country.id[param3] ~= nil then StaSetCountry = country.id[param3]
         elseif country.id[param4] ~= nil then StaSetCountry = country.id[param4]
         else StaSetCountry = country.id[StaOriCountry]
         end
         
         if tonumber(param2) then StaHeading = tonumber(param2)
         elseif tonumber(param3) then StaHeading = tonumber(param3)
         elseif tonumber(param4) then StaHeading = tonumber(param4)
         end
         
         if param2 == "cargo" or param3 == "cargo" or param4 == "cargo"
  
          then StaIsCargo = true
    
         end
    
         if GMFunc.Spawns[param1] == nil 
    
          then GMFunc.Spawns[param1] = SPAWNSTATIC:NewFromStatic(param1, StaSetCountry)
  
         end
         
         StaSpawnObject = GMFunc.Spawns[param1]     
         StaSpawnObject:InitCountry(StaSetCountry)
         
         if GMFunc.DebugMode == true
                      
          then trigger.action.outText("DEBUG: Land auf " .. StaSetCountry .. " gesetzt!", 10)
                      
         end
         
         StaticSpawn = StaSpawnObject:SpawnFromCoordinate(mcoord, StaHeading)
         
         if StaIsCargo == true
                   
          then local StaCargoName = StaticSpawn:GetName()
               local StaCargo = CARGO_CRATE:New(StaticSpawn, "Static", StaCargoName, 500, 500)                     
                         
         end
         
  end       

end

--Init spawn function for CTLD crates (Contribution by fargo007)------------------------------------------------------------------------------------------------

function GMFunc.SpawnCTLDCrate(param1, param2, mcoord)
 
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Spawn-Funktion CTLD-Kisten aktiv!", 10)
    
  end
  
  if ctld ~= nil 
  
    then param2 = tonumber(param2)
    
         ctld.spawnCrateAtPoint(param1, param2, mcoord)
    
  end
  
end

--Init spawn function for CTLD extractable groups (Contribution by fargo007)-----------------------------------------------------------------------------------

function GMFunc.SpawnExtractableCTLD(param1, param2, param3, mcoord)

  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Spawn-Funktion CTLD-Gruppen aktiv!", 10)
    
  end

  if ctld ~= nil 
  
    then param2 = tonumber(param2)
         param3 = tonumber(param3)
    
         ctld.spawnGroupAtPoint(param1, param2, mcoord, param3)
  
  end
  
end

--Init Spawn-Function-----------------------------------------------------------------------------------------------------------------------------------------
    
function GMFunc.Spawn(param1, param2, param3, param4, param5, param6, param7, mcoord)
    
  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Spawn-Funktion aktiv!", 10)
    
  end
  
  local SpawnObject = nil
  local SpawnGroupDCS = nil
  local SpawnCat = nil
  local OriCountryID = nil
  local OriCountry = nil
  local KeepTasking = false
  local NoMarkSp = false
  local AirbaseSpawn = false
  local IsCargo = false
  local SetCountry = nil
  local SpawnAlt = nil
  local SetSkill = GMFunc.DefaultSkill
  local SetROE = GMFunc.DefaultROE
  
  if Group.getByName(param1)
  
    then SpawnGroupDCS = Group.getByName(param1)
         SpawnCat = SpawnGroupDCS:getCategory()
         
         if GMFunc.DefaultCountry == nil 
         
          then OriCountryID = SpawnGroupDCS:getUnit(1):getCountry()
               OriCountry = country.name[OriCountryID]
               
         else OriCountryID = country.id[GMFunc.DefaultCountry]
              OriCountry = GMFunc.DefaultCountry
         
         end
         
         if GMFunc.DebugMode == true
  
          then trigger.action.outText("DEBUG: LandID = " .. OriCountryID .. ", Landname =  " .. OriCountry, 10)
    
         end
         
  end
  
  if GMFunc.Spawns[param1] == nil 
    
    then GMFunc.Spawns[param1] = SPAWN:New(param1)
  
  end
  
  SpawnObject = GMFunc.Spawns[param1]
  
  if param2 == "kt" or param3 == "kt" or param4 == "kt" or param5 == "kt" or param6 == "kt" or param7 == "kt"
  
    then KeepTasking = true
    
  end
  
  if param2 == "op" or param3 == "op" or param4 == "op" or param5 == "op" or param6 == "op" or param7 == "op"
  
    then NoMarkSp = true
    
  end
  
  if param2 == "ground" or param3 == "ground" or param4 == "ground" or param5 == "ground" or param6 == "ground" or param7 == "ground"
  
    then AirbaseSpawn = true
    
  end
  
  if param2 == "cargo" or param3 == "cargo" or param4 == "cargo" or param5 == "cargo" or param6 == "cargo" or param7 == "cargo"
  
    then IsCargo = true
    
  end
  
  if tonumber(param2) then SpawnAlt = param2  
  elseif tonumber(param3) then SpawnAlt = param3
  elseif tonumber(param4) then SpawnAlt = param4
  elseif tonumber(param5) then SpawnAlt = param5
  elseif tonumber(param6) then SpawnAlt = param6
  elseif tonumber(param6) then SpawnAlt = param7 
  end
  
  if country.id[param2] ~= nil then SetCountry = country.id[param2]
  elseif country.id[param3] ~= nil then SetCountry = country.id[param3]
  elseif country.id[param4] ~= nil then SetCountry = country.id[param4]
  elseif country.id[param5] ~= nil then SetCountry = country.id[param5]
  elseif country.id[param6] ~= nil then SetCountry = country.id[param6]
  elseif country.id[param7] ~= nil then SetCountry = country.id[param7]
  elseif GMFunc.DefaultCountry ~= nil then SetCountry = country.id[GMFunc.DefaultCountry]
  end
  
  if param2 == "a" or param3 == "a" or param4 == "a" or param5 == "a" or param6 == "a" or param7 == "a"  then SetSkill = "Average"
  elseif param2 == "g" or param3 == "g" or param4 == "g" or param5 == "g" or param6 == "g" or param7 == "g" then SetSkill = "Good"
  elseif param2 == "h" or param3 == "h" or param4 == "h" or param5 == "h" or param6 == "h" or param7 == "h" then SetSkill = "High"
  elseif param2 == "e" or param3 == "e" or param4 == "e" or param5 == "e" or param6 == "e"  or param7 == "e" then SetSkill = "Excellent"
  elseif param2 == "r" or param3 == "r" or param4 == "r" or param5 == "r" or param6 == "r" or param7 == "r" then SetSkill = "Random"
  end
  
  if param2 == "free" or param3 == "free" or param4 == "free" or param5 == "free" or param6 == "free" or param7 == "free"  then SetROE = ENUMS.ROE.WeaponFree
  elseif param2 == "hold" or param3 == "hold" or param4 == "hold" or param5 == "hold" or param6 == "hold" or param7 == "hold"  then SetROE = ENUMS.ROE.WeaponHold
  elseif param2 == "return" or param3 == "return" or param4 == "return" or param5 == "return" or param6 == "return" or param7 == "return"  then SetROE = ENUMS.ROE.ReturnFire
  end
  
  if SetCountry ~= nil 
         
   then SpawnObject:InitCountry(SetCountry)
          
        if GMFunc.DebugMode == true
                      
          then trigger.action.outText("DEBUG: Land auf " .. SetCountry .. " gesetzt!", 10)
                      
        end
          
  end
  
  if SetSkill ~= nil
  
    then SpawnObject:InitSkill(SetSkill)
    
         if GMFunc.DebugMode == true
                      
          then trigger.action.outText("DEBUG: Skill auf " .. SetSkill .. " gesetzt!", 10)
                      
         end
         
  end
  
  if SpawnCat < 2
  
    then if SpawnAlt == nil
    
          then mcoord.y = mcoord.y + 1000
          
               if GMFunc.DebugMode == true
                      
                then trigger.action.outText("DEBUG: Flughoehe auf Standard gesetzt!", 10)
                      
               end
         
         elseif SpawnAlt ~= nil 
         
           then mcoord.y = SpawnAlt   
           
                if GMFunc.DebugMode == true
                      
                 then trigger.action.outText("DEBUG: Flughoehe auf " .. mcoord.y .. " gesetzt!", 10)
                     
                end
                
         end
         
         local AIPlane = nil
         
         if NoMarkSp == true 
         
            then AIPlane = SpawnObject:Spawn()
            
         else AIPlane = SpawnObject:SpawnFromCoordinate(mcoord)
         
         end
         
         if AIPlane ~= nil and AirbaseSpawn == true
         
          then local function ABReSpawn(AIPlane)
               
                  AIPlane:RespawnAtCurrentAirbase(nil, SPAWN.Takeoff.Cold, true)
        
                  if GMFunc.DebugMode == true
    
                    then trigger.action.outText("Gruppe " .. AIPlane:GetName() .. " ohne Pilot am Boden gespawnt!", 10)
    
                  end 
               
               end
                
               timer.scheduleFunction(ABReSpawn, AIPlane, timer.getTime() + 0.25)
 
         elseif AIPlane ~= nil and KeepTasking ~= true
         
          then local PlanePosVec3 = AIPlane:GetPositionVec3()
               local PlanePosCoord = COORDINATE:NewFromVec3(PlanePosVec3)
               local AITaskCAP = AIPlane:TaskOrbitCircle( mcoord.y, 250, PlanePosCoord )
               AIPlane:SetTask( AITaskCAP, 1 )
               
               if GMFunc.EPLRS == true
                         
                then AIPlane:CommandEPLRS(true, 2)
                          
               end
               
               if SetROE ~= nil 
               
                then AIPlane:OptionROE(SetROE)
                
              end
               
         end
               
  elseif SpawnCat == 2
       
       then if mcoord:GetSurfaceType() ~= 2 and mcoord:GetSurfaceType() ~= 3  
              
              then local SpawnHeading = SpawnAlt
                   
                   if SpawnHeading ~= nil
                   
                    then SpawnObject:InitHeading(SpawnHeading, SpawnHeading)
                    
                   end
                   
                   local AIGround = nil                 
      
                   if NoMarkSp ~= true
                       
                     then AIGround = SpawnObject:SpawnFromCoordinate(mcoord)
                    
                   else AIGround = SpawnObject:Spawn()
                            
                   end
                   
                   if KeepTasking ~= true
                   
                    then local AIHold = AIGround:TaskHold()
                         AIGround:SetTask(AIHold, 1)
                         
                         if GMFunc.EPLRS == true
                         
                          then AIGround:CommandEPLRS(true, 2)
                          
                         end
                         
                         if SetROE ~= nil 
               
                          then AIGround:OptionROE(SetROE)
                
                         end
                    
                   end         
                   
                   if IsCargo == true
                   
                    then local CargoName = AIGround:GetName()
                         local CargoGroup = CARGO_GROUP:New(AIGround, "Mobile", CargoName, 500, 10)
                         
                         if CargoGroup:GetCoalition() == 1 and ctld ~= nil --CTLD integration starts here
                         
                          then table.insert(ctld.droppedTroopsRED, CargoGroup:GetObjectName())
                          
                         elseif CargoGroup:GetCoalition() == 2 and ctld ~= nil
                         
                          then table.insert(ctld.droppedTroopsBLUE, CargoGroup:GetObjectName())
                          
                         end                         
                         
                   end
  
            end
          
  elseif SpawnCat == 3
        
        then if mcoord:GetSurfaceType() == 2 or mcoord:GetSurfaceType() == 3
  
              then local AIShip = nil
                   local SpawnHeading = SpawnAlt
                   
                   if SpawnHeading ~= nil
                   
                    then SpawnObject:InitHeading(SpawnHeading, SpawnHeading)
                    
                   end
                   
                   if NoMarkSp ~= true 
                   
                    then AIShip = SpawnObject:SpawnFromCoordinate(mcoord)
                    
                   else AIShip = SpawnObject:Spawn()
                   
                   end
                   
                   if KeepTasking ~= true
                   
                    then local AIHold = AIShip:TaskHold()
                         AIShip:SetTask(AIHold, 1)
                         
                         if GMFunc.EPLRS == true
                         
                          then AIShip:CommandEPLRS(true, 2)
                          
                         end
                         
                         if SetROE ~= nil 
               
                          then AIShip:OptionROE(SetROE)
                
                         end
                    
                   end
         
             end
  
  end         
  
  SpawnObject:InitCountry(country.id[OriCountry])--resets the country back to default for the next spawn
  
end

--Init group/unit list function (inspired by work from fargo007)------------------------------------------------------------------------------------------

function GMFunc.ObjectList(param1,param2,param3,mcoord)

  if GMFunc.DebugMode == true
  
    then trigger.action.outText("DEBUG: Funktion aktiv!", 10)
    
  end

  local paramTable = { param1 = param1,
                       param2 = param2,
                       param3 = param3 }
  
  local msgOut = false
                     
  local objNum = 0
  local curObjNum = 0
  local lineCounter = 0
  local curMarkText = nil
  
  for i, param in pairs(paramTable)
  
    do if param == "msg" then msgOut = true end
    
  end
  
  local function msgComposer(object)
  
    local msg = MESSAGE:New(object:GetName(), GMFunc.MsgDispTime)
    
    if GMFunc.RestrToCoal == 1
    
      then msg:ToRed()
      
    elseif GMFunc.RestrToCoal == 2
    
      then msg:ToBlue()
      
    elseif GMFunc.RestrToCoal == 0
    
      then msg:ToCoalition(coalition.side.NEUTRAL)
      
    else msg:ToAll()
      
    end
  
  end
  
  local function markComposer(object, objNum, mcoord)
    
    curObjNum = curObjNum +1

    if lineCounter == 0 and curObjNum ~= objNum
    
      then curMarkText = object:GetName()
           lineCounter = lineCounter + 1
           
    elseif lineCounter == 0 and curObjNum == objNum
    
      then curMarkText = object:GetName()
           mcoord:MarkToAll(curMarkText)
           curMarkText = nil
    
    elseif lineCounter < 4 and curObjNum ~= objNum
    
      then curMarkText = curMarkText .. "\n" .. object:GetName()
           lineCounter = lineCounter + 1
    
    elseif lineCounter == 4 or curObjNum == objNum
   
      then curMarkText = curMarkText .. "\n" .. object:GetName()
           mcoord:MarkToAll(curMarkText)
           curMarkText = nil
           
           if lineCounter == 4 then lineCounter = 0 end     
      
    end
    
  end
  
  if param1 == "sta"
  
    then local staSet = SET_STATIC:New()  
         
         for i, param in pairs(paramTable)
         
          do if param ~= nil and param ~= "sta" and param ~= "msg" then staSet:FilterPrefixes(param) end
          
         end
         
         staSet:FilterOnce()
         
         if msgOut == true
         
          then staSet:ForEachStatic(msgComposer)
          
         else objNum = staSet:Count()
              staSet:ForEachStatic(markComposer, objNum, mcoord)
              
         end
         
  elseif param1 == "cargo"
   
    then local cargoSet = SET_CARGO:New()
         
         for i,param in pairs(paramTable)
         
          do if param ~= nil and param ~= "cargo" and param ~= "msg" then cargoSet:FilterPrefixes(param) end
          
         end
         
         cargoSet:FilterOnce()
         
         if msgOut == true 
         
          then cargoSet:ForEachCargo(msgComposer)
         
         else objNum = cargoSet:Count()
              cargoSet:ForEachCargo(markComposer, objNum, mcoord)
              
         end
  
  elseif param1 == "helo" or param1 == "plane" or param1 == "ship" or param1 == "ground" or param1 == "group" 
  
    then local groupSet = SET_GROUP:New()

         for i,param in pairs(paramTable)
         
          do if param == "helo" then groupSet:FilterCategoryHelicopter()
          
             elseif param == "plane" then groupSet:FilterCategoryAirplane()
          
             elseif param == "ship" then groupSet:FilterCategoryShip()
          
             elseif param == "ground" then groupSet:FilterCategoryGround()
             
             elseif param ~= nil and param ~= "msg" then groupSet:FilterPrefixes(param)
             
             end
          
         end
         
         groupSet:FilterOnce()
         
         if msgOut == true
         
          then groupSet:ForEachGroup(msgComposer)
          
         else objNum = groupSet:Count()
              groupSet:ForEachGroup(markComposer, objNum, mcoord)
              
         end
    
  else local staSet = SET_STATIC:New()
       local cargoSet = SET_CARGO:New()
       local groupSet = SET_GROUP:New()
       
       for i, param in pairs(paramTable)
       
        do if param ~= nil and param ~= "msg"
        
            then staSet:FilterPrefixes(param)
                 cargoSet:FilterPrefixes(param)
                 groupSet:FilterPrefixes(param)
           
           end
           
       end
       
       staSet:FilterOnce()
       cargoSet:FilterOnce()
       groupSet:FilterOnce()
       
       if msgOut == true
       
        then staSet:ForEachStatic(msgComposer)
             cargoSet:ForEachCargo(msgComposer)
             groupSet:ForEachGroup(msgComposer)
             
       else objNum = staSet:Count()
            staSet:ForEachStatic(markComposer, objNum, mcoord)
           
            objNum = cargoSet:Count()
            curObjNum = 0
            lineCounter = 0
            cargoSet:ForEachCargo(markComposer, objNum, mcoord)
           
            objNum = groupSet:Count()
            curObjNum = 0
            lineCounter = 0
            groupSet:ForEachGroup(markComposer, objNum, mcoord)
            
       end          
  
  end

end

--Init coord function------------------------------------------------------------------------------------------------------------------------

function GMFunc.ShowCoord(mcoord)

  local Lat = nil
  local Long = nil

  local LatDeg = nil
  local LatMin = nil
  local LatSec = nil
  local LatDecMin = nil
  local LatDecSec = nil
  
  local LongDeg = nil
  local LongMin = nil
  local LongSec = nil
  local LongDecMin = nil
  local LongDecSec = nil
  
  local MGRS = nil
  
  Lat, Long = coord.LOtoLL(mcoord)
  
  LatDeg, LatMin = math.modf(Lat)
  LatMin = LatMin * 60
  LatMin, LatDecMin = math.modf(LatMin)
  LatSec = LatDecMin * 60
  LatDecMin = LatDecMin * 1000
  LatSec, LatDecSec = math.modf(LatSec)
  LatDecSec = math.floor((LatDecSec*100)+0.5)
  
  LongDeg, LongMin = math.modf(Long)
  LongMin = LongMin * 60
  LongMin, LongDecMin = math.modf(LongMin)
  LongSec = LongDecMin * 60
  LongDecMin = LongDecMin * 1000
  LongSec, LongDecSec = math.modf(LongSec)
  LongDecSec = math.floor((LongDecSec*100)+0.5)
  
  MGRS = coord.LLtoMGRS(Lat, Long)
  
  local LatString = string.format("%.2i %.2i %.2i", LatDeg, LatMin, LatSec)
  local LongString = string.format("%.3i %.2i %.2i", LongDeg, LongMin, LongSec)
  
  local LatDecString = string.format("%.2i %.2i.%.3d", LatDeg, LatMin, LatDecMin)
  local LongDecString = string.format("%.3i %.2i.%.3d", LongDeg, LongMin, LongDecMin)
  
  local LatPrecString = string.format("%.2i %.2i %.2i.%.2i", LatDeg, LatMin, LatSec, LatDecSec) 
  local LongPrecString = string.format("%.3i %.2i %.2i.%.2i", LongDeg, LongMin, LongSec, LongDecSec)

  local LLString = LatString .. " / " .. LongString
  local DecString = LatDecString .. " / " .. LongDecString
  local PrecString = LatPrecString .. " / " .. LongPrecString
  
  local MGRSString = string.format("%s %s %05d %05d", MGRS.UTMZone, MGRS.MGRSDigraph, MGRS.Easting, MGRS.Northing)
  
  local CoordMarker = MARKER:New(mcoord, "LL: " .. LLString .. "\nLL Dec: " .. DecString .. "\nLL Prec: " .. PrecString .. "\nMGRS: " .. MGRSString)
  
  if GMFunc.RestrToCoal == 1
  
    then CoordMarker:ToCoaliton(coaliton.side.RED)
  
  elseif GMFunc.RestrToCoal == 2
       
    then CoordMarker:ToCoaliton(coaliton.side.BLUE)
      
  else CoordMarker:ToAll()
  
  end

end

--Init function call-------------------------------------------------------------------------------------------------------------------------

function GMFunc.CallFunction(param1, param2, param3, param4, param5, param6, param7, mcoord)
  
  if GMFunc.DebugMode == true
    
    then trigger.action.outText("Functioncall aktiv", 10)
    
  end 
  
  local paramTable = { param2 = param2,
                       param3 = param3,
                       param4 = param4,
                       param5 = param5,
                       param6 = param6,
                       param7 = param7 }
  
  local remString = nil
  local firstIndex = nil
  local firstIndexStart = nil
  local sndIndex = nil
  local sndIndexStart = nil
  local trdIndex = nil
  local trdIndexStart = nil
  
  if string.find(param1, ".", 1, true)
  
    then sndIndexStart = string.find(param1, ".", 1, true)
         firstIndex = string.sub(param1, 0, sndIndexStart-1)
         remString = string.sub(param1, sndIndexStart+1)
                     
         if string.find(remString, ".", 1, true)
                     
          then trdIndexStart = string.find(remString, ".", 1, true)
               sndIndex = string.sub(remString, 0, trdIndexStart-1)
               trdIndex = string.sub(remString, trdIndexStart+1)
               
         else sndIndex = remString
               
         end
         
  else firstIndex = param1
    
  end
  
  for i, param in pairs(paramTable) --check if any of the given parameters are references to global vars 
  
    do if _G[param] ~= nil 
    
        then paramTable[i] = _G[param]
    
       elseif string.find(param, ".", 1, true)
    
        then local varSndIndexStart = string.find(param, ".", 1, true)
             local varFstIndex = string.sub(param, 0, varSndIndexStart-1)
             local varSndIndex = nil
             local varTrdIndex = nil
             local varRemString = string.sub(param, varSndIndexStart+1)
             
             if string.find(varRemString, ".", 1, true)
             
              then local varTrdIndexStart = string.find(varRemString, ".", 1, true)
                   varSndIndex = string.sub(param, 0, varTrdIndexStart-1)
                   varTrdIndex = string.sub(param, varTrdIndexStart+1)
              
             else varSndIndex = varRemString
             
             end
             
             if varSndIndex ~=nil and varTrdIndex ~= nil and _G[varFstIndex][varSndIndex][varTrdIndex] ~= nil
             
              then paramTable[i] = _G[varFstIndex][varSndIndex][varTrdIndex]
                    
             elseif varSndIndex ~=nil and _G[varFstIndex][varSndIndex]
              
              then paramTable[i] = _G[varFstIndex][varSndIndex]
              
             end
        
       elseif param == "mcoord" 
       
        then paramTable[i] = mcoord
       
       end
 
  end 
  
  if GMFunc.DebugMode == true
    
    then if firstIndex ~= nil then trigger.action.outText("1st Index = " .. firstIndex, 10) end
         if sndIndex ~= nil then trigger.action.outText("2nd Index = " .. sndIndex, 10) end
         if trdIndex ~= nil then trigger.action.outText("3rd Index = " .. trdIndex, 10) end
    
  end 
  
  if firstIndex ~= nil and sndIndex ~= nil and trdIndex ~= nil
  
    then _G[firstIndex][sndIndex][trdIndex](paramTable.param2, paramTable.param3, paramTable.param4, paramTable.param5, paramTable.param6, paramTable.param7, mcoord)
    
  elseif firstIndex ~= nil and sndIndex ~= nil 
  
    then _G[firstIndex][sndIndex](paramTable.param2, paramTable.param3, paramTable.param4, paramTable.param5, paramTable.param6, paramTable.param7, mcoord)
    
  elseif firstIndex ~= nil 
  
    then _G[firstIndex](paramTable.param2, paramTable.param3, paramTable.param4, paramTable.param5, paramTable.param6, paramTable.param7, mcoord)
    
  end

end

--Init Beacon Function-----------------------------------------------------------------------------------------------------------------------

function GMFunc.CreateNavBeacon(param1,param2,param3,param4)
  
  local bUnit = nil
  local bType = nil
  local bSystem = nil
  local freq = nil
  local tXY = nil
  local tAA = false
  
  if param1 == "navtcn" 
  
    then bType = 4
         bSystem = 3
         tXY = "X"
         
  elseif param1 == "aatcnx"
  
    then bType = 4 
         bSystem = 4
         tXY = "X"
         tAA = true
         
  elseif param1 == "aatcny" 
  
    then bType = 4
         bSystem = 5
         tXY = "Y"
         tAA = true
  
  elseif param1 == "gndtcnx" 
  
    then bType = 4
         bSystem = 18
         tXY = "X"
  
  elseif param1 == "gndtcny" 
  
    then bType = 4
         bSystem = 19
         tXY = "Y"

  end 
  
  if bType == 4 
    
    then freq = GMFunc.getTACANFrequency(tonumber(param2), tXY)
  
  else freq = tonumber(param2) * 1000000
  
  end
  
  if Unit.getByName(param3) ~= nil
  
    then local bUnit = Unit.getByName(param3)
         local bController = bUnit:getGroup():getController()
         local bUnitID = bUnit:getID()
         local bTask = nil
         local bCS = bUnit:getName()
         
         if param4 ~= nil 
         
          then bCS = tostring(param4)
          
         end
         
         if bType == 4
         
          then local tChannel = tonumber(param2) 
          
               bTask = {  ["id"] = "ActivateBeacon",
                          ["params"] = { ["type"] = bType,
                                         ["AA"] = tAA,
                                         ["unitId"] = bUnitID,
                                         ["modeChannel"] = tXY,
                                         ["channel"] = tChannel,
                                         ["system"] = bSystem,
                                         ["callsign"] = bCS,
                                         ["bearing"] = true,
                                         ["frequency"] = freq }
                        }               
         
         end
         
         bController:setCommand(bTask)
         
         --trigger.action.outText("DEBUG: Beacon gestartet! bType: " .. bType .." bSystem: " .. bSystem .. " freq: " .. freq , 60)
         
  end
  
end

--Init homing beacon function-------------------------------------------------------------------------------------------------------------------------

function GMFunc.CreateHomBeacon(param1, param2, param3, param4, param5, param6, mvec3)

  local paramTable = { param3 = param3,
                       param4 = param4,
                       param5 = param5,
                       param6 = param6 }
                       
  local radioGroup = nil
  local rMod = 0
  local rLoop = true
  local rPower = 1000
  local rFreq = nil
  local soundPath = "l10n/DEFAULT/" .. param2
  
  if string.find(param1, "khz")
  
    then rFreq = string.sub(param1, 1, string.find(param1, "khz") - 1) * 1000
    
  else rFreq = tonumber(param1) * 1000000
  
  end
  
  for i, param in pairs(paramTable)
  
    do if param == "noloop" then rLoop = false
       elseif tonumber(param) then rPower = tonumber(param)
       elseif param == "fm" then rMod = 1
       elseif Group.getByName(param) then radioGroup = Group.getByName(param)
       end
       
  end 
       
  if radioGroup ~= nil
  
    then local radioContr = radioGroup:getController()
    
         local setFreq = { id = 'SetFrequency', 
                           params = { frequency = rFreq, 
                                      modulation = rMod,
                                      power = rPower } 
                          }
                         
         local startTrans = { id = "TransmitMessage",
                              params = { loop = rLoop,
                                         file = soundPath }
                             }             
         
         radioContr:setCommand(setFreq)
         
         radioContr:setCommand(startTrans)
         
  else trigger.action.radioTransmission(soundPath,mvec3,rMod,rLoop,rFreq,rPower)
         
  end
  

end

--Init function to remove homing beacons from groups-----------------------------------------------------------------------------------------

function GMFunc.RemHomBeacon(param1)

  local remRadio = { id = "StopTransmission", 
                     params = {} 
                   }
                  
  if Group.getByName(param1)
  
    then local rContr = Group.getByName(param1):getController()
    
         rContr:setCommand(remRadio)
         
  end

end

--Init function to disable navigation beacons-------------------------------------------------------------------------------------------------

function GMFunc.RemNavBeacon(param1)

  local remBeacon = { id = "DeactivateBeacon", 
                      params = {} 
                     }
                  
  if Unit.getByName(param1)
  
    then local bContr = Unit.getByName(param1):getGroup():getController()
    
         bContr:setCommand(remBeacon)
         
  end

end
    
--Init Eventhandler for Marks----------------------------------------------------------------------------------------------------------------
    
GMFunc.MarkHandler = {}
    
function GMFunc.MarkHandler:onEvent(event)
    
    if event.id == 25 and GMFunc.DebugMode ~= true
    
        then trigger.action.outText(" ", 0, true) --Überschreibt "Created New Mark" 
    
    elseif event.id == 27 and string.find(event.text, GMFunc.CmdSymbol) and (GMFunc.PW == nil or string.find(event.text, GMFunc.PW) ~= nil)
    
        then if event.coalition == GMFunc.RestrToCoal or GMFunc.RestrToCoal == nil
        
                then local full = nil
                     local remString = nil
                     local cmd = nil
                     local paramStart = nil
                     local allParams = {}

                     local mcoord = COORDINATE:New(event.pos.x, event.pos.y, event.pos.z)
                     local mvec3 = event.pos
                     
                     if GMFunc.PW ~= nil 
                     
                      then full = string.sub(event.text, string.len(GMFunc.PW)+2)
                     
                     else full = string.sub(event.text, 2)
                     
                     end
                     
                     full = full .. GMFunc.CmdSymbol --necessary to make the while loop below catch the last parameter in the input
                                      
                     if string.find(full, GMFunc.CmdSymbol)
             
                      then local paramStart = string.find(full, GMFunc.CmdSymbol)
                           local paramNum = 1                          
                           cmd = string.sub(full, 0, paramStart - 1)
                           remString = string.sub(full, paramStart + 1) 
                           
                           while string.find(remString, GMFunc.CmdSymbol)
                           
                            do local paramInd = "param" .. paramNum
                               paramStart = string.find(remString, GMFunc.CmdSymbol)
                               allParams[paramInd] = string.sub(remString, 0, paramStart - 1)
                               remString = string.sub(remString, paramStart + 1)
                               paramNum = paramNum + 1
                            
                           end    
                
                     end                     
                     
                     if GMFunc.DebugMode == true
             
                     then trigger.action.outText("Voller Text = " .. full, 10)
                          trigger.action.outText("Befehl = " .. cmd, 10)
                          for i, param in pairs(allParams)
                          
                            do trigger.action.outText(i .. " = " .. param, 10)
                            
                          end
                          
                     end  
             
                     if cmd == "drawdel"
              
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Loeschen-Funktion, Zeichnung gestartet!", 10)
                    
                           end
                   
                           GMFunc.DeleteDrawing(allParams.param1, mcoord)
                     
                     elseif cmd == "del"
              
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Loeschen-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.Delete(allParams.param1, mcoord)
                   
                     elseif cmd == "flag"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Flaggen-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.UserFlagSet(allParams.param1, allParams.param2)
                           
                     elseif cmd == "drawline"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Zeichenfunktion, Linie gestartet!", 10)
                    
                           end
                   
                           GMFunc.DrawLine(allParams.param1, allParams.param2, allParams.param3, allParams.param4, allParams.param5, mcoord)
                           
                     elseif cmd == "drawarrow"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Zeichenfunktion, Pfeil gestartet!", 10)
                    
                           end
                   
                           GMFunc.DrawArrow(allParams.param1,allParams.param2,allParams.param3,allParams.param4,allParams.param5,allParams.param6,allParams.param7,mcoord)
                           
                     elseif cmd == "drawcircle"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Zeichenfunktion, Kreis gestartet!", 10)
                    
                           end
                   
                           GMFunc.DrawCircle(allParams.param1,allParams.param2,allParams.param3,allParams.param4,allParams.param5,allParams.param6,allParams.param7,mcoord)
                     
                     elseif cmd == "drawrect"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Zeichenfunktion, Rechteck gestartet!", 10)
                    
                           end
                   
                           GMFunc.DrawRect(allParams.param1, allParams.param2, allParams.param3, allParams.param4, allParams.param5, allParams.param6, allParams.param7, mcoord)
                           
                     elseif cmd == "drawpoly"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Zeichenfunktion, Poly gestartet!", 10)
                    
                           end
                   
                           GMFunc.DrawPoly(allParams.param1, allParams.param2, allParams.param3, allParams.param4, allParams.param5, allParams.param6, allParams.param7, mcoord)
                           
                     elseif cmd == "drawtext"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Zeichenfunktion, Text gestartet!", 10)
                    
                           end
                   
                           GMFunc.DrawText(allParams.param1,allParams.param2,allParams.param3,allParams.param4,allParams.param5,allParams.param6,allParams.param7,mcoord)
                           
                     elseif cmd == "ctrlon"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Control-Toggle-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.ControlToggle(allParams.param1)
             
                     elseif cmd == "sound"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Sound-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.PlaySound(allParams.param1, allParams.param2)
              
                     elseif cmd == "text"
             
                      then if GMFunc.DebugMode == true 
              
                            then trigger.action.outText("DEBUG: Text-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.ShowText(allParams.param1, allParams.param2, allParams.param3, allParams.param4)
                   
                     elseif cmd == "flare"
             
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Flare-Funktion gestartet!", 10)
                    
                           end
                    
                           GMFunc.Flare(allParams.param1, allParams.param2, allParams.param3, mcoord)
                   
                     elseif cmd == "smoke"
             
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Smoke-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.Smoke(allParams.param1, allParams.param2, mcoord)
                   
                     elseif cmd == "exp"
             
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Explode-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.ExplodeAtMark(allParams.param1, allParams.param2, allParams.param3, mcoord)
                   
                     elseif cmd == "illum"
             
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Leuchtgranaten-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.IllumAtMark(allParams.param1, allParams.param2, mcoord)
                   
                     elseif cmd == "sf"
             
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Rauch und Feuer Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.FXFireSmoke(allParams.param1, allParams.param2, mvec3)
             
                     elseif cmd == "inv"
              
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Invisible-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.SetInvisible(allParams.param1, allParams.param2)
                   
                     elseif cmd == "imm"
              
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Invincible-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.SetImmortal(allParams.param1, allParams.param2)
                   
                     elseif cmd == "ai"
             
                      then if GMFunc.DebugMode == true
                    
                            then trigger.action.outText("DEBUG: KI-Togglefunktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.ToggleAI(allParams.param1, allParams.param2, mcoord) 
                                
                     elseif cmd == "wp"
             
                      then if GMFunc.DebugMode == true
                    
                            then trigger.action.outText("DEBUG: Wegpunkt-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.RouteAI(allParams.param1, allParams.param2, allParams.param3, allParams.param4, mcoord)
                   
                     elseif cmd == "rtb"
             
                      then if GMFunc.DebugMode == true
                    
                            then trigger.action.outText("DEBUG: RTB-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.AIReturnToBase(allParams.param1, allParams.param2, mcoord)
                   
                     elseif cmd == "lz"
             
                      then if GMFunc.DebugMode == true
                    
                            then trigger.action.outText("DEBUG: Helikopter-Landefunktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.HeloLand(allParams.param1, allParams.param2, mcoord)      
             
                     elseif cmd == "orbit"
             
                      then if GMFunc.DebugMode == true
                    
                            then trigger.action.outText("DEBUG: Orbit-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.Orbit(allParams.param1, allParams.param2, allParams.param3, allParams.param4, mcoord)
              
                     elseif cmd == "esc"
             
                      then if GMFunc.DebugMode == true
                    
                            then trigger.action.outText("DEBUG: Eskorten-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.AirEscortAI(allParams.param1, allParams.param2, allParams.param3, allParams.param5, allParams.param6)
                           
                     elseif cmd == "fol"
             
                      then if GMFunc.DebugMode == true
                    
                            then trigger.action.outText("DEBUG: Follow-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.AirFollowAI(allParams.param1, allParams.param2, allParams.param3, allParams.param4, allParams.param5)
                   
                     elseif cmd == "unboard"
             
                      then if GMFunc.DebugMode == true
                    
                            then trigger.action.outText("DEBUG: Aussteige-Funktion, mobile Gruppe gestartet!", 10)
                    
                           end
                   
                           GMFunc.CargoDrop(allParams.param1, mcoord)
                   
                     elseif cmd == "board"
             
                      then if GMFunc.DebugMode == true
                    
                            then trigger.action.outText("DEBUG: Pickup-Funktion, mobile Gruppe gestartet!", 10)
                    
                           end
                   
                           GMFunc.CargoPickup(allParams.param1, allParams.param2, mcoord)
             
                     elseif cmd == "sta"
             
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Static Spawn-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.StaticSpawn(allParams.param1, allParams.param2, allParams.param3, allParams.param4, mcoord)
                           
                     elseif cmd == "ctldcr"
             
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Funktion zum Spawnen von CTLD-Crates gestartet!", 10)
                    
                           end
                   
                           GMFunc.SpawnCTLDCrate(allParams.param1, allParams.param2, mcoord)
                           
                     elseif cmd == "ctldgr"
             
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Funktion zum Spawnen von CTLD-Gruppen gestartet!", 10)
                    
                           end
                   
                           GMFunc.SpawnExtractableCTLD(allParams.param1, allParams.param2, allParams.param3, mcoord)
                     
                     elseif cmd == "s"
             
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Spawn-Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.Spawn(allParams.param1, allParams.param2, allParams.param3, allParams.param4, allParams.param5, allParams.param6, allParams.param7, mcoord)
                   
                     elseif cmd == "act"
      
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Late Activation Funktion gestartet!", 10)
                    
                           end
                   
                           GMFunc.LateActivate(allParams.param1)
                           
                     elseif cmd == "?"
      
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Query Funktion gestartet!", 10)
                    
                           end
                           
                           GMFunc.WhatsThis(allParams.param1, mcoord)
                           
                     elseif cmd == "func"
      
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Funktionscall gestartet!", 10)
                    
                           end
                           
                           GMFunc.CallFunction(allParams.param1,allParams.param2,allParams.param3,allParams.param4,allParams.param5,allParams.param6,allParams.param7, mcoord)
                           
                     elseif cmd == "coord" 
      
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Koordinatenausgabe gestartet!", 10)
                    
                           end
                           
                           GMFunc.ShowCoord(mcoord)
                           
                     elseif cmd == "list" 
      
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Objektliste gestartet!", 10)
                    
                           end
                           
                           GMFunc.ObjectList(allParams.param1,allParams.param2,allParams.param3,mcoord)
                           
                     elseif cmd == "actnavbcn" 
      
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Funktion gestartet!", 10)
                    
                           end
                           
                           GMFunc.CreateNavBeacon(allParams.param1,allParams.param2,allParams.param3,allParams.param4,mcoord)
                           
                     elseif cmd == "acthombcn" 
      
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Funktion gestartet!", 10)
                    
                           end
                           
                           GMFunc.CreateHomBeacon(allParams.param1,allParams.param2,allParams.param3,allParams.param4,allParams.param5,allParams.param6,mvec3)
                           
                     elseif cmd == "remhombcn" 
      
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Funktion gestartet!", 10)
                    
                           end
                           
                           GMFunc.RemHomBeacon(allParams.param1)
                           
                     elseif cmd == "remnavbcn" 
      
                      then if GMFunc.DebugMode == true
              
                            then trigger.action.outText("DEBUG: Funktion gestartet!", 10)
                    
                           end
                           
                           GMFunc.RemNavBeacon(allParams.param1)
              
                     end
             end
    end       
    
end
    
world.addEventHandler(GMFunc.MarkHandler)

trigger.action.outText( "Gamemaster_Functions loaded!", 10 )