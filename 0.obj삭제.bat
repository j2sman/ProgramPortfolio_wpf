@echo off

@echo FxAdmin Object 삭제
RMDIR FxAdmin\obj /S /Q

@echo Result 삭제
RMDIR Result /S /Q

@echo Log 삭제
DEL *.log /S /Q

@echo User 설정 삭제
DEL *.user /S /Q

pause
