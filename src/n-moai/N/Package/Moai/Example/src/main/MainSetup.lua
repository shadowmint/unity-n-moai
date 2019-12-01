api.debug("LUA setup completed");

local script, errorMessage = loadfile("helpers/Something.lua")
if not script then
    print("Could not load Util script! - "..errorMessage)
    assert(false)
end
script()

x = fact()
print("fact: " .. x)