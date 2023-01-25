httpd = {}
httpd.bind_address = "localhost"
httpd.bind_port = 12080
httpd.allow_cross_origin = true
httpd.version = "2023-01-24T22:14:45+00:00"
do
    local require = require
    local loadfile = loadfile

    local httpd_state = {}

    -- this function converts base64 to string
    function from_base64(data)
        local b = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/'
        data = string.gsub(data, '[^' .. b .. '=]', '')
        return (data:gsub('.', function(x)
            if (x == '=') then
                return ''
            end
            local r, f = '', (b:find(x) - 1)
            for i = 6, 1, -1 do
                r = r .. (f % 2 ^ i - f % 2 ^ (i - 1) > 0 and '1' or '0')
            end
            return r;
        end):gsub('%d%d%d?%d?%d?%d?%d?%d?', function(x)
            if (#x ~= 8) then
                return ''
            end
            local c = 0
            for i = 1, 8 do
                c = c + (x:sub(i, i) == '1' and 2 ^ (8 - i) or 0)
            end
            return string.char(c)
        end))
    end

    -- include some functions from lua-users.org/wiki/StringRecipes
    function string:split(sSeparator, nMax, bRegexp)
        assert(sSeparator ~= '')
        assert(nMax == nil or nMax >= 1)

        local aRecord = {}

        if self:len() > 0 then
            local bPlain = not bRegexp
            nMax = nMax or -1

            local nField = 1
            nStart = 1
            local nFirst, nLast = self:find(sSeparator, nStart, bPlain)
            while nFirst and nMax ~= 0 do
                aRecord[nField] = self:sub(nStart, nFirst - 1)
                nField = nField + 1
                nStart = nLast + 1
                nFirst, nLast = self:find(sSeparator, nStart, bPlain)
                nMax = nMax - 1
            end
            aRecord[nField] = self:sub(nStart)
        end

        return aRecord
    end
    local function url_decode(str)
        str = string.gsub(str, "+", " ")
        str = string.gsub(str, "%%(%x%x)", function(h)
            return string.char(tonumber(h, 16))
        end)
        str = string.gsub(str, "\r\n", "\n")
        return str
    end

    package.path = package.path .. ";.\\LuaSocket\\?.lua"
    package.cpath = package.cpath .. ";.\\LuaSocket\\?.dll"

    local socket = require("socket")
    local string = require("string")
    local JSON = loadfile("Scripts\\JSON.lua")()
    httpd.JSON = JSON
    httpd_state.httpd_acceptor = nil
    httpd_state.http_acceptor = socket.bind(httpd.bind_address, httpd.bind_port, 10)
    httpd_state.http_acceptor:settimeout(.0001)
    httpd_state.http_map = {} -- connection -> { handler => handler_func, txbuf => string } map

    httpd.url_handlers = {}
    
    httpd.url_handlers["^/$"] = function(request, response)
        response:return_text("UP")
    end
    
    httpd.url_handlers["^/health$"] = function(requst, response)
        res = {}
        res.status = "UP"
        res.version = httpd.version
        response:return_json(res)
    end
    
    httpd.url_handlers["^/api$"] = function(request, response)
        local commandstring = from_base64(request._GET["command"])
    
        env.info("Executing command:" .. commandstring)
    
        local result = assert(loadstring(commandstring))()
    
        response:return_json(result)
    end

    local function http_handler(conn, conn_info)
        local request = {}

        local request_line = coroutine.yield()

        local url_string -- url + query string
        request.method, url_string, request.http_version = request_line:gmatch("(%S+) (%S+) (%S+)")()
        -- parse URL
        local split_url_string = url_string:split("?", 1)
        request.url = split_url_string[1]
        request._GET = {}
        if split_url_string[2] then -- parse the query string
            local _GET_elements = split_url_string[2]:split("&")
            for i, elem in pairs(_GET_elements) do
                local split_elem = elem:split("=", 1)
                if #split_elem == 2 then
                    request._GET[url_decode(split_elem[1])] = url_decode(split_elem[2])
                end
            end
        end

        request.headers = {}
        while true do
            local header_line = coroutine.yield()
            if header_line == "" then
                break
            end

            local split_header = header_line:split(":", 1)
            if #split_header == 2 then
                request.headers[split_header[1]] = split_header[2]:gsub("^%s*(.-)%s*$", "%1")
            end
        end

        local response = {}
        response.headers = {
            ["Server"] = "DCS Mission Webserver " .. httpd.version,
            ["Connection"] = "close",
            ["Cache-Control"] = "no-cache, no-store, must-revalidate",
            ["Pragma"] = "no-cache",
            ["Expires"] = "0"
        }
        response.set_status = function(response, status)
            response.status = status
            response.status_text = "Unknown"
            if status == 200 then
                response.status_text = "OK"
            end
            if status == 404 then
                response.status_text = "Not Found"
            end
            if status == 500 then
                response.status_text = "Internal Server Error"
            end
        end
        response.error_404 = function(response)
            response:return_text("404 - Not Found!")
            response:set_status(404)
        end
        response.return_json = function(response, data)
            response:set_status(200)
            response.content_type = "application/json"
            response.body = JSON:encode(data)
        end
        response.return_text = function(response, data)
            response:set_status(200)
            response.content_type = "text/plain"
            response.body = data
        end
        response.return_html = function(response, data)
            response:set_status(200)
            response.content_type = "text/html"
            response.body = data
        end

        response:return_text("No URL handler for the following URL: " .. request.url)
        response:set_status(404)

        local handler_found = false

        if not handler_found then
            for url_prefix, url_handler in pairs(httpd.url_handlers) do
                if request.url:match(url_prefix) then
                    handler_found = true
                    response:return_text("URL handler did not set a response.")
                    response:set_status(500)
                    request.url_remainder = request.url:sub(url_prefix:len())
                    local bool, err = pcall(url_handler, request, response)
                    if not bool then
                        response:return_text("failed to execute url handler for " .. request.url .. ", error: " ..
                                tostring(err))
                        response:set_status(500)
                    end
                    break
                end
            end
        end

        local function tx(data)
            conn_info.txbuf = conn_info.txbuf .. data
        end

        response.headers["Content-Type"] = response.content_type
        response.headers["Content-Length"] = string.len(response.body)
        if httpd.allow_cross_origin then
            response.headers["Access-Control-Allow-Origin"] = "*"
            response.headers["Access-Control-Allow-Headers"] = request.headers["Access-Control-Request-Headers"]
        end

        tx("HTTP/1.1 " .. response.status .. " " .. response.status_text .. "\r\n")
        for header_name, header_value in pairs(response.headers) do
            tx(header_name .. ": " .. header_value .. "\r\n")
        end
        tx("\r\n")
        tx(response.body)

        return "close"
    end

    local function doComms(arg, time)
        local http_newconn = httpd_state.http_acceptor:accept()
        if http_newconn then
            http_newconn:settimeout(.0001)
            local newconn_info = {
                handler = coroutine.create(http_handler),
                txbuf = "",
                close_when_done = false
            }
            coroutine.resume(newconn_info.handler, http_newconn, newconn_info)
            httpd_state.http_map[http_newconn] = newconn_info
        end

        local dead_connections = {}
        for conn, conn_info in pairs(httpd_state.http_map) do
            -- httpd_state.env.info("processing a connection")
            if conn_info.txbuf:len() > 0 then -- transmit
                httpd_state.env.info("-- tx, buflen=" .. conn_info.txbuf:len())
                local bytes_sent = nil
                conn:settimeout(1)
                local ret1, ret2, ret3 = conn:send(conn_info.txbuf)
                conn:settimeout(.0001)
                if ret1 then
                    bytes_sent = ret1
                    httpd_state.env.info("-- tx: ret1")
                else
                    httpd_state.env.info("-- tx: not ret1")
                    bytes_sent = ret3
                end
                httpd_state.env.info("-- tx: bytes_sent is: " .. bytes_sent)
                httpd_state.env.info("-- txbuf len before: " .. conn_info.txbuf:len())
                conn_info.txbuf = conn_info.txbuf:sub(bytes_sent + 1)
                httpd_state.env.info("-- txbuf len after: " .. conn_info.txbuf:len())
            elseif conn_info.txbuf:len() == 0 and conn_info.close_when_done then -- close connection
                httpd_state.env.info("-- closing")
                conn:close()
                dead_connections[#dead_connections + 1] = conn
            else -- receive
                httpd_state.env.info("-- rx")
                local num_repeats_left = 10
                while num_repeats_left > 0 do
                    -- httpd_state.env.info("recv loop iteration")
                    num_repeats_left = num_repeats_left - 1
                    local line, err = conn:receive()
                    if err == "closed" then
                        dead_connections[#dead_connections + 1] = conn
                        break
                    end
                    if err then
                        -- httpd_state.env.info("rx error: "..tostring(err))
                        break
                    end

                    local result = nil
                    err, result = coroutine.resume(conn_info.handler, line)
                    if result == "close" then
                        conn_info.close_when_done = true
                        break
                    end
                end
            end
        end

        for i, conn in pairs(dead_connections) do
            httpd_state.http_map[conn] = nil
        end
    end

    function httpd_step()
        doComms(nil, 0)
    end

    function httpd_start(timer, env)
        httpd_state.timer = timer
        httpd_state.env = env
        httpd_state.env.info("starting DCS httpd..." .. tostring(httpd_state))
        timer.scheduleFunction(function(arg, time)

            local bool, err = pcall(httpd_step)
            if not bool then
                httpd_state.env.info("httpd_step() error: " .. tostring(err))
            end
            return httpd_state.timer.getTime() + .1
        end, nil, timer.getTime() + .1)
        env.info("DCS Fiddle httpd server running on " .. httpd.bind_address .. ":" .. 12080)
    end
    httpd.start = httpd_start

end

