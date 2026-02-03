using Microsoft.Data.Sqlite;

/// <summary>
/// データの取得・追加・更新・削除を行うリポジトリクラス
/// </summary>
class ExpenseRepository
{
    /// <summary>
    /// データベースとテーブルを初期化する
    /// テーブルが存在しない場合は新規作成する
    /// </summary>
    public void Initialize()
    {
        using var connection = new SqliteConnection("Data Source=expenses.db");
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS expenses (
                id INTEGER PRIMARY KEY,
                ""date"" TEXT,
                price INTEGER,
                category TEXT,
                memo TEXT
            );
        ";
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// 新しい支出データをデータベースに追加する
    /// </summary>
    /// <param name="expense">追加する支出データ</param>
    public void Insert(Expense expense)
    {
        // DBへ接続
        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();

            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // INSERT文で値を入れる
            command.CommandText = @"
                INSERT INTO expenses(id, ""date"", price, category, memo)
                VALUES(
                    (SELECT IFNULL(MAX(id), 0) + 1 FROM expenses),
                    @date,
                    @price,
                    @category,
                    @memo
                );
            ";

            // SQL文内の@dateにdateを渡す
            command.Parameters.AddWithValue("@date", expense.Date);
            // SQL文内の@priceにpriceを渡す
            command.Parameters.AddWithValue("@price", expense.Price);
            // SQL文内の@categoryにcategoryを渡す
            command.Parameters.AddWithValue("@category", expense.Category);
            // SQL文内の@memoにmemoを渡す
            command.Parameters.AddWithValue("@memo", expense.Memo);

            // INSERT文の実行
            command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// DBに登録されている全支出データを取得する
    /// </summary>
    /// <returns>支出データのリスト</returns>
    public List<Expense> GetAll()
    {
        var list = new List<Expense>();

        // DBへ接続
        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();

            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // 全件取得するSELECT文
            command.CommandText = @"
                SELECT id, ""date"", price, category, memo
                FROM expenses;
            ";

            // SQLを実行して結果を読み取る
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var expense = new Expense
                {
                    // DBの値をC#の型に変換
                    Id = Convert.ToInt32(reader["id"]),
                    Date = reader["date"].ToString(),
                    Price = Convert.ToInt32(reader["price"]),
                    Category = reader["category"].ToString(),
                    Memo = reader["memo"].ToString()
                };

                list.Add(expense);
            }
        }
        return list;
    }

    /// <summary>
    /// 指定したIDの支出データを1件取得する
    /// </summary>
    /// <param name="id">取得したいデータのID</param>
    /// <returns>該当データ(存在しない場合はnull)</returns>
    public Expense GetById(int id)
    {
        // DBへ接続
        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();

            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // 1件取得するSELECT文
            command.CommandText = @"
                SELECT id, ""date"", price, category, memo
                FROM expenses
                WHERE id = @id;
            ";

            // SQL文内の@idにidを渡す
            command.Parameters.AddWithValue("@id", id);

            // SELECT文を実行
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                return new Expense
                {
                    // DBの値をC#の型に変換
                    Id = Convert.ToInt32(reader["id"]),
                    Date = reader["date"].ToString(),
                    Price = Convert.ToInt32(reader["price"]),
                    Category = reader["category"].ToString(),
                    Memo = reader["memo"].ToString()
                };
            }
        }
        return null;
    }

    /// <summary>
    /// 指定したIDの支出データを更新
    /// </summary>
    /// <param name="expense">更新内容を含むデータ</param>
    public void Update(Expense expense)
    {
        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();

            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // 指定したIDのデータをUPDATE
            command.CommandText = @"
                UPDATE expenses
                SET ""date"" = @date,
                price = @price,
                category = @category,
                memo = @memo
                WHERE id = @id;
            ";

            // パラメータをSQLに渡す
            command.Parameters.AddWithValue("@date", expense.Date);
            command.Parameters.AddWithValue("@price", expense.Price);
            command.Parameters.AddWithValue("@category", expense.Category);
            command.Parameters.AddWithValue("@memo", expense.Memo);
            command.Parameters.AddWithValue("@id", expense.Id);

            // UPDATE文の実行
            command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 指定したIDのデータを削除
    /// </summary>
    /// <param name="id">削除対象のID</param>
    public void Delete(int id)
    {
        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();
            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // IDのデータを取得しデータが存在するのか確認
            command.CommandText = @"
                DELETE FROM expenses
                WHERE id = @id;
            ";

            // SQL文内の@idにidを渡す
            command.Parameters.AddWithValue("@id", id);

            command.ExecuteNonQuery();
        }
    }
}