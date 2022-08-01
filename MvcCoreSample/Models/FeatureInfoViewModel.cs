using OSGeo.MapGuide.MaestroAPI.Feature;

namespace MvcCoreSample.Models;

public class FeatureInfoViewModel
{
    public FeatureInfoViewModel(IFeatureReader reader)
    {
        //Collect the field names
        var properties = new FeaturePropertyValue[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
        {
            properties[i].Name = reader.GetName(i);
        }

        //Write out the attribute table
        while (reader.ReadNext())
        {
            for (int i = 0; i < properties.Length; i++)
            {
                //Just like the MgFeatureReader, you must test for null before
                //attempting extraction of values, but unlike MgFeatureReader this
                //offers an indexer property that returns System.Object which allows
                //a nice and easy way to string coerce all property values.
                properties[i].Value = !reader.IsNull(i) ? reader[i]?.ToString() : null;
            }
        }

        this.Properties = properties;
    }

    public FeaturePropertyValue[] Properties { get; private set; } = null!;
}

public class FeaturePropertyValue
{
    public string Name { get; set; } = null!;

    public string? Value { get; set; }
}
