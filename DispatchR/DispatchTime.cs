using System;

namespace DispatchR
{
    /// <summary>
    /// DispatchTime designates when a Dispatchee should be executed
    /// </summary>
    /// <remarks>
    /// Days span from 1 to 31.
    /// Hours are in military time so they span from 0 to 23.
    /// Minutes span from 0 to 59.
    /// </remarks>
    public interface IDispatchTime
    {
    }
}
