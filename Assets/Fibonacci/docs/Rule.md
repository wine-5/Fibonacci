# C# 命名規則

## 🏷️ 基本ルール

### クラス・構造体・インターフェース・列挙型
- **PascalCase** を使用
- インターフェースは `I` で始める

### メソッド・プロパティ
- **PascalCase** を使用
- メソッドは動詞で始める

### フィールド・変数・引数
- **camelCase** を使用

### 定数
- **UPPER_SNAKE_CASE** を使用

---

## 📁 ファイル・フォルダ

### プロジェクト構造
```
Assets/Fibonacci/
├── Scripts/
│   ├── Player/
│   ├── Enemy/
│   ├── UI/
│   └── Manager/
├── Prefabs/
├── Materials/
└── Scenes/
```

### ファイル命名
- **PascalCase** を使用
- 機能が分かる名前にする

---

## 🔧 Unity 固有

### ScriptableObject
- `SO` サフィックスを付ける

### Inspector
- `[Header]` と `[SerializeField]` を適切に使用
- NaughtyAttributesの使用でもあり

---

## 🎯 ベストプラクティス

### イベント
- `On` で始める

### bool 型
- `Is`, `Has`, `Can` で始める

### コレクション
- 複数形にする

---

## ⚠️ 禁止事項

1. 略語の使用（一般的なものを除く）
2. 数字での区別
3. 日本語の使用（コメントを除く）

## 明示的に書いてほしいもの
- privateは明示的に書く