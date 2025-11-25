using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Compartment
{
    public partial class FormScript : Form
    {
        public FormMain formParent;

        const string defaultBasicID = "0";
        const string defaultLabelID = "0(default)";

        private bool isSorting = false;
        private string openFilePath = "";

        private bool isModified = false;

        public FormScript()
        {
            InitializeComponent();
            this.ControlBox = false;
            userControl11.SendJsonEventHandler += new EventHandler(RefreshJsonString);
            userControl11.UndoActionEventHandler += new EventHandler(UndoAction);
            userControl11.ModifyActionEventHandler += new EventHandler(ModifiedScript);
            InitializeToolTips();

        }

        private void FormScript_Load(object sender, EventArgs e)
        {
            //多重化 初期時はdefault id＝0
            string json = formParent.uob.GetJsonRegisterActionParam();
            var ids = formParent.ucOperationDataStore.GetIDs();

            //MutliID対応につきSave動線は登録ボタンに移行
            buttonSave.Visible = false;

            //defaultファイル探す
            try
            {
                textBoxScript.Text = UcOperationBlock.JsonToScript(json);
                userControl11.JsonLoad(json);
                if (ids.Count > 0)
                {
                    foreach (var id in ids)
                    {
                        if (id != defaultBasicID)
                        {
                            comboBoxIds.Items.Add(id);
                        }
                    }
                }

                //comboBoxIdsのindex0以外を登録
                for (int i = 1; i < comboBoxIds.Items.Count; i++)
                {
                    comboBoxAddIDs.Items.Add(comboBoxIds.Items[i]);
                }
                comboBoxIds.SelectedIndex = 0;

            }
            catch (Exception)
            {

                //throw;
            }
        }


        private void FormScript_Activated(object sender, EventArgs e)
        {
            var keys = formParent.recentIdHelper.Keys;
            foreach (var key in keys)
            {
                if (!comboBoxAddIDs.Items.Contains(key))
                {
                    comboBoxAddIDs.Items.Add(key);
                }
            }
            comboBoxAddIDs.SelectedIndex = -1;
            formParent.ToggleCloseButton(false);
        }
        //private async Task MainAsync()
        //{
        //    string s = JsonConvert.DeserializeObject(textBoxScript.Text).ToString();
        //    var result = await CSharpScript.EvaluateAsync<int>(s);
        //    Debug.WriteLine(result);
        //}

        private void InitializeToolTips()
        {
            // Set up the delays for the ToolTip.
            toolTip1.AutoPopDelay = 5000;
            toolTip1.InitialDelay = 1000;
            toolTip1.ReshowDelay = 1000;
            // Force the ToolTip text to be displayed whether or not the form is active.
            toolTip1.ShowAlways = true;

            // Set up the ToolTip text for the Button and Checkbox.
            toolTip1.SetToolTip(comboBoxIds, "ID 選択をします");
            toolTip1.SetToolTip(idsearchTextBox, "ID検索 一部の文字を入れると検索可能です");
            toolTip1.SetToolTip(idSetTextBox, "IDを任意の文字列入力で追加・削除");
            toolTip1.SetToolTip(comboBoxAddIDs, "入室履歴IDから追加・削除");

        }

        private void ModifiedScript(object sender, EventArgs e)
        {
            isModified = true;
        }

        private bool SaveFileRelatedActionParam(FileRelatedActionParam frap, string id)
        {
            //// 既にファイルを開いていたら上書き保存
            //if (openFilePath != "")
            //{
            //    frap.FilePath = openFilePath;
            //    frap.SaveToJson();
            //    return true;
            //}

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory = frap.FilePath == "" ? ".\\" : Path.GetDirectoryName(frap.FilePath);
                saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.RestoreDirectory = true;
                //saveFileDialog.FileName = Path.GetFileName(frap.FilePath) != "" ? Path.GetFileName(frap.FilePath) : "default_block.json";
                saveFileDialog.FileName = DateTime.Today.ToString("yyyyMMdd") + "_" + id;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    frap.FilePath = saveFileDialog.FileName;
                    //formParent.uob.OperationProcToJson(frap.FilePath);
                    frap.SaveToJson();
                    return true;
                }
                else
                {
                    //MessageBox.Show("ファイル作成と登録を中止しました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
        }

        private void RegisterActionParam(string jsonString)
        {
            string selectItem = comboBoxIds.Items[comboBoxIds.SelectedIndex].ToString();
            if (selectItem == defaultLabelID)
            {
                selectItem = defaultBasicID;
            }

            FileRelatedActionParam fileRelatedActionParam = formParent.ucOperationDataStore.GetEntry(selectItem) ?? new FileRelatedActionParam { ActionParams = "" };
            fileRelatedActionParam.ActionParams = jsonString;

            if (isModified || (!isModified && fileRelatedActionParam.FilePath == "" && openFilePath == ""))
            {
                if (SaveFileRelatedActionParam(fileRelatedActionParam, selectItem))
                {
                    formParent.ucOperationDataStore.AddOrUpdateOperation(selectItem, fileRelatedActionParam);
                    formParent.uob.JsonToOperationProc(selectItem, fileRelatedActionParam.ActionParams);
                    formParent.uob.OperationProcToJson(selectItem, fileRelatedActionParam.FilePath);
                    textBoxIdFileName.Text = Path.GetFileName(fileRelatedActionParam.FilePath);
                    isModified = false;

                    MessageBox.Show("ID: " + selectItem + "の登録に成功しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("ファイル作成と登録を中止しました。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                if (openFilePath != "")
                    fileRelatedActionParam.FilePath = openFilePath;
                formParent.ucOperationDataStore.AddOrUpdateOperation(selectItem, fileRelatedActionParam);
                formParent.uob.JsonToOperationProc(selectItem, fileRelatedActionParam.ActionParams);
                formParent.uob.OperationProcToJson(selectItem, fileRelatedActionParam.FilePath);
                textBoxIdFileName.Text = Path.GetFileName(fileRelatedActionParam.FilePath);
                MessageBox.Show("ID: " + selectItem + "の登録に成功しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RefreshJsonString(object sender, EventArgs e)
        {
            try
            {
                //登録
                formParent.EnableChangeTask(false);

                RegisterActionParam((string)sender);

                // Multi_idにあたって手動閉じに？
                //Hide();

                //** ↑2025/01 多層化対応↑ **//
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UndoAction(object sender, EventArgs e)
        {
            formParent.uob.UndoOperationProc();
        }

        private void buttonScriptToJSON_Click(object sender, EventArgs e)
        {
            if (textBoxScript.Text == "")
            {
                MessageBox.Show("Script not found!");
                return;
            }
            string json = UcOperationBlock.ScriptToJson(textBoxScript.Text);

            try
            {
                formParent.EnableChangeTask(false);

                RegisterActionParam(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            Hide();
            formParent.ToggleCloseButton(true);
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            formParent.uob.RegisterBlockFunc();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            string selectItem = comboBoxIds.Items[comboBoxIds.SelectedIndex].ToString();
            if (selectItem == defaultLabelID)
            {
                selectItem = defaultBasicID;
            }

            var currentItem = formParent.ucOperationDataStore.GetEntry(selectItem) ?? new FileRelatedActionParam();

            if (Path.GetFileName(currentItem.FilePath) != textBoxIdFileName.Text)
            {
                // カレントのDataStore FileNameとtextBoxIdFileNameを比較して違う場合は警告メッセージを表示
                DialogResult ret = MessageBox.Show("ファイルロードされていますが登録されていません。登録しますか？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (ret == DialogResult.OK)
                {
                    // DataStoreのFileNameを更新
                    if (openFilePath != "")
                        currentItem.FilePath = openFilePath;
                    formParent.ucOperationDataStore.AddOrUpdateOperation(selectItem, currentItem);
                }
                else
                {
                    textBoxIdFileName.Text = Path.GetFileName(currentItem.FilePath);
                }
            }
            if (isModified)
            {
                DialogResult ret = MessageBox.Show("編集されていますが登録されていません。登録しますか？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (ret == DialogResult.OK)
                {
                    // 元々外部参照想定でないのでTest不十分あやしげ
                    userControl11.JsonOutput(this, null);
                }
                else
                {
                    //Loadしなおしたら再度読み込まれるので編集は破棄
                }
            }

            var checkEntryList = formParent.ucOperationDataStore.CheckEmptyPath();
            foreach (var checkEntry in checkEntryList)
            {
                if (checkEntry.Item1)
                {
                    var ret = MessageBox.Show("ID:" + checkEntry.Item2 + "ファイル指定がされていません。登録してからCloseしてください。\n保存せずにIDを削除しますか？", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                    if (ret == DialogResult.OK)
                    {
                        // delete id
                        if (!string.IsNullOrWhiteSpace(checkEntry.Item2))
                        {
                            // コンボボックス内に同じIDが存在するか確認
                            if (comboBoxIds.Items.Contains(checkEntry.Item2))
                            {
                                // 確認メッセージを表示
                                DialogResult result = MessageBox.Show("ID:" + checkEntry.Item2 + " このIDを削除してもよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                                // ユーザーが「Yes」を選択した場合に削除を実行
                                if (result == DialogResult.Yes)
                                {
                                    // 現在選択されているIDを取得
                                    string selectedId = comboBoxIds.SelectedItem?.ToString();

                                    // コンボボックスからIDを削除
                                    comboBoxIds.Items.Remove(checkEntry.Item2);
                                    formParent.ucOperationDataStore.RemoveEntry(checkEntry.Item2);

                                    // 削除したIDが選択されていた場合は"0"を表示
                                    if (selectedId == checkEntry.Item2)
                                    {
                                        comboBoxIds.SelectedItem = defaultLabelID;
                                    }

                                    // 削除成功メッセージを表示
                                    MessageBox.Show("削除に成功しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            // openFilePathの初期化
            openFilePath = "";
            //formParent.uob.Start();
            Hide();
            formParent.ToggleCloseButton(true);
        }

        private void TestJsonButton_Click(object sender, EventArgs e)
        {
            textBoxScript.Text = "DrawScreenReset\r\nViewTriggerImage\r\nWaitTouchTrigger\r\nDrawScreenReset\r\nDelay(100-1000)\r\nViewCorrectImage\r\nDelay(3000)\r\nDrawScreenReset\r\nMatchBeforeDelay(1000-2000)\r\nViewCorrectWrongImage\r\nDelay(100-500)\r\nWaitCorrectTouchTrigger\r\nDrawScreenReset\r\nPlaySound\r\nFeedLamp(1000)\r\nDelay(500)\r\nFeed(500-2000)\r\nFeedSound(1000-1000)\r\nOutputResult\r\nTouchDelay(800-1000)";
        }

        private void buttonOpenTips_Click(object sender, EventArgs e)
        {
            FormTips formTips;
            using (formTips = new FormTips())
            {
                formTips.ShowDialog();
            }
        }
        // MutilIDに伴って廃止
        private void SaveJsonPreference()
        {
            var preferenceFolder = "\\preference\\";
            var preferenceFilePath = Path.GetDirectoryName(saveScriptFileDialog.FileName) + preferenceFolder + Path.GetFileNameWithoutExtension(saveScriptFileDialog.FileName) + "_preference.json";

            // 問答無用で作成
            Directory.CreateDirectory(Path.GetDirectoryName(saveScriptFileDialog.FileName) + preferenceFolder);
            formParent.uob.SavePreferenceJson(preferenceFilePath);
        }
        private void OpenJsonPreference()
        {
            var preferenceFolder = "\\preference\\";
            var preferenceFilePath = Path.GetDirectoryName(openScriptFileDialog.FileName) + preferenceFolder + Path.GetFileNameWithoutExtension(openScriptFileDialog.FileName) + "_preference.json";

            if (File.Exists(preferenceFilePath))
            {
                formParent.uob.OpenPreferenceJson(preferenceFilePath);
            }
            else
            {
                MessageBox.Show(Path.GetFileName(preferenceFilePath) + " が見つかりません。\nペアとなる保存ファイルが移動されています。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // MutilIDに伴って廃止
        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == saveScriptFileDialog.ShowDialog())
            {
                if (!Regex.IsMatch(saveScriptFileDialog.FileName, @"preference"))
                {
                    try
                    {
                        formParent.uob.OperationProcToJson(saveScriptFileDialog.FileName);

                        //string selectItem = comboBoxIds.SelectedItem.ToString();
                        //if (selectItem == defaultLabelID)
                        //{
                        //    selectItem = defaultBasicID;
                        //}

                        //UcValues ucValues = formParent.ucOperationDataStore.GetUcValues(selectItem);
                        //formParent.uob.OperationProcToJson(ucValues);

                        //string saveStrings = JsonConvert.SerializeObject(formParent.uob.GetJsonRegisterActionParam());
                        //if (saveStrings != "")
                        //    File.WriteAllText(saveScriptFileDialog.FileName, saveStrings);

                        // BlockProgramming由来Preference利用廃止
                        ////PreferenceDatセーブも追加
                        //SaveJsonPreference();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    MessageBox.Show("ファイル名にpreferenceは含めることはできません。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openScriptFileDialog.ShowDialog())
            {
                string readStrings = File.ReadAllText(openScriptFileDialog.FileName);
                try
                {
                    string readJson = formParent.uob.JsonToActionParams(readStrings);
                    userControl11.JsonLoad(readJson);
                    textBoxScript.Text = UcOperationBlock.JsonToScript(readStrings);
                    //(string)JsonConvert.DeserializeObject(readStrings);
                    //OpenJsonPreference();

                    // FileName表示だけして登録ボタンが押されたらDatastoreのFileNameへ更新
                    textBoxIdFileName.Text = Path.GetFileName(openScriptFileDialog.FileName);
                    openFilePath = openScriptFileDialog.FileName;

                    // 変更フラグをリセット
                    isModified = false;

                }
                catch (Exception)
                {
                    MessageBox.Show("ファイル形式が違います");
                }
            }
        }

        private void FormScript_FormClosing(object sender, FormClosingEventArgs e)
        {

            switch (e.CloseReason)
            {
                case CloseReason.UserClosing:
                    //ユーザーインターフェイスによるCloseのみ処理
                    e.Cancel = true;
                    Hide();
                    formParent.ToggleCloseButton(true);
                    break;
                case CloseReason.None:
                default:
                    break;
            }
        }
        // MutilIDに伴って廃止
        private void buttonSavePreference_Click(object sender, EventArgs e)
        {
            SaveJsonPreference();
        }

        private void ComboBoxIds_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSorting) return;

            try
            {
                string selectedItem = ((ComboBox)sender).Items[((ComboBox)sender).SelectedIndex].ToString();
                LoadActionParam(selectedItem);
            }
            catch (Exception)
            {

                throw;
            }

            openFilePath = "";
        }

        private void LoadActionParam(string selectedId)
        {
            if (selectedId == defaultLabelID)
            {
                selectedId = defaultBasicID;
            }

            var fileRelatedActionParam = formParent.ucOperationDataStore.GetEntry(selectedId) ?? new FileRelatedActionParam();

            string readJson = fileRelatedActionParam.ActionParams;
            // 一回Desirializeして戻す 整形
            UcOperationBlock.ActionParam[] actionParams = JsonConvert.DeserializeObject<UcOperationBlock.ActionParam[]>(readJson);
            readJson = JsonConvert.SerializeObject(actionParams, Formatting.None);

            if (readJson != null)
            {
                // Load中に更新するとBindingError
                userControl11.JsonLoad(readJson);
                textBoxScript.Text = UcOperationBlock.JsonToScript(readJson);
                textBoxIdFileName.Text = Path.GetFileName(fileRelatedActionParam.FilePath);
                isModified = false;
            }

        }
        public void ReloadCurrentActionParam()
        {
            string selectedId = comboBoxIds.SelectedItem.ToString();
            try
            {
                LoadActionParam(selectedId);
            }
            catch (Exception)
            {

                throw;
            }
        }
        private void SearchIDTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // エンターキーが押されたか確認
            if (e.KeyCode == Keys.Enter)
            {
                // テキストボックスの入力に基づいてコンボボックスの項目を検索
                string filter = idsearchTextBox.Text.ToLower();
                for (int i = 0; i < comboBoxIds.Items.Count; i++)
                {
                    if (comboBoxIds.Items[i].ToString().ToLower().Contains(filter))
                    {
                        comboBoxIds.SelectedIndex = i;
                        break;
                    }
                }

                // エンターキーのデフォルト動作を無効化
                e.SuppressKeyPress = true;
            }
        }

        private void AddIDButton_Click(object sender, EventArgs e)
        {
            // テキストボックスの内容を取得
            string newId = idSetTextBox.Text;

            // 空の場合はComboBoxから追加導線
            if (string.IsNullOrWhiteSpace(newId))
            {
                if (comboBoxAddIDs.SelectedIndex >= 0)
                {
                    newId = comboBoxAddIDs.SelectedItem.ToString();
                }
                else
                    return;
            }

            // テキストボックスが空でないか確認
            if (!string.IsNullOrWhiteSpace(newId))
            {
                // コンボボックス内に同じIDが存在するか確認
                if (comboBoxIds.Items.Contains(newId) || newId == defaultBasicID || newId == defaultLabelID)
                {
                    // 警告メッセージを表示
                    MessageBox.Show("このIDは既に存在します。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    // 確認メッセージを表示
                    DialogResult result = MessageBox.Show("このIDを追加してもよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    // ユーザーが「Yes」を選択した場合に削除を実行
                    if (result == DialogResult.Yes)
                    {
                        // コンボボックスにIDを追加
                        comboBoxIds.Items.Add(newId);

                        // 画面スクリプトからJSON文字列を生成
                        string json = UcOperationBlock.ScriptToJson(textBoxScript.Text);

                        FileRelatedActionParam fileRelatedActionParam = formParent.ucOperationDataStore.GetEntry(defaultBasicID);
                        fileRelatedActionParam = new FileRelatedActionParam { ActionParams = json };

                        formParent.ucOperationDataStore.AddOrUpdateOperation(newId, fileRelatedActionParam);
                        idSetTextBox.Clear(); // テキストボックスをクリア

                        // コンボボックスのアイテムをソート
                        SortComboBoxItems(comboBoxIds);

                        // 追加成功メッセージを表示
                        MessageBox.Show("追加に成功しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //newIdを選択
                        for (int i = 0; i < comboBoxIds.Items.Count; i++)
                        {
                            if (comboBoxIds.Items[i].ToString() == newId)
                            {
                                comboBoxIds.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void SortComboBoxItems(ComboBox comboBox)
        {
            // ソート中フラグを立てる
            isSorting = true;

            // 現在選択されているアイテムを保持
            string selectedItem = comboBox.SelectedItem?.ToString();

            // コンボボックスのアイテムをリストにコピー
            List<string> items = comboBox.Items.Cast<string>().ToList();

            // アイテムをソート
            items.Sort();

            // コンボボックスをクリアしてソートされたアイテムを追加
            comboBox.Items.Clear();
            foreach (string item in items)
            {
                comboBox.Items.Add(item);
            }

            // 元の選択アイテムを再選択
            if (selectedItem != null)
            {
                comboBox.SelectedItem = selectedItem;
            }

            // ソート中フラグを解除
            isSorting = false;
        }

        private void DeleteIDButton_Click(object sender, EventArgs e)
        {
            // テキストボックスの内容を取得
            string oldId = idSetTextBox.Text;

            // 空の場合はComboBoxから追加導線
            if (string.IsNullOrWhiteSpace(oldId))
            {
                if (comboBoxAddIDs.SelectedIndex >= 0)
                {
                    oldId = comboBoxAddIDs.SelectedItem.ToString();
                }
                else
                    return;
            }

            if (oldId == "0" || oldId == defaultLabelID)
            {
                MessageBox.Show("このIDはデフォルトの為、削除はできません。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                // テキストボックスが空でないか確認
                if (!string.IsNullOrWhiteSpace(oldId))
                {
                    // コンボボックス内に同じIDが存在するか確認
                    if (comboBoxIds.Items.Contains(oldId))
                    {
                        // 確認メッセージを表示
                        DialogResult result = MessageBox.Show("このIDを削除してもよろしいですか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                        // ユーザーが「Yes」を選択した場合に削除を実行
                        if (result == DialogResult.Yes)
                        {
                            // 現在選択されているIDを取得
                            string selectedId = comboBoxIds.SelectedItem?.ToString();

                            // コンボボックスからIDを削除
                            comboBoxIds.Items.Remove(oldId);
                            formParent.ucOperationDataStore.RemoveEntry(oldId);

                            // 削除したIDが選択されていた場合は"0"を表示
                            if (selectedId == oldId)
                            {
                                comboBoxIds.SelectedItem = defaultLabelID;
                            }

                            idSetTextBox.Clear(); // テキストボックスをクリア

                            // 削除成功メッセージを表示
                            MessageBox.Show("削除に成功しました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        // 警告メッセージを表示
                        MessageBox.Show("このIDは存在していません。再度確認してください。", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
        private void idSetTextBox_Enter(object sender, EventArgs e)
        {
            VisualDisabledComboBox(comboBoxAddIDs, false);
        }

        private void comboBoxAddIDs_Enter(object sender, EventArgs e)
        {
            idSetTextBox.ReadOnly = true;
        }

        private void comboBoxAddIDs_Leave(object sender, EventArgs e)
        {
            idSetTextBox.ReadOnly = false;
        }

        private void idSetTextBox_Leave(object sender, EventArgs e)
        {
            VisualDisabledComboBox(comboBoxAddIDs, true);
        }
        private void VisualDisabledComboBox(ComboBox comboBox, bool enable)
        {
            if (comboBox == null)
                return;

            if (enable)
            {
                comboBox.BackColor = SystemColors.Window;
                comboBox.ForeColor = SystemColors.WindowText;
            }
            else
            {
                comboBox.BackColor = SystemColors.Control;
                comboBox.ForeColor = SystemColors.GrayText;
            }
        }

        private void flowLayoutPanel1_SizeChanged(object sender, EventArgs e)
        {
            button5.Width = flowLayoutPanel1.Width - 20;
            button4.Width = flowLayoutPanel1.Width - 20;
            buttonSave.Width = flowLayoutPanel1.Width - 20;
            buttonOpen.Width = flowLayoutPanel1.Width - 20;
            idSelectGroupBox.Width = flowLayoutPanel1.Width - 20;
            comboBoxIds.Width = flowLayoutPanel1.Width - 30;
            idsearchTextBox.Width = flowLayoutPanel1.Width - 30;
            idAddDeleteGroupBox.Width = flowLayoutPanel1.Width - 20;
            idSetTextBox.Width = flowLayoutPanel1.Width - 30;
            comboBoxAddIDs.Width = flowLayoutPanel1.Width - 30;
            groupBox1.Width = flowLayoutPanel1.Width - 20;

        }
    }
}