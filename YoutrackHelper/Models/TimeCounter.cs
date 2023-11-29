using System;
using System.Collections.Generic;

namespace YoutrackHelper.Models
{
    public class TimeCounter
    {
        private readonly Dictionary<string, DateTime> trackingTimeDictionary = new ();

        /// <summary>
        ///     指定したキーで時間計測の開始時刻を設定します。
        ///     同一のキーで複数回の値時刻の登録は無効です。最初に登録した時刻のみ有効です。
        /// </summary>
        /// <param name="trackingName">時間計測の情報を判別する文字列</param>
        /// <param name="dateTime">計測の開始時刻</param>
        public void StartTimeTracking(string trackingName, DateTime dateTime)
        {
            trackingTimeDictionary.TryAdd(trackingName, dateTime);
        }

        /// <summary>
        ///     StartTimeTracking() で登録した開始時刻から、このメソッドで入力した終了時刻までの時間を取得します。
        /// </summary>
        /// <param name="trackingName">時間計測の情報を判別する文字列</param>
        /// <param name="dateTime">計測の終了時刻</param>
        /// <returns>
        ///     指定したキーで登録されている開始時刻から、dateTime までの時間を返します。
        ///     指定したキーで開始時刻の登録がない場合、また開始時刻よりも終了時刻が前である場合は TimeSpan.Zero を返します。
        /// </returns>
        public TimeSpan FinishTimeTracking(string trackingName, DateTime dateTime)
        {
            if (!trackingTimeDictionary.ContainsKey(trackingName))
            {
                return TimeSpan.Zero;
            }

            var result = dateTime > trackingTimeDictionary[trackingName]
                ? dateTime - trackingTimeDictionary[trackingName]
                : TimeSpan.Zero;

            trackingTimeDictionary.Remove(trackingName);
            return result;
        }

        public bool IsTrackingNameRegistered(string trackingName)
        {
            return trackingTimeDictionary.ContainsKey(trackingName);
        }
    }
}