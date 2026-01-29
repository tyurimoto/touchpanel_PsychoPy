@echo off
REM Compartment API Test Script (Windows)
REM
REM 使い方:
REM   test_api.bat

setlocal enabledelayedexpansion
set API_URL=http://localhost:5000/api

echo ======================================
echo Compartment API Test Script
echo ======================================
echo.

echo === 基本APIテスト ===
echo.

echo Testing: 入室センサー読み取り
curl -s %API_URL%/sensor/entrance
echo.
echo.

echo Testing: 退室センサー読み取り
curl -s %API_URL%/sensor/exit
echo.
echo.

echo Testing: 在室センサー読み取り
curl -s %API_URL%/sensor/stay
echo.
echo.

echo Testing: レバースイッチ読み取り
curl -s %API_URL%/sensor/lever
echo.
echo.

echo Testing: RFID読み取り
curl -s %API_URL%/rfid/read
echo.
echo.

echo Testing: RFIDクリア
curl -s -X DELETE %API_URL%/rfid
echo.
echo.

echo Testing: ドアを開く
curl -s -X POST %API_URL%/door/open
echo.
echo.

echo Testing: ドア状態取得
curl -s %API_URL%/door/status
echo.
echo.

echo Testing: ドアを閉じる
curl -s -X POST %API_URL%/door/close
echo.
echo.

echo Testing: レバーを出す
curl -s -X POST %API_URL%/lever/extend
echo.
echo.

echo Testing: レバーを引っ込める
curl -s -X POST %API_URL%/lever/retract
echo.
echo.

echo Testing: 給餌（1秒）
curl -s -X POST -H "Content-Type: application/json" -d "{\"durationMs\":1000}" %API_URL%/feed/dispense
echo.
echo.

echo Testing: 給餌状態取得
curl -s %API_URL%/feed/status
echo.
echo.

echo === デバッグAPIテスト ===
echo.

echo Testing: デバッグモード状態確認
curl -s %API_URL%/debug/status
echo.
echo.

echo Testing: 入室センサーON
curl -s -X POST -H "Content-Type: application/json" -d "{\"sensor\":\"entrance\",\"state\":true}" %API_URL%/debug/sensor/set
echo.
echo.

echo Testing: 全センサー状態取得
curl -s %API_URL%/debug/sensors/all
echo.
echo.

echo Testing: 入室センサーOFF
curl -s -X POST -H "Content-Type: application/json" -d "{\"sensor\":\"entrance\",\"state\":false}" %API_URL%/debug/sensor/set
echo.
echo.

echo Testing: RFID設定
curl -s -X POST -H "Content-Type: application/json" -d "{\"id\":\"1234567890123456\"}" %API_URL%/debug/rfid/set
echo.
echo.

echo Testing: RFID読み取り確認
curl -s %API_URL%/rfid/read
echo.
echo.

echo Testing: ランダムRFID生成
curl -s -X POST %API_URL%/debug/rfid/random
echo.
echo.

echo Testing: 課題状態取得
curl -s %API_URL%/debug/task/status
echo.
echo.

echo Testing: 試行履歴取得
curl -s -X POST %API_URL%/debug/task/history
echo.
echo.

echo Testing: デバッグ状態リセット
curl -s -X POST %API_URL%/debug/reset
echo.
echo.

echo ======================================
echo Test completed!
echo ======================================

pause
