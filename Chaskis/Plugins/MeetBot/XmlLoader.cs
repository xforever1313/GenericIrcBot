﻿//
//          Copyright Seth Hendrick 2020.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)
//

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using SethCS.Basic;

namespace Chaskis.Plugins.MeetBot
{
    public class XmlLoader
    {
        // ---------------- Fields ----------------

        private readonly GenericLogger logger;

        // ---------------- Constructor ----------------

        public XmlLoader( GenericLogger logger )
        {
            this.logger = logger;
        }

        // ---------------- Functions ----------------

        public IList<CommandDefinition> ParseDefaultFile()
        {
            using( Stream s = typeof( MeetBot ).Assembly.GetManifestResourceStream( "Chaskis.Plugins.MeetBot.Config.SampleCommands.xml" ) )
            {
                return ParseCommandFile( s, true );
            }
        }

        public IList<CommandDefinition> ParseCommandFile( Stream stream, bool isDefault )
        {
            List<CommandDefinition> cmdDefs = new List<CommandDefinition>();

            XDocument doc = XDocument.Load( stream );

            const string expectedRootName = "meetbotcommands";

            XElement root = doc.Root;
            if( expectedRootName.Equals( root.Name.LocalName ) == false )
            {
                throw new ArgumentException(
                    $"XML root name does not match {expectedRootName}, got {root.Name.LocalName}"
                );
            }

            foreach( XElement child in root.Elements() )
            {
                if( CommandDefinitionExtensions.CommandXmlElementName.Equals( child.Name.LocalName ) )
                {
                    CommandDefinition def = new CommandDefinition();
                    def.FromXml( child, this.logger );
                    cmdDefs.Add( def );
                    def.IsDefault = isDefault;
                }
                else
                {
                    this.logger.WarningWriteLine(
                        "Unexpected XML element: " + child.Name
                    );
                }
            }

            return cmdDefs;
        }
    }
}
