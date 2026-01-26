using System.Text;

class Program
{
    static void Main()
    {
        // DBとテーブルを用意
        var repo = new ExpenseRepository();
        repo.Initialize();

        // メニュー処理
        var manager = new ExpenseManager();

        while(true)
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