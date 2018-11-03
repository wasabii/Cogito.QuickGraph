using System;

namespace Cogito.QuickGraph
{

    static class GraphUtil
    {

        /// <summary>
        /// Returns the edges for a given dimension value.
        /// </summary>
        /// <param name="getter"></param>
        /// <returns></returns>
        public static global::QuickGraph.TryFunc<T, TResult> GetTryGetFunc<T, TResult>(Func<T, TResult> getter)
        {
            // capture implementation
            bool TryGetFuncImpl(T arg, out TResult result)
            {
                result = getter(arg);
                return result != null;
            }

            // return internal impl
            return TryGetFuncImpl;
        }

    }

}
