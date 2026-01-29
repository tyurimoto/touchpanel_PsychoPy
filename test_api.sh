#!/bin/bash
# Compartment API Test Script
#
# 使い方:
#   chmod +x test_api.sh
#   ./test_api.sh

API_URL="http://localhost:5000/api"

echo "======================================"
echo "Compartment API Test Script"
echo "======================================"
echo ""

# 色定義
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# テスト関数
test_endpoint() {
    local method=$1
    local endpoint=$2
    local data=$3
    local description=$4

    echo -e "${YELLOW}Testing:${NC} $description"
    echo "  $method $API_URL$endpoint"

    if [ "$method" = "GET" ]; then
        response=$(curl -s -w "\n%{http_code}" "$API_URL$endpoint")
    elif [ "$method" = "POST" ]; then
        if [ -n "$data" ]; then
            response=$(curl -s -w "\n%{http_code}" -X POST -H "Content-Type: application/json" -d "$data" "$API_URL$endpoint")
        else
            response=$(curl -s -w "\n%{http_code}" -X POST "$API_URL$endpoint")
        fi
    elif [ "$method" = "DELETE" ]; then
        response=$(curl -s -w "\n%{http_code}" -X DELETE "$API_URL$endpoint")
    fi

    http_code=$(echo "$response" | tail -n1)
    body=$(echo "$response" | sed '$d')

    if [ "$http_code" = "200" ]; then
        echo -e "${GREEN}✓ Success${NC} (HTTP $http_code)"
        echo "  Response: $body"
    else
        echo -e "${RED}✗ Failed${NC} (HTTP $http_code)"
        echo "  Response: $body"
    fi
    echo ""
}

# 基本API テスト
echo "=== 基本APIテスト ==="
echo ""

test_endpoint "GET" "/sensor/entrance" "" "入室センサー読み取り"
test_endpoint "GET" "/sensor/exit" "" "退室センサー読み取り"
test_endpoint "GET" "/sensor/stay" "" "在室センサー読み取り"
test_endpoint "GET" "/sensor/lever" "" "レバースイッチ読み取り"

test_endpoint "GET" "/rfid/read" "" "RFID読み取り"
test_endpoint "DELETE" "/rfid" "" "RFIDクリア"

test_endpoint "POST" "/door/open" "" "ドアを開く"
test_endpoint "GET" "/door/status" "" "ドア状態取得"
test_endpoint "POST" "/door/close" "" "ドアを閉じる"

test_endpoint "POST" "/lever/extend" "" "レバーを出す"
test_endpoint "POST" "/lever/retract" "" "レバーを引っ込める"

test_endpoint "POST" "/feed/dispense" '{"durationMs":1000}' "給餌（1秒）"
test_endpoint "GET" "/feed/status" "" "給餌状態取得"

# デバッグAPI テスト
echo "=== デバッグAPIテスト ==="
echo ""

test_endpoint "GET" "/debug/status" "" "デバッグモード状態確認"

test_endpoint "POST" "/debug/sensor/set" '{"sensor":"entrance","state":true}' "入室センサーON"
test_endpoint "GET" "/debug/sensors/all" "" "全センサー状態取得"
test_endpoint "POST" "/debug/sensor/set" '{"sensor":"entrance","state":false}' "入室センサーOFF"

test_endpoint "POST" "/debug/rfid/set" '{"id":"1234567890123456"}' "RFID設定"
test_endpoint "GET" "/rfid/read" "" "RFID読み取り確認"
test_endpoint "POST" "/debug/rfid/random" "" "ランダムRFID生成"

test_endpoint "GET" "/debug/task/status" "" "課題状態取得"
test_endpoint "GET" "/debug/task/history" "" "試行履歴取得"

test_endpoint "POST" "/debug/reset" "" "デバッグ状態リセット"

echo "======================================"
echo "Test completed!"
echo "======================================"
