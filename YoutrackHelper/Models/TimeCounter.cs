using System;
using System.Collections.Generic;

namespace YoutrackHelper.Models
{
    public class TimeCounter
    {
        private readonly Dictionary<string, DateTime> trackingTimeDictionary = new ();

        public void StartTimeTracking(string trackingName, DateTime dateTime)
        {
            trackingTimeDictionary.TryAdd(trackingName, dateTime);
        }

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
    }
}