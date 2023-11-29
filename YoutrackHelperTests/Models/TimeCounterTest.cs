using System;
using NUnit.Framework;
using YoutrackHelper.Models;

namespace YoutrackHelperTests.Models
{
    public class TimeCounterTest
    {
        [Test]
        public void StartTimeTrackingTest()
        {
            var timeCounter = new TimeCounter();
            timeCounter.StartTimeTracking("testKey", new DateTime());
        }

        [Test]
        public void FinishTimeTrackingTest()
        {
            var timeCounter = new TimeCounter();
            timeCounter.StartTimeTracking("testKey", new DateTime());
            var result = timeCounter.FinishTimeTracking("testKey", new DateTime(1000));

            Assert.That(result.Ticks, Is.EqualTo(1000));
        }

        [Test]
        public void FinishTimeTrackingTest_2()
        {
            var timeCounter = new TimeCounter();
            timeCounter.StartTimeTracking("testKey", new DateTime());
            timeCounter.StartTimeTracking("testKey", new DateTime(100)); // ２度目の入力は無効
            var result = timeCounter.FinishTimeTracking("testKey", new DateTime(1000));

            Assert.That(result.Ticks, Is.EqualTo(1000));
        }

        [Test]
        public void FinishTimeTrackingTest_3()
        {
            var timeCounter = new TimeCounter();
            timeCounter.StartTimeTracking("testKey", new DateTime());
            timeCounter.StartTimeTracking("testKey2", new DateTime(100)); //　別の ID の入力は別計測
            var result = timeCounter.FinishTimeTracking("testKey", new DateTime(1000));
            var result2 = timeCounter.FinishTimeTracking("testKey2", new DateTime(1000));

            Assert.That(result.Ticks, Is.EqualTo(1000));
            Assert.That(result2.Ticks, Is.EqualTo(900));
        }

        [Test]
        public void FinishTimeTrackingTest_出力後にログが消えているか()
        {
            var timeCounter = new TimeCounter();
            timeCounter.StartTimeTracking("testKey", new DateTime());
            _ = timeCounter.FinishTimeTracking("testKey", new DateTime(1000));
            var result = timeCounter.FinishTimeTracking("testKey", new DateTime(1000));

            Assert.That(result.Ticks, Is.EqualTo(0), "一度目の出力の時点で、トラッキングがリセットされているはずなので 0 が出力されるはず");
        }
    }
}