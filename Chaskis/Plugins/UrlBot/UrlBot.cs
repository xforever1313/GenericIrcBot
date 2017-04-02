﻿//
//          Copyright Seth Hendrick 2016.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file ../../LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//
using System.Collections.Generic;
using ChaskisCore;

namespace Chaskis.Plugins.UrlBot
{
    /// <summary>
    /// This class takes a URL from the channel, reads the HTML From it, finds
    /// the description meta tag, and then prints the description to the channel.
    /// </summary>
    [ChaskisPlugin( "urlbot" )]
    public class UrlBot : IPlugin
    {
        // -------- Fields --------

        public const string VersionStr = "0.1.0";

        /// <summary>
        /// The handlers for this plugin.
        /// </summary>
        private readonly List<IIrcHandler> handlers;

        private readonly UrlReader urlReader;

        // -------- Constructor --------

        /// <summary>
        /// Constructor.
        /// </summary>
        public UrlBot()
        {
            this.urlReader = new UrlReader();
            this.handlers = new List<IIrcHandler>();
        }

        // -------- Properties --------

        /// <summary>
        /// The location of the source code.
        /// </summary>
        public string SourceCodeLocation
        {
            get
            {
                return "https://github.com/xforever1313/Chaskis/tree/master/Chaskis/Plugins/UrlBot";
            }
        }

        /// <summary>
        /// This plugin's version.
        /// </summary>
        public string Version
        {
            get
            {
                return VersionStr;
            }
        }

        /// <summary>
        /// About this plugin.
        /// </summary>
        public string About
        {
            get
            {
                return "When someone posts a URL, I will go to the URL and print its title information. I will ignore sites that do not have a title tag or are over 1Mb large.";
            }
        }

        // -------- Functions --------

        /// <summary>
        /// Initializes the plugin.  This includes loading any configuration files,
        /// starting services, etc.  Allowed to throw Exceptions.
        ///
        /// This function should be used to validates that the environment is good for the plugin.
        /// For example, it has all dependencies installed, config files are in the correct spot, etc.
        /// It should also load GetHandlers() with the handlers.
        /// </summary>
        /// <param name="pluginPath">
        /// The absolute path to the plugin, including the file name.  To just get
        /// the path to the plugin, call Path.GetDirectoryName on this argument.
        /// </param>
        /// <param name="ircConfig">The IRC config we are using.</param>
        public void Init( string pluginPath, IIrcConfig ircConfig )
        {
            MessageHandler handler = new MessageHandler(
                ".+",
                this.HandleMessage
            );

            this.handlers.Add( handler );
        }

        /// <summary>
        /// Handles the help command.
        /// </summary>
        public void HandleHelp( IIrcWriter writer, IrcResponse response, string[] args )
        {
            writer.SendMessageToUser(
                this.About,
                response.Channel
            );
        }

        /// <summary>
        /// Gets the handlers that should be added to the main bot.
        /// </summary>
        /// <returns>The list of handlers to awtch.</returns>
        public IList<IIrcHandler> GetHandlers()
        {
            return this.handlers.AsReadOnly();
        }

        /// <summary>
        /// Tears down the plugin.
        /// </summary>
        public void Teardown()
        {
        }

        /// <summary>
        /// Handles a message from the channel.
        /// </summary>
        /// <param name="writer">The IRC Writer to write to.</param>
        /// <param name="response">The response from the channel.</param>
        private async void HandleMessage( IIrcWriter writer, IrcResponse response )
        {
            string url;
            if( UrlReader.TryParseUrl( response.Message, out url ) )
            {
                UrlResponse urlResponse = await this.urlReader.GetDescription( url );

                if( urlResponse.IsValid )
                {
                    writer.SendMessageToUser(
                        string.Format( "Title: {0}", urlResponse.TitleShortened ),
                        response.Channel
                    );
                }
            }
        }
    }
}