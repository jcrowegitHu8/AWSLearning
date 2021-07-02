using System;
using System.Collections.Generic;
using System.Text;

namespace FeatureFlag.Shared.Helper
{
    public static class Guard
    {
        public static void AgainstNull(object thing, string name)
        {
            if (thing == null)
            {
                throw new ArgumentNullException($"{name} can not be NULL.");
            }
        }
    }
}
