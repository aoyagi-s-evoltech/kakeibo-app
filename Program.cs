using System.ComponentModel;
using Microsoft.Data.Sqlite;

class Program
{
    static void Main()
    {

        // データベース準備
        InitializeDatabase();

        Console.WriteLine("=== 家計簿アプリ ===");
        Console.WriteLine("1. 支出を追加する");
        Console.WriteLine("2. 支出一覧を見る");
        Console.WriteLine("3. 支出を編集する");
        Console.WriteLine("4. 支出を削除する");
        Console.WriteLine("5. 終了");
        Console.Write("番号を選んでください: ");

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

    static void InitializeDatabase()
    {
        Console.WriteLine("DB初期化処理");

        // 接続先情報
        var connectionString = "Data Source = expenses.db";

        // インスタンスの生成
        using (var connection = new SqliteConnection(connectionString))
        {
            // 接続開始
            connection.Open();

            // SQL実行のためのコマンドを作成
            using var command = connectionString.CreateCommand();

            // expensesテーブルの作成
            command.CommandText = @"CREATE TABLE IF NOT EXISTS expenses(
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                date INTEGER NOT NULL,
                price INTEGER NOT NULL,
                category TEXT,
                memo TEXT
                );
            ";
            command.ExecuteNonQuery();

            Console.WriteLine("データベースの準備完了");
        }
    }
    static void AddExpense()
    {
        Console.WriteLine("支出追加処理");
    }

    static void EditExpense()
    {
        Console.WriteLine("支出編集処理");
    }

    static void DeleteExpense()
    {
        Console.WriteLine("支出削除処理");
    }

    static void ShowExpenses()
    {
        Console.WriteLine("一覧表示");
    }
}