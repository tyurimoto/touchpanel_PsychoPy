 1. 行36: 「PsychoPyはPythonプロセスだけど、インターフェースはタッチパネルを使う事がよくある」

  現状: PsychoPy状態機械は concurrentQueueFromTouchPanel、formSub.callbackTouchPoint、opImage
  を一切使っていません。Python側が独自にタッチ入力を処理している前提の設計です。

  課題: PsychoPyのPythonタスクが FormSub ウィンドウ上のタッチパネルを使う場合、C#側でタッチイベントをキューに入れる仕
  組みが必要になる可能性があります。ただし、PsychoPy/pygame が独自ウィンドウを作ってタッチを受け取るのか、FormSub
  を使うのかで設計が変わります。
  →C#側でのタッチイベントはいらない。課題が始まったらPsychoPy側でタッチイベントをキューに入れる仕組みにする。

  2. 行40: 「タッチ座標も重要な情報なので、PsychoPyでも記録できるようにしたい」

  現状: PsychoPyのCSV出力で opCollection.TouchPoint.X/Y は常に初期値 (0,0) です。タッチ座標が記録されません。
  →PsychoPy側でタッチ座標をキューに入れる仕組みにする。

  対処方法の選択肢:
  - A: Python側がstdoutで TOUCH:123,456 のような追加情報を出力し、C#がパースして opCollection.TouchPoint に設定する
  - B: C#側で concurrentQueueFromTouchPanel を有効にして、PsychoPy実行中もタッチ座標を監視・記録する
  - C: Python側のCSVにタッチ座標を記録する（選択肢Cの「Python側で課題詳細」に相当）
  →PsychoPy側でタッチ座標をcsvに入れる仕組みにする。

  3. 行53: 「PsychoPyもタッチパネル描写あり」

  現状: PsychoPyは opImage（C#側の描画エンジン）を使っていません。Pythonが独自に画面描画します。
  →PsychoPy側で描画する。

  質問: PsychoPyタスクの画面描画は:
  - (a) Python/PsychoPy が独自ウィンドウに描画する？
  - (b) C#の FormSub (タッチパネル画面) に描画する？
  - (c) 両方のケースがある？
  (a)を選択する。

  この回答によって、タッチパネル連携の設計が大きく変わります。

  未反映 (その他)

  行45: 「RFIDの読み取りと記録、課題の実行がうまくできるようにしたい」
  → 部分反映。OpeClearIdCode()
  の追加、RFID専用フェーズの分離は済み。「うまくできるように」の具体的な要件があれば教えてください。
  うまくできるようにとはそのまま、入退室に際して動物のIDコードを初期化したり、中に入っている動物のIDが確実にわかる設定で

  行61: 「課題側で確率制御等を実施する可能性あり」
  → 未反映。現在PsychoPyは RESULT:CORRECT の場合は常に給餌します。Python側が確率制御する場合、RESULT:CORRECT_NOFEED
  のような追加プロトコルが必要か、あるいは Python 側が RESULT:INCORRECT
  を返すことで給餌をスキップする設計にするか、方針の決定が必要です。
  pythonでrandomを取って、ある値以上のときは給餌、ある値以下のときは給餌しないようにしたい。
そのさいに、必ずあげるときは1,必ずあげないときは0を設定するようにしたい。
  ---

