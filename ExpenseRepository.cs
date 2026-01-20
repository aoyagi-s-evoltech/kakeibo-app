using Microsoft.Data.Sqlite;

class ExpenseRepository
{
    private const string ConnectionString = "Data Source = expenses.db";

    public void Insert(string date, int price, string category, string memo)
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
    }

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
                SELECT id, date, price, category, memo
                FROM expenses;
            ";

            // SQLを実行して結果を読み取る
            using var reader = command.ExecuteReader();

            while(reader.Read())
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

        public List<Expense> GetById(int id)
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
                SELECT id, date, price, category, memo
                FROM expenses
                WHERE id = @id;
            ";

            // SQL文内の@idにidを渡す
            command.Parameters.AddWithValue("@id", id);

            // SELECT文を実行
            using var reader = command.ExecuteReader();

            while(reader.Read())
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
        return null;
    }
}