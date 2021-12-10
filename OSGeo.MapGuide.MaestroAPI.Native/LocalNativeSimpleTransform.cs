﻿#region Disclaimer / License

// Copyright (C) 2011, Jackie Ng
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

using OSGeo.MapGuide.MaestroAPI.CoordinateSystem;
using System;

namespace OSGeo.MapGuide.MaestroAPI.Native
{
    public class LocalNativeSimpleTransform : ISimpleTransform
    {
        private MgCoordinateSystemTransform _trans;

        internal LocalNativeSimpleTransform(string sourceWkt, string targetWkt)
        {
            var fact = new MgCoordinateSystemFactory();
            var source = fact.Create(sourceWkt);
            var target = fact.Create(targetWkt);

            _trans = fact.GetTransform(source, target);
        }

        public bool Transform(double x, double y, out double tx, out double ty)
        {
            tx = Double.NaN;
            ty = Double.NaN;

            var coord = _trans.Transform(x, y);

            tx = coord.X;
            ty = coord.Y;
            return true;
        }

        ~LocalNativeSimpleTransform()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _trans?.Dispose();
                _trans = null;
            }
        }
    }
}