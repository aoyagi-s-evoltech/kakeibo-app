class ExpenseManager
{
    /// <summary>
    /// 支出を1件データベースに追加する処理。
    /// </summary>
    /// <remarks>
    /// ユーザーから日付・金額・カテゴリ・メモの入力を受け取る
    /// expensesテーブルへ追加(INSERT)する。
    /// </remarks>

    public void AddExpense()
    {
        Console.WriteLine("支出追加処理");

        // 入力を受け取る
        Console.WriteLine("日付を入力してください");
        var date = Console.ReadLine();

        Console.WriteLine("金額を入力してください");
        var price = Console.ReadLine();

        Console.WriteLine("カテゴリを入力してください");
        var category = Console.ReadLine();

        Console.WriteLine("メモを入力してください");
        var memo = Console.ReadLine();

        var repository = new ExpenseRepository();
        repository.Insert(date, price, category, memo);

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

        // 編集したいidを入力
        Console.WriteLine("編集したいidを入力してください");
        var idText = Console.ReadLine();
        var id = int.Parse(idText);

        var repository = new ExpenseRepository();
        var expense = repository.GetById(id);
    
        // データがない場合
        if(expense == null)
        {
            Console.WriteLine("データがありません");
            return;
        }

        // 現在の内容を表示
        Console.WriteLine($"現在の内容：日付:{expense.Date}  金額:{expense.Price}  カテゴリ:{expense.Category}  メモ:{expense.Memo}");

        // 新しい値を入力
        Console.WriteLine("新しい日付を入力してください");
        expense.Date = Console.ReadLine();

        Console.WriteLine("新しい金額を入力してください");
        expense.Price = int.Parse(Console.ReadLine());

        Console.WriteLine("新しいカテゴリを入力してください");
        expense.Category = Console.ReadLine();

        Console.WriteLine("新しいメモを入力してください");
        expense.Memo = Console.ReadLine();

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

        using (var connection = new SqliteConnection("Data Source = expenses.db"))
        {
            // 接続開始
            connection.Open();
            // SQL実行のためのコマンドを作成
            using var command = connection.CreateCommand();

            // 削除したいidを入力
            Console.WriteLine("削除したいidを入力してください");
            var idText = Console.ReadLine();
            var id = int.Parse(idText);
            
            // idのデータを取得しデータが存在するのか確認
            command.CommandText = @"
                SELECT id, date, price, category, memo
                FROM expenses
                WHERE id = @id;
            ";
            
            // SQL文内の@idにidを渡す
            command.Parameters.AddWithValue("@id", id);

            var reader = command.ExecuteReader(); 

            // idが取得できればtrue
            bool exists = reader.Read();
            reader.Close();

            // idが取得できなければ、メッセージを出力し終了
            if(!exists)
            {
                Console.WriteLine("入力されたidがありません");
                Console.WriteLine("Enterキーで戻ります");
                Console.ReadLine();
                return;
            }

            // 取得できた場合はDELETE文を実行
            command.CommandText = @"
                DELETE FROM expenses
                WHERE id = @id;
            ";
            command.ExecuteNonQuery();

            Console.WriteLine("削除しました");
            Console.WriteLine("Enterキーで戻ります");
            Console.ReadLine();
        }
    }
}
