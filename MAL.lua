function string:split( inSplitPattern, outResults )
  if not outResults then
    outResults = { }
  end
  local theStart = 1
  local theSplitStart, theSplitEnd = string.find( self, inSplitPattern, theStart )
  while theSplitStart do
    table.insert( outResults, string.sub( self, theStart, theSplitStart-1 ) )
    theStart = theSplitEnd + 1
    theSplitStart, theSplitEnd = string.find( self, inSplitPattern, theStart )
  end
  table.insert( outResults, string.sub( self, theStart ) )
  return outResults
end
JSON = nil
function Initialize()
    PluginDataMeasure = SKIN:GetMeasure('measurePluginData')
	  PluginData = PluginDataMeasure:GetStringValue()

    InitializeMeters()
    JSON = dofile(SKIN:MakePathAbsolute("JSON.lua"))

    local raw_json_text = PluginData

    local plugin_data_table = JSON:decode(raw_json_text) -- decode example
    print(PluginData)

    local upcoming_request_state = plugin_data_table.UpcomingAnime.UpcomingRequestStatus -- false means processing,true means done
    local latest_updates_state = plugin_data_table.LatestUpdates.LatestUpdatesRequestStatus

    local shouldUpdate = false

    if upcoming_request_state == false then
      shouldUpdate = true
    end

    if latest_updates_state == false then
      shouldUpdate = true
    end

    ParsePluginData(plugin_data_table)

    -- -- Parse Upcoming Anime
    -- for i = 0, #upcoming_anime_array do
    --     local animeName = upcoming_anime_array[i+1].Name
    --     local day = upcoming_anime_array[i+1].UserTimezone.rd_weekday
    --     local hour = upcoming_anime_array[i+1].UserTimezone.rd_time
    --     print(hour)

    --     SKIN:Bang('!SetOption','TitleLine'..i,'Text',animeName)
    --     SKIN:Bang('!SetOption','DatesDaysLine'..i,'Text',day)
    --     SKIN:Bang('!SetOption','DatesHoursLine'..i,'Text',hour)
    -- end

    -- local splitTable = PluginData:split("\\n")
    -- for i = 1, #splitTable do
    --     -- Split again
    --     local seperatorSplit = splitTable[i]:split("\\seperator")

    --     local animeName = seperatorSplit[1]
    --     local day = seperatorSplit[2]
    --     local hour = seperatorSplit[3]
    --     print(hour)
    -- end
end

function ParsePluginData(pluginDataTable)
  local upcoming_request_state = pluginDataTable.UpcomingAnime.UpcomingRequestStatus -- false means processing,true means done
  local latest_updates_state = pluginDataTable.LatestUpdates.LatestUpdatesRequestStatus

  local needs_update = not (upcoming_request_state and latest_updates_state)

  if needs_update then
    SKIN:Bang('!SetOption','measurePluginData','UpdateDivider',1)
  else
    SKIN:Bang('!SetOption','measurePluginData','UpdateDivider',-1)
  end
  
  -- Upcoming Anime
  if upcoming_request_state then
    local upcoming_anime_array = pluginDataTable.UpcomingAnime.UpcomingAnime
    for i = 1, #upcoming_anime_array do
      local animeName = upcoming_anime_array[i].Name
      local day = upcoming_anime_array[i].UserTimezone.rd_weekday
      local hour = upcoming_anime_array[i].UserTimezone.rd_time

      local mouseUpAction = '!Execute ["http://myanimelist.net/anime/'..upcoming_anime_array[i].MalId

      local r = i - 1

      SKIN:Bang('!SetOption','TitleLine'..r,'Text',animeName)
      SKIN:Bang('!SetOption','TitleLine'..r,'LeftMouseUpAction',mouseUpAction..'"]')

      SKIN:Bang('!SetOption','DatesDaysLine'..r,'Text',day)
      SKIN:Bang('!SetOption','DatesHoursLine'..r,'Text',hour)
    end
  end

  -- Latest Updates
  if latest_updates_state then
    local latest_updates_array = pluginDataTable.LatestUpdates.LatestUpdates
    for i = 1, #latest_updates_array do
      local animeName = latest_updates_array[i].Title
      local userStatus = latest_updates_array[i].UserStatus
      local statusText = "..."

      local mouseUpAction = '!Execute ["http://myanimelist.net/anime/'..latest_updates_array[i].MalId

      if userStatus == 2 then
        statusText = "Completed"
      end
      if userStatus == 1 then
        local watchedEpisodes = latest_updates_array[i].WatchedEpisodes
        local totalEpisodes = latest_updates_array[i].TotalEpisodes

        statusText = watchedEpisodes .. "/" .. totalEpisodes
      end
      if userStatus == 3 then
        statusText = "On Hold"
      end
      if userStatus == 4 then
        statusText = "Dropped"
      end
      if userStatus == 6 then
        statusText = "PTW"
      end

      local r = i - 1
      local latestUpdateLineText = "LatestUpdateLine" ..r
      local latestUpdateLineStatusText = "LatestUpdateLine" ..r

      SKIN:Bang('!SetOption',latestUpdateLineText ..'Title','Text',animeName)
      SKIN:Bang('!SetOption',latestUpdateLineText ..'Title','LeftMouseUpAction',mouseUpAction..'"]')
      SKIN:Bang('!SetOption',latestUpdateLineStatusText ..'Status','Text',statusText)
    end    
  end
end

function Update()
  PluginDataMeasure = SKIN:GetMeasure('measurePluginData')
	PluginData = PluginDataMeasure:GetStringValue()
  ParsePluginData(JSON:decode(PluginData))
end


function InitializeMeters()
  for i = 0, 4,1 do
    -- Upcoming Anime
    SKIN:Bang('!SetOption','TitleLine'..i,'Text',"Fetching...")
    SKIN:Bang('!SetOption','DatesDaysLine'..i,'Text','...')
    SKIN:Bang('!SetOption','DatesHoursLine'..i,'Text','...')

    local latestUpdateLineText = "LatestUpdateLine" ..i
    local latestUpdateLineStatusText = "LatestUpdateLine" ..i
    -- Latest Updates
    SKIN:Bang('!SetOption',latestUpdateLineText ..'Title','Text',"Fetching...")
    SKIN:Bang('!SetOption',latestUpdateLineStatusText ..'Status','Text',"..")
  end
end