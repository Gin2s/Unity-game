# Rogue-like Dungeon Exploration

このプロジェクトは、Unityで動く2D探索型ダンジョンゲームの基盤です。

プレイヤーはマウスクリックで隣接する部屋を選択し、電力リソースを管理しながら探索を進めます。

## できること

- ランダムに生成される迷路状の部屋マップ
- 隣接部屋をマウスクリックで移動
- 最初の部屋は `Basement` で、そこから探索を開始
- 部屋移動ごとに電力を1消費
- `Basement` で電力を全回復
- イベントプールから抽選される部屋イベント
- フェードアウト／フェードイン付きのイベント画面
- 一度発生したイベントは再発生しない

## 主要スクリプト

- `Rogue/Assets/Scripts/DungeonGenerator.cs`
  - 迷宮生成、部屋接続、電力管理、イベントプール処理
- `Rogue/Assets/Scripts/DungeonRoom.cs`
  - 部屋データ、表示/非表示、クリック移動
- `Rogue/Assets/Scripts/RoomEventManager.cs`
  - 部屋イベント画面とフェード制御
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
   - `eventSpawnChance`：イベント発生確率
8. シーンを保存し、`Play` で動作確認

## 電力とイベントの仕組み

- `Power` は初期値 `10`
- 部屋移動ごとに `Power` が `1` 減少
- `Power` が `0` になるとゲームオーバー
- `Basement` に移動すると `Power` が `10` に回復
- 部屋を踏むたびにイベントプールから抽選
- 一度使われたイベントはプールから削除され、再発生しない
- 現在は「電力を3得るイベント」がプールに1つだけ入っており、探索開始直後に発生することを想定

## UIについて

- `DungeonGenerator.powerText` を割り当てると指定した `Text` に表示される
- 割り当てがない場合は自動で `Canvas` と `Text` を生成し、電力を表示する
- `RoomEventManager` はイベント画面も自動生成する

## 今後追加したい機能

- 複数種類の部屋イベント
- 戦闘イベント
- アイテムや装備
- マップ遷移
- ボス戦とクリア条件
