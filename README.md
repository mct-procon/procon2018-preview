# procon2018-preview
第29回全国高等専門学校プログラミングコンテスト 競技部門 提出用. Developerd by 松江高専（Go Suzuki, Naoto Kawakami, Renju Aoki, RIkuto Sueta）

## ディレクトリ構造
開発では，procon2018-AI-A, B, Cとprocon2018-Interface，procon2018-protocolの3つのリポジトリに分けて開発しました．そのため，ディレクトリもそのように分かれています．  
それぞれのリポジトリの内容は以下の通りです．
> procon2018-AI-A：実験用AI（居田作成）  
procon2018-AI-B：実験用AI（青木作成）  
procon2018-AI-C：メインAI（鈴木，川上（サブメンバー）作成）  
procon2018-Interface：操作インターフェース（居田，鈴木作成）  
procon2018-protocol：プロセス間通信，AIの骨組み（鈴木作成）

## 動作環境
動作環境は以下の通りです．  
> OS：Windows 10 2018 April以降, 7 SP1  
メモリ：8GB以上  
CPU：最近のCPU  
.Netのバージョン：.Net Framework 4.7.2（Interface），.Net Core 2.1（AI）

# procon2018-AI
すべてのAIは，ポートを指定することでInterfaceと接続し，計算をします．  
AngryBee\AIディレクトリ下にそれぞれAIの実装があり，AngryBee下のProgram.csで，new部分の指定を変えることで動かすAIの種類を変更することができます．

## AI-A
AIの種類は以下の通りです．また，PointEvaluator/Disperison.csは自陣の座標の分散を考慮に入れた評価関数です．どのAIにも使われていませんが，考察が終わり次第使う予定です．
### AI_IterativePriSurround
囲みの評価を高めたAIです．
### AI_PriorityErasing
相手の領域を消すときの評価を高めたAIです．

## AI-B
AIの種類は以下の通りです．ここに挙げた以外のAIはすべてここに挙げたAIの下位互換で，使わなくなったAIです．
### AI_hanpuku
反復深化のテスト実装です．AI-CのAIに取り込まれました．
### AI_hanpuku_SF
エージェント1と2を同時に動かすのではなく，順番に動かして探索するノード数を減らしたAIです．
### AI_kousi
縦横に動かなくしたAIです．
### AI_kousi_dx
AI_kousiがベースで，斜め方向すべてが自陣になっていた場合，縦横にも移動するAIです．
### AI_outside
ただただフィールドの縁に沿って動くAIです．

## AI-C
AIの種類は以下の通りです．
### AI
メインのAIです．反復深化し，Minimax法で解きます．評価関数はゲームの得点だけになっていますが，本番では，AI-AのPointEvaluator/Disperison.csを使用します．
### AhoAI
周囲8マスで一番得点の多い場所に進みます．

# procon2018-Interface
プログラムを起動して，Menu→新規ゲームでゲームが始まります．AIと対戦するときはAIに入力したポートとゲーム設定のポートを合わせてください．  

# procon2018-protocol
プロセス間通信の要，AIの骨組みです．