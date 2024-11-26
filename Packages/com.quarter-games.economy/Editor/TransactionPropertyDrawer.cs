using UnityEditor;

namespace QG.Managers.Economy.Editor
{
    using QG.Managers.Economy.Transactions;
    using QG.Managers.Editor;
    [CustomPropertyDrawer(typeof(Transaction))]
    public class TransactionPropertyDrawer : GenericPropertyDrawer<Transaction>
    {
    }
}
