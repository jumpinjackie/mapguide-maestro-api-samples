﻿#region Disclaimer / License

// Copyright (C) 2010, Jackie Ng
// https://github.com/jumpinjackie/mapguide-maestro
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//

#endregion Disclaimer / License

using OSGeo.MapGuide.MaestroAPI.Feature;
using OSGeo.MapGuide.MaestroAPI.Internal;
using OSGeo.MapGuide.MaestroAPI.Schema;
using System;

namespace OSGeo.MapGuide.MaestroAPI.Native
{
    public class LocalNativeDataReader : ReaderBase
    {
        private MgDataReader _reader;
        private FixedWKTReader _mgReader;
        private MgAgfReaderWriter _agfRw;
        private MgWktReaderWriter _wktRw;

        public LocalNativeDataReader(MgDataReader reader)
        {
            _reader = reader;
            _mgReader = new FixedWKTReader();
            _agfRw = new MgAgfReaderWriter();
            _wktRw = new MgWktReaderWriter();
        }

        protected override IRecord ReadNextRecord()
        {
            if (_reader.ReadNext())
                return new LocalNativeRecord(_reader, _mgReader, _agfRw, _wktRw);

            return null;
        }

        public override void Close()
        {
            if (_reader != null)
            {
                try
                {
                    _reader.Close();
                    _reader.Dispose();
                }
                catch (MgException ex)
                {
                    ex.Dispose();
                }
                _reader = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Close();
                if (_agfRw != null)
                {
                    try
                    {
                        _agfRw.Dispose();
                    }
                    catch (MgException ex)
                    {
                        ex.Dispose();
                    }
                    _agfRw = null;
                }
                if (_wktRw != null)
                {
                    try
                    {
                        _wktRw.Dispose();
                    }
                    catch (MgException ex)
                    {
                        ex.Dispose();
                    }
                    _wktRw = null;
                }
            }
            base.Dispose(disposing);
        }

        public override ReaderType ReaderType => ReaderType.Data;

        public override PropertyValueType GetPropertyType(int index) => (PropertyValueType)_reader.GetPropertyType(index);

        public override PropertyValueType GetPropertyType(string name) => (PropertyValueType)_reader.GetPropertyType(name);

        public override string GetName(int index) => _reader.GetPropertyName(index);

        public override Type GetFieldType(int i)
        {
            string name = GetName(i);
            //The enum uses the same values as MgPropertyType
            var type = (PropertyValueType)_reader.GetPropertyType(name);
            return ClrFdoTypeMap.GetClrType(type);
        }
    }
}