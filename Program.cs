using Microsoft.Data.Sqlite;
using System.Text;

class Program
{
    static void Main()
    {
        // データベース準備
        InitializeDatabase();

        var menu = new StringBuilder();

        menu.AppendLine("=== 家計簿アプリ ===");
        menu.AppendLine("1. 支出を追加する");
        menu.AppendLine("2. 支出一覧を見る");
        menu.AppendLine("3. 支出を編集する");
        menu.AppendLine("4. 支出を削除する");
        menu.AppendLine("5. 終了");
        menu.Append("番号を選んでください: ");

        Console.Write(menu.ToString());

        var input = Console.ReadLine();

        switch (input)
        {
            // 追加
            case "1":
                AddExpense();
                break;

            // 一覧
            case "2":
                ShowExpenses();
                break;

            // 編集
            case "3":
                EditExpense();
                break;
            
            // 削除
            case "4":
                DeleteExpense();
                break;

            case "5":
                Console.WriteLine("アプリを終了します。");
                return;
            default:
                Console.WriteLine("正しい番号を入力してください。");
                break;
        }
        Console.WriteLine();
    }

    /// <summary>
    /// データベースを用意する
    /// </summary>
    /// <remarks>
    /// expenses.dbに接続し、expensesテーブルを作成する。
    /// </remarks>
    static void InitializeDatabase()
    {
        Console.WriteLine("DB初期化処理");

        // 接続先
        var connectionString = "Data Source = expenses.db";

        // DBへ接続
        using (var connection = new SqliteConnection(connectionString))
        {
            // 接続開始
            connection.Open();

            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // テーブルの作成
            command.CommandText = @"CREATE TABLE IF NOT EXISTS expenses(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                date INTEGER NOT NULL,
                price INTEGER NOT NULL,
                category TEXT,
                memo TEXT
                );
            ";

            // CREATE文を実行
            command.ExecuteNonQuery();

            Console.WriteLine("データベースの準備完了");
        }
    }
    /// <summary>
    /// 支出を1件データベースに追加する処理。
    /// </summary>
    /// <remarks>
    /// ユーザーから日付・金額・カテゴリ・メモの入力を受け取る
    /// expensesテーブルへ追加(INSERT)する。
    /// </remarks>
    
    static void AddExpense()
    {
        Console.WriteLine("支出追加処理");

        // 入力を受け取る
        Console.WriteLine("日付を入力してください");
        var date = Console.ReadLine();

        Console.WriteLine("金額を入力してください");
        var price = Console.ReadLine();

        Console.WriteLine("カテゴリを入力してください");
        var category = Console.ReadLine();

        Console.WriteLine("メモを入力してください");
        var memo = Console.ReadLine();

        // DBへ接続
        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();

            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // INSERT文で値を入れる
            command.CommandText = @"
                INSERT INTO expenses(date, price, category, memo)
                VALUES(@date, @price, @category, @memo);
            ";

            // SQL文内の@dateにdateを渡す
            command.Parameters.AddWithValue("@date", date);
            // SQL文内の@priceにpriceを渡す
            command.Parameters.AddWithValue("@price", price);
            // SQL文内の@categoryにcategoryを渡す
            command.Parameters.AddWithValue("@category", category);
            // SQL文内の@memoにmemoを渡す
            command.Parameters.AddWithValue("@memo", memo);

            // INSERT文の実行
            command.ExecuteNonQuery();
        }
        Console.WriteLine("支出を追加しました");
    }

    /// <summary>
    /// 支出データを編集
    /// </summary>
    /// <remarks>
    /// ユーザーに編集したいidを入力してもらい、
    /// 対象データが存在する場合現在の内容を表示し、
    /// ユーザーより新しい値を入力してもらった後更新。
    /// </remarks>
    static void EditExpense()
    {
        Console.WriteLine("支出編集処理");

        // DBへ接続
        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();

            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // 編集したいidを入力
            Console.WriteLine("編集したいidを入力してください");
            var idText = Console.ReadLine();
            var id = int.Parse(idText);

            // idのデータを取得
            command.CommandText = @"
                SELECT id, date, price, category, memo
                FROM expenses
                WHERE id = @id;
            ";

            // SQL文内の@idにidを渡す
            command.Parameters.AddWithValue("@id", id);

            // SELECT文を実行
            using var reader = command.ExecuteReader();

            // データが1件でも登録されているか判定
            bool hasData = false;

            // 1件でもある場合
            while (reader.Read())
            {
                hasData = true;

                // 現在の内容を表示
                Console.WriteLine($"日付:{reader["date"]}  金額:{reader["price"]}  カテゴリ:{reader["category"]}  メモ:{reader["memo"]}");

                // 新しい値を入力
                Console.WriteLine("日付を入力してください");
                var date = Console.ReadLine();

                Console.WriteLine("金額を入力してください");
                var price = Console.ReadLine();

                Console.WriteLine("カテゴリを入力してください");
                var category = Console.ReadLine();

                Console.WriteLine("メモを入力してください");
                var memo = Console.ReadLine();

                // URDATEを実行するため、readerを閉じる
                reader.Close();

                // UPDATE文で上書き
                command.CommandText = @"
                    UPDATE expenses 
                    SET date = @date, price = @price, category = @category, memo = @memo
                    WHERE id = @id;
                ";

                // 前のSELECTで使ったパラメータを一度全部消す
                command.Parameters.Clear();

                // SQL文内の@idにidを渡す
                command.Parameters.AddWithValue("@id", id);
                // SQL文内の@dateにdateを渡す
                command.Parameters.AddWithValue("@date", date);
                // SQL文内の@priceにpriceを渡す
                command.Parameters.AddWithValue("@price", price);
                // SQL文内の@categoryにcategoryを渡す
                command.Parameters.AddWithValue("@category", category);
                // SQL文内の@memoにmemoを渡す
                command.Parameters.AddWithValue("@memo", memo);
                
                // UPDATE文の実行
                command.ExecuteNonQuery();

                Console.WriteLine("更新しました");
                break;
            }

            // データがない場合
            if(!hasData)
            {
                Console.WriteLine("入力されたidがありません");
            }
        }
    }

    /// <summary>
    /// 支出データを削除する
    /// </summary>
    /// <remarks>
    /// 一覧を表示し、ユーザーに削除したいidを入力してもらう
    /// 対象データが存在する場合、該当データを削除する
    /// </remarks>
    static void DeleteExpense()
    {
        Console.WriteLine("支出削除処理");

        // 一覧表示
        bool hasData = ShowExpenses();

        // データがない場合はメッセージを出力し、終了
        if(!hasData)
        {
            Console.WriteLine("削除可能なデータがありません");
            Console.WriteLine("Enterキーで戻ります");
            Console.ReadLine();
            return;
        }

        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();
            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // 削除したいidを入力
            Console.WriteLine("削除したいidを入力してください");
            var idText = Console.ReadLine();
            var id = int.Parse(idText);
            
            // idのデータを取得しデータが存在するのか確認
            command.CommandText = @"
                SELECT id, date, price, category, memo
                FROM expenses
                WHERE id = @id;
            ";
            
            // SQL文内の@idにidを渡す
            command.Parameters.AddWithValue("@id", id);

            var reader = command.ExecuteReader(); 

            // idが取得できればtrue
            bool exists = reader.Read();
            reader.Close();

            // idが取得できなければ、メッセージを出力し終了
            if(!exists)
            {
                Console.WriteLine("入力されたidがありません");
                Console.WriteLine("Enterキーで戻ります");
                Console.ReadLine();
                return;
            }

            // 取得できた場合はDELETE文を実行
            command.CommandText = @"
                DELETE FROM expenses
                WHERE id = @id;
            ";
            command.ExecuteNonQuery();

            Console.WriteLine("削除しました");
            Console.WriteLine("Enterキーで戻ります");
            Console.ReadLine();
        }
    }

    /// <summary>
    /// 支出として登録されているデータを一覧で表示する
    /// </summary>
    /// <remarks>
    /// id / 日付 / 金額 / カテゴリ / メモ を1行ずつ読み取り、整形して出力する。
    /// データが1件もない場合、「データがありません」と表示
    /// </remarks>
    static bool ShowExpenses()
    {
        Console.WriteLine("支出一覧");
        
        // DBへ接続
        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();

            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // expensesから全件SELECT
            command.CommandText = @"
                SELECT id, date, price, category, memo
                FROM expenses
            ";

            // SQLを実行し、結果を読み取る
            using var reader = command.ExecuteReader();

            // データが1件でも登録されているか判定
            bool hasData = false;

            // 1行ずつ読み取って表示
            while (reader.Read())
            {
                // データが1件でもあればtrue
                hasData = true;
                // id/date/price/category/memoを見やすくし、表示
                Console.WriteLine($"日付:{reader["date"]}  金額:{reader["price"]}  カテゴリ:{reader["category"]}  メモ:{reader["memo"]}");
            }

            // データがない場合
            if(!hasData)
            {
                Console.WriteLine("データがありません");
            }
            return hasData;
        }
    }
}