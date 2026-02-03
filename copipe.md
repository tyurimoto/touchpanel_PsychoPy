 方法1: パッケージマネージャーコンソールで直接インストール

  Visual Studio 2022のパッケージマネージャーコンソールで実行：

  # Svgパッケージをインストール
  Install-Package Svg -ProjectName Compartment

  # または、特定バージョンを指定
  Install-Package Svg -Version 3.4.1 -ProjectName Compartment

  方法2: NuGetパッケージマネージャーGUIを使用

  1. ツール → NuGetパッケージマネージャー → ソリューションのNuGetパッケージの管理
  2. 参照 タブを選択
  3. 検索ボックスに「Svg」と入力
  4. Svg パッケージを選択（作者: svg-net）
  5. 右側で Compartment プロジェクトにチェック
  6. インストール ボタンをクリック

  方法3: packages.configを確認してパッケージを復元

  すでにpackages.configにSvgが記載されている場合：

  # ソリューション全体のパッケージを復元
  dotnet restore

  # または
  nuget restore Compartment.sln

