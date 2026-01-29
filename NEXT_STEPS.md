# 次のステップ - 実装継続ガイド

最終更新: 2026-01-29

## 現在の状態

✅ **完了項目:**
- C# APIサーバー（OWIN/Katana）
- 基本APIエンドポイント（センサー、RFID、ドア、レバー、給餌）
- デバッグモード（IoMicrochipDummyEx、IoHybridBoard、RFIDReaderDummy）
- デバッグAPI（センサーシミュレート、課題状態取得）
- Python APIクライアントライブラリ
- PsychoPy訓練課題サンプル
- デバッグツール（キーボードコントロール）

❌ **未完了項目:**
- C#ビルド・動作確認（Windows環境必要）
- UserControlDebugPanel（GUI、Windows Forms Designer必要）
- Preferences UI更新（デバッグモード設定、Windows Forms Designer必要）
- ExternalControlモード実装
- 既存課題のPython移行
- 中央オーケストレーター（Phase 6）

---

## 優先順位別タスク

### 🔴 最優先（Windows環境で実施）

#### タスク1: C#プロジェクトのビルドと動作確認

**目的:** 実装したコードが正しくコンパイル・動作するか確認

**手順:**
1. Windows PCでVisual Studio 2019/2022を開く
2. `/cs/Compartment/Compartment.sln` を開く
3. NuGetパッケージ復元
   ```
   ツール → NuGet パッケージ マネージャー → ソリューションのNuGetパッケージの復元
   ```
4. ビルド → ソリューションのビルド
5. エラーがあれば修正

**想定される問題:**
- usingディレクティブ不足
  - `Compartment.Controllers` (FormMain.cs)
  - `System.Linq` (HardwareService.cs)
- 名前空間の不一致
- XMLシリアライズ属性の問題

**確認ポイント:**
- [ ] ビルドエラー 0件
- [ ] 警告は許容範囲内
- [ ] Compartment.exe が生成される

**所要時間:** 30分～1時間

---

#### タスク2: APIサーバー起動テスト

**目的:** OWINサーバーが正しく起動するか確認

**手順:**
1. Compartment.exe を実行
2. デバッグ出力ウィンドウを確認
   ```
   [API] Web API server started at http://localhost:5000/
   ```
3. ブラウザでアクセス
   ```
   http://localhost:5000/api/sensor/entrance
   ```
4. JSON レスポンスが返ることを確認

**トラブルシューティング:**
- ポート5000が使用中 → Preferences.xmlでApiServerPortを変更
- ファイアウォールエラー → Windows Defenderでアプリ許可
- 起動時クラッシュ → Visual Studioデバッガで原因特定

**確認ポイント:**
- [ ] APIサーバーが起動する
- [ ] GET /api/sensor/entrance が応答する
- [ ] POST /api/door/open が動作する
- [ ] デバッグ出力にエラーがない

**所要時間:** 30分～1時間

---

#### タスク3: デバッグモード動作確認

**目的:** ハードウェアなしでAPIが動作するか確認

**手順:**
1. Preferences.xml を編集
   ```xml
   <EnableDebugMode>true</EnableDebugMode>
   <DebugModeType>FullDummy</DebugModeType>
   ```
2. Compartment.exe を再起動
3. デバッグ出力確認
   ```
   [Debug] Initialized IoMicrochipDummyEx (Full Dummy Mode)
   [Debug] Initialized RFIDReaderDummy
   ```
4. デバッグAPIテスト（curlまたはPostman）
   ```bash
   # デバッグモード状態確認
   curl http://localhost:5000/api/debug/status

   # センサー設定
   curl -X POST http://localhost:5000/api/debug/sensor/set \
     -H "Content-Type: application/json" \
     -d '{"sensor":"entrance","state":true}'

   # センサー状態取得
   curl http://localhost:5000/api/debug/sensors/all

   # RFID設定
   curl -X POST http://localhost:5000/api/debug/rfid/set \
     -H "Content-Type: application/json" \
     -d '{"id":"1234567890123456"}'

   # RFID読み取り
   curl http://localhost:5000/api/rfid/read
   ```

**確認ポイント:**
- [ ] デバッグモードが有効化される
- [ ] センサー設定APIが動作する
- [ ] RFID設定APIが動作する
- [ ] 課題状態取得APIが動作する
- [ ] 状態リセットAPIが動作する

**所要時間:** 30分～1時間

---

#### タスク4: PsychoPyからAPI呼び出しテスト

**目的:** PythonクライアントとC# APIサーバーが正しく連携するか確認

**手順:**
1. Python環境セットアップ
   ```bash
   pip install psychopy requests
   ```
2. デバッグツール実行
   ```bash
   cd /path/to/compartment/psychopy
   python debug_keyboard_control.py
   ```
3. キーボード操作でAPI呼び出し確認
   - Eキー: 入室シミュレート
   - Rキー: RFID生成
   - Tキー: 課題状態表示
   - SPACEキー: センサー状態表示

4. 訓練課題実行（デバッグモード）
   ```bash
   python training_task_example.py --debug --window
   ```

**確認ポイント:**
- [ ] debug_keyboard_control.py が起動する
- [ ] キーボード操作でセンサーが変化する
- [ ] training_task_example.py が起動する
- [ ] 課題フローが最後まで動作する（入室→RFID→課題→退室）

**所要時間:** 1～2時間

---

### 🟡 次の優先（Windows Forms Designer必要）

#### タスク5: UserControlDebugPanel作成

**目的:** GUIでデバッグ状態を確認・制御できるパネル作成

**手順:**
1. Visual Studioで新規UserControl作成
   ```
   プロジェクト → 新しい項目の追加 → ユーザーコントロール
   名前: UserControlDebugPanel.cs
   ```

2. Designerでコントロール配置
   ```
   GroupBox "センサー - 手動シミュレート"
     - Label "入室センサー"
     - Button "ON"
     - Button "OFF"
     - Label (LED表示用) → BackColor変更でON/OFF表示
     - CheckBox "手動"
     （他のセンサーも同様）

   GroupBox "デバイス状態 - 実機から取得"
     - Label "ドア: Close (実機センサー)"
     - Label "レバー: In (実機センサー)"

   GroupBox "RFID - 手動設定"
     - TextBox (RFID入力)
     - Button "ランダム生成"
     - Button "設定"
     - Button "クリア"

   GroupBox "ログ"
     - TextBox (MultiLine, ReadOnly, ScrollBars)
   ```

3. コードビハインド実装
   ```csharp
   // UserControlDebugPanel.cs
   public partial class UserControlDebugPanel : UserControl
   {
       private FormMain _formMain;
       private Timer _updateTimer;

       public UserControlDebugPanel(FormMain formMain)
       {
           InitializeComponent();
           _formMain = formMain;

           // 100ms周期で状態更新
           _updateTimer = new Timer { Interval = 100 };
           _updateTimer.Tick += UpdateDisplay;
           _updateTimer.Start();

           // イベントハンドラ登録
           buttonEntranceOn.Click += (s, e) => SetSensor(IoBoardDInLogicalName.RoomEntrance, true);
           // ...
       }

       private void SetSensor(IoBoardDInLogicalName sensor, bool state)
       {
           if (_formMain.ioBoardDevice is IoMicrochipDummyEx dummyBoard)
           {
               dummyBoard.SetSensorState(sensor, state);
               AddLog($"{sensor} {(state ? "ON" : "OFF")}");
           }
           else if (_formMain.ioBoardDevice is IoHybridBoard hybridBoard)
           {
               hybridBoard.SetManualSensorState(sensor, state);
               AddLog($"{sensor} {(state ? "ON" : "OFF")} (手動)");
           }
       }

       private void UpdateDisplay(object sender, EventArgs e)
       {
           // センサー状態更新
           bool state;
           _formMain.ioBoardDevice?.GetUpperStateOfSaveDIn(IoBoardDInLogicalName.RoomEntrance, out state);
           labelEntranceStatus.BackColor = state ? Color.Green : Color.Gray;
           // ...
       }

       private void AddLog(string message)
       {
           string log = $"{DateTime.Now:HH:mm:ss} - {message}\r\n";
           textBoxLog.AppendText(log);
       }
   }
   ```

4. FormMainに統合
   ```csharp
   // FormMain.cs - FormMain_Loadイベント
   if (preferencesDatOriginal.EnableDebugMode)
   {
       userControlDebugPanel = new UserControlDebugPanel(this);
       userControlDebugPanel.Dock = DockStyle.Right;
       userControlDebugPanel.Width = 450;
       this.Controls.Add(userControlDebugPanel);
       userControlDebugPanel.BringToFront();
   }
   ```

**確認ポイント:**
- [ ] パネルがFormMain右側に表示される
- [ ] ボタンでセンサー状態を変更できる
- [ ] LED表示がリアルタイムで更新される
- [ ] ログが表示される

**所要時間:** 2～3時間

---

#### タスク6: Preferences UI更新

**目的:** GUIでデバッグモード設定を変更可能にする

**手順:**
1. UserControlPreferencesTab.Designer.csを開く
2. 新しいGroupBoxを追加
   ```
   GroupBox "Debug Mode"
     - CheckBox "Enable Debug Mode"
     - RadioButton "Full Dummy (完全ダミー)"
     - RadioButton "Hybrid (実機＋手動シミュレート)"
   ```
3. データバインディング設定
   ```csharp
   // UserControlPreferencesTab.cs
   private void LoadPreferences()
   {
       checkBoxEnableDebugMode.Checked = preferences.EnableDebugMode;
       radioButtonFullDummy.Checked = (preferences.DebugModeType == EDebugModeType.FullDummy);
       radioButtonHybrid.Checked = (preferences.DebugModeType == EDebugModeType.Hybrid);
   }

   private void SavePreferences()
   {
       preferences.EnableDebugMode = checkBoxEnableDebugMode.Checked;
       preferences.DebugModeType = radioButtonFullDummy.Checked
           ? EDebugModeType.FullDummy
           : EDebugModeType.Hybrid;
   }
   ```

**確認ポイント:**
- [ ] チェックボックスで有効/無効切替
- [ ] ラジオボタンでモード選択
- [ ] 設定が保存される
- [ ] ソフト再起動で設定が反映される

**所要時間:** 1～2時間

---

### 🟢 中優先（機能拡張）

#### タスク7: ExternalControlモード実装

**目的:** C#側の課題ロジックを無効化し、完全にPsychoPy制御にする

**手順:**
1. PreferencesDat.cs修正
   ```csharp
   public enum ECpTask
   {
       Training = 0,
       DelayMatch = 1,
       None = 2,
       TrainingEasy = 3,
       UnConditionalFeeding = 4,
       ExternalControl = 5  // 追加
   }
   ```

2. OpCollection.cs修正
   ```csharp
   // Sequencer.Execute()
   public void Execute()
   {
       // ExternalControlモード時はスキップ
       if (_preferences.OpeTypeOfTask == ECpTask.ExternalControl)
       {
           return;  // 何もしない
       }

       // 既存のステートマシン処理
       switch (State)
       {
           // ...
       }
   }
   ```

3. FormMain.cs修正
   ```csharp
   private void UpdateFormSubVisibility()
   {
       if (preferencesDatOriginal.OpeTypeOfTask == ECpTask.ExternalControl)
       {
           formSub?.Hide();
       }
       else
       {
           formSub?.Show();
       }
   }
   ```

4. Preferences UIに選択肢追加
   ```csharp
   // UserControlPreferencesTab.Designer.cs
   comboBoxTypeOfTask.Items.Add("ExternalControl");
   ```

**確認ポイント:**
- [ ] ExternalControlモードを選択できる
- [ ] Startボタンを押してもC#の課題ロジックが動かない
- [ ] FormSubが非表示になる
- [ ] APIは正常に動作する

**所要時間:** 2～3時間

---

#### タスク8: DelayMatch課題のPython実装

**目的:** 既存課題をPsychoPyに移行

**手順:**
1. 既存のDelayMatch課題仕様を確認
2. `/psychopy/delaymatch_task.py` を作成
3. 基本フロー実装
   - サンプル刺激提示
   - 遅延期間
   - 比較刺激提示
   - 正誤判定
4. データ記録
5. テスト

**所要時間:** 4～6時間

---

### 🔵 低優先（将来の拡張）

#### タスク9: データ記録API実装

**目的:** PsychoPyから試行データをC#に記録

**APIエンドポイント:**
```
POST /api/log/write
{
  "idCode": "3920145000567278",
  "result": "correct",
  "trialNumber": 5,
  "touchX": 123,
  "touchY": 456,
  "timestamp": "2026-01-29T12:34:56Z"
}
```

**所要時間:** 2～3時間

---

#### タスク10: 中央オーケストレーター（Phase 6）

**目的:** 複数部屋を統合管理

**主要機能:**
- 複数部屋の状態監視
- 条件付きシーケンス制御（部屋A完了→部屋B利用可能）
- データ集約・分析
- Webダッシュボード

**所要時間:** 2～3週間

詳細は `IMPLEMENTATION_PLAN.md` のPhase 6参照

---

## 推奨作業順序（初回）

### Day 1: 基本動作確認（2～3時間）

1. ✅ タスク1: C#ビルド（30分）
2. ✅ タスク2: API起動テスト（30分）
3. ✅ タスク3: デバッグモード確認（1時間）
4. ✅ タスク4: PsychoPyテスト（1時間）

**ゴール:** PC単体（デバッグモード）で完全動作確認

---

### Day 2: 実機テスト（2～3時間）

1. 実機接続
2. 通常モードで起動
3. センサー動作確認
4. RFID動作確認
5. PsychoPy課題実行（実機）

**ゴール:** 実機での動作確認

---

### Day 3: GUI実装（3～4時間）

1. ✅ タスク5: UserControlDebugPanel作成（2～3時間）
2. ✅ タスク6: Preferences UI更新（1～2時間）

**ゴール:** デバッグパネルで視覚的に状態確認可能

---

### Day 4: ExternalControlモード（2～3時間）

1. ✅ タスク7: ExternalControlモード実装（2～3時間）
2. テスト・検証

**ゴール:** C#課題ロジックを完全にバイパス

---

### Day 5以降: 課題移行・拡張

1. DelayMatch課題移行
2. データ記録API実装
3. その他の課題移行

---

## チェックリスト

### 最初のマイルストーン: 基本動作確認

- [ ] C#プロジェクトがビルドできる
- [ ] APIサーバーが起動する
- [ ] デバッグモードが動作する
- [ ] PsychoPyからAPI呼び出しができる
- [ ] 訓練課題が最後まで動作する（デバッグモード）
- [ ] 訓練課題が最後まで動作する（実機モード）

### 第2マイルストーン: GUI実装

- [ ] UserControlDebugPanelが表示される
- [ ] ボタンでセンサーを制御できる
- [ ] Preferences UIでデバッグモード設定ができる
- [ ] 設定が保存・復元される

### 第3マイルストーン: ExternalControlモード

- [ ] ExternalControlモードが選択できる
- [ ] C#課題ロジックがスキップされる
- [ ] FormSubが非表示になる
- [ ] PsychoPyから完全制御できる

---

## トラブルシューティング用メモ

### ビルドエラーが出た場合

1. **using不足**
   - FormMain.cs に `using Compartment.Controllers;` 追加
   - HardwareService.cs に `using System.Linq;` 追加

2. **型が見つからない**
   - プロジェクト参照を確認
   - 名前空間が正しいか確認

3. **NuGetパッケージエラー**
   - packages.config と実際のパッケージが一致するか確認
   - NuGet パッケージマネージャーで復元

### API起動エラーが出た場合

1. **ポート使用中エラー**
   - Preferences.xml でApiServerPort変更
   - または他のアプリを終了

2. **権限エラー**
   - 管理者権限で実行
   - または別のポート番号使用

3. **ファイアウォールエラー**
   - Windows Defender設定でアプリ許可

### Python側エラーが出た場合

1. **接続エラー**
   - C#ソフトが起動しているか確認
   - URLが正しいか確認（/apiが必要）
   - ポート番号が一致するか確認

2. **モジュールエラー**
   - `pip install psychopy requests`

3. **画面表示エラー**
   - `screen=0` に変更（プライマリディスプレイ）
   - `fullscr=False` に変更（ウィンドウモード）

---

## 連絡事項・メモ欄

### 実装時の気づき

- IoMicrochipDummyEx は自動的にドア・レバー応答をシミュレート（Task.Delay使用）
- IoHybridBoard はデフォルトで entrance/exit=手動、他=実機
- RFID読み取りは CurrentIDCode.Value に保存（SyncObject使用）

### 今後の改善案

- WebSocket導入でリアルタイム通知
- データベース連携（試行データ蓄積）
- Webダッシュボード（リアルタイム監視）
- 複数部屋統合管理（Phase 6）

---

## 参考資料

- [実装状況](./STATUS.md) - 完了/未完了の詳細
- [操作方法マニュアル](./USAGE.md) - 使い方ガイド
- [実装計画詳細](./IMPLEMENTATION_PLAN.md) - 全体計画
- [PsychoPy README](./psychopy/README.md) - Python側の使い方
