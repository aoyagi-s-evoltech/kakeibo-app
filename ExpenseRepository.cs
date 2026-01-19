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
}