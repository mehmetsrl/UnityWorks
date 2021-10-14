echo Starting VR Config...

::EyeTracker
cd /d %EYE_TRACKER_SOFTWARE_PATH% & start .\pupil_capture.exe


::PVI
echo Do you want to run this project 'NAV_JOINED_V2_R'? Write 'y' or 'n' & set /P answer=Answer:
set condition=F
if /I "%answer%"=="Y" (
	set condition=T
)
if /I "%answer%"=="Yes" (
	set condition=T
)

if /I "%condition%"=="T" (
  set projectName=NAV_JOINED_V2_R
) else (
  echo Write VAPS Project Name
  set /P projectName=ProjectName:
)

setx VAPS_PROJECT_NAME %projectName%

cd /d %VAPS_PROJECT%%projectName%
start .\Code.exe  -OpenGL -norefresh -noborder -geometry 1350x700+0+0
::cd /d %VAPS_PROJECT%%projectName%\Messenger
::start .\Messenger_V1.exe %projectName%



