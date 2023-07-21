using UnityEngine;

namespace LubyLib.Core
{
    /// <summary>
    /// Shows variable on Inspector. But does not allow any change to its values.
    /// </summary>
    /// <remarks>Mosta a variável no Inspector. Mas não permite nenuma mudança nos valores.</remarks>
    public class ReadOnlyAttribute : PropertyAttribute
    {
    }

}