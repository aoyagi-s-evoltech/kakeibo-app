using System.Text;

/// <summary>
/// 家計簿アプリを実行するクラス
/// </summary>
class Program
{
    static void Main()
    {
        // DBとテーブルを用意
        var repo = new ExpenseRepository();
        repo.Initialize();

        // メニュー処理
        var manager = new ExpenseManager();

        while (true)
        {
            var sb = new StringBuilder();

            sb.AppendLine("=== 家計簿アプリ ===");
            sb.AppendLine("1. 支出を追加する");
            sb.AppendLine("2. 支出一覧を見る");
            sb.AppendLine("3. 支出を編集する");
            sb.AppendLine("4. 支出を削除する");
            sb.AppendLine("5. 終了");
            sb.Append("番号を選んでください: ");

            Console.Write(sb.ToString());

            var input = Console.ReadLine();
            Console.WriteLine();

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
        }
    }
}