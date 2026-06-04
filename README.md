# Rogue-like Dungeon Exploration

このプロジェクトは、Unityで動く2D探索型ダンジョンゲームの基盤です。

プレイヤーはマウスクリックで隣接する部屋を選択し、電力リソースを管理しながら探索を進めます。

## できること

- 3層構造のダンジョン生成
- 各層はランダムに生成される部屋の集合で構成
- 階段部屋で「次の層へ進む / 留まる」選択が可能
- 3層目の最奥にボス部屋を配置
- 隣接部屋をマウスクリックで移動
- 部屋移動ごとに電力を1消費
- 2層以上へ進むと前の層は非表示になり、現在の層のみが表示される
- 各層のスタート部屋は `Basement` 扱いで、階層移動後に全回復
- フェード付きのイベント画面を表示

## 主要スクリプト

- `Rogue/Assets/Scripts/DungeonGenerator.cs`
  - マルチフロア迷宮生成、部屋配置、階層遷移、電力管理、イベント割り当て
- `Rogue/Assets/Scripts/DungeonRoom.cs`
  - 部屋状態、層情報、表示/非表示、クリック移動
- `Rogue/Assets/Scripts/RoomEventManager.cs`
  - イベント画面、選択UI、フェード制御
- `Rogue/Assets/Scripts/CameraFollow.cs`
  - カメラを現在部屋に追従

## Unityでのセットアップ手順

1. `Rogue` フォルダをUnityで開く
2. `Rogue/Assets/Scripts` のスクリプトがプロジェクトに存在することを確認
3. `Hierarchy` に空のオブジェクト `DungeonManager` を作成
   - `DungeonGenerator` をアタッチ
4. `Hierarchy` に空のオブジェクト `RoomParent` を作成
   - `DungeonGenerator.roomParent` に割り当て
5. ルーム用プレハブ `RoomPrefab` を作成
   - `Create Empty` で `RoomPrefab` を作成
   - `Add Component` で `Sprite Renderer` を追加し、スプライトを割り当てる
   - `Add Component` で `Box Collider 2D` を追加
   - `Add Component` で `DungeonRoom` を追加
   - `Project` ウィンドウにドラッグしてプレハブ化
6. `DungeonManager.roomPrefab` に `RoomPrefab` を割り当て
7. `DungeonGenerator` の設定を調整
   - `roomCount`：生成する部屋数
   - `gridSize`：マップの広さ
   - `floorCount`：階層数
   - `floorSpacing`：層間の表示距離
   - `eventSpawnChance`：イベント発生確率
8. シーンを保存し、`Play` で動作確認

## 電力とイベントの仕組み

- `Power` は初期値 `10`
- 部屋移動ごとに `Power` が `1` 減少
- `Power` が `0` になるとゲームオーバー
- `Basement` に移動すると `Power` が全回復
- 階段を使って次の層へ進むと、その層の最初の部屋が `Basement` 扱いになる
- 部屋にランダムイベントが割り当てられた場合、イベント画面が表示される
- 「電力回復イベント」は1度きりで、再度同じ部屋で発生しない

## UIについて

- `DungeonGenerator.powerText` を割り当てると指定した `Text` に電力と階層を表示
- `powerText` が未割当の場合は自動で `Canvas` と `Text` が生成される
- `RoomEventManager` はイベント画面と2ボタン選択UIを自動生成する

## 進行中の仕様

- 3層目の最奥部屋は `Boss` ルームに設定
- 2層以降では前の層は非表示になり、現在の層のみ表示される
- `roomPrefab` テンプレートは生成時に非表示にしてHierarchyに残さない

## 今後追加したい機能

- 複数種類の部屋イベント
- 戦闘イベント
- アイテムや装備
- マップおよびステージの進行管理
- 本格的なボス戦とクリア条件
