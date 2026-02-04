 🔍 今すぐ確認すべき事項

  1. 実験データの記録

  質問:
  - PsychoPyから制御した時、C#側のログ/データ記録は有効ですか？
  - トライアル結果（正解/不正解、反応時間など）はどこに記録されますか？
実験ログデータは、必要です。
エラーログや設定のログはstartボタンを押すたびに新しく作成し、C#側とPsychoPy側で別々のファイルに記録されるようにしたい。
トライアル結果は独立して設定して、startを押すタイミングにとらわれず、課題が思考されるたびに下に追記するようにしたい。
（これは今と同じです。）

  懸念点:
  # PsychoPyで制御
  api.open_door()
  api.extend_lever()
  # この時、C#のIdControlHelperにデータは記録される？
  確認してください。

⏺ Search(pattern: "ExternalControl|SaveData|Log.*Trial", path: "Downloads/compartment/cs/Compartment/Compartment",
        output_mode: "content", head_limit: 15)
  ⎿  Found 10 lines (ctrl+o to expand)

⏺ 2. 複数トライアルでのリソース管理

  質問:
  - 100トライアル、200トライアルなど長時間実験で問題ありませんか？
  - メモリリークやコネクションの蓄積は？
  基本的につけっぱなしにして、1日20-30トライ、で365日繰り返す予定です。
  なので、数千トライアルの実施が想定されます。
  メモリは必要なときに解放する必要がありそうですね。

  懸念点:
  # 200トライアルループ
  for trial in range(200):
      api.open_door()  # HTTPコネクションは毎回開く？閉じる？
      api.get_door_status()
      # メモリは解放される？

  ---
  3. 同時アクセス制御

  質問:
  - 複数のPsychoPyスクリプトから同時にAPIを呼べますか？
  - 複数の部屋を同時に制御する可能性は？
  1つの部屋を1つのスクリプトで制御する予定です。なので、競合は起こらないはずです。

  懸念点:
  # スクリプト1: Room 0を制御中
  api1.open_door()  # Room 0

  # スクリプト2: Room 0に同時アクセス
  api2.close_door()  # 競合？

  ---
  4. エラー時のハードウェア状態

  質問:
  - PsychoPyスクリプトが途中でクラッシュした場合：
    - レバーは出たまま？
    - ドアは開いたまま？
    - 給餌モーターは動き続ける？
  全てのモーターの動きを止めてください。自動ドアはモーターが動いていなければ手動で開くので、動物は部屋から出ることができます。
  

  懸念点:
  api.extend_lever()  # レバー出す
  api.open_door()     # ドア開ける
  # ここでPsychoPyクラッシュ！
  # → レバーとドアはどうなる？

  ---
  5. タイミング精度

  質問:
  - 反応時間測定の精度は重要ですか？
  - HTTP APIのレイテンシ（数十ms）は許容範囲？
  （数十ms）程度であれば問題ありません。

  懸念点:
  # PsychoPyでレバー押しの反応時間を測定
  start = time.time()
  while not api.get_lever_switch():  # HTTP通信のレイテンシ
      pass
  rt = time.time() - start  # 実際の反応時間 + HTTP遅延

  ---
  6. Preferences設定の変更

  質問:
  - ExternalControlモードに設定する手順は？
    - GUI操作？
    - 設定ファイル編集？
  - 実験者がミスなく設定できますか？
  ExternalControlモードとは？PsychoPyで制御のことでしょうか？

⏺ Search(pattern: "comboBox.*Task|OpeTypeOfTask", path: "Downloads/compartment/cs/Compartment/Compartment",
        output_mode: "content", head_limit: 10)
  ⎿  Found 10 lines (ctrl+o to expand)

⏺ Search(pattern: "ExternalControl.*ToString|Items\.Add.*External", path:
        "Downloads/compartment/cs/Compartment/Compartment/UcPreferencesTab.cs", output_mode: "content")
  ⎿  Found 0 lines 

⏺ 7. 🚨 重大な問題を発見：ExternalControl選択不可

  GUIにExternalControlが表示されません！
  解決してください。
  おそらく、旧エンジン、ブロックプログラミングエンジンと並列で表記する予定でしょうか？

⏺ Read(Downloads/compartment/cs/Compartment/Compartment/UcPreferencesTab.cs)
  ⎿  Read 15 lines

⏺ ---
  8. ネットワークエラー対策

  質問:
  - API呼び出し失敗時のリトライ処理は必要？
  - ネットワーク切断時の挙動は？
  リトライ処理は必要ありません。ネットワーク切断時は、エラーログを出力して、実験を停止してください。

  ---
  9. デバッグモードとの併用

  質問:
  - ExternalControl + DebugMode を同時に使用しますか？
  - 本番実験ではExternalControl + 実ハードウェア？

本番実験ではExternalControl + 実ハードウェアです。

  10. 給餌時間の管理

  質問:
  - 給餌中に次のコマンドを送った場合：

  api.dispense_feed(duration_ms=5000)  # 5秒給餌開始
  time.sleep(0.1)
  api.dispense_feed(duration_ms=1000)  # すぐに別の給餌命令
  # → どうなる？キューイング？上書き？エラー？
キューイングでお願いします。
多分そういう課題設計にはしないです。

  ---
  📝 最も重要な質問

  質問リスト（優先度順）

  1. 🚨 ExternalControlをGUIから選択できるようにする必要がありますか？
    - 現在、comboBoxに項目がありません
    選択できるようにしてください。
  2. 📊 データ記録はどうしますか？
    - C#側で自動記録？PsychoPy側で記録？
    実験ログデータは、必要です。
エラーログや設定のログはstartボタンを押すたびに新しく作成し、C#側とPsychoPy側で別々のファイルに記録されるようにしたい。
トライアル結果は独立して設定して、startを押すタイミングにとらわれず、課題が思考されるたびに下に追記するようにしたい。
（これは今と同じです。）
  3. ⏱ 反応時間測定の精度要件は？
    - ミリ秒単位で正確である必要がある？
  （数十ms）程度であれば問題ありません。

  4. 🔄 エラー時のハードウェアリセットは必要？
    - スクリプトクラッシュ時に自動でレバー引っ込め、ドア閉じるなど
      全てのモーターの動きを止めてください。自動ドアはモーターが動いていなければ手動で開くので、動物は部屋から出ることができます。

  5. 🏢 複数部屋の同時制御はありますか？
    - 1つのAPIサーバーで複数部屋を制御？
  1つの部屋を1つのスクリプトで制御する予定です。なので、競合は起こらないはずです。

  6. 📏 長時間実験（何時間？何トライアル？）の予定は？
  基本的につけっぱなしにして、1日20-30トライ、で365日繰り返す予定です。
  なので、数千トライアルの実施が想定されます。
  メモリは必要なときに解放する必要がありそうですね。

  