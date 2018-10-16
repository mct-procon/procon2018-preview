using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameInterface.ViewModels
{
    public class NotifyDataErrorInfoViewModel : ViewModelBase, INotifyDataErrorInfo
    {
        private Dictionary<string, HashSet<string>> Errors = new Dictionary<string, HashSet<string>>();

        /// <summary>
        /// エラーを追加するメソッド
        /// </summary>
        /// <param name="PropertyName">プロパティ名</param>
        /// <param name="ErrorText">エラー文</param>
        /// <returns>同じエラー文が無ければtrue、あればfalse</returns>
        protected bool AddError(string ErrorText, [CallerMemberName]string PropertyName = "")
        {
            if (string.IsNullOrEmpty(PropertyName))
                throw new ArgumentException(nameof(PropertyName));
            if (string.IsNullOrEmpty(ErrorText))
                throw new ArgumentException(nameof(ErrorText));

            if (!Errors.ContainsKey(PropertyName))
                Errors[PropertyName] = new HashSet<string>();

            bool ret = Errors[PropertyName].Add(ErrorText);
            if (ret)
                RaiseErrorsChanged(PropertyName);
            return ret;
        }

        /// <summary>
        /// エラーを消去するメソッド
        /// </summary>
        /// <param name="PropertyName">プロパティ名</param>
        /// <returns>削除できればtrue、元から無ければfalse</returns>
        protected bool ResetError([CallerMemberName] string PropertyName = "")
        {
            if (string.IsNullOrEmpty(PropertyName))
                throw new ArgumentException(nameof(PropertyName));

            bool ret = Errors.Remove(PropertyName);
            if (ret)
                RaiseErrorsChanged(PropertyName);
            return ret;
        }

        /// <summary>
        /// エラーがあるかを取得するメソッド
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>エラー内容</returns>
        public System.Collections.IEnumerable GetErrors([CallerMemberName] string propertyName = "")
        {
            return GetErrorsG(propertyName);
        }

        /// <summary>
        /// GetErrorsのジェネリック版
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        /// <returns>エラー内容</returns>
        public IEnumerable<string> GetErrorsG([CallerMemberName] string propertyName = "")
        {
            if (string.IsNullOrEmpty(propertyName))
                return Errors.Values.SelectMany(p => p).ToList().AsReadOnly();    //エンティティレベルでのエラー
            else if (Errors.ContainsKey(propertyName))
                return Errors[propertyName].ToList().AsReadOnly();
            else
                return Enumerable.Empty<string>();
        }

        /// <summary>
        /// エラーがあるかどうか
        /// </summary>
        public bool HasErrors => Errors.Any();

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// エラー変更を発行するメソッド
        /// </summary>
        /// <param name="propertyName">プロパティ名</param>
        void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
