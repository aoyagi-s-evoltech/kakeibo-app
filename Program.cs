class Program
{
    static void Main()
    {
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