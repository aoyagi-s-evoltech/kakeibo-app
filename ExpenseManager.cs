using System.Text;

/// <summary>
/// 支出データの追加・更新・削除・表示などを管理するクラス
/// </summary>
class ExpenseManager
{
    /// <summary>
    /// 支出を1件データベースに追加する処理
    /// </summary>
    /// <remarks>
    /// ユーザーから日付・金額・カテゴリ・メモの入力を受け取る
    /// expensesテーブルへINSERTする
    /// cancelと入力された場合は処理を中断してメニューに戻る
    /// </remarks>
    public void AddExpense()
    {
        Console.WriteLine("支出追加処理");
        Console.WriteLine("入力をやめるときは「cancel」と入力してください");

        var repository = new ExpenseRepository();

        // 日付入力（cancelで中止。正しい日付が入るまでGetRequiredDateで再入力）
        Console.WriteLine("日付を入力してください（例: 2026/01/21）(cancelで中止)");
        var date = GetRequiredDate();
        if (date == default)
        {
            return;
        }

        // 金額入力（cancelで中止。正しい数値が入るまでGetRequiredIntで再入力）
        Console.WriteLine("金額を入力してください（数字のみ）(cancelで中止)");
        var price = GetRequiredInt();
        if (price == default)
        {
            return;
        }

        // カテゴリ入力（cancelで中止。空文字は再入力。GetRequiredStringで処理）
        Console.WriteLine("カテゴリを入力してください(cancelで中止)");
        var category = GetRequiredString();
        if (category == null)
        {
            return;
        }

        // メモ入力（任意入力。cancelで中止。空文字も許可）
        Console.WriteLine("メモを入力してください(cancelで中止)");
        var memo = GetOptionalString();
        if (memo == null)
        {
            return;
        }

        // 取得しidと入力内容をDBに登録
        var expense = new Expense
        {
            Date = date.ToString("yyyy/MM/dd"),
            Price = price,
            Category = category,
            Memo = memo
        };

        repository.Insert(expense);

        Console.WriteLine("支出を追加しました");
    }

    /// <summary>
    /// 支出として登録されているデータを一覧で表示する
    /// </summary>
    /// <remarks>
    /// Repositoryから取得した支出データ（id/日付/金額/カテゴリ/メモ）を出力する
    /// データが1件もない場合、「データがありません」と表示
    /// </remarks>
    /// <returns>
    /// データが1件以上存在する場合はtrue、
    /// データが存在しない場合はfalse
    /// </returns>
    public bool ShowExpenses()
    {
        Console.WriteLine("支出一覧");

        // Repositoryから全件取得
        var repository = new ExpenseRepository();
        var list = repository.GetAll();

        // データがない場合
        if (list.Count == 0)
        {
            Console.WriteLine("データがありません");
            return false;
        }

        // データがある場合
        var sb = new StringBuilder();
        foreach (var expense in list)
        {
            sb.AppendLine("ーーーーーーーーーー");
            sb.AppendLine($"id:{expense.Id}");
            sb.AppendLine($"日付:{expense.Date}");
            sb.AppendLine($"金額:{expense.Price}");
            sb.AppendLine($"カテゴリ:{expense.Category}");
            sb.AppendLine($"メモ:{expense.Memo}");
        }
        sb.AppendLine("ーーーーーーーーーー");

        Console.WriteLine(sb.ToString());
        return true;
    }

    /// <summary>
    /// 支出データを編集
    /// </summary>
    /// <remarks>
    /// ユーザーに編集したいidを入力してもらい、
    /// Repositoryから対象データを取得
    /// データが存在する場合、現在の内容を表示し、
    /// ユーザーより新しい値を入力してもらった後更新
    /// </remarks>
    public void EditExpense()
    {
        Console.WriteLine("支出編集処理");

        // ユーザーが変更したいidを選びやすいよう、一覧を表示
        var hasData = ShowExpenses();
        if (!hasData)
        {
            Console.WriteLine("現在データはありません");
            return;
        }

        var repository = new ExpenseRepository();

        // id入力（cancelで中止。正しい数値が入るまでGetRequiredIntで再入力）
        Console.Write("編集したいidを入力してください(cancelで中止): ");
        var id = GetRequiredInt();
        if (id == default)
        {
            return;
        }

        // 入力されたidに対応する支出データを取得する
        var expense = repository.GetById(id);
        if (expense == null)
        {
            Console.WriteLine("入力されたidのレコードが存在しません");
            return;
        }

        // ユーザーが変更内容を確認できるよう、現在の値を表示
        Console.WriteLine($"現在の内容：日付:{expense.Date}  金額:{expense.Price}  カテゴリ:{expense.Category}  メモ:{expense.Memo}");

        while (true)
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("何を編集しますか？");
            sb.AppendLine("1: 日付");
            sb.AppendLine("2: 金額");
            sb.AppendLine("3: カテゴリ");
            sb.AppendLine("4: メモ");
            sb.AppendLine("5: 編集を終了する");

            Console.WriteLine(sb.ToString());

            var choice = GetRequiredInt();
            if (choice == default)
            {
                return;
            }

            switch (choice)
            {
                case 1:
                    Console.WriteLine("新しい日付を入力してください（例: 2026/01/21）(cancelで中止)");
                    if (!UpdateDate(expense)) return;
                    break;

                case 2:
                    Console.WriteLine("新しい金額を入力してください(cancelで中止)");
                    if (!UpdatePrice(expense)) return;
                    break;

                case 3:
                    Console.WriteLine("新しいカテゴリを入力してください(cancelで中止)");
                    if (!UpdateCategory(expense)) return;
                    break;

                case 4:
                    Console.WriteLine("新しいメモを入力してください(cancelで中止)");
                    if (!UpdateMemo(expense)) return;
                    break;

                case 5:
                    repository.Update(expense);
                    Console.WriteLine("更新しました");
                    return;

                default:
                    Console.WriteLine("1〜5の番号を入力してください");
                    break;
            }
        }
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

        // 一覧にデータがない場合は、メッセージを表示、一旦停止してから戻る
        if (!ShowExpenses())
        {
            Console.WriteLine("Enterキーで戻ります");
            Console.ReadLine();
            return;
        }

        // 削除したいid入力（cancelで中止。整数が入力されるまでGetRequiredIntで再入力）
        Console.WriteLine("削除したいidを入力してください(cancelで中止)");
        var id = GetRequiredInt();
        if (id == default)
        {
            return;
        }

        var repository = new ExpenseRepository();

        // idが存在するか確認
        var expense = repository.GetById(id);

        // idが取得できなければ、メッセージを出力し終了
        if (expense == null)
        {
            Console.WriteLine("入力されたidのレコードが存在しません");
            Console.WriteLine("Enterキーで戻ります");
            Console.ReadLine();
            return;
        }

        // idが取得できた場合は削除前に確認
        Console.WriteLine($"id:{expense.Id} を本当に削除しますか？ (yes/no)");
        var confirm = Console.ReadLine();

        // yes(大文字まじりOK)以外の場合削除しない
        if (confirm?.ToLower() != "yes")
        {
            Console.WriteLine("削除をキャンセルしました");
            return;
        }

        // 取得できた場合は削除を実行
        repository.Delete(id);

        Console.WriteLine("削除しました");
        Console.WriteLine("Enterキーで戻ります");
        Console.ReadLine();
    }

    /// <summary>
    /// 必須の日付入力を受け取り、正しい形式になるまで再入力を求める
    /// cancelまたはnullが入力された場合はdefault(DateTime)を返す
    /// </summary>
    /// <param name="message">入力を促すメッセージ</param>
    /// <returns>正しい日付、中断時はdefault</returns>
    private DateTime GetRequiredDate()
    {
        while (true)
        {
            var text = GetInput();

            // cancelが入力された場合は中断し、元の場所へ戻る
            if (ShouldStopInput(text))
            {
                ShowCancelMessage();
                return default;
            }

            // 入力された文字列が正しい日付として読み取れた場合、その日付を返す
            if (DateTime.TryParse(text, out var date))
            {
                return date;
            }

            // 読み取れなかった場合は再入力を促す
            Console.WriteLine("正しい日付を入力してください");
        }
    }

    /// <summary>
    /// 必須の数値入力を受け取り、正しい整数値になるまで再入力を求める
    /// cancelまたはnullが入力された場合はdefault(int)を返す
    /// </summary>
    /// <param name="message">入力を促すメッセージ</param>
    /// <returns>正しい整数値、中断時はdefault</returns>
    private int GetRequiredInt()
    {
        while (true)
        {
            var text = GetInput();

            // cancelと入力されたら中断し、元の場所に戻る
            if (ShouldStopInput(text))
            {
                ShowCancelMessage();
                return default;
            }

            // 正しい整数として読み取れたら返す
            if (int.TryParse(text, out var value))
            {
                return value;
            }

            // 読み取れなかった場合は再入力を促す
            Console.WriteLine("数値を入力してください");
        }
    }

    /// <summary>
    /// 必須の文字列入力を受け取り、空文字や空白のみの場合は再入力を求める
    /// cancelまたはnullが入力された場合はnullを返す
    /// </summary>
    /// <param name="message">入力を促すメッセージ</param>
    /// <returns>空白以外の文字列、中断時はnull</returns>
    private string GetRequiredString()
    {
        while (true)
        {
            var text = GetInput();

            // cancelと入力されたら処理を中断し、元の場所に戻る
            if (ShouldStopInput(text))
            {
                ShowCancelMessage();
                return null;
            }

            // 空白以外の文字列なら返す
            if (!string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
            // 空白のみの場合は再入力を促す
            Console.WriteLine("値が入力されていません");
        }
    }

    /// <summary>
    /// 任意の文字列入力を受け取る（空文字も可）
    /// cancelまたはnullが入力された場合はnullを返す
    /// </summary>
    /// <param name="message">入力を促すメッセージ</param>
    /// <returns>入力された文字列、中断時はnull</returns>
    private string GetOptionalString()
    {
        var text = GetInput();

        // cancelと入力されたら処理を中断し、元の場所に戻る
        if (ShouldStopInput(text))
        {
            ShowCancelMessage();
            return null;
        }

        return text;
    }

    /// <summary>
    /// 日付の更新処理。cancelが入力された場合は更新せずfalseを返す
    /// </summary>
    /// <param name="message">ユーザーに表示する入力メッセージ</param>
    /// <param name="expense">更新対象の支出データ</param>
    /// <returns>更新できた場合true、中断した場合false</returns>
    private bool UpdateDate(Expense expense)
    {
        var newDate = GetRequiredDate();

        // cancelの場合は更新しない
        if (newDate == default)
        {
            return false;
        }

        // 正しい日付が入力されたら更新する
        expense.Date = newDate.ToString("yyyy/MM/dd");
        return true;
    }

    /// <summary>
    /// 金額の更新処理。cancelが入力された場合は更新せずfalseを返す
    /// </summary>
    /// <param name="message">ユーザーに表示する入力メッセージ</param>
    /// <param name="expense">更新対象の支出データ</param>
    /// <returns>更新できた場合 true、中断した場合 false</returns>
    private bool UpdatePrice(Expense expense)
    {
        var newPrice = GetRequiredInt();
        if (newPrice == default)
        {
            return false;
        }

        expense.Price = newPrice;
        return true;
    }

    /// <summary>
    /// カテゴリの更新処理。cancelが入力された場合は更新せずfalseを返す
    /// </summary>
    /// <param name="message">入力を促すメッセージ</param>
    /// <param name="expense">更新対象の支出データ</param>
    /// <returns>更新できた場合true、中断した場合false</returns>
    private bool UpdateCategory(Expense expense)
    {
        var newCategory = GetRequiredString();
        if (newCategory == null)
        {
            return false;
        }

        expense.Category = newCategory;
        return true;
    }

    /// <summary>
    /// メモの更新処理。cancelが入力された場合は更新せずfalseを返す
    /// </summary>
    /// <param name="message">入力を促すメッセージ</param>
    /// <param name="expense">更新対象の支出データ</param>
    /// <returns>更新できた場合true、中断した場合false</returns>
    private bool UpdateMemo(Expense expense)
    {
        var newMemo = GetOptionalString();
        if (newMemo == null)
        {
            return false;
        }

        expense.Memo = newMemo;
        return true;
    }

    /// <summary>
    /// メッセージを表示し、ユーザーの入力を受け取る
    /// </summary>
    /// <param name="message">画面に表示するメッセージ</param>
    /// <returns>入力された文字列</returns>
    private static string GetInput()
    {
        return Console.ReadLine();
    }

    /// <summary>
    /// 入力された値がcancel(大文字可)かどうかを判断する
    /// </summary>
    /// <param name="input">ユーザーが入力した文字列</param>
    /// <returns>cancelの場合true、それ以外false</returns>
    private static bool IsCancel(string input)
    {
        return input?.ToLower() == "cancel";
    }

    /// <summary>
    /// 入力がnullまたはcancelの場合にtrueを返す
    /// 入力処理を中断するかどうかを判断する
    /// </summary>
    /// <param name="input">ユーザーが入力した文字列</param>
    /// <returns>中断する場合true、それ以外false</returns>
    private static bool ShouldStopInput(string? input)
    {
        return input == null || IsCancel(input);
    }

    /// <summary>
    /// 中止時のメッセージを表示する
    /// </summary>
    private static void ShowCancelMessage()
    {
        Console.WriteLine("入力を中止しました");
    }
}
