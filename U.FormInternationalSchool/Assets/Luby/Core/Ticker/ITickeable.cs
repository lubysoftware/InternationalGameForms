namespace LubyLib.Core.Ticker
{
    public interface ITickeable
    {
        /// <summary>
        ///     <para>Method called by the Ticker after every tick, if subscribed.</para>
        /// </summary>
        /// <param name="tickTime"></param>
        public void OnTick();
    }
}