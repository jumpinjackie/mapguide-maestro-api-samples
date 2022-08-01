using OSGeo.MapGuide.MaestroAPI.Mapping;
using OSGeo.MapGuide.MaestroAPI.Schema;

namespace MvcCoreSample.Models;

public record PropertyDefn(string Name, string Type, bool IsIdentity = false);

public class DescribeLayerViewModel
{
    public string Name { get; set; } = null!;

    public string LegendLabel { get; set; } = null!;

    public double DisplayOrder { get; set; }

    public bool ExpandInLegend { get; set; }

    public bool ShowInLegend { get; set; }

    public bool Visible { get; set; }

    public string LayerDefinitionID { get; set; }

    public bool HasTooltips { get; set; }

    public string Filter { get; set; }

    public string Schema { get; set; }

    public string ClassName { get; set; }

    public List<PropertyDefn> Properties { get; } = new List<PropertyDefn>();

    public DescribeLayerViewModel(RuntimeMapLayer layer, ClassDefinition clsDef)
    {
        this.Name = layer.Name;
        this.LegendLabel = layer.LegendLabel;
        this.DisplayOrder = layer.DisplayOrder;
        this.ExpandInLegend = layer.ExpandInLegend;
        this.ShowInLegend = layer.ShowInLegend;
        this.Visible = layer.Visible;
        this.LayerDefinitionID = layer.LayerDefinitionID;
        this.HasTooltips = layer.HasTooltips;
        this.Filter = layer.Filter;

        this.Schema = clsDef.QualifiedName.Split(':')[0];
        this.ClassName = clsDef.Name;

        for (int i = 0; i < clsDef.Properties.Count; i++)
        {
            PropertyDefinition prop = clsDef.Properties[i];
            bool isIdentity = false;

            if (prop.Type == PropertyDefinitionType.Data)
            {
                isIdentity = clsDef.IdentityProperties.Contains((DataPropertyDefinition)prop);
            }

            this.Properties.Add(new PropertyDefn(prop.Name, prop.Type.ToString(), isIdentity));
        }
    }
}
