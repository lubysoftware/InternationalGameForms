using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace LubyLib.Core.Ticker
{
    /// <summary>
    ///     <para>Tick System</para>
    /// </summary>
    public static class Ticker
    {
        private static List<ITickeable> _tickeables = new List<ITickeable>();
        
        public static float DeltaTime { get; private set; }
        
        static Ticker()
        {
#pragma warning disable 4014
            TickAsync();
#pragma warning restore 4014
        }
        
        /// <summary>
        ///     <para>Subscribe tickeable to receive calls in every tick.</para>
        /// </summary>
        /// <param name="tickeable"></param>
        public static void Subscribe(ITickeable tickeable)
        {
            _tickeables.Add(tickeable);
        }

        /// <summary>
        ///     <para>Unsubscribe tickeable to stop receive calls.</para>
        /// </summary>
        /// <param name="tickeable"></param>
        public static void Unsubscribe(ITickeable tickeable)
        {
            _tickeables.Remove(tickeable);
        }

        private static async Task TickAsync()
        {
            while (Application.isPlaying)
            {
                var time = Time.time;
                await Task.Yield();
                float frameTime = Time.time - time;
                DeltaTime = frameTime;
                CallTickeables();
            }
        }
        
        private static void CallTickeables()
        {
            for (int i = 0; i < _tickeables.Count; i++)
            {
                _tickeables[i].OnTick();
            }
        }
    }
}

