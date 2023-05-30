using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ObjectInterface
{
    /// <summary>
    /// 挙動オブジェクトインターフェースクラス
    /// </summary>
    public interface IActor
    {
        /// <summary>
        /// 初期化関数
        /// </summary>
        void Initialization();
        /// <summary>
        /// 継続更新関数
        /// </summary>
        void Update();
    }
}
