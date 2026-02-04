Phase 1 API検証手順

  ステップ1: アプリケーション起動とAPIサーバー確認

  1. Compartment.exe を起動してください
  2. APIサーバーが起動したか確認：
    - Visual StudioのOutput Windowで「[API] Web API server started at http://localhost:5000/」というメッセージを探す
    - または、アプリ内のメッセージエリアに「API起動: http://localhost:5000/」が表示されているか確認

  ステップ2: デバッグモードを有効化

  1. アプリケーションのメイン画面で**「デバッグモード」チェックボックス**をON
  2. アプリケーションを再起動
  3. デバッグコントロールパネルが表示されることを確認

  ステップ3: ブラウザでAPIテスト（簡単）

  以下のURLをブラウザで開いてください：

  http://localhost:5000/api/debug/status

  期待される結果:
  {
    "roomId": "0",
    "debugModeEnabled": true,
    "hardwareConnected": false,
    "timestamp": "2026-02-04T..."
  }

  ステップ4: curlでAPIテスト（詳細）

  WindowsのコマンドプロンプトまたはPowerShellで以下を実行：

  4-1. センサー状態取得

  curl http://localhost:5000/api/sensor/entrance

  4-2. デバッグモードでセンサー設定

  curl -X POST http://localhost:5000/api/debug/sensor/set -H "Content-Type: application/json" -d "{\"sensor\":\"entrance\",\"state\":true}"

  4-3. RFID設定

  curl -X POST http://localhost:5000/api/debug/rfid/set -H "Content-Type: application/json" -d "{\"id\":\"1234567890123456\"}"

  4-4. 全センサー状態取得

  curl http://localhost:5000/api/debug/sensors/all

  ステップ5: エラーがあれば報告

  何かエラーが出たら教えてください。次のステップに進みます。

  ---
  まずはステップ1から始めて、APIサーバーが起動したか確認してください！
