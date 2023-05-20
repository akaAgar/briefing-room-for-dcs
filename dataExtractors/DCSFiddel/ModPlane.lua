-- USAGE:
-- Go to: https://develop.dcsfiddle.pages.dev/ (complete setup steps and confirm working)
-- Set Environment to GUI
-- Paste Code into window
-- Change targetModule to the one you want
-- Send command
-- Copy returned data and paste objects into DatabaseJSON\UnitPlanesMod.json (Remove Starting [ and trailing ] and remember , is needed between each object)
-- WARNING: LUA => JSON parsing isn't ideal some fields will show as {} but need to be []  (known ones are categories, extraProps, specificCallnames, payloadPresets (tasks and pylons))
-- WARNING: Be careful of any Custom Liveries you have added they will show up here too

--- GUI:default:type
local targetModule = "F-16C bl.50 AI" -- Set your target module here this is the one found in missions
local loadoutUtils = require('me_loadoututils')
me_loadoututils.initBriefingRoomPayloads(nil, nil, nil)
local units = me_db_api.db.Units.Planes.Plane
local _list = {}
local banList = {
                  -- Rows of data that seems to be very broken
    [34] = true,  -- MiG-29S
    -- [69] = true, -- KC130 fix pending: WorldID == WSTYPE_PLACEHOLDER,  =>  WorldID = WSTYPE_PLACEHOLDER, DCS World OpenBeta\CoreMods\aircraft\AV8BNA\KC130.lua
    -- [70] = true, -- KC135MPRS fix pending: WorldID == WSTYPE_PLACEHOLDER,  =>  WorldID = WSTYPE_PLACEHOLDER, DCS World OpenBeta\CoreMods\aircraft\AV8BNA\KC135MPRS.lua
}
for k, v in pairs(units) do
    if banList[k] == nil and v[1] == nil and v['_origin'] == targetModule then
        local schemes = {}
        for ck, cv in pairs(me_db.db.Countries) do -- This is slow need to find a way to only iterate though countries that are actually used
            local sub_scheme = {}
            local liveriesData = DCS.getObjectLiveriesNames(string.gsub(v.type, '/', '_'), cv.ShortName,
                string.lower(require('i18n').getLocale()))

            if liveriesData and not (next(liveriesData) == nil) then
                for k, v in ipairs(liveriesData) do
                    table.insert(sub_scheme, v[2])
                end
                schemes[tostring(cv.WorldID)] = sub_scheme
            end
        end
        v['paintSchemes'] = schemes
        v['payloadPresets'] = me_loadoututils.getUnitPayloads(v.type)
        local extraProps = {}
        for kProp, vProp in pairs(v.AddPropAircraft) do
            table.insert(extraProps, {
                id = vProp.id,
                defValue = vProp.defValue
            })
        end
        local countries = {}
        local countriesWorldID = {}
        for kCountry, vCountry in pairs(me_db.db.Countries) do
            for kCountryUnit, vCountryUnit in pairs(vCountry.Units.Planes.Plane) do
                if vCountryUnit.Name == v.type then
                    if vCountryUnit.in_service ~= nil then
                        table.insert(countries, vCountry.Name)
                        table.insert(countriesWorldID, vCountry.WorldID)
                    end
                    break
                end
            end
        end
        table.insert(_list, {
            type = v.type,
            displayName = v.DisplayName,
            categories = v.Categories,
            module = v._origin,
            tasks = v.Tasks,
            paintSchemes = v.paintSchemes,
            payloadPresets = v.payloadPresets,
            EPLRS = v.EPLRS,
            fuel = v.M_fuel_max,
            flares = v.passivCounterm.flare.default,
            chaff = v.passivCounterm.chaff.default,
            extraProps = extraProps,
            panelRadio = v.panelRadio,
            radio = {
                frequency = v.HumanRadio.frequency,
                modulation = v.HumanRadio.modulation,
            },
            ammoType = v.ammo_type_default,
            countries = countries,
            countriesWorldID = countriesWorldID,
            inheriteCommonCallnames = v.InheriteCommonCallnames,
            specificCallnames = v.SpecificCallnames,
            maxAlt = v.H_max,
            cruiseSpeed = v.V_opt,
            shape = v.Shape,
            height = v.height,
            length = v.length,
            width = v.width
        })
    end
end

return _list
