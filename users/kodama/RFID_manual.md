<!--- pandoc RFID_manual.md -s -o RFID_manual.pdf --pdf-engine=lualatex -V documentclass=ltjsarticle -V luatexjapresetoptions=yu-win -N  --->

# RFID board specification


|Caption|DATA|
|:---- | :----: |
|demension|35x36mm|
|frequency|AM 134.2kHz|
|PowerSupplyVoltage|5~9V|
|Working current|recommend 80mA@6V|
|Working temperature|-40~+80℃|
|Target anntenna inductance|580uH|

### Pin definitions

|Pin|Function|
|:--|:--:|
|1|+5V to +9V pure linear Power Supply recommend 6V|
|2|RST reset,LowLevelEffective|
|3|TX serial port number output TTL5V level|
|4|GND|
|L1|Antenna1|
|L2|Antenna2(360Vpp)|


\clearpage

### TX Data fromat
- ASCII
- Baud rate 9600 8N2

<!-- 
|LSB| - |  |  | | | | | | | | | |  |  |  | | | | | | | |  |  |  | | | | | |MSB |
|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|-|
|02|36|34|45|38|34|33|32|31|30|37|45|33|30|30|31|30|30|31|30|30|30|30|30|30|30|30|30|30|0B|F4|03| -->
02 31 37 31 41 39 32 35 33 41 33 34 38 33 30 30 31 30 30 30 30 30 30 30 30 30 30 07 F8 03


|LSB| - ||||||||MSB|
|-|-|-|-|-|-|-|-|-|-|
|02|card number 10byte| country number 4byte|data block 1byte |animal flag 1byte|reserved 4byte| userdata 6byte|checksum 1byte|/checksum 1byte|03|
|1|2|3|4|5|6|7|8|9|10|

1. 02 start number (fixed)
1. 10bit HEX format ASCII card number, LSB first.
1. 4 bit HEX format ASCII country number, LSB first.
1. Data flag, 0 or 1.
1. Animal flag, 0 or 1.
1. reserved.
1. User data( it canbe rewritten with a writer.)
1. Checksum, all 26bity ACSII HEX XOR.
1. Checksum Bitwise invert.
1. 03 end number (fixed)


- 接続時RFIDタグを認識すると基板上、青LEDが点灯します。
その際以下のようにモジュールよりデータが出力されます。

例:
タグ表記:900250000023921 
10進プリフィックス:900
カードナンバー:250000023921

モジュールの出力は
02 31 37 31 41 39 32 35 33 41 33 34 38 33 30 30 31 30 30 30 30 30 30 30 30 30 30 07 F8 03

ASCII: 171A9253A34830010000000000
17A9253A3がカードナンバー、国ナンバーとして483が得られます (LSB first)

10進変換で 250000023921 国ナンバー 900

チェックサムは全体バイトのXORで演算され 07 です。