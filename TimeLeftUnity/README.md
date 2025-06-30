# Time Left Unity

Windows FormsアプリケーションをUnityに移植したバージョンです。今日の残り時間を視覚的に表示します。

## ファイル構成

### Scripts/
- `TimeLeftDisplay.cs` - UI Canvas版（推奨）
- `TimeLeft2D.cs` - 2D Sprite版
- `TimeLeftManager.cs` - セットアップ用マネージャー

## セットアップ方法

### 方法1: UI Canvas版（推奨）
1. 空のGameObjectを作成
2. `TimeLeftDisplay.cs`スクリプトをアタッチ
3. 実行

### 方法2: 2D Sprite版
1. 空のGameObjectを作成
2. `TimeLeft2D.cs`スクリプトをアタッチ
3. カメラを適切な位置に配置（推奨: Position (0, 0, -10)）
4. 実行

### 方法3: マネージャー使用
1. 空のGameObjectを作成
2. `TimeLeftManager.cs`スクリプトをアタッチ
3. 実行

## 表示内容

- **青い四角形**: 残り時間（大きなサイズ）
- **緑青色の四角形**: 残り分（中サイズ）
- **赤い四角形**: 残り秒（小サイズ）

## カスタマイズ

### 色の変更
各スクリプトのInspectorで以下の色を変更できます：
- `Hour Color` - 時間の色
- `Minute Color` - 分の色  
- `Second Color` - 秒の色

### サイズ調整
- `Hour Square Size` - 時間四角形のサイズ
- `Minute Square Size` - 分四角形のサイズ
- `Second Square Size` - 秒四角形のサイズ

### レイアウト
- `Hour Columns` - 時間四角形の列数
- `Minute Columns` - 分四角形の列数
- `Second Columns` - 秒四角形の列数

## 動作要件

- Unity 2019.4 以降
- .NET Framework 4.x または .NET Standard 2.1

## 注意事項

- リアルタイムで更新されるため、実行中はFrameRateが高くなる可能性があります
- 大量のGameObjectが生成されるため、パフォーマンスを考慮して使用してください
- モバイルビルド時は最適化を検討してください
