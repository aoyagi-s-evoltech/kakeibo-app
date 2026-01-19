using Microsoft.Data.Sqlite;
using System.Text;

class Program
{
    static void Main()
    {
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

        var manager = new ExpenseManager();
        manager.AddExpense();

        switch (input)
        {
            // 追加
            case "1":
                manager.AddExpense();
                break;

            // 一覧
            case "2":
                manager.ShowExpenses();
                break;

            // 編集
            case "3":
                manager.EditExpense();
                break;
            
            // 削除
            case "4":
                manager.DeleteExpense();
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
}