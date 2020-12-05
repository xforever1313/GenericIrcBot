//
//          Copyright Seth Hendrick 2016-2020.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using System.Text.RegularExpressions;
using SethCS.Exceptions;

namespace Chaskis.Core
{
    public delegate void ReceiveHandlerAction( ReceiveHandlerArgs args );

    /// <summary>
    /// This class will fire for ALL IRC messages that are RECEIVED
    /// and pass in the raw IRC message as the message string.
    /// 
    /// Note, this should really only be used when you want to get ALL output
    /// from the server without any filtering, or there is syntax you expect
    /// from the server that you want but the bot does not support.  To filter
    /// use any of the other handlers.
    /// </summary>
    public sealed class ReceiveHandler : IIrcHandler
    {
        // ---------------- Fields ----------------

        private readonly ReceiveHandlerConfig config;

        // ---------------- Constructor ----------------

        public ReceiveHandler( ReceiveHandlerConfig allConfig )
        {
            ArgumentChecker.IsNotNull( allConfig, nameof( allConfig ) );

            allConfig.Validate();

            this.config = allConfig.Clone();
            this.KeepHandling = true;
        }

        // ---------------- Properties ----------------

        /// <summary>
        /// The action to take when ANY message appears from IRC (JOIN, PART, PRIVMSG, PING, etc).
        /// As far as the passed in IrcResponse to the action goes, the channel and remote user
        /// will be String.Empty, since this class does no parsing of the IRC message.
        /// It just grabs the line from the IRC channel and passes it into the AllAction
        /// with no parsing.  It is up to the AllAction to parse the channel and user
        /// name if they so desire.
        /// </summary>
        public ReceiveHandlerAction LineAction
        {
            get
            {
                return this.config.LineAction;
            }
        }

        /// <summary>
        /// Whether or not the handler should keep handling or not.
        /// Set to true to keep handling the event when it appears in the chat.
        /// Set to false so when the current IRC message is finished processing being,
        /// it leaves the event queue and never
        /// happens again.   Useful for events that only need to happen once.
        ///
        /// This is a public get/set.  Either classes outside of the handler can
        /// tell the handler to cancel the event, or it can cancel itself.
        ///
        /// Note: when this is set to false, there must be one more IRC message that appears
        /// before it is removed from the queue.
        ///
        /// Defaulted to true.
        /// </summary>
        public bool KeepHandling { get; set; }

        // ---------------- Functions ----------------

        /// <summary>
        /// Handles the event.
        /// </summary>
        public void HandleEvent( HandlerArgs args )
        {
            ArgumentChecker.IsNotNull( args, nameof( args ) );

            // Do not handle Chaskis Events or inter-plugin events.  This handler is only
            // for receiving messages via IRC.
            if( AnyChaskisEventHandler.Regex.IsMatch( args.Line ) )
            {
                return;
            }
            else if( InterPluginEventHandler.Regex.IsMatch( args.Line ) )
            {
                return;
            }

            ReceiveHandlerArgs allArgs = new ReceiveHandlerArgs( args.IrcWriter, args.Line );

            this.LineAction( allArgs );
        }
    }
}