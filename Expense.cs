/// <summary>
/// 支出データ1件分を表すクラス
/// </summary>
class Expense
{
    // 支出のid
    public int Id { get; set; }
    // 支出が発生した日付
    public string Date { get; set; }
    // 支出金額
    public int Price { get; set; }
    // 支出のカテゴリ（食費・交通費など）
    public string Category { get; set; }
    // 支出に関するメモ（任意）
    public string Memo { get; set; }
}