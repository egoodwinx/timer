# Skipper's Background Changing Timer
Will change the background colour at set intervals and track times.

* Pressing the reset button will log times and reset the count to 0 and persist the state the timer was in (such as continue if timer was on or being stopped at 0 if the timer was stopped).
* If the break time reaches 0 the timer will reset to 0 and log the time shown on the timer.
* Closing the application will also add the log time to the file.
* The file may be edited from "test.log" (or whatever your log file is called) in the configuration file and uses a newline to seperate entries.

# Configuration
Can change the background with the .config file, just open with notepad and edit appropriate values.
| Variable | Description
| --- | ---
| breakSeconds | used to set how long a break (stopping the timer) will last before resetting the time.
| backgroundCheckSeconds | used to set how often in seconds the background should be checked to be updated changing colour
| backgroundChangeMinutes | how often the background should change in minutes (the interval it should be updated)
| logFile | the log file name, can also set the directory of the file using pathing (the working directory is the directory the .exe is in)
| backgroundFirstColour | the first/default colour the window should be, use hex value (#FFFFFF), by default it is red
| backgroundSecondColour | the second colour the window should be, use hex value (#FFFFFF), by default it is blue
| backgroundThirdColour | the third colour the window should be, use hex value (#FFFFFF), by default it is green
| backgroundFourthColour | the fourth colour the window should be, use hex value (#FFFFFF), by default it is purple
| backgroundFifthColour | the fifth colour the window should be, use hex value (#FFFFFF), by default it is gold

