//
//          Copyright Seth Hendrick 2016-2021.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using System.Collections.Generic;

namespace Chaskis.Core
{
    /// <summary>
    /// Configuration for setting up <see cref="SendActionEventHandler"/>.
    /// </summary>
    public sealed class SendActionEventConfig :
        BaseCoreEvent<SendActionEventConfig, SendActionEventHandlerAction, SendActionEventArgs>
    {
        // ---------------- Constructor ----------------

        public SendActionEventConfig()
        {
        }

        // ---------------- Functions ----------------

        public override SendActionEventConfig Clone()
        {
            return (SendActionEventConfig)this.MemberwiseClone();
        }

        protected override IEnumerable<string> ValidateChild()
        {
            // Nothing to validate.
            return null;
        }
    }
}
