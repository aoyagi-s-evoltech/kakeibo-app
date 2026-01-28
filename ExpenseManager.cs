using System.Text;

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

        // 日付入力（cancelで中止。正しい日付が入るまでGetRequiredDateで再入力）
        var date = GetRequiredDate("日付を入力してください（例: 2026/01/21）(cancelで中止)");
        if(date == default)
        {
            return;
        }

        // 金額入力（cancelで中止。正しい数値が入るまでGetRequiredIntで再入力）
        var price = GetRequiredInt("金額を入力してください（数字のみ）(cancelで中止)");
        if(price == default)
        {
            return;
        }

        // カテゴリ入力（cancelで中止。空文字は再入力。GetRequiredStringで処理）
        var category = GetRequiredString("カテゴリを入力してください(cancelで中止)");
        if (category == null)
        {
            return;
        } 

        // メモ入力（任意入力。cancelで中止。空文字も許可）
        var memo = GetOptionalString("メモを入力してください(cancelで中止)");
        if(memo == null)
        {
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
        var sb = new StringBuilder();
        foreach(var expense in list)
        {
            sb.AppendLine($"ID:{expense.Id}");
            sb.AppendLine($"日付:{expense.Date}");
            sb.AppendLine($"金額:{expense.Price}");
            sb.AppendLine($"カテゴリ:{expense.Category}");
            sb.AppendLine($"メモ:{expense.Memo}");
        }
        Console.WriteLine(sb.ToString());
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

        // ID入力（cancelで中止。正しい数値が入るまでGetRequiredIntで再入力）
        var id = GetRequiredInt("編集したいidを入力してください(cancelで中止)");
        if (id == default)
        {
            return;
        }

        var expense = repository.GetById(id);
        if (expense == null)
        {
            Console.WriteLine("入力されたidがありません。");
            return;
        }

        // 現在の内容を表示
        Console.WriteLine($"現在の内容：日付:{expense.Date}  金額:{expense.Price}  カテゴリ:{expense.Category}  メモ:{expense.Memo}");

        // 新しい日付入力（cancelで中止。正しい日付が入るまでGetRequiredDateで再入力）
        var newDate = GetRequiredDate("新しい日付を入力してください（例: 2026/01/21）(cancelで中止)");
        if (newDate == default) return;
        expense.Date = newDate.ToString("yyyy/MM/dd");

        // 新しい金額入力（cancelで中止。正しい数値が入るまでGetRequiredIntで再入力）
        var newPrice = GetRequiredInt("新しい金額を入力してください(cancelで中止)");
        if (newPrice == default) return;
        expense.Price = newPrice;

        // 新しいカテゴリ入力（cancelで中止。空文字は再入力。GetRequiredStringで処理）
        var newCategory = GetRequiredString("新しいカテゴリを入力してください(cancelで中止)");
        if (newCategory == null) return;
        expense.Category = newCategory;

        // 新しいメモ入力（cancelで中止。空文字も許可。GetOptionalStringで処理）
        var newMemo = GetOptionalString("新しいメモを入力してください(cancelで中止)");
        if (newMemo == null) return;
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

        // 削除したいID入力（cancelで中止。正しい数値が入るまでGetRequiredIntで再入力）
        var id = GetRequiredInt("削除したいidを入力してください(cancelで中止)");
        if (id == default)
        {
            return;
        }
        
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
    /// メッセージを表示し、ユーザーの入力を受け取る
    /// </summary>
    /// <param name="message">画面に表示するメッセージ</param>
    /// <returns>入力された文字列</returns>
    public static string GetInput(string message)
    {
        Console.Write(message);
        return Console.ReadLine();
    }

    /// <summary>
    /// 入力された値がcancel(大文字可)かどうかを判断する
    /// </summary>
    /// <param name="input">ユーザーが入力した文字列</param>
    /// <returns>cancelの場合true、それ以外false</returns>
    public static bool IsCancel(string input)
    {
        return input?.ToLower() == "cancel";
    }

    /// <summary>
    /// 中止時のメッセージを表示する
    /// </summary>
    public static void ShowCancelMessage()
    {
        Console.WriteLine("入力を中止しました。");
    }

    private DateTime GetRequiredDate(string message)
    {
        while (true)
        {
            var text = GetInput(message);
            if (ShouldStopInput(text))
            {
                ShowCancelMessage();
                return default;
            }

            if (DateTime.TryParse(text, out var date))
            {
                return date;
            }

        Console.WriteLine("正しい日付を入力してください。");
        }
    }

    private int GetRequiredInt(string message)
    {
        while (true)
        {
            var text = GetInput(message);
            if (ShouldStopInput(text))
            {
                ShowCancelMessage();
                return default;
            }

            if (int.TryParse(text, out var value))
            {
                return value;
            }

        Console.WriteLine("数値を入力してください。");
        }
    }

    private string GetRequiredString(string message)
    {
        while (true)
        {
            var text = GetInput(message);
            if (ShouldStopInput(text))
            {
                ShowCancelMessage();
                return null;
            }

            if (!string.IsNullOrWhiteSpace(text))
            {
                return text;
            }

        Console.WriteLine("値が入力されていません。");
        }
    }

    private string GetOptionalString(string message)
    {
        var text = GetInput(message);

        if (ShouldStopInput(text))
        {
            ShowCancelMessage();
            return null;
        }

        return text;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ShouldStopInput(string? input)
    {
        return input == null || IsCancel(input);
    }
}
