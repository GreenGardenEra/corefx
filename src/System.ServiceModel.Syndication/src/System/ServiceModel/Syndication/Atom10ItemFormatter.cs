// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Schema;
using System.Runtime.CompilerServices;

namespace System.ServiceModel.Syndication
{
    [XmlRoot(ElementName = Atom10Constants.EntryTag, Namespace = Atom10Constants.Atom10Namespace)]
    public class Atom10ItemFormatter : SyndicationItemFormatter, IXmlSerializable
    {
        private Atom10FeedFormatter _feedSerializer;
        private Type _itemType;
        private bool _preserveAttributeExtensions;
        private bool _preserveElementExtensions;

        public Atom10ItemFormatter()
            : this(typeof(SyndicationItem))
        {
        }

        public Atom10ItemFormatter(Type itemTypeToCreate)
            : base()
        {
            if (itemTypeToCreate == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("itemTypeToCreate");
            }
            if (!typeof(SyndicationItem).IsAssignableFrom(itemTypeToCreate))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("itemTypeToCreate",
                    SR.Format(SR.InvalidObjectTypePassed, "itemTypeToCreate", "SyndicationItem"));
            }
            _feedSerializer = new Atom10FeedFormatter();
            _feedSerializer.PreserveAttributeExtensions = _preserveAttributeExtensions = true;
            _feedSerializer.PreserveElementExtensions = _preserveElementExtensions = true;
            _itemType = itemTypeToCreate;
        }

        public Atom10ItemFormatter(SyndicationItem itemToWrite)
            : base(itemToWrite)
        {
            // No need to check that the parameter passed is valid - it is checked by the c'tor of the base class
            _feedSerializer = new Atom10FeedFormatter();
            _feedSerializer.PreserveAttributeExtensions = _preserveAttributeExtensions = true;
            _feedSerializer.PreserveElementExtensions = _preserveElementExtensions = true;
            _itemType = itemToWrite.GetType();
        }

        public bool PreserveAttributeExtensions
        {
            get { return _preserveAttributeExtensions; }
            set
            {
                _preserveAttributeExtensions = value;
                _feedSerializer.PreserveAttributeExtensions = value;
            }
        }

        public bool PreserveElementExtensions
        {
            get { return _preserveElementExtensions; }
            set
            {
                _preserveElementExtensions = value;
                _feedSerializer.PreserveElementExtensions = value;
            }
        }

        public override string Version
        {
            get { return SyndicationVersions.Atom10; }
        }

        protected Type ItemType
        {
            get
            {
                return _itemType;
            }
        }

        public override bool CanRead(XmlReader reader)
        {
            if (reader == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
            }
            return reader.IsStartElement(Atom10Constants.EntryTag, Atom10Constants.Atom10Namespace);
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The IXmlSerializable implementation is only for exposing under WCF DataContractSerializer. The funcionality is exposed to derived class through the ReadFrom\\WriteTo methods")]
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The IXmlSerializable implementation is only for exposing under WCF DataContractSerializer. The funcionality is exposed to derived class through the ReadFrom\\WriteTo methods")]
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("reader");
            }
            SyndicationFeedFormatter.TraceItemReadBegin();
            ReadItem(reader);
            SyndicationFeedFormatter.TraceItemReadEnd();
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Justification = "The IXmlSerializable implementation is only for exposing under WCF DataContractSerializer. The funcionality is exposed to derived class through the ReadFrom\\WriteTo methods")]
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            SyndicationFeedFormatter.TraceItemWriteBegin();
            WriteItem(writer);
            SyndicationFeedFormatter.TraceItemWriteEnd();
        }

        public override void ReadFrom(XmlReader reader)
        {
            SyndicationFeedFormatter.TraceItemReadBegin();
            if (!CanRead(reader))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR.Format(SR.UnknownItemXml, reader.LocalName, reader.NamespaceURI)));
            }
            ReadItem(reader);
            SyndicationFeedFormatter.TraceItemReadEnd();
        }

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writer");
            }
            SyndicationFeedFormatter.TraceItemWriteBegin();
            writer.WriteStartElement(Atom10Constants.EntryTag, Atom10Constants.Atom10Namespace);
            WriteItem(writer);
            writer.WriteEndElement();
            SyndicationFeedFormatter.TraceItemWriteEnd();
        }

        protected override SyndicationItem CreateItemInstance()
        {
            return SyndicationItemFormatter.CreateItemInstance(_itemType);
        }

        private void ReadItem(XmlReader reader)
        {
            SetItem(CreateItemInstance());
            _feedSerializer.ReadItemFrom(XmlDictionaryReader.CreateDictionaryReader(reader), this.Item);
        }

        private void WriteItem(XmlWriter writer)
        {
            if (this.Item == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR.Format(SR.ItemFormatterDoesNotHaveItem)));
            }
            XmlDictionaryWriter w = XmlDictionaryWriter.CreateDictionaryWriter(writer);
            _feedSerializer.WriteItemContents(w, this.Item);
        }
    }

    [XmlRoot(ElementName = Atom10Constants.EntryTag, Namespace = Atom10Constants.Atom10Namespace)]
    public class Atom10ItemFormatter<TSyndicationItem> : Atom10ItemFormatter
        where TSyndicationItem : SyndicationItem, new()
    {
        // constructors
        public Atom10ItemFormatter()
            : base(typeof(TSyndicationItem))
        {
        }
        public Atom10ItemFormatter(TSyndicationItem itemToWrite)
            : base(itemToWrite)
        {
        }

        protected override SyndicationItem CreateItemInstance()
        {
            return new TSyndicationItem();
        }
    }
}
