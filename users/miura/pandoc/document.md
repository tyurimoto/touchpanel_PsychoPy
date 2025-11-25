---
title: "LIXIL様向け RX561ベースプロジェクト"
subtitle: "設計資料"
author: アトミック株式会社
date: February 2020
output:
    word_document:
        toc: true
        path: document.docx
        pandoc_args: ["--reference-doc=template.docx"]
toc-title: "目次"
---

# ほげ
ほげほげ  
## ほげほげ
ほげほげ1

<hr>

ほげほげ2

# はじめに
本書は LIXIL様向け RX561ベースプロジェクトの設計仕様書である。

# 尿成分センサ
## 全体
### 概要
```plantuml
@startuml
    title 尿成分センサ全体概要
    left to right direction

    rectangle PC

    actor 使用者
    使用者 -> PC : コマンド制御

    frame "マイコン" {
        rectangle シーケンス制御部
        使用者 --> シーケンス制御部 : 開始要求
        PC --> シーケンス制御部 : 開始要求

        rectangle パルス生成部
        シーケンス制御部 --> パルス生成部 : 実行

        rectangle データ転送部
        PC <== データ転送部 : データ

        rectangle サンプリング部
        データ転送部 <== サンプリング部 : バイナリデータ
    }

    frame "回路部" {
        rectangle 発光回路
        パルス生成部 ==> 発光回路 : パルス
  
        rectangle 測定部
        発光回路 ==> 測定部  : パルス
  
        rectangle 光電センサ
        光電センサ <== 測定部 : 処理対象波形
        サンプリング部 <== 光電センサ : アナログデータ
    }
@enduml
```

### シーケンス図
```plantuml
@startuml
    title 尿成分センサ全体シーケンス図
    hide footbox

    participant 使用者
    participant PC

    participant データ転送部
    participant シーケンス制御部
    participant パルス生成部
    participant サンプリング部

    participant 発光回路
    participant 光電センサ
    participant 測定部

    使用者 -> シーケンス制御部 : 開始要求

    activate シーケンス制御部
    シーケンス制御部 -> サンプリング部 : 実行
    activate サンプリング部
    シーケンス制御部 -> パルス生成部 : 実行
    deactivate シーケンス制御部

    activate パルス生成部
    パルス生成部 -> 発光回路 : パルス
    発光回路 -> 測定部
    deactivate パルス生成部
    hnote over パルス生成部 : 最大20秒

    activate 測定部
    測定部 -> 測定部 : 測定
    光電センサ <- 測定部 
    deactivate 測定部

    サンプリング部 <- 光電センサ : アナログデータ

    データ転送部 <- サンプリング部 : バイナリデータ
    activate データ転送部
    deactivate サンプリング部
    hnote over サンプリング部 : 最大20秒

    PC <- データ転送部 : データ
    deactivate データ転送部
    hnote over データ転送部 : 随時処理

@enduml
```

## サンプリング部
### 概要
```plantuml
@startuml
    left to right direction

    component CMT1
    note bottom of CMT1
        10kHz(100us)
        10000連続サンプリング/1s
    endnote

    interface サンプリング開始
    サンプリング開始 ~> CMT1 : "要求"

    component ELC
    CMT1 ..> ELC : "イベント"

    interface データ転送部完了0
    interface データ転送部完了1

    frame "サンプリング部" {
        frame "系統0" {
            component ADC0
            note top of ADC0
                シングルススキャン
                12ビット
                2チャンネル
            endnote
            ELC ..> ADC0 : "AD変換開始トリガ"

            component DTC0
            ADC0 .> DTC0 : "ADスキャン完了割込"
            データ転送部完了0 <.. DTC0 : "コールバック"

            database バッファ0
            DTC0 ==> バッファ0 : "DTC転送"
            note bottom of バッファ0
                40kByte =
                16ビット x
                2チャンネル x
                1個
            endnote
        }

        frame "系統1" {
            component ADC1
            ELC ..> ADC1 : "AD変換開始トリガ"

            component DTC1
            ADC1 .> DTC1 : "ADスキャン完了割込"

            database バッファ1
            DTC1 ==> バッファ1 : "DTC転送"
            データ転送部完了1 <.. DTC1 : "コールバック"
        }
    }

    frame "データ転送部" {
        queue データ転送部キュー0
        バッファ0 ==> データ転送部キュー0 : "バースト処理"

        queue データ転送部キュー1
        バッファ1 ==> データ転送部キュー1 : "バースト処理"
    }

@enduml
```

### シーケンス図
```plantuml
@startuml
    hide footbox

    participant ステートマシン
    participant システム
    participant CMT1
    participant ELC
    participant "ADC0[1]" as ADC
    participant "キュー0[1]" as キュー
    participant "データ転送部キュー0[1]" as データ転送部キュー

    ステートマシン -> システム : 開始要求

    loop 10000サンプリング/1s
        activate システム
        システム -> CMT1 : Start
        activate CMT1
        CMT1 ->> ELC : 割込
        システム <<-- CMT1 : コールバック
        ELC ->> ADC : トリガ
        ADC ->> キュー : データ転送
        システム <<-- ADC : コールバック
        システム -> キュー : データ転送要求
        キュー -> データ転送部キュー : データ転送
        ...
        ステートマシン -> システム : 停止要求
        システム -> CMT1 : Stop
        deactivate CMT1
        deactivate システム
    end

@enduml
```

# 超音波センサ
## 全体
### 概要
```plantuml
@startuml
    left to right direction

    rectangle PC

    actor 使用者
    使用者 -> PC : コマンド制御

    frame "マイコン" {
        rectangle シーケンス制御部
        使用者 --> シーケンス制御部 : 開始要求
        PC --> シーケンス制御部 : 開始要求

        rectangle スイープ波形生成部
        シーケンス制御部 --> スイープ波形生成部 : 実行

        rectangle データ転送部
        PC <== データ転送部 : データ

        rectangle サンプリング部
        データ転送部 <== サンプリング部 : バイナリデータ
    }

    frame "回路部" {
        rectangle 送信部
        スイープ波形生成部 ==> 送信部 : スイープ波形部

        rectangle 対象物
        送信部 ==> 対象物  : スイープ波形

        rectangle 受信部
        受信部 <== 対象物 : 処理対象波形
        サンプリング部 <== 受信部 : アナログデータ
    }

@enduml
```

### シーケンス図
```plantuml
@startuml
    hide footbox

    participant 使用者
    participant PC

    participant データ転送部
    participant シーケンス制御部
    participant スイープ波形生成部
    participant サンプリング部

    participant 送信部
    participant 受信部
    participant 対象物

    使用者 -> シーケンス制御部 : 開始要求

    loop システムのステートマシン
        loop 10000サンプリング/333ms
            activate シーケンス制御部
            シーケンス制御部 -> サンプリング部 : 実行
            activate サンプリング部
            シーケンス制御部 -> スイープ波形生成部 : 実行

            activate スイープ波形生成部
            スイープ波形生成部 ->> 送信部 : スイープ波形
            送信部 -> 対象物
            deactivate スイープ波形生成部
            hnote over スイープ波形生成部 : 最大20秒

            activate 対象物
            対象物 ->> 対象物 : 処理
            受信部 <<- 対象物 
            deactivate 対象物

            サンプリング部 <<-- 受信部 : アナログデータ

            データ転送部 <<-- サンプリング部 : バイナリデータ
            activate データ転送部
            deactivate サンプリング部
            hnote over サンプリング部 : 最大20秒

            PC <<-- データ転送部 : データ
            deactivate データ転送部
            hnote over データ転送部 : 随時処理
        end
    end

    使用者 -> シーケンス制御部 : 停止要求
    deactivate シーケンス制御部

@enduml
```

## サンプリング部
### 概要
```plantuml
@startuml
    left to right direction

    component CMT1
    note bottom of CMT1
        1MHz(1us)
        10000バーストサンプリング/333ms
    endnote

    interface サンプリング開始
    サンプリング開始 ~> CMT1 : "要求"

    component ELC
    CMT1 ..> ELC : "イベント"

    interface データ転送部完了0
    interface データ転送部完了1

    frame "サンプリング部" {
        frame "系統0" {
            component ADC0
            note top of ADC0
                シングルススキャン
                12ビット
                2チャンネル
            endnote
            ELC ..> ADC0 : "AD変換開始トリガ"

            component DTC0
            ADC0 .> DTC0 : "ADスキャン完了割込"
            データ転送部完了0 <.. DTC0 : "コールバック"

            database バッファ0
            DTC0 ==> バッファ0 : "DTC転送"
            note bottom of バッファ0
                40kByte =
                16ビット x
                2チャンネル x
                10000個
            endnote
        }

        frame "系統1" {
            component ADC1
            ELC ..> ADC1 : "AD変換開始トリガ"

            component DTC1
            ADC1 .> DTC1 : "ADスキャン完了割込"

            database バッファ1
            DTC1 ==> バッファ1 : "DTC転送"
            データ転送部完了1 <.. DTC1 : "コールバック"
        }
    }

    frame "データ転送部" {
        queue データ転送部キュー0
        バッファ0 ==> データ転送部キュー0 : "バースト処理"

        queue データ転送部キュー1
        バッファ1 ==> データ転送部キュー1 : "バースト処理"
    }

@enduml
```

### シーケンス図
```plantuml
@startuml
    hide footbox

    participant ステートマシン
    participant システム
    participant CMT1
    participant ELC
    participant "ADC0[1]" as ADC
    participant "DTC0[1]" as DTC
    participant "バッファ0[1]" as バッファ
    participant "データ転送部キュー0[1]" as データ転送部キュー

    ステートマシン -> システム : 開始要求

    loop 10000サンプリング/333ms
        activate システム
        システム -> CMT1 : Start
        activate CMT1
        CMT1 ->> ELC : 割込
        システム <<-- CMT1 : コールバック
        ELC ->> ADC : トリガ
        ADC ->> DTC : 割込
        DTC ->> バッファ : データ転送
        システム <<-- DTC : コールバック
        システム -> バッファ : データ転送要求
        バッファ -> データ転送部キュー : データ転送
        ...
        hnote over システム : 10000回でStop
        システム -> CMT1 : Stop
        deactivate CMT1
        deactivate システム
    end

@enduml
```

# データ転送部概要
```plantuml
@startuml
    left to right direction


    frame "サンプリング部" {
            database データ空間0
            database データ空間1
    }

    frame "データ転送部" {
        queue データ転送部キュー0
        データ空間0 ==> データ転送部キュー0 : "データ転送"

        queue データ転送部キュー1
        データ空間1 ==> データ転送部キュー1 : "データ転送"

        node データ処理部
        データ転送部キュー0 ==> データ処理部 : Dequeue
        データ転送部キュー1 ==> データ処理部 : Dequeue
    }
    note top of データ転送部
        転送速度(バイナリ、見積もり)
            1,920,000[Bit/s]=
            x 16ビット
            x 4チャンネル
            x 10000個
            x 3バースト

        現在のテキスト転送フォーマット
        24バイト
        @[index][AD0][AD1][AD2][AD3][CR]
        @: 1バイト,ASCII
        index: 6バイト,10進数,0～999999
        AD[0..3]: 4バイト,10進数,0～4095
        [CR]:1バイト,ASCII

        30000サンプル/秒の転送速度は
        5,760,000bps

        シリアル転送の転送効率を頑張っても8割とすると
        15Mbpsで実効値10Mbps
    endnote

    interface シリアルデータ転送
    シリアルデータ転送 ~> データ処理部 : "要求"

    component SCI4
    component SCI5
    component SCI10
    データ処理部 <--> SCI4 : "制御(無線)"
    データ処理部 <-- SCI5 : "制御"
    データ処理部 ==> SCI5 : "データ(非同期シリアル)"
    データ処理部 ==> SCI10 : "データ(同期シリアル)"

@enduml
```

# 諸設定

## SCI使用状況(開発ベース)

|SCI|bps|デバイス|用途|
|:-:|:-:|:-:|-|
|4|115200|RN42|コンソール|
|5|921600|PL2303|コンソールまたはXMODEM|
|10|7.5M|FT232H|高速シリアル通信|


## タイマ使用状況(開発ベース)

|コンポーネント|インターバル|誤差|用途|使用先|
|:-:|:-:|:-:|-|-|
|CMT0|1ms|無|同期|1s同期<br>333ms同期<br>ステートマシン(1ms)|
|CMT1|1us<br>|有|ADC<br>DAC|サンプリング部<br>波形生成|
|CMT2|100ms|無|時間制御で使う|パルス発生<br>サンプリング部<br>波形生成<br>|
|CMT3|10us|無|DAC<br>(デバッグ用)|波形生成|
|TMR0|1ms|有|同期|XMODEM|
|TMR1|100us|有|同期|データ転送部|


# さいごに
