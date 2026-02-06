"""
Compartment PsychoPy ハイブリッドエンジン 簡易テストスクリプト

新プロトコル:
  起動: python simple_test.py <rfid_value>
  結果: stdout に RESULT:CORRECT または RESULT:INCORRECT を出力
  終了: exit code 0 = 正常

C#が入室/退室/給餌を制御し、このスクリプトは課題のみを実行する
"""

import sys
import time
import random


def main():
    # RFID引数を受け取り
    if len(sys.argv) >= 2:
        rfid = sys.argv[1]
    else:
        rfid = "UNKNOWN"

    print(f"=== Compartment Simple Test ===")
    print(f"RFID: {rfid}")
    print(f"課題を開始します...")

    # 簡易タスクシミュレーション（3秒待機）
    print("3秒間の課題シミュレーション中...")
    time.sleep(3)

    # ランダムに正解/不正解を決定（テスト用）
    is_correct = random.random() > 0.3  # 70%の確率で正解

    if is_correct:
        print("課題結果: 正解")
        print("RESULT:CORRECT")
    else:
        print("課題結果: 不正解")
        print("RESULT:INCORRECT")

    print("=== テスト完了 ===")


if __name__ == "__main__":
    sys.exit(main() or 0)
