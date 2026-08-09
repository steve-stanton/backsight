using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.ViewModels;
using Backsight.Model;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using IFeature = Mapsui.IFeature;

namespace Backsight.Map.Editor.Mapping;

class MapProvider : IProvider
{
    private readonly IMapEditorViewModel _viewModel;
    private readonly IMapStore _store;
    
    internal MapProvider(IMapEditorViewModel viewModel, IMapStore store)
    {
        _viewModel = viewModel;
        _store = store;
    }

    MRect? IProvider.GetExtent()
    {
        return _store.Model.Extent.ToMRect();
    }

    Task<IEnumerable<IFeature>> IProvider.GetFeaturesAsync(FetchInfo fetchInfo)
    {
        var result = new List<IFeature>();
        var requiredTypes = _viewModel.GetTypesAtCurrentScale();
        if (requiredTypes == SpatialType.None)
            return Task.FromResult<IEnumerable<IFeature>>(result);

        Debug.Assert(_store is not null);
            
        var window = fetchInfo.Window;
        Console.WriteLine("fetching data");

        result.AddRange(_store.Query<Model.LineFeature>(window).Select(x => new Line(x)));
        
        if (requiredTypes.HasFlag(SpatialType.Point))
            result.AddRange(_store.Query<Model.PointFeature>(window).Select(x => new Point(x)));
        
        if (requiredTypes.HasFlag(SpatialType.Text))
            result.AddRange(_store.Query<Model.TextFeature>(window).Select(x => new Text(x)));
        
        Console.WriteLine("found " + result.Count);
        return Task.FromResult<IEnumerable<IFeature>>(result);
    }

    string? IProvider.CRS
    {
        get => "EPSG:26914"; // UTM zone 14 (assume we're in Manitoba)
        set => throw new NotImplementedException();
    }

}