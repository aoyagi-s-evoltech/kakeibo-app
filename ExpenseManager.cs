class ExpenseManager
{
    /// <summary>
    /// 支出を1件データベースに追加する処理。
    /// </summary>
    /// <remarks>
    /// ユーザーから日付・金額・カテゴリ・メモの入力を受け取る
    /// expensesテーブルへ追加(INSERT)する。
    /// cancel と入力された場合は処理を中断してメニューに戻る
    /// </remarks>

    public void AddExpense()
    {
        Console.WriteLine("支出追加処理");
        Console.WriteLine("入力をやめるときは「cancel」と入力してください");

        var repository = new ExpenseRepository();

        // 日付(必須)正しい日付形式が入力されるまで繰り返す
        DateTime date;
        while (true)
        {
            var dateText = Prompt("日付を入力してください（例: 2026/01/21）(cancelで中止)");
            if (dateText is null)
            {
                Console.WriteLine("入力を中止しました。");
                return;
            }

            // 日付以外が入った場合エラーメッセージを表示
            if (DateTime.TryParse(dateText, out date))
            {
                break;
            }
            Console.WriteLine("正しい日付を入力してください。");
        }

        // 金額(必須)数字が入力されるまで繰り返す
        int price;
        while (true)
        {
            var priceText = Prompt("金額を入力してください（数字のみ）(cancelで中止)");
            if(priceText is null)
            {
                Console.WriteLine("入力を中止しました。");
                return;
            }

            // 数値以外が入った場合エラーメッセージを表示
            if (int.TryParse(priceText, out price))
            {
                break;
            }
            Console.WriteLine("数値を入力してください。");
        }

        // カテゴリ(必須)null不可
        string category;
        while (true)
        {
            var categoryText = Prompt("カテゴリを入力してください(cancelで中止)");
            if(categoryText is null)
            {
                Console.WriteLine("入力を中止しました。");
                return;
            }

            if (!string.IsNullOrWhiteSpace(categoryText))
            {
                category = categoryText;
                break;
            }
            Console.WriteLine("カテゴリが入力されていません。");
        }

        // メモ(任意)null可
        var memo = Prompt("メモを入力してください(cancelで中止)");
        if(memo is null)
        {
            Console.WriteLine("入力を中止しました。");
            return;
        }

        // 取得したidと入力内容をDBに登録
        repository.Insert(date.ToString("yyyy/MM/dd"), price, category, memo);

        Console.WriteLine("支出を追加しました");
    }

    /// <summary>
    /// 支出として登録されているデータを一覧で表示する
    /// </summary>
    /// <remarks>
    /// Repositoryから取得した支出データ（id/日付/金額/カテゴリ/メモ）を出力する
    /// データが1件もない場合、「データがありません」と表示
    /// </remarks>
    public bool ShowExpenses()
    {
        Console.WriteLine("支出一覧");
        
        // Repositoryから全件取得
        var repository = new ExpenseRepository();
        var list = repository.GetAll();

        // データがない場合
        if(list.Count == 0)
        {
            Console.WriteLine("データがありません");
            return false;
        }

        // データがある場合
        foreach(var expense in list)
        {
            Console.WriteLine($"ID:{expense.Id}  日付:{expense.Date}  金額:{expense.Price}  カテゴリ:{expense.Category}  メモ:{expense.Memo}");
        }
            return true;
    }

    /// <summary>
    /// 支出データを編集
    /// </summary>
    /// <remarks>
    /// ユーザーに編集したいidを入力してもらい、
    /// Repositoryから対象データを取得。
    /// データが存在する場合、現在の内容を表示し、
    /// ユーザーより新しい値を入力してもらった後更新。
    /// </remarks>
    public void EditExpense()
    {
        Console.WriteLine("支出編集処理");

        ShowExpenses();

        var repository = new ExpenseRepository();

        // 編集したいidを入力
        int id;
        Expense expense;

        while (true)
        {
            var idText = Prompt("編集したいidを入力してください(cancelで中止)");
            if (idText is null)
            {
                Console.WriteLine("入力を中止しました。");
                return;
            }

            if (!int.TryParse(idText, out id))
            {
                Console.WriteLine("数字を入力してください。");
                continue;
            }

            expense = repository.GetById(id);
            if (expense == null)
            {
                Console.WriteLine("入力されたidがありません。もう一度入力してください。");
                continue;
            }

            break;
        }

        // 現在の内容を表示
        Console.WriteLine($"現在の内容：日付:{expense.Date}  金額:{expense.Price}  カテゴリ:{expense.Category}  メモ:{expense.Memo}");

        // 新しい値を入力
        DateTime newDateValue;

        while (true)
        {
            var newDateText = Prompt("新しい日付を入力してください（例: 2026/01/21）(cancelで中止)");
            if (newDateText is null)
            {
                Console.WriteLine("入力を中止しました。");
                return;
            }

            // 日付以外が入った場合エラーメッセージを表示
            if (DateTime.TryParse(newDateText, out newDateValue))
            {
                break;
            }
            Console.WriteLine("正しい日付を入力してください。");
        }
        expense.Date = newDateValue.ToString("yyyy/MM/dd");

        
        while(true)
        {
            var newPriceText = Prompt("新しい金額を入力してください(cancelで中止)");
            if(newPriceText is null)
            {
                Console.WriteLine("入力を中止しました。");
                return;
            }

            if(int.TryParse(newPriceText, out var newPrice))
            {
                expense.Price = newPrice;
                break;
            }
            Console.WriteLine("数値を入力してください");
        }

        var newCategory = Prompt("新しいカテゴリを入力してください(cancelで中止)");
        if(newCategory is null)
        {
            Console.WriteLine("入力を中止しました。");
            return;
        }
        expense.Category = newCategory;

        var newMemo = Prompt("新しいメモを入力してください(cancelで中止)");
        if(newMemo is null)
        {
            Console.WriteLine("入力を中止しました。");
            return;
        }
        expense.Memo = newMemo;

        // 更新処理
        repository.Update(expense);

        Console.WriteLine("更新しました");
    }

    /// <summary>
    /// 支出データを削除する
    /// </summary>
    /// <remarks>
    /// 一覧を表示し、ユーザーに削除したいidを入力してもらう
    /// 対象データが存在する場合、該当データを削除する
    /// </remarks>
    public void DeleteExpense()
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

        // 削除したいidを入力
        var idText = Prompt("削除したいidを入力してください(cancelで中止)");
        if(idText is null)
        {
            Console.WriteLine("入力を中止しました。");
            return;
        }

        var id = int.Parse(idText);

        var repository = new ExpenseRepository();

        // idが存在するか確認
        var expense = repository.GetById(id);

        // idが取得できなければ、メッセージを出力し終了
        if(expense == null)
        {
            Console.WriteLine("入力されたidがありません");
            Console.WriteLine("Enterキーで戻ります");
            Console.ReadLine();
            return;
        }

        // idが取得できた場合は削除前に確認
        Console.WriteLine($"ID:{expense.Id} を本当に削除しますか？ (yes/no)");
        var confirm = Console.ReadLine();

        // yes(大文字まじりOK)以外の場合削除しない
        if(confirm?.ToLower() != "yes")
        {
            Console.WriteLine("削除をキャンセルしました。");
            return;
        }

        // 取得できた場合は削除を実行
        repository.Delete(id);

        Console.WriteLine("削除しました");
        Console.WriteLine("Enterキーで戻ります");
        Console.ReadLine();
    }

    /// <summary>
    /// ユーザーにメッセージを表示して入力を受け取る
    /// 「cancel」と入力された場合は処理を中止できるようnullを返す。
    /// </summary>
    /// <param name="message">画面に表示するメッセージ</param>
    /// <returns>入力された文字列。cancelの場合はnull</returns>
    public static string Prompt(string message)
    {
        Console.Write(message);
        var input = Console.ReadLine();

        if (input?.ToLower() == "cancel")
        {
            Console.WriteLine("キャンセルされました。");
            return null;
        }

        return input;
    }
}
