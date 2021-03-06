//
//          Copyright Seth Hendrick 2016-2021.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using System.Collections.Generic;
using System.Xml.Linq;
using SethCS.Exceptions;
using SethCS.Extensions;

namespace Chaskis.Core
{
    /// <summary>
    /// Args that are passed into <see cref="SendKickEventHandler"/> when
    /// the bot sends the kick command to a channel.
    /// </summary>
    public sealed class SendKickEventArgs : BaseCoreEventArgs
    {
        // ---------------- Fields ----------------

        internal const string XmlRootName = "chaskis_sendkick_event";

        // ---------------- Constructor ----------------

        internal SendKickEventArgs() :
            base()
        {
            this.Channel = null;
            this.Reason = null;
        }

        // ---------------- Properties ----------------

        public IIrcWriter Writer { get; internal set; }

        /// <summary>
        /// The channel the bot sent the message from.
        /// </summary>
        public string Channel { get; internal set; }

        /// <summary>
        /// The reason the user was kicked from the channel.
        /// Set to <see cref="string.Empty"/> if there was no reason.
        /// </summary>
        public string Reason { get; internal set; }

        protected override string XmlElementName => XmlRootName;

        protected override IEnumerable<XElement> ChildToXml()
        {
            return new List<XElement>
            {
                new XElement( "channel", this.Channel ),
                new XElement( "reason", this.Reason ?? string.Empty )
            };
        }
    }

    /// <summary>
    /// Extensions to <see cref="SendKickEventArgs"/>
    /// </summary>
    internal static class SendKickEventArgsExtensions
    {
        public static SendKickEventArgs FromXml( string xmlString, IIrcWriter writer )
        {
            SendKickEventArgs args = new SendKickEventArgs
            {
                Writer = writer
            };

            XElement root = BaseCoreEventArgs.ParseXml( args, xmlString );
            BaseCoreEventArgs.ParseBaseXml( args, root );

            foreach( XElement child in root.Elements() )
            {
                if( "channel".EqualsIgnoreCase( child.Name.LocalName ) )
                {
                    args.Channel = child.Value;
                }
                else if( "reason".EqualsIgnoreCase( child.Name.LocalName ) )
                {
                    args.Reason = child.Value;
                }
            }

            if( ( args.Channel == null ) || ( args.Reason == null ) )
            {
                throw new ValidationException(
                    $"Could not find all required properties when creating {nameof( SendKickEventArgs )}"
                );
            }

            return args;
        }
    }
}
